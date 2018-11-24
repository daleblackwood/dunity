# dunity

My grab bag of Unity bits I keep reusing between projects. C#, built on Unity 2018.

Drop the folder into `Assets/` and go.

## What's in it

- **AI** – state and behaviour AI, waypoint followers (2D and 3D) and a platformer sensor so baddies don't walk off ledges.
- **Actor** – the player and NPC base: movement, head and neck look-at, shadows, item holding and pickups, spawners.
- **Behave** – a tiny behaviour tree.
- **Cam** – follow and orbit cameras, plus "camboxes" that swap camera setups as you walk between zones.
- **Triggers** – stack-them-together triggers for puzzles and level scripting: switches, dependant triggers, movers, talk scripts and map markers.
- **Game** – game state, factions and teams, time and sequence managers, and a dev mode.
- **Input** – input manager with device mapping.
- **Entity** – pushable, fireable and collectable things, plus basic attacks.
- **FX**, **Audio**, **UI** – particles and dust, a world curve effect, sound and music players, and simple screen management.
- **Data** – save data.
- **Utils** – the usual helpers, world paths, terrain tools and a randomiser.

Most components come with a custom inspector in their `Editor` folders.

## Credits

`Utils/SimpleJSON` is by Bunny83 (MIT, see its LICENSE).
