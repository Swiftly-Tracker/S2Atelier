#!/usr/bin/env python3
"""Initialize the Linux IDA EULA registry state using the official HCLI convention.
Run once during an authorized unattended installation, with LD_LIBRARY_PATH set
and two arguments: IDA installation directory and persistent IDA user directory.
"""
import ctypes
import os
from pathlib import Path
import sys

installation, state = map(lambda p: Path(p).resolve(), sys.argv[1:])
state.mkdir(parents=True, exist_ok=True)
os.environ['IDAUSR'] = str(state)
os.environ['IDADIR'] = str(installation)
kernel = ctypes.CDLL(str(installation / 'libida.so'), mode=ctypes.RTLD_GLOBAL)
library = ctypes.CDLL(str(installation / 'libidalib.so'))
library.init_library.argtypes = [ctypes.c_int, ctypes.c_void_p]
library.init_library.restype = ctypes.c_int
if library.init_library(0, None) != 0:
    raise RuntimeError('Cannot initialize IDA; check installation and license.')
kernel.reg_int_op.argtypes = [ctypes.c_char_p, ctypes.c_bool, ctypes.c_int, ctypes.c_char_p]
kernel.reg_int_op.restype = ctypes.c_int
for version in range(90, 95):
    kernel.reg_int_op(f'EULA {version}'.encode(), True, 1, None)
