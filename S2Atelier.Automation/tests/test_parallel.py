#!/usr/bin/env python3
"""Exercise the real scheduler/checkpoint flow with a controlled analyzer process."""
import json
import os
from pathlib import Path
import sys
import tempfile
import threading
import time
import unittest
from unittest.mock import patch
sys.path.insert(0, str(Path(__file__).parents[1]))
import pipeline


class ParallelTests(unittest.TestCase):
    def test_parallel_isolation_and_verified_resume(self):
        with tempfile.TemporaryDirectory() as directory:
            root = Path(directory)
            jobdir = root / 'job/linux'; jobdir.mkdir(parents=True)
            downloads = jobdir / 'binaries'; downloads.mkdir()
            for name in ('liba.so', 'libb.so'):
                (downloads / name).write_bytes(name.encode())
            for name in ('tools/atelier/linux/S2Atelier', 'tools/downloader/SteamDepotDownload.App'):
                path = root / name; path.parent.mkdir(parents=True, exist_ok=True); path.write_bytes(b'tool')
            (root / 'state/linux').mkdir(parents=True)
            tracked = {str(i): ['regex:.+?\\.so$'] for i in (2347770, 2347773, 2347779)}
            job = {'Id': 'a'*32, 'Request': {'DumpsCommit': 'b'*40, 'PublishRelease': False, 'ImportSchema': False}}
            barrier = threading.Barrier(2)
            commands = []
            synchronize = [True]
            def run(args, **kwargs):
                if args[0] != 'docker': return
                commands.append(args)
                if synchronize[0]: barrier.wait(timeout=3)
                output = Path(args[args.index('--root') + 1]) / args[args.index('test-image') + 1]
                Path(str(output) + '.i64').write_bytes(output.name.encode() * 20)
            def compress(archive, files, **kwargs):
                archive.write_bytes(b''.join(p.read_bytes() for p in files))
            def git(repo, *args):
                return json.dumps(tracked) if args[0] == 'show' else 'b'*40
            def snapshot(repo, revision, target): target.mkdir()
            with patch.dict(os.environ, {'S2A_ANALYSIS_WORKERS': '2'}), \
                 patch.object(pipeline, 'git', git), patch.object(pipeline, 'snapshot', snapshot), \
                 patch.object(pipeline, 'resolve_manifest', return_value={'appId':730, 'depotId':2347773, 'manifestId':'123'}), \
                 patch.object(pipeline, 'binary_platform', side_effect=lambda p: 'linux' if p.suffix == '.so' else None), \
                 patch.object(pipeline.subprocess, 'check_output', return_value='test-image'), \
                 patch.object(pipeline, 'run', run), patch.object(pipeline, 'compress', compress):
                first = pipeline.analyze(root, job, jobdir, 'linux', root/'sdk', root/'dumps')
                self.assertEqual(len(first['artifacts']), 2)
                self.assertEqual(len(commands), 2)
                self.assertEqual(len({c[c.index('--name') + 1] for c in commands}), 2)
                self.assertTrue(all('s2atelier.job=' + job['Id'] in c for c in commands))
                states = [next(str(v) for v in c if str(v).endswith(':/root/.idapro')) for c in commands]
                self.assertEqual(len(set(states)), 2)
                self.assertTrue(all(a.get('archiveSha256') for a in first['artifacts']))
                synchronize[0] = False
                commands.clear()
                second = pipeline.analyze(root, job, jobdir, 'linux', root/'sdk', root/'dumps')
                self.assertEqual(second, first)
                self.assertEqual(commands, [])
                # Corrupt one archive: rerun only that module, reusing the other checkpoint.
                (jobdir / 'artifacts/liba.so.i64.7z').write_bytes(b'corrupted')
                third = pipeline.analyze(root, job, jobdir, 'linux', root/'sdk', root/'dumps')
                self.assertEqual(len(commands), 1)
                self.assertEqual(commands[0][commands[0].index('test-image') + 1], 'liba.so')
                self.assertEqual(third['archiveHashes'], first['archiveHashes'])

    def test_worker_limit(self):
        for value in ('0', '17', 'invalid'):
            with patch.dict(os.environ, {'S2A_ANALYSIS_WORKERS': value}), self.assertRaises((ValueError, RuntimeError)):
                pipeline.analysis_workers()


if __name__ == '__main__': unittest.main()
