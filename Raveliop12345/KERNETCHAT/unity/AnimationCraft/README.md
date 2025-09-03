# AnimationCraft (MVP)

Unity 2022.3 LTS (URP) sandbox voxel game in creative mode. Solo TPS, infinite world streamed by chunks with greedy meshing. Target: 60 FPS @ 1080p on Intel UHD 620.

## Requirements
- Unity 2022.3 LTS (any 2022.3.x)
- URP enabled
- Packages: Burst, Collections, Mathematics, Input System, URP

## Setup
1. Open the project folder `unity/AnimationCraft` with Unity 2022.3 LTS.
2. On first open, the Editor auto-setup will:
   - Create URP Global pipeline and assign it
   - Create default scenes: `Assets/Scenes/Menu.unity` and `Assets/Scenes/Main.unity`
   - Create material `Assets/Materials/Chunk.mat`
   - Switch Active Input Handling to Input System, set Linear color space, vsync off, targetFrameRate 60
3. Press Play on `Menu.unity`.

## Controls
- Move: WASD
- Look: Mouse
- Up: Space
- Down: Left Ctrl
- Break block: Left Mouse Button
- Place block: Right Mouse Button
- Cycle block: Mouse Wheel
- Toggle debug overlay: F3

## World
- Chunk size: 16×16×128, default view radius 6 (configurable at runtime)
- Base blocks: Air(0), Bedrock(1), Stone(2), Dirt(3), Grass(4), Sand(5)
- Colors: flat vertex colors per block
- Generation: 2D heightmap with FastNoiseLite (seeded)

## Save Data
- Path: `Saves/ANIMATION_CRAFT/<worldId>/chunks/`
- `world.json`: { version, seed, createdAt, lastPlayed }
- Chunk files: `x_y_z.bin`, little-endian, header + RLE block payload

## Limitations (MVP)
- No global/dynamic lighting, no water, no mobs, no multiplayer, basic biomes only
- Single material per chunk, greedy meshing per face

## Project Structure
```
Assets/
  Scenes/ (Menu, Main)
  Scripts/
    Core/
    Voxel/
    Chunks/
    Meshing/
    World/
    Generation/
    Interaction/
    Save/
    Input/
  Materials/
  Settings/
  Plugins/FastNoiseLite/
  Editor/
```

## License
MIT
