# Third party notices

Thumbico bundles the third party material listed below. Everything else in this repository is covered by [LICENSE](LICENSE).

## Fluent UI System Icons

`Source/Thumbico/Assets/Thumbico.Icons.ttf` is a subset of Microsoft's Fluent UI System Icons, cut down to the nineteen glyphs the interface uses. The full font is 1.53 MB; the subset is 4 KB.

- Upstream: <https://github.com/microsoft/fluentui-system-icons>
- Source file: `fonts/FluentSystemIcons-Resizable.ttf`
- Release: 1.1.333, commit `d4819e48891824acab2b6ab5077cbc52885ae4a7`
- License: MIT, reproduced in full below

### Regenerating the subset

The subset is committed rather than built, so a clone needs no font tooling. Regenerate it only when an icon is added or removed. The code points come from the upstream `fonts/FluentSystemIcons-Resizable.json` map, never from a glyph picker, because that map is what names each icon.

```
pip install fonttools
pyftsubset FluentSystemIcons-Resizable.ttf --unicodes=<comma separated U+XXXX list> --output-file=Thumbico.Icons.ttf
```

The current list is the set of `const string` values in `Source/Thumbico/Glyphs.cs`, which records the upstream icon name beside each code point.

### License

MIT License

Copyright (c) 2020 Microsoft Corporation

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
