#!/usr/bin/env python3
"""Permit the harmless RTTI parsing flag required by HL2SDK template bodies."""
from pathlib import Path
import re
import sys
for installation in sys.argv[1:]:
    path = Path(installation) / 'cfg/idaclang.cfg'
    source = path.read_text()
    def update(match):
        options = match[1].split()
        if '-frtti' not in options:
            options.append('-frtti')
        return 'CLANG_ARGV_PERMITTED_OPTIONS = "' + ' '.join(options) + '";'
    source, count = re.subn(r'CLANG_ARGV_PERMITTED_OPTIONS\s*=\s*"([^"]*)";', update, source)
    if count != 1:
        raise RuntimeError(f'Expected one CLANG_ARGV_PERMITTED_OPTIONS setting in {path}')
    path.write_text(source)
