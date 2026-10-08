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
import urllib.error
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
        (self.root / 'release-target.json').write_text(json.dumps({'tag':'cs2-test-unique','marker':'<!-- test-publication -->'}))

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
        return dict(id=123, draft=self.draft, body='<!-- test-publication -->', html_url='https://github.com/test/release', upload_url='https://uploads.github.com/test{?name}')

    def test_unique_tag_collision_and_retry_identity(self):
        # A persisted candidate belongs to somebody else; allocate a new identity.
        created = {}
        def api(method, url, payload=None, path=None):
            if method == 'GET':
                if url.endswith('/cs2-test-unique'):
                    return {'id': 1, 'body': 'another publication'}
                if '/releases/tags/' in url and created:
                    return created
                raise urllib.error.HTTPError(url, 404, 'missing', {}, None)
            self.assertEqual(method, 'POST')
            self.assertEqual(url, release.API + '/releases')
            created.update(payload, id=2)
            return created
        with patch.object(release, 'api', api):
            tag, _ = release.release_target(self.root, 'a'*40, 'build')
            self.assertTrue(tag.startswith('cs2-' + 'a'*40 + '-'))
            self.assertNotEqual(tag, 'cs2-test-unique')
            retry_tag, retry = release.release_target(self.root, 'a'*40, 'build')
            self.assertEqual(tag, retry_tag)
            self.assertEqual(retry['id'], 2)

    def test_existing_bare_tag_is_skipped(self):
        (self.root / 'release-target.json').unlink()
        tags = []
        def api(method, url, payload=None, path=None):
            if '/releases/tags/' in url:
                tags.append(url.rsplit('/', 1)[-1])
                raise urllib.error.HTTPError(url, 404, 'missing', {}, None)
            if '/git/ref/tags/' in url:
                if len(tags) == 1:
                    return {'ref': 'occupied'}
                raise urllib.error.HTTPError(url, 404, 'missing', {}, None)
            return {'id': 2, **payload}
        with patch.object(release, 'api', api):
            tag, _ = release.release_target(self.root, 'a'*40, 'build')
        self.assertEqual(len(tags), 2)
        self.assertNotEqual(tags[0], tags[1])
        self.assertEqual(tag, tags[1])

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

    def test_fast_compression_options_and_validation(self):
        with patch.dict(os.environ, {'S2A_COMPRESSION_LEVEL':'3','S2A_COMPRESSION_THREADS':'4'}):
            self.assertEqual(release.compression_options(), ['-mx=3','-m0=lzma2','-md=32m','-mfb=64','-ms=on','-mmt=4'])
        for value in ('0','10','invalid'):
            with patch.dict(os.environ, {'S2A_COMPRESSION_LEVEL':value}), self.assertRaises(ValueError):
                release.compression_options()

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

    def test_notes_report_disagreements_and_regressions(self):
        reports = [('windows', {'Module': 'server.dll', 'Warnings': ['[vtable-drift] CFoo: SDK names held.'],
                                'Regressions': []}),
                   ('linux', {'Module': 'libserver.so', 'Warnings': [],
                              'Regressions': ['entity-classes.classes: 516 -> 12 (-97%)']})]
        notes = release.release_notes(reports, {'windows': 'cs2-' + 'b'*40, 'linux': None})
        self.assertIn('Compared with windows `cs2-' + 'b'*40 + '`.', notes)
        self.assertIn('- `server.dll` (windows): [vtable-drift] CFoo: SDK names held.', notes)
        self.assertIn('### Pass regressions', notes)
        self.assertIn('- `libserver.so` (linux): entity-classes.classes: 516 -> 12 (-97%)', notes)
        clean = release.release_notes([('windows', {'Module': 'server.dll', 'Warnings': [], 'Regressions': []})], {})
        self.assertIn('No previous release to compare with.', clean)
        self.assertIn('No SDK disagreements', clean)
        long = release.release_notes([('windows', {'Module': 'm', 'Warnings': ['x' * 1000] * 200, 'Regressions': []})], {})
        self.assertLessEqual(len(long), release.MAX_NOTES + 100)
        self.assertTrue(long.endswith('full report.)'))

    def test_notes_become_the_published_body(self):
        bodies = []
        def api(method, url, payload=None, path=None):
            if method == 'PATCH':
                bodies.append(payload.get('body'))
            return self.api(method, url, payload, path)
        with patch.object(release, 'api', api):
            release.publish(self.root, 'a'*40, 'build', [self.archive], '## Analysis report')
        self.assertIn('## Analysis report', bodies[-1])
        self.assertTrue(bodies[-1].endswith('<!-- test-publication -->'))

    def test_baseline_is_the_newest_other_published_release(self):
        asset = {'name': 'snapshots-windows.7z', 'url': 'https://api.github.com/assets/1'}
        listing = [dict(tag_name='cs2-' + 'c'*40, draft=True, assets=[asset]),
                   dict(tag_name='cs2-' + 'a'*40, draft=False, assets=[asset]),
                   dict(tag_name='cs2-' + 'd'*40, draft=False, assets=[]),
                   dict(tag_name='cs2-' + 'e'*40, draft=False, assets=[asset])]
        with patch.object(release, 'api', return_value=listing):
            self.assertEqual(release.previous_release('cs2-' + 'a'*40, 'snapshots-windows.7z'),
                             ('cs2-' + 'e'*40, asset))
            self.assertIsNone(release.previous_release('cs2-' + 'e'*40, 'snapshots-linux.7z'))

    def test_local_allowlist_and_platform_intersection(self):
        tracked = pipeline.load_tracked_files()
        windows = pipeline.tracked_patterns(tracked, 'windows')
        linux = pipeline.tracked_patterns(tracked, 'linux')
        self.assertEqual(set(windows), {'2347771'})
        self.assertEqual(set(linux), {'2347773'})
        def matches(patterns, path):
            return pipeline.selected_binary(Path('/downloads') / path, Path('/downloads'), patterns)
        self.assertTrue(matches(windows, 'game/bin/win64/engine2.dll'))
        self.assertTrue(matches(windows, 'game/csgo/bin/win64/server.dll'))
        for path in ('game/bin/win64/tool.exe', 'game/bin/win64/unlisted.dll', 'game/pak_dir.vpk', 'game/bin/win64/assetrename.dll'):
            self.assertFalse(matches(windows, path))
        self.assertTrue(matches(linux, 'game/bin/linuxsteamrt64/libtier0.so'))
        self.assertFalse(matches(linux, 'game/bin/linuxsteamrt64/libunlisted.so'))
        narrowed = pipeline.tracked_patterns(tracked, 'windows', 'server')
        self.assertTrue(matches(narrowed, 'game/csgo/bin/win64/server.dll'))
        self.assertFalse(matches(narrowed, 'game/bin/win64/engine2.dll'))


if __name__ == '__main__': unittest.main()
