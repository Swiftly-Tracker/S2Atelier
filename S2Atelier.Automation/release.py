"""7z packaging and verified, resumable GitHub Release publication."""
import hashlib
import json
import os
from pathlib import Path
import re
import shutil
import subprocess
import time
import urllib.error
import urllib.parse
import urllib.request

REPO = 'Swiftly-Tracker/CS2-IDA-Dumps'
API = 'https://api.github.com/repos/' + REPO
# GitHub rejects a release body over 125000 characters; the full report is in the snapshot assets.
MAX_NOTES = 60000


def digest(path):
    with path.open('rb') as stream:
        value = hashlib.sha256()
        for chunk in iter(lambda: stream.read(1024 * 1024), b''):
            value.update(chunk)
        return value.hexdigest()


def compress(archive, files, base_dir=None):
    executable = shutil.which('7zz') or shutil.which('7z')
    if not executable:
        raise RuntimeError('7-Zip is required (install Debian package 7zip).')
    members = [p.relative_to(base_dir) if base_dir else Path(p.name) for p in files]
    if len({str(p).casefold() for p in members}) != len(files):
        raise RuntimeError('Duplicate filenames inside archive')
    # Flatten only the archive members, not their source paths.
    staging = archive.parent / (archive.name + '.members')
    if staging.is_symlink():
        raise RuntimeError('Refusing symlink archive staging directory')
    if staging.exists():
        shutil.rmtree(staging)
    staging.mkdir()
    try:
        for path, member in zip(files, members):
            (staging / member).parent.mkdir(parents=True, exist_ok=True)
            os.link(path, staging / member)
        archive.unlink(missing_ok=True)
        subprocess.run([executable, 'a', '-t7z', '-mx=9', '-m0=lzma2', '-md=64m',
                        '-mfb=273', '-ms=on', '-mmt=2', str(archive.resolve()), '--',
                        *[str(p) for p in members]], cwd=staging, check=True)
        subprocess.run([executable, 't', str(archive.resolve())], check=True)
    finally:
        shutil.rmtree(staging)


BODY = ('IDA 9.4 databases for [CS2-Dumps `{revision}`](https://github.com/Swiftly-Tracker/CS2-Dumps/commit/{revision}).\n\n'
        'Individual databases and common bundles use 7z/LZMA2 maximum compression. '
        'Each common bundle contains server, engine2 and tier0 databases. See provenance.json for inputs and hashes. '
        'snapshots-<platform>.7z holds each module\'s analysis snapshot, the baseline of the next release.')


def release_title(subject):
    return re.sub(r'\s*\|\s*\d+\s+modified\s*(?=\|)', ' ', subject).strip()


def api(method, url, payload=None, path=None):
    token = os.environ.get('S2A_GITHUB_TOKEN')
    if not token:
        raise RuntimeError('S2A_GITHUB_TOKEN is not configured')
    for attempt in range(5):
        headers = {'Authorization': 'Bearer ' + token, 'Accept': 'application/vnd.github+json',
                   'X-GitHub-Api-Version': '2022-11-28', 'User-Agent': 'S2Atelier'}
        stream = None
        try:
            if path is not None:
                stream = path.open('rb')
                data = stream
                headers.update({'Content-Type': 'application/octet-stream', 'Content-Length': str(path.stat().st_size)})
            else:
                data = json.dumps(payload).encode() if payload is not None else None
                if data is not None:
                    headers['Content-Type'] = 'application/json'
            request = urllib.request.Request(url, data=data, headers=headers, method=method)
            with urllib.request.urlopen(request, timeout=600) as response:
                content = response.read()
                return json.loads(content) if content else None
        except urllib.error.HTTPError as error:
            if error.code not in (429, 500, 502, 503, 504) or attempt == 4:
                raise
        except (OSError, TimeoutError):
            if attempt == 4:
                raise
        finally:
            if stream:
                stream.close()
        time.sleep(2 ** attempt)


def download(url, target):
    """Downloads a release asset through the API (its url, not browser_download_url)."""
    token = os.environ.get('S2A_GITHUB_TOKEN')
    if not token:
        raise RuntimeError('S2A_GITHUB_TOKEN is not configured')
    headers = {'Authorization': 'Bearer ' + token, 'Accept': 'application/octet-stream',
               'X-GitHub-Api-Version': '2022-11-28', 'User-Agent': 'S2Atelier'}
    temporary = target.with_name(target.name + '.part')
    with urllib.request.urlopen(urllib.request.Request(url, headers=headers), timeout=600) as response, \
            temporary.open('wb') as stream:
        shutil.copyfileobj(response, stream)
    temporary.replace(target)


def previous_release(tag, asset_name):
    """The newest published release other than tag that carries asset_name: the baseline of this one."""
    for release in api('GET', API + '/releases?per_page=30') or []:
        if release.get('draft') or release.get('tag_name') == tag or not release.get('tag_name', '').startswith('cs2-'):
            continue
        asset = next((a for a in release.get('assets', []) if a.get('name') == asset_name), None)
        if asset is not None:
            return release['tag_name'], asset
    return None


def release_notes(reports, baseline_tags):
    """Markdown for the release body: what the SDK disagrees with and which passes fell, per module.

    reports holds (platform, report) pairs, report being an analyzer <module>.health.json."""
    warnings = [(platform, r['Module'], w) for platform, r in reports for w in r.get('Warnings', [])]
    regressions = [(platform, r['Module'], x) for platform, r in reports for x in r.get('Regressions', [])]
    against = ', '.join(f'{platform} `{tag}`' for platform, tag in sorted(baseline_tags.items()) if tag)
    lines = ['## Analysis report', '']
    lines.append(f'Compared with {against}.' if against else 'No previous release to compare with.')
    if not warnings and not regressions:
        lines += ['', 'No SDK disagreements and no pass fell below the previous release.']
    for title, items, explanation in (
            ('SDK disagreements', warnings,
             'Declarations in hl2sdk that do not match this build. A drifted vtable keeps its SDK names back and '
             'a contradicted entity class layout its types, until the SDK is fixed; layout differences of '
             'schema types are reported only.'),
            ('Pass regressions', regressions,
             'Counts that fell well below the previous release: a pass no longer recognises a code shape, '
             'or SDK headers stopped parsing.')):
        if items:
            lines += ['', '### ' + title, '', explanation, '']
            lines += [f'- `{module}` ({platform}): {text}' for platform, module, text in items]
    notes = '\n'.join(lines)
    if len(notes) > MAX_NOTES:
        notes = notes[:MAX_NOTES].rsplit('\n', 1)[0] + '\n\n(Truncated; see the snapshots assets for the full report.)'
    return notes


def check_publish_access():
    repository = api('GET', API)
    if not repository.get('permissions', {}).get('push'):
        raise RuntimeError('S2A_GITHUB_TOKEN requires write access to ' + REPO + '; no analysis was started.')


def assets(release_id):
    result = []
    page = 1
    while True:
        batch = api('GET', f'{API}/releases/{release_id}/assets?per_page=100&page={page}')
        result.extend(batch)
        if len(batch) < 100:
            return result
        page += 1


def matches(asset, path):
    return (asset.get('state') == 'uploaded' and asset['size'] == path.stat().st_size and
            asset.get('digest') == 'sha256:' + digest(path))


def publish(jobdir, revision, subject, archives, notes=None):
    if not archives or len({p.name.casefold() for p in archives}) != len(archives):
        raise RuntimeError('Empty or colliding Release asset names')
    if any(p.stat().st_size >= 2 * 1024**3 for p in archives):
        raise RuntimeError('A Release asset exceeds GitHub’s 2 GiB limit')
    tag = 'cs2-' + revision
    try:
        release = api('GET', API + '/releases/tags/' + tag)
    except urllib.error.HTTPError as error:
        if error.code != 404:
            raise
        release = api('POST', API + '/releases', {
            'tag_name': tag, 'name': release_title(subject), 'draft': True,
            'body': BODY.format(revision=revision)})
    files = archives + [jobdir / 'provenance.json']
    existing = {a['name']: a for a in assets(release['id'])}
    for path in files:
        asset = existing.get(path.name)
        if asset and matches(asset, path):
            continue
        if not release['draft']:
            raise RuntimeError('Published release differs; refusing to overwrite: ' + path.name)
        if asset:
            api('DELETE', API + '/releases/assets/' + str(asset['id']))
        url = release['upload_url'].split('{')[0] + '?' + urllib.parse.urlencode({'name': path.name})
        try:
            api('POST', url, path=path)
        except (OSError, urllib.error.HTTPError):
            # A timed-out upload may have completed on GitHub. Verify before retrying.
            uploaded = {a['name']: a for a in assets(release['id'])}.get(path.name)
            if not uploaded or not matches(uploaded, path):
                raise
    verified = {a['name']: a for a in assets(release['id'])}
    if set(verified) != {p.name for p in files} or any(not matches(verified[p.name], p) for p in files):
        raise RuntimeError('Release asset set/size/SHA-256 verification failed; retaining local files')
    if release['draft']:
        update = {'draft': False, 'name': release_title(subject)}
        if notes:
            update['body'] = BODY.format(revision=revision) + '\n\n' + notes
        release = api('PATCH', API + '/releases/' + str(release['id']), update)
    # Read back publication before authorizing local cleanup.
    release = api('GET', API + '/releases/' + str(release['id']))
    if release['draft']:
        raise RuntimeError('Release is still a draft; retaining local files')
    return {'url': release['html_url'], 'tag': tag, 'id': release['id'],
            'assets': [{'name': p.name, 'url': verified[p.name]['browser_download_url'],
                        'sha256': digest(p), 'bytes': p.stat().st_size} for p in files]}


def cleanup_payload(jobdir):
    # Fixed payload directories only; never remove tools, IDA, state, sources or job metadata.
    receipt = json.loads((jobdir / 'release.json').read_text())
    if not receipt.get('url') or not receipt.get('assets'):
        raise RuntimeError('No verified publication receipt; refusing cleanup')
    for name in ('windows', 'linux'):
        path = jobdir / name
        if path.exists():
            if path.is_symlink():
                raise RuntimeError('Refusing symlink payload cleanup')
            shutil.rmtree(path)
