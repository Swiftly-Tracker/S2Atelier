#!/usr/bin/env python3
"""Serialized job runner. All subprocess arguments are lists; never invoke a shell."""
import hashlib
import json
import os
import re
from pathlib import Path
import shutil
import subprocess
import sys
import tarfile
import traceback
from release import compress, publish, cleanup_payload, check_publish_access


def run(args, **kwargs):
    print('+', ' '.join(map(str, args)), flush=True)
    return subprocess.run(list(map(str, args)), check=True, **kwargs)


def git(repo, *args):
    return subprocess.check_output(['git', '-C', str(repo), *args], text=True).strip()


def sync(root, name, url, branch):
    repo = root / 'sources' / name
    if not (repo / '.git').exists():
        run(['git', 'clone', '--single-branch', '--branch', branch, url, repo])
    run(['git', '-C', repo, 'pull', '--ff-only'])
    run(['git', '-C', repo, 'submodule', 'update', '--init', '--recursive'])
    return repo


def snapshot(repo, revision, target):
    target.mkdir(parents=True)
    archive = target.parent / (target.name + '.tar')
    run(['git', '-C', repo, 'archive', '-o', archive, revision, 'protobufs', 'dump', 'manifests'])
    with tarfile.open(archive) as source:
        source.extractall(target, filter='data')
    archive.unlink()


def sha256(path):
    with path.open('rb') as f:
        digest = hashlib.sha256()
        for chunk in iter(lambda: f.read(1024 * 1024), b''):
            digest.update(chunk)
        return digest.hexdigest()


def binary_platform(path):
    with path.open('rb') as f:
        header = f.read(64)
        if header[:5] == b'\x7fELF\x02' and header[18:20] == b'\x3e\x00':
            return 'linux'
        if header[:2] == b'MZ' and len(header) == 64:
            f.seek(int.from_bytes(header[60:64], 'little'))
            if f.read(6) == b'PE\x00\x00\x64\x86':
                return 'windows'
    return None


def resolve_manifest(snapshot_dir, platform, depot=None):
    """Resolve only from the archived commit, never from the mutable checkout."""
    target_depot = depot or {'windows': 2347771, 'linux': 2347773}[platform]
    candidates = []
    for path in sorted((snapshot_dir / 'manifests').glob('*.txt')):
        content = path.read_text(encoding='utf-8-sig')
        revision = re.search(r'^SourceRevision\s*:\s*([0-9]+)\s*$', content, re.M)
        if revision is None:
            raise RuntimeError(f'Missing SourceRevision in {path.name}')
        candidates.append((int(revision[1]), path, content))
    if not candidates:
        raise RuntimeError('No manifest files found in the specified Dumps commit.')
    newest = max(item[0] for item in candidates)
    resolved = []
    for revision, path, content in candidates:
        if revision != newest:
            continue
        apps = re.findall(r'^App\s*ID\s*:\s*([0-9]+)\s*$', content, re.M | re.I)
        if not apps or set(apps) != {'730'}:
            raise RuntimeError(f'Expected CS2 app 730 in {path.name}')
        entries = re.split(r'^Content Manifest for Depot\s+([0-9]+)\s*$', content, flags=re.M)
        matches = []
        for depot, section in zip(entries[1::2], entries[2::2]):
            if int(depot) != target_depot:
                continue
            ids = re.findall(r'^Manifest ID / date\s*:\s*([0-9]+)\s*/', section, re.M)
            if len(ids) != 1 or not 0 < int(ids[0]) <= 2**64 - 1:
                raise RuntimeError(f'Invalid manifest ID for depot {depot} in {path.name}')
            matches.append((int(depot), ids[0]))
        if len(matches) != 1:
            raise RuntimeError(f'Expected one {platform} depot in {path.name}')
        resolved.append((matches[0], path))
    if len({entry for entry, _ in resolved}) != 1:
        raise RuntimeError('Conflicting manifest files for the latest SourceRevision in this commit.')
    (depot, manifest), path = resolved[-1]
    return {'appId': 730, 'depotId': depot, 'manifestId': manifest,
            'manifestFile': str(path.relative_to(snapshot_dir)), 'sourceRevision': newest}


def analyze(root, job, jobdir, platform, sdk, dumps):
    request = job['Request']
    jobdir.mkdir(exist_ok=True)
    revision = request['DumpsCommit']
    if not revision or not re.fullmatch(r'[0-9a-fA-F]{40}', revision):
        raise RuntimeError('A full dumpsCommit SHA is required; legacy requests must be resubmitted.')
    revision = git(dumps, 'rev-parse', '--verify', revision + '^{commit}')
    snapshot(dumps, revision, jobdir / 'dumps')
    resolved = resolve_manifest(jobdir / 'dumps', platform)
    manifest, depot = resolved['manifestId'], resolved['depotId']
    print('Resolved manifest:', json.dumps(resolved), flush=True)
    provenance = {**resolved, 'platform': platform,
                  'hl2sdkCommit': git(sdk, 'rev-parse', 'HEAD'), 'dumpsCommit': revision,
                  'idaSdk': '9.4',
                  'analyzerSha256': sha256(root / 'tools/atelier' / platform / ('S2Atelier.exe' if platform == 'windows' else 'S2Atelier')),
                  'downloaderSha256': sha256(root / 'tools/downloader/SteamDepotDownload.App'),
                  'containerImage': subprocess.check_output(['docker', 'image', 'inspect', '--format', '{{.Id}}', 's2atelier-' + platform + ':local'], text=True).strip(),
                  'artifacts': []}
    (jobdir / 'provenance.json').write_text(json.dumps(provenance, indent=2))
    downloads = jobdir / 'binaries'
    downloads.mkdir()
    filelist = jobdir / 'filelist.json'
    tracked = json.loads(git(dumps, 'show', revision + ':tracked_files.json'))
    (jobdir / 'tracked_files.json').write_text(json.dumps(tracked, indent=2))
    selections = tracked_patterns(tracked, platform, request.get('BinaryRegex'))
    provenance['depots'] = []
    for depot, patterns in selections.items():
        source = resolve_manifest(jobdir / 'dumps', platform, int(depot))
        provenance['depots'].append(source)
        filelist.write_text(json.dumps({depot: patterns}))
        run([root / 'tools/downloader/SteamDepotDownload.App', '-app', source['appId'], '-depot', depot,
             '-manifest', source['manifestId'], '-os', platform, '-osarch', '64', '-filelist', filelist, '-dir', downloads],
            stdin=subprocess.DEVNULL)
    (jobdir / 'provenance.json').write_text(json.dumps(provenance, indent=2))
    binaries = sorted(p for p in downloads.rglob('*') if p.is_file() and '.sdd' not in p.parts and binary_platform(p) == platform)
    if not binaries:
        raise RuntimeError('No matching x64 binaries downloaded for requested platform.')
    binaries.sort(key=lambda p: (p.name.casefold(), str(p)))
    protoc = root / 'tools/protobuf-build/protoc'
    artifacts = jobdir / 'artifacts'
    artifacts.mkdir()
    for index, binary in enumerate(binaries):
        project = binary.stem
        if platform == 'linux' and project.startswith('lib'):
            project = project[3:]
        proto = jobdir / 'dumps/protobufs' / project
        generated = jobdir / 'generated' / project
        generated.mkdir(parents=True, exist_ok=True)
        if proto.is_dir():
            # Compile separately per module; different binaries may carry conflicting messages.
            for source in sorted(proto.rglob('*.proto')):
                run([protoc, '-I', proto, '-I', sdk / 'thirdparty/protobuf-3.21.8/src',
                     '--cpp_out=' + str(generated), source])
        relative = binary.relative_to(downloads)
        output = artifacts / relative
        output.parent.mkdir(parents=True, exist_ok=True)
        shutil.copy2(binary, output)
        def mounted(path):
            # Same absolute mount inside both containers; Wine maps Unix / to Z:\.
            return ('Z:' + str(path).replace('/', '\\')) if platform == 'windows' else str(path)
        args = [output.name, '--root', mounted(output.parent),
                '--ida-path', 'Z:\\ida' if platform == 'windows' else '/ida', '--cores', '1',
                '--patch-plt', '--name-convars', '--name-fnptr-tables', '--import-interfaces', '--hl2sdk', mounted(sdk)]
        if any(generated.rglob('*.pb.h')):
            args += ['--import-protobufs', mounted(generated)]
        if request['ImportSchema']:
            args += ['--import-schema', mounted(jobdir / 'dumps/dump/sdk.json')]
        diagnostics = jobdir / 'diagnostics'
        diagnostics.mkdir(exist_ok=True)
        command = ['docker', 'run', '--rm', '--init', '--name', f"s2a-{job['Id']}-{platform}", '--network', 'none',
                   '--cpus', '2', '--memory', '4g', '--pids-limit', '256',
                   '-e', 'S2ATELIER_CLANG_RESOURCE_DIR=' + mounted(Path('/clang')),
                   '-v', f'{root}/tools/clang21:/clang:ro',
                   '-v', f'{diagnostics}:' + ('/wine/drive_c/users/root/AppData/Local/Temp' if platform == 'windows' else '/tmp'),
                   '-v', f'{jobdir}:{jobdir}', '-v', f'{sdk}:{sdk}:ro',
                   '-v', f'{root}/ida/{platform}:/ida:ro',
                   '-v', f'{root}/state/{platform}:/root/.idapro',
                   '-v', f'{root}/tools/msvc:/msvc:ro',
                   '-v', f'{root}/tools/atelier/{platform}:/atelier:ro',
                   provenance['containerImage']]
        run(command + args, stdin=subprocess.DEVNULL)
        database = Path(str(output) + '.i64')
        if not database.is_file() or database.stat().st_size == 0:
            raise RuntimeError(f'IDA reported success but did not produce {database}')
        provenance['artifacts'].append({'path': str(database.relative_to(jobdir)), 'binarySha256': sha256(binary),
                                        'sha256': sha256(database), 'bytes': database.stat().st_size})
        (jobdir / 'provenance.json').write_text(json.dumps(provenance, indent=2))
        archive = artifacts / (database.name + '.7z')
        # Same basename in multiple tracked directories shares one asset, retaining paths.
        group_complete = index + 1 == len(binaries) or binaries[index + 1].name.casefold() != binary.name.casefold()
        members = sorted(artifacts.rglob(database.name))
        if group_complete:
            compress(archive, members, base_dir=artifacts if len(members) > 1 else None)
        provenance['artifacts'][-1]['archive'] = archive.name
        (jobdir / 'provenance.json').write_text(json.dumps(provenance, indent=2))
        output.unlink()
        if group_complete and project not in ('server', 'engine2', 'tier0'):
            for member in members:
                member.unlink()
    common = []
    for module in ('server', 'engine2', 'tier0'):
        name = ('lib' + module + '.so' if platform == 'linux' else module + '.dll') + '.i64'
        distro = 'linuxsteamrt64' if platform == 'linux' else 'win64'
        canonical = artifacts / ('game/csgo/bin' if module == 'server' else 'game/bin') / distro / name
        found = [canonical] if canonical.is_file() else list(artifacts.rglob(name))
        if len(found) != 1:
            if request.get('PublishRelease', True):
                raise RuntimeError(f'Missing common bundle member: {name}')
            break
        common.append(found[0])
    else:
        compress(artifacts / ('common-' + platform + '.7z'), common)
        for database in common:
            database.unlink()
    return provenance


def tracked_patterns(tracked, platform, override=None):
    # Keep upstream regex semantics, intersected with native binary extensions.
    # Shared depots can contain Windows tools/DLLs as well as platform depots.
    depots = ('2347770', '2347771', '2347779') if platform == 'windows' else ('2347770', '2347773', '2347779')
    extension = r'.*\.(dll|exe)$' if platform == 'windows' else r'.*\.so$'
    result = {}
    for depot in depots:
        patterns = [entry[6:] for entry in tracked.get(depot, []) if entry.startswith('regex:')]
        if not patterns:
            raise RuntimeError(f'No tracked regex for depot {depot}')
        result[depot] = ['regex:^(?=' + extension + ')' +
                         ('(?=.*(?:' + override + '))' if override else '') +
                         '(?:' + '|'.join('(?:' + p + ')' for p in patterns) + ')']
    return result


def pipeline(root, jobfile):
    job = json.loads(jobfile.read_text())
    request = job['Request']
    jobdir = jobfile.parent
    if request.get('PublishRelease', True):
        check_publish_access()
    sdk = sync(root, 'hl2sdk', 'https://github.com/alliedmodders/hl2sdk.git', 'cs2')
    dumps = sync(root, 'CS2-Dumps', 'https://github.com/Swiftly-Tracker/CS2-Dumps.git', 'main')
    platforms = ('windows', 'linux') if request['Platform'] == 'all' else (request['Platform'],)
    provenance = {'dumpsCommit': request['DumpsCommit'], 'platforms': []}
    for platform in platforms:
        provenance['platforms'].append(analyze(root, job, jobdir / platform, platform, sdk, dumps))
        (jobdir / 'provenance.json').write_text(json.dumps(provenance, indent=2))
    if request.get('PublishRelease', True):
        subject = git(dumps, 'show', '-s', '--format=%s', request['DumpsCommit'])
        archives = sorted(jobdir.glob('*/artifacts/*.7z'))
        receipt = publish(jobdir, request['DumpsCommit'], subject, archives)
        # Receipt is durable before any payload is removed; logs and hashes remain.
        (jobdir / 'release.json').write_text(json.dumps(receipt, indent=2))
        cleanup_payload(jobdir)



if __name__ == '__main__':
    root, jobfile = map(Path, sys.argv[1:])
    with (jobfile.parent / 'pipeline.log').open('a', buffering=1) as log:
        os.dup2(log.fileno(), 1)
        os.dup2(log.fileno(), 2)
        try:
            pipeline(root, jobfile)
        except Exception:
            traceback.print_exc()
            sys.exit(1)
