"""Host-wide admission shared by both platforms; IDA remains process isolated."""
from contextlib import contextmanager
import math
import os
from pathlib import Path
import re
import threading

GIB = 1024 ** 3


def memory_bytes(value):
    match = re.fullmatch(r'(\d+(?:\.\d+)?)\s*([kmgt]?)(?:i?b)?', str(value).strip().lower())
    if not match:
        raise ValueError('Invalid memory size: ' + str(value))
    result = int(float(match[1]) * 1024 ** ('kmgt'.find(match[2]) + 1 if match[2] else 0))
    if result <= 0:
        raise ValueError('Memory size must be positive')
    return result


def available_memory():
    for line in Path('/proc/meminfo').read_text().splitlines():
        if line.startswith('MemAvailable:'):
            return int(line.split()[1]) * 1024
    raise RuntimeError('Cannot determine available host memory')


def cpu_count():
    return len(os.sched_getaffinity(0)) if hasattr(os, 'sched_getaffinity') else (os.cpu_count() or 1)


def analysis_workers():
    value = os.environ.get('S2A_ANALYSIS_WORKERS', 'auto')
    if value.lower() == 'auto':
        reserve = memory_bytes(os.environ.get('S2A_MEMORY_RESERVE', '4g'))
        minimum = memory_bytes(os.environ.get('S2A_WORKER_RESERVATION', '768m'))
        return max(1, min(64, cpu_count(), (available_memory() - reserve) // minimum))
    count = int(value)
    if not 1 <= count <= 64:
        raise RuntimeError('S2A_ANALYSIS_WORKERS must be auto or between 1 and 64')
    return count


def container_cpus():
    value = float(os.environ.get('S2A_ANALYSIS_CPUS', '0'))
    if not math.isfinite(value) or value < 0:
        raise ValueError('S2A_ANALYSIS_CPUS must be zero (unlimited) or positive')
    return ['--cpus', str(value)] if value else []


class MemoryAdmission:
    def __init__(self):
        self.reserve = memory_bytes(os.environ.get('S2A_MEMORY_RESERVE', '4g'))
        self.minimum = memory_bytes(os.environ.get('S2A_WORKER_RESERVATION', '768m'))
        self.limit = memory_bytes(os.environ.get('S2A_ANALYSIS_MEMORY', '3g'))
        self.budget = max(self.minimum, available_memory() - self.reserve)
        self.used = 0
        self.condition = threading.Condition()

    def estimate(self, binaries):
        # Each group analyzes its members sequentially. Large modules reserve more.
        return min(self.budget, self.limit, max(self.minimum, max(p.stat().st_size for p in binaries) * 12))

    @contextmanager
    def acquire(self, binaries):
        amount = self.estimate(binaries)
        with self.condition:
            while self.used + amount > self.budget or available_memory() < self.reserve + amount:
                self.condition.wait(timeout=1)
            self.used += amount
        try:
            yield
        finally:
            with self.condition:
                self.used -= amount
                self.condition.notify_all()


def group_priority(group, history):
    # Measured duration wins; size is a fallback for unseen modules.
    return (sum(history.get(p.name.casefold(), p.stat().st_size / (1024 ** 2) * 5) for p in group),
            sum(p.stat().st_size for p in group))
