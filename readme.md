# Leaving Earth

Digitation of Joseph Fatula's [Leaving Earth](https://boardgamegeek.com/boardgame/173064/leaving-earth). Submission to HAGJ 13.

Map style is inspired by: https://www.rmg.co.uk/stories/space-astronomy/origin-solar-system

## Usage

Build WASM:

In SpaceRace.Browser

```shell
dotnet publish -c Release -p:CompressionEnabled=false
```

If `CompressionEnabled=false` is not used. `br` and `gz` would be created as well. `br` means `Brotli compression format` which is also used by Unity Webgl build.
