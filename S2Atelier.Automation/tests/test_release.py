#!/usr/bin/env python3
import importlib.util
import json
import os
from pathlib import Path
import re
import sys
import shutil
import subprocess
import tempfile
import unittest
from unittest.mock import patch
sys.path.insert(0, str(Path(__file__).parents[1]))
import pipeline
import release


class ReleaseTests(unittest.TestCase):
    def setUp(self):
        self.temp = tempfile.TemporaryDirectory()
        self.addCleanup(self.temp.cleanup)
        self.root = Path(self.temp.name)
        self.archive = self.root / 'server.dll.i64.7z'
        self.archive.write_bytes(b'archive')
        (self.root / 'provenance.json').write_text('{}')
        self.draft = True
        self.remote = {}
        self.events = []

    def api(self, method, url, payload=None, path=None):
        self.events.append((method, url))
        if '/assets?' in url:
            return list(self.remote.values())
        if path:
            self.remote[path.name] = dict(name=path.name, state='uploaded', size=path.stat().st_size,
                digest='sha256:' + release.digest(path), browser_download_url='https://github.com/test/' + path.name)
            return self.remote[path.name]
        if method == 'PATCH':
            self.draft = payload['draft']
        return dict(id=123, draft=self.draft, html_url='https://github.com/test/release', upload_url='https://uploads.github.com/test{?name}')

    @unittest.skipUnless(shutil.which('7z') or shutil.which('7zz'), '7-Zip unavailable')
    def test_real_archive_flat_common_and_duplicate_paths(self):
        for name in ('server.dll.i64', 'engine2.dll.i64', 'tier0.dll.i64'):
            (self.root / name).write_bytes((name * 100).encode())
        files = list(self.root.glob('*.i64'))
        archive = self.root / 'common-windows.7z'
        release.compress(archive, files)
        output = self.root / 'extracted'
        exe = shutil.which('7zz') or shutil.which('7z')
        subprocess.run([exe, 'x', str(archive), '-o' + str(output)], check=True, stdout=subprocess.DEVNULL)
        self.assertEqual({p.name for p in output.iterdir()}, {p.name for p in files})
        for path in files: self.assertEqual(path.read_bytes(), (output / path.name).read_bytes())
        nested = self.root / 'other'; nested.mkdir()
        duplicate = nested / files[0].name; duplicate.write_bytes(b'different')
        release.compress(self.root / 'duplicate.7z', [files[0], duplicate], base_dir=self.root)
        subprocess.run([exe, 'x', str(self.root / 'duplicate.7z'), '-o' + str(self.root / 'duplicates')],
                       check=True, stdout=subprocess.DEVNULL)
        self.assertEqual((self.root / 'duplicates/other' / duplicate.name).read_bytes(), b'different')

    def test_publish_access_fails_before_analysis(self):
        with patch.object(release, 'api', return_value={'permissions': {'push': False}}):
            with self.assertRaisesRegex(RuntimeError, 'write access'):
                release.check_publish_access()
        with patch.object(release, 'api', return_value={'permissions': {'push': True}}):
            release.check_publish_access()

    def test_title(self):
        self.assertEqual(release.release_title('25218825 - 1.41.8.1 | 3 modified | Sep 09 2026 15:23:58'),
                         '25218825 - 1.41.8.1 | Sep 09 2026 15:23:58')

    def test_publish_verifies_then_cleanup_keeps_metadata(self):
        (self.root / 'windows').mkdir()
        (self.root / 'windows' / 'binary').write_bytes(b'payload')
        with patch.object(release, 'api', self.api):
            receipt = release.publish(self.root, 'a'*40, 'build | 3 modified | date', [self.archive])
        self.assertFalse(self.draft)
        self.assertEqual(len(receipt['assets']), 2)
        with self.assertRaises(FileNotFoundError):
            release.cleanup_payload(self.root)
        (self.root / 'release.json').write_text(json.dumps(receipt))
        release.cleanup_payload(self.root)
        self.assertFalse((self.root / 'windows').exists())
        self.assertTrue((self.root / 'provenance.json').exists())

    def test_failed_digest_never_publishes_or_cleans(self):
        def api(*args, **kwargs):
            result = self.api(*args, **kwargs)
            if isinstance(result, list):
                for asset in result: asset['digest'] = 'sha256:wrong'
            return result
        with patch.object(release, 'api', api), self.assertRaises(RuntimeError):
            release.publish(self.root, 'a'*40, 'build', [self.archive])
        self.assertTrue(self.draft)
        self.assertTrue(self.archive.exists())
        self.assertFalse(any(method == 'PATCH' for method, _ in self.events))

    def test_published_release_is_not_overwritten(self):
        self.draft = False
        with patch.object(release, 'api', self.api), self.assertRaises(RuntimeError):
            release.publish(self.root, 'a'*40, 'build', [self.archive])
        self.assertFalse(any(method in ('DELETE', 'POST') for method, _ in self.events))

    def test_upstream_regex_and_platform_intersection(self):
        tracked = json.loads((Path(__file__).parent / 'tracked_files.fixture.json').read_text())
        windows = pipeline.tracked_patterns(tracked, 'windows')
        linux = pipeline.tracked_patterns(tracked, 'linux')
        def matches(patterns, depot, path):
            return any(re.search(p[6:], path, re.I) for p in patterns[depot])
        self.assertTrue(matches(windows, '2347771', 'game/bin/win64/engine2.dll'))
        self.assertTrue(matches(windows, '2347779', 'game/bin/win64/tool.exe'))
        self.assertFalse(matches(windows, '2347779', 'game/import_scripts/bin/tool.dll'))
        self.assertFalse(matches(windows, '2347770', 'game/pak_dir.vpk'))
        self.assertFalse(matches(windows, '2347770', 'game/image.jpg'))
        self.assertTrue(matches(linux, '2347773', 'game/bin/linuxsteamrt64/libtier0.so'))
        self.assertFalse(matches(linux, '2347773', 'game/start.sh'))


if __name__ == '__main__': unittest.main()
