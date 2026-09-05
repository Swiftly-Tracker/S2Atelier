# Third Party Notices

## IDA SDK

`thirdparty/ida-sdk/` holds one directory per vendored SDK, named after its `major.minor` line.
Each contains headers vendored verbatim from the Hex-Rays IDA SDK under `include/`, and a `VERSION`
file naming the exact release. These headers are the input to `S2Atelier.Ida.Codegen`, which
generates the P/Invoke bindings in `S2Atelier.Ida/Generated/`.

- [GitHub Repository](https://github.com/HexRaysSA/ida-sdk)
- **Vendored versions**:
  - `9.2` — [`9.2.0-sdk.1`](https://github.com/HexRaysSA/ida-sdk/tree/v9.2.0-sdk.1)
  - `9.3` — [`9.3.0-sdk.3`](https://github.com/HexRaysSA/ida-sdk/tree/v9.3.0-sdk.3)

```
MIT License

Copyright (c) 2025 Hex-Rays SA

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
```

## PltPatcher

`S2Atelier.Ida/PltPatcher.cs` is a C# port of `plt_patcher.py`'s algorithm, driven through this
project's own generated bindings instead of idapython. The original's `add_extern_entry` fallback
(creating a brand-new extern stub via `add_func_ex` when a name is found nowhere in the database)
is not ported — that needs a correctly laid out `func_t` argument, and this project only trusts
struct layouts it has verified field-by-field against the header, which `func_t`'s full layout has
not been. An import that cannot be found anywhere is reported as unresolved instead.

- [GitHub Repository](https://github.com/GAMMACASE/PltPatcher)
- **Author**: GAMMACASE

```
MIT License

Copyright (c) 2025 GAMMACASE

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
```
