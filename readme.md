Build WASM:

In SpaceRace.Browser

```shell
dotnet publish -c Release -p:CompressionEnabled=false
```

If `CompressionEnabled=false` is not used. `br` and `gz` would be created as well. `br` means `Brotli compression format` which is also used by Unity Webgl build.