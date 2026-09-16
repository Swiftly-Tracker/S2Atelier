#!/usr/bin/env python3
"""Serialized jobs with bounded parallel binary analysis. All subprocess arguments are lists; never invoke a shell."""
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
from concurrent.futures import ThreadPoolExecutor, as_completed, CancelledError
from collections import defaultdict
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
    if (jobdir / 'dumps').exists() and not (jobdir / 'provenance.json').exists():
        shutil.rmtree(jobdir / 'dumps')
    if not (jobdir / 'dumps').exists():
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
    previous = []
    provenance_file = jobdir / 'provenance.json'
    if provenance_file.exists():
        old = json.loads(provenance_file.read_text())
        for key in ('dumpsCommit', 'hl2sdkCommit', 'analyzerSha256', 'downloaderSha256', 'containerImage'):
            if old.get(key) != provenance[key]:
                raise RuntimeError(f'Cannot resume with changed {key}; use a new job for changed inputs.')
        previous = old.get('artifacts', [])
        provenance['artifacts'] = previous.copy()
        if old.get('complete') and all((jobdir / 'artifacts' / name).is_file() and
                sha256(jobdir / 'artifacts' / name) == value for name, value in old['archiveHashes'].items()):
            return old
    atomic_json(provenance_file, provenance)
    downloads = jobdir / 'binaries'
    downloads.mkdir(exist_ok=True)
    filelist = jobdir / 'filelist.json'
    tracked = json.loads(git(dumps, 'show', revision + ':tracked_files.json'))
    (jobdir / 'tracked_files.json').write_text(json.dumps(tracked, indent=2))
    selections = tracked_patterns(tracked, platform, request.get('BinaryRegex'))
    provenance['depots'] = []
    for depot, patterns in selections.items():
        source = resolve_manifest(jobdir / 'dumps', platform, int(depot))
        provenance['depots'].append(source)
        filelist.write_text(json.dumps({depot: patterns}))
        auth = ['-username', os.environ['S2A_STEAM_USERNAME'], '-remember-password'] if os.environ.get('S2A_STEAM_USERNAME') else []
        run([root / 'tools/downloader/SteamDepotDownload.App', *auth, '-app', source['appId'], '-depot', depot,
             '-manifest', source['manifestId'], '-os', platform, '-osarch', '64', '-filelist', filelist, '-dir', downloads],
            stdin=subprocess.DEVNULL)
    atomic_json(provenance_file, provenance)
    binaries = sorted(p for p in downloads.rglob('*') if p.is_file() and '.sdd' not in p.parts and binary_platform(p) == platform)
    if not binaries:
        raise RuntimeError('No matching x64 binaries downloaded for requested platform.')
    binaries.sort(key=lambda p: (p.name.casefold(), str(p)))
    artifacts = jobdir / 'artifacts'
    artifacts.mkdir(exist_ok=True)
    groups = defaultdict(list)
    for binary in binaries:
        groups[binary.name.casefold()].append(binary)
    workers = analysis_workers()
    provenance['analysisWorkers'] = workers
    print(f'Analyzing {len(binaries)} {platform} binaries with {workers} parallel containers', flush=True)
    # A group owns its basename/archive; duplicate names cannot race during compression.
    with ThreadPoolExecutor(max_workers=workers) as executor:
        futures = {executor.submit(analyze_group, root, job, jobdir, platform, sdk, provenance,
                                   group, previous): name for name, group in groups.items()}
        errors = []
        for future in as_completed(futures):
            try:
                records = future.result()
                paths = {record['path'] for record in records}
                provenance['artifacts'] = [a for a in provenance['artifacts'] if a['path'] not in paths] + records
                provenance['artifacts'].sort(key=lambda a: a['path'])
                atomic_json(provenance_file, provenance)
            except CancelledError:
                pass
            except Exception as error:
                errors.append(f'{futures[future]}: {error}')
                for pending in futures:
                    pending.cancel()
        if errors:
            raise RuntimeError('Binary analysis failed; completed work retained: ' + '; '.join(errors))
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
    provenance['archiveHashes'] = {p.name: sha256(p) for p in artifacts.glob('*.7z')}
    provenance['complete'] = True
    atomic_json(provenance_file, provenance)
    return provenance


def atomic_json(path, value):
    temporary = path.with_suffix(path.suffix + '.tmp')
    temporary.write_text(json.dumps(value, indent=2))
    temporary.replace(path)


def analysis_workers():
    value = int(os.environ.get('S2A_ANALYSIS_WORKERS', '2'))
    if not 1 <= value <= 16:
        raise RuntimeError('S2A_ANALYSIS_WORKERS must be between 1 and 16')
    return value


def analyze_group(root, job, jobdir, platform, sdk, provenance, binaries, previous):
    artifacts = jobdir / 'artifacts'
    downloads = jobdir / 'binaries'
    archive = artifacts / (binaries[0].name + '.i64.7z')
    group_id = hashlib.sha256(binaries[0].name.casefold().encode()).hexdigest()[:16]
    checkpoint = jobdir / 'checkpoints' / (group_id + '.json')
    expected = {str((artifacts / p.relative_to(downloads)).relative_to(jobdir)) + '.i64': p for p in binaries}
    records = json.loads(checkpoint.read_text()) if checkpoint.exists() else [a for a in previous if a['path'] in expected]
    common_names = {'server', 'engine2', 'tier0'}
    def module(binary):
        name = binary.stem
        return name[3:] if platform == 'linux' and name.startswith('lib') else name
    if (len(records) == len(expected) and {r['path'] for r in records} == set(expected) and archive.is_file()
            and all(r.get('archive') == archive.name and r['binarySha256'] == sha256(expected[r['path']]) for r in records)):
        archive_hash = sha256(archive)
        if all(r.get('archiveSha256', archive_hash) == archive_hash for r in records) and all(
                module(expected[r['path']]) not in common_names or
                ((jobdir / r['path']).is_file() and sha256(jobdir / r['path']) == r['sha256']) for r in records):
            executable = shutil.which('7zz') or shutil.which('7z')
            run([executable, 't', archive], stdout=subprocess.DEVNULL)
            for record in records: record['archiveSha256'] = archive_hash
            print('Reusing verified archive: ' + archive.name, flush=True)
            return records
    records = []
    members = []
    for binary in binaries:
        project = module(binary)
        relative = binary.relative_to(downloads)
        unique = hashlib.sha256(str(relative).encode()).hexdigest()[:16]
        proto = jobdir / 'dumps/protobufs' / project
        generated = jobdir / 'generated' / unique
        generated.mkdir(parents=True, exist_ok=True)
        if proto.is_dir():
            for source in sorted(proto.rglob('*.proto')):
                run([root / 'tools/protobuf-build/protoc', '-I', proto, '-I', sdk / 'thirdparty/protobuf-3.21.8/src',
                     '--cpp_out=' + str(generated), source])
        output = artifacts / relative
        output.parent.mkdir(parents=True, exist_ok=True)
        # Only remove this unfinished module's IDA files; completed archives are separate.
        for suffix in ('.i64', '.i64.id0', '.i64.id1', '.i64.id2', '.i64.nam', '.i64.til', '.id0', '.id1', '.id2', '.nam', '.til'):
            Path(str(output) + suffix).unlink(missing_ok=True)
        shutil.copy2(binary, output)
        def mounted(path):
            return ('Z:' + str(path).replace('/', chr(92))) if platform == 'windows' else str(path)
        args = [output.name, '--root', mounted(output.parent),
                '--ida-path', 'Z:' + chr(92) + 'ida' if platform == 'windows' else '/ida', '--cores', '1',
                '--patch-plt', '--name-convars', '--name-fnptr-tables', '--import-interfaces', '--hl2sdk', mounted(sdk)]
        if any(generated.rglob('*.pb.h')):
            args += ['--import-protobufs', mounted(generated)]
        if job['Request']['ImportSchema']:
            args += ['--import-schema', mounted(jobdir / 'dumps/dump/sdk.json')]
        diagnostics = jobdir / 'diagnostics' / unique
        diagnostics.mkdir(parents=True, exist_ok=True)
        ida_state = jobdir / 'ida-state' / unique
        shutil.copytree(root / 'state' / platform, ida_state, dirs_exist_ok=True)
        command = ['docker', 'run', '--rm', '--init', '--name', f"s2a-{job['Id']}-{platform}-{unique}",
                   '--label', 's2atelier.job=' + job['Id'], '--network', 'none',
                   '--cpus', '2', '--memory', os.environ.get('S2A_ANALYSIS_MEMORY', '3g'), '--pids-limit', '256',
                   '-e', 'S2ATELIER_CLANG_RESOURCE_DIR=' + mounted(Path('/clang')),
                   '-v', f'{root}/tools/clang21:/clang:ro',
                   '-v', f'{diagnostics}:' + ('/wine/drive_c/users/root/AppData/Local/Temp' if platform == 'windows' else '/tmp'),
                   '-v', f'{jobdir}:{jobdir}', '-v', f'{sdk}:{sdk}:ro',
                   '-v', f'{root}/ida/{platform}:/ida:ro', '-v', f'{ida_state}:/root/.idapro',
                   '-v', f'{root}/tools/msvc:/msvc:ro', '-v', f'{root}/tools/atelier/{platform}:/atelier:ro',
                   provenance['containerImage']]
        run(command + args, stdin=subprocess.DEVNULL)
        database = Path(str(output) + '.i64')
        if not database.is_file() or database.stat().st_size == 0:
            raise RuntimeError(f'IDA reported success but did not produce {database}')
        records.append({'path': str(database.relative_to(jobdir)), 'binarySha256': sha256(binary),
                        'sha256': sha256(database), 'bytes': database.stat().st_size, 'archive': archive.name})
        members.append(database)
        output.unlink()
    compress(archive, members, base_dir=artifacts if len(members) > 1 else None)
    archive_hash = sha256(archive)
    for record in records: record['archiveSha256'] = archive_hash
    checkpoint.parent.mkdir(exist_ok=True)
    atomic_json(checkpoint, records)
    if module(binaries[0]) not in common_names:
        for member in members: member.unlink()
    return records


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
        atomic_json(jobdir / 'provenance.json', provenance)
    if request.get('PublishRelease', True):
        subject = git(dumps, 'show', '-s', '--format=%s', request['DumpsCommit'])
        archives = sorted(jobdir.glob('*/artifacts/*.7z'))
        receipt = publish(jobdir, request['DumpsCommit'], subject, archives)
        # Receipt is durable before any payload is removed; logs and hashes remain.
        atomic_json(jobdir / 'release.json', receipt)
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
