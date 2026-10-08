import os
from pathlib import Path
import sys
import tempfile
import threading
import unittest
from unittest.mock import patch
sys.path.insert(0, str(Path(__file__).parents[1]))
import scheduling as s
import pipeline
from concurrent.futures import ThreadPoolExecutor


class SchedulingTests(unittest.TestCase):
    def test_auto_respects_memory_and_affinity(self):
        with patch.dict(os.environ, {'S2A_ANALYSIS_WORKERS':'auto','S2A_MEMORY_RESERVE':'4g','S2A_WORKER_RESERVATION':'1g'}), patch.object(s,'available_memory',return_value=12*s.GIB), patch.object(s,'cpu_count',return_value=32):
            self.assertEqual(s.analysis_workers(),8)
        with patch.dict(os.environ, {'S2A_ANALYSIS_CPUS':'0'}):
            self.assertEqual(s.container_cpus(),[])
        with patch.dict(os.environ, {'S2A_ANALYSIS_CPUS':'2'}):
            self.assertEqual(s.container_cpus(),['--cpus','2.0'])

    def test_memory_released_after_failure_and_blocks_admission(self):
        with tempfile.TemporaryDirectory() as d, patch.dict(os.environ, {'S2A_MEMORY_RESERVE':'1g','S2A_WORKER_RESERVATION':'1g','S2A_ANALYSIS_MEMORY':'3g'}), patch.object(s,'available_memory',return_value=2*s.GIB):
            binary=Path(d)/'module'; binary.write_bytes(b'x')
            gate=s.MemoryAdmission()
            entered=threading.Event()
            waiting=threading.Event()
            def second():
                waiting.set()
                with gate.acquire([binary]): entered.set()
            with ThreadPoolExecutor(1) as pool:
                with self.assertRaisesRegex(RuntimeError,'failure'):
                    with gate.acquire([binary]):
                        future=pool.submit(second)
                        self.assertTrue(waiting.wait(2))
                        self.assertFalse(entered.wait(.1))
                        raise RuntimeError('failure')
                future.result(timeout=2)
            self.assertTrue(entered.is_set())
            self.assertEqual(gate.used,0)

    def test_history_then_size_order(self):
        with tempfile.TemporaryDirectory() as d:
            a=Path(d)/'a'; a.write_bytes(b'x')
            b=Path(d)/'b'; b.write_bytes(b'x'*10)
            self.assertEqual(sorted([[a],[b]],key=lambda g:s.group_priority(g,{}),reverse=True),[[b],[a]])
            self.assertEqual(sorted([[a],[b]],key=lambda g:s.group_priority(g,{'a':30}),reverse=True),[[a],[b]])

    def test_unseen_large_module_does_not_follow_every_known_module(self):
        with tempfile.TemporaryDirectory() as d:
            large=Path(d)/'large'; large.write_bytes(b'x'*(2*1024*1024))
            small=Path(d)/'small'; small.write_bytes(b'x')
            self.assertGreater(s.group_priority([large],{'small':1}),s.group_priority([small],{'small':1}))

    def test_platforms_overlap_and_share_one_pool(self):
        with tempfile.TemporaryDirectory() as d:
            root=Path(d); job=root/'job.json'
            job.write_text('{"Request":{"DumpsCommit":"'+'a'*40+'","Platform":"all","PublishRelease":false}}')
            barrier=threading.Barrier(2); pools=[]; gates=[]
            def analyze(root,job,jobdir,platform,sdk,dumps,pool,gate):
                pools.append(pool); gates.append(gate)
                barrier.wait(timeout=2)
                return pool.submit(lambda:{'platform':platform}).result(timeout=2)
            with patch.object(pipeline,'sync',return_value=root), patch.object(pipeline,'analyze',side_effect=analyze), patch.object(pipeline,'analysis_workers',return_value=2):
                pipeline.pipeline(root,job)
            self.assertIs(pools[0],pools[1]); self.assertIs(gates[0],gates[1])
            self.assertTrue((root/'provenance.json').is_file())


if __name__=='__main__': unittest.main()
