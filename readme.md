# Leaving Earth

A simplified version of Joseph Fatula's [Leaving Earth](https://boardgamegeek.com/boardgame/173064/leaving-earth). Submission to HAGJ 13. Incomplete game.

Map style is inspired by: https://www.rmg.co.uk/stories/space-astronomy/origin-solar-system

## Screenshot

<img src="https://img.itch.zone/aW1hZ2UvNTIyOTQ2OC8zMTM0NTg5My5qcGc=/original/rEHESz.jpg">

## How to play:

- Research to unlock thrusters (rockets & Ion thruster)
- Buy thrusters and other components (Probe, Capsule, Astronaut, Supply, ...)
- Assemble them into a spacecraft
- Maneuver spacecraft to Earth's orbit and other planets
- Survey Earth's orbit & other planet to earn score (long-range survey, landing, human landing and sustaining human life for one year)

## Current limitation

Due to game jam's time limit, the game is incomplete:

- The game is not tested.
- Capsule capacity is not implemented

## Usage

Build WASM:

In SpaceRace.Browser

```shell
dotnet publish -c Release -p:CompressionEnabled=false
```

If `CompressionEnabled=false` is not used. `br` and `gz` would be created as well. `br` means `Brotli compression format` which is also used by Unity Webgl build.
