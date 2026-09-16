#!/usr/bin/env python3
"""Regression tests for commit isolation and CS2 manifest selection."""
import importlib.util
from pathlib import Path
import subprocess
import tempfile
import sys
sys.path.insert(0, str(Path(__file__).parents[1]))
import unittest

spec = importlib.util.spec_from_file_location('pipeline', Path(__file__).parents[1] / 'pipeline.py')
pipeline = importlib.util.module_from_spec(spec)
spec.loader.exec_module(pipeline)


def manifest(revision, windows, linux):
    return f'''AppId : 730
App ID : 730
SourceRevision : {revision}
Content Manifest for Depot 2347771
Manifest ID / date : {windows} / 2026-09-09
Content Manifest for Depot 2347773
Manifest ID / date : {linux} / 2026-09-09
'''


class ManifestTests(unittest.TestCase):
    def setUp(self):
        self.temp = tempfile.TemporaryDirectory()
        self.addCleanup(self.temp.cleanup)
        self.root = Path(self.temp.name)
        (self.root / 'manifests').mkdir()

    def write(self, name, content):
        (self.root / 'manifests' / name).write_text(content)

    def test_latest_source_revision_and_both_platforms(self):
        self.write('z-old.txt', manifest(9, 10, 11))
        self.write('a-new.txt', manifest(10, 5806169188224907599, 8639120305802825922))
        for platform, depot, value in [('windows', 2347771, '5806169188224907599'), ('linux', 2347773, '8639120305802825922')]:
            result = pipeline.resolve_manifest(self.root, platform)
            self.assertEqual((result['depotId'], result['manifestId']), (depot, value))
            self.assertEqual(result['manifestFile'], 'manifests/a-new.txt')

    def test_missing_invalid_or_conflicting_data_fails(self):
        for content in ['', manifest(10, 2, 0), manifest(10, 2, 2**64), manifest(10, 2, 3).replace('2347773', '2347779'), manifest(10, 2, 3).replace('730', '740')]:
            self.write('a.txt', content)
            with self.assertRaises(RuntimeError): pipeline.resolve_manifest(self.root, 'linux')
        self.write('a.txt', manifest(10, 2, 3))
        self.write('b.txt', manifest(10, 2, 4))
        with self.assertRaises(RuntimeError): pipeline.resolve_manifest(self.root, 'linux')

    def test_identical_duplicate_manifest_is_allowed(self):
        for name in ['a.txt', 'b.txt']: self.write(name, manifest(10, 2, 3))
        self.assertEqual(pipeline.resolve_manifest(self.root, 'linux')['manifestId'], '3')

    def test_archive_uses_requested_commit_not_checkout(self):
        def git(*args):
            return subprocess.check_output(['git', '-C', str(self.root), *args], text=True).strip()
        git('init', '-q')
        git('config', 'user.email', 'test@example.invalid'); git('config', 'user.name', 'Test')
        for name in ['dump', 'protobufs']:
            (self.root / name).mkdir(); (self.root / name / '.keep').touch()
        self.write('old.txt', manifest(9, 10, 11))
        git('add', '.'); git('commit', '-qm', 'old')
        revision = git('rev-parse', 'HEAD')
        self.write('new.txt', manifest(10, 20, 21))
        git('add', '.'); git('commit', '-qm', 'new')
        target = self.root / 'snapshot'
        pipeline.snapshot(self.root, revision, target)
        self.assertEqual(pipeline.resolve_manifest(target, 'linux')['manifestId'], '11')
        self.assertEqual(pipeline.resolve_manifest(self.root, 'linux')['manifestId'], '21')


if __name__ == '__main__': unittest.main()
