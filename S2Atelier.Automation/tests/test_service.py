#!/usr/bin/env python3
"""Real HTTP/durable queue tests with a controlled pipeline, no Steam/IDA required.
Usage: python3 test_service.py /path/to/build/or/publish/output
"""
import json
import os
from pathlib import Path
import shutil
import socket
import subprocess
import sys
import tempfile
import time
import urllib.error
import urllib.request

with tempfile.TemporaryDirectory(prefix='s2a-http-') as directory:
    root = Path(directory)
    app = root / 'app'
    shutil.copytree(sys.argv[1], app)
    (app / 'pipeline.py').write_text('''import json, pathlib, sys, time
j = pathlib.Path(sys.argv[2]); data = json.loads(j.read_text())
(j.parent / 'pipeline.log').write_text('test pipeline')
if data['Request']['DumpsCommit'] == 'f' * 40: sys.exit(7)
time.sleep(0.25)
a = j.parent / 'artifacts'; a.mkdir(); (a / 'test.dll.i64.7z').write_bytes(b'test-database')
if data['Request']['PublishRelease']:
    (j.parent / 'release.json').write_text(json.dumps({'assets':[{'url':'https://github.com/Swiftly-Tracker/CS2-IDA-Dumps/releases/download/test/test.dll.i64.7z'}]}))
''')
    with socket.socket() as listener:
        listener.bind(('127.0.0.1', 0))
        port = listener.getsockname()[1]
    base = f'http://127.0.0.1:{port}'
    env = dict(os.environ, S2A_ROOT=str(root), S2A_API_KEY='a' * 32, ASPNETCORE_URLS=base)
    output = (root / 'host.log').open('w')
    def start():
        command = [str(app / 'S2Atelier.Automation')] if (app / 'S2Atelier.Automation').exists() else ['dotnet', str(app / 'S2Atelier.Automation.dll')]
        p = subprocess.Popen(command, env=env, stdout=output, stderr=output)
        for _ in range(100):
            if p.poll() is not None:
                raise RuntimeError((root / 'host.log').read_text())
            try:
                with urllib.request.urlopen(base + '/health', timeout=1): return p
            except OSError: time.sleep(.1)
        p.terminate()
        raise TimeoutError('Service did not start')
    def call(path, data=None, authenticated=True):
        request = urllib.request.Request(base + path, data=json.dumps(data).encode() if data is not None else None,
            headers={'Content-Type': 'application/json', **({'Authorization': 'Bearer ' + 'a' * 32} if authenticated else {})})
        try:
            with urllib.request.urlopen(request, timeout=5) as response: return response.status, response.read()
        except urllib.error.HTTPError as response: return response.code, response.read()
    def wait_job(id):
        for _ in range(100):
            data = json.loads(call('/jobs/' + id)[1])
            if data['state'] in ('failed', 'succeeded'): return data
            time.sleep(.1)
        raise TimeoutError(id)
    request = dict(dumpsCommit='a' * 40, binaryRegex=r'(^|/)test\.dll$', platform='windows', publishRelease=False)
    process = start()
    try:
        assert call('/jobs', authenticated=False)[0] == 401
        assert call('/jobs', {**request, 'manifestId': '123'})[0] == 400
        assert call('/jobs', {**request, 'depotId': 2347773})[0] == 400
        assert call('/jobs', {**request, 'appId': 730})[0] == 400
        assert call('/jobs', {k: v for k, v in request.items() if k != 'dumpsCommit'})[0] == 400
        assert call('/jobs', {**request, 'binaryRegex': '('})[0] == 400
        assert call('/jobs', {**request, 'binaryRegex': 'x\nregex:.*'})[0] == 400
        assert call('/jobs', {**request, 'platform': '../linux'})[0] == 400
        assert call('/jobs', {**request, 'dumpsCommit': '--help'})[0] == 400
        assert call('/jobs', {k: v for k, v in request.items() if k != 'platform'})[0] == 400
        assert call('/jobs', {**request, 'publishRelease': True})[0] == 400
        code, data = call('/jobs', {**request, 'platform': 'linux'})
        assert code == 202 and json.loads(data)['request']['platform'] == 'linux'
        status, data = call('/jobs', request)
        assert status == 202
        id = json.loads(data)['id']
        assert wait_job(id)['state'] == 'succeeded'
        assert call(f'/jobs/{id}/artifacts/0')[1] == b'test-database'
        assert call(f'/jobs/{id}/artifacts/-1')[0] == 404
        assert call(f'/jobs/{id}/artifacts/1')[0] == 404
        failed = json.loads(call('/jobs', {**request, 'dumpsCommit': 'f' * 40})[1])['id']
        assert wait_job(failed)['state'] == 'failed'
        assert call(f'/jobs/{failed}/artifacts/0')[0] == 404
        publish = {'dumpsCommit': 'e' * 40, 'platform': 'all'}
        code, data = call('/jobs', publish)
        assert code == 202, data
        published_id = json.loads(data)['id']
        assert json.loads(call('/jobs', publish)[1])['id'] == published_id
        published = wait_job(published_id)
        assert published['state'] == 'succeeded', published
        assert published['artifacts'][0].startswith('https://github.com/')
        assert json.loads(call('/jobs', publish)[1])['id'] == published_id
        retry_request = {'dumpsCommit': 'f' * 40, 'platform': 'all'}
        retry_id = json.loads(call('/jobs', retry_request)[1])['id']
        assert wait_job(retry_id)['state'] == 'failed'
        assert json.loads(call('/jobs', retry_request)[1])['id'] == retry_id
        assert wait_job(retry_id)['state'] == 'failed'
        process.terminate(); process.wait(timeout=20)
        # Simulate a machine crash leaving one queued and one running job.
        stored = json.loads((root / 'jobs' / id / 'job.json').read_text())
        for extra_id, state in [('b' * 32, 'queued'), ('c' * 32, 'running')]:
            path = root / 'jobs' / extra_id; path.mkdir()
            (path / 'job.json').write_text(json.dumps({**stored, 'Id': extra_id, 'State': state, 'Files': None}))
        legacy_path = root / 'jobs' / ('d' * 32); legacy_path.mkdir()
        legacy_request = dict(DepotId=2347773, ManifestId='123', BinaryRegex='test', Platform='linux', AppId=730)
        (legacy_path / 'job.json').write_text(json.dumps({**stored, 'Id': 'd' * 32, 'State': 'queued', 'Request': legacy_request}))
        process = start()
        assert 'format changed' in json.loads(call('/jobs/' + 'd' * 32)[1])['error']
        assert json.loads(call('/jobs/' + id)[1])['state'] == 'succeeded'
        assert wait_job('b' * 32)['state'] == 'succeeded'
        interrupted = json.loads(call('/jobs/' + 'c' * 32)[1])
        assert interrupted['state'] == 'failed' and 'interrupted' in interrupted['error']
        print('PASS HTTP auth, validation, jobs, artifacts, failures and restart recovery')
    finally:
        if process.poll() is None: process.terminate(); process.wait(timeout=20)
        output.close()
