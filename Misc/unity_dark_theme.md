# Dark theme

- open: `unity.exe`
- offset: `0000000000AE7020`
- find: `84 C0 75 08 33 C0 48 83 C4 20 5B C3 8B 03 48 83 C4 20 5B C3`
- `75` -> `74`

## Linux

```
Debugger: edb
Symbols: GetSkinIdx
74 04 41 8b 55 00
75
```
