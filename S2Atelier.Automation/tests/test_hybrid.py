import json
from pathlib import Path
import re
import sys
import tempfile
import unittest
from unittest.mock import patch
sys.path.insert(0, str(Path(__file__).parents[1]))
import pipeline


class HybridTests(unittest.TestCase):
    def test_blacklist_download_and_cached_outputs(self):
        tracked = {str(d): ['regex:.*'] for d in (2347770, 2347771, 2347773, 2347779)}
        for host, blocked, allowed in [('windows', 'assetrename.dll', 'assetbrowser.dll'),
                                       ('linux', 'libassetrename.so', 'libassetsystem.so')]:
            regex = next(iter(pipeline.tracked_patterns(tracked, host).values()))[0][6:]
            for path in (blocked, 'game/bin/' + blocked, 'game/bin/' + blocked.upper()):
                self.assertIsNone(re.search(regex, path))
                self.assertTrue(pipeline.excluded_binary(path))
                self.assertTrue(pipeline.excluded_binary(path + '.i64.7z'))
            self.assertIsNotNone(re.search(regex, 'game/bin/' + allowed))
            self.assertFalse(pipeline.excluded_binary(allowed))

    def test_native_database_is_passed_to_wine_with_schema_project(self):
        with tempfile.TemporaryDirectory() as directory:
            root = Path(directory); jobdir = root/'job'
            binary = jobdir/'binaries/game/bin/win64/server.dll'
            binary.parent.mkdir(parents=True); binary.write_bytes(b'PE')
            (jobdir/'artifacts').mkdir()
            schema = jobdir/'dumps/dump/sdk.json'; schema.parent.mkdir(parents=True)
            schema.write_text(json.dumps({'classes':[{'project':'server'}], 'enums':[]}))
            calls = []
            def analyzer(root, job, jobdir, sdk, host, unique, image, input_path, args):
                calls.append((host, input_path.name, args))
                if host == 'linux':
                    self.assertEqual(args, [])
                    Path(str(input_path)+'.i64').write_bytes(b'native-database')
                else:
                    self.assertEqual(input_path.read_bytes(), b'native-database')
                    self.assertIn('--import-schema', args)
                    self.assertEqual(args[args.index('--schema-project')+1], 'server')
                    input_path.write_bytes(b'imported-database')
            def compress(archive, members, **kwargs):
                self.assertEqual(members[0].read_bytes(), b'imported-database')
                archive.write_bytes(b'7z')
            with patch.object(pipeline, 'run_analyzer', analyzer), patch.object(pipeline, 'compress', compress):
                records = pipeline.analyze_group(root, {'Id':'test', 'Request':{'ImportSchema':True}}, jobdir,
                    'windows', root/'sdk', {'containerImage':'wine','baseAnalysis':{'containerImage':'native'}}, [binary], [])
            self.assertEqual([(c[0], c[1]) for c in calls], [('linux','server.dll'), ('windows','server.dll.i64')])
            self.assertEqual(records[0]['analysisMode'], 'linux-base-wine-import')
            self.assertIn('baseAnalysisSeconds', records[0])
            self.assertIn('importSeconds', records[0])


if __name__ == '__main__': unittest.main()
