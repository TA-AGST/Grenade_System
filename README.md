# Grenade System

A first person grenade system made in Unity (URP) with four grenade types, a cook-and-throw mechanic and a predicted throw arc.

> **Note:** Logic build mein AI ki help li gayi hai.

## Controls
| Key | Action |
|---|---|
| W A S D | Move |
| Mouse | Look |
| Space | Jump |
| 1 / 2 / 3 / 4 | Select Frag / Flash / Smoke / Fire |
| Hold left mouse button | Pull the pin (fuse starts) and aim |
| Release left mouse button | Throw |
| Q | Switch short / long throw |
| F1 | Toggle explosion visibility debug |
| R | Restart the scene |
| Esc | Free the cursor |

## Features
- **Four grenade types** (Frag, Flash, Smoke, Fire), each a `GrenadeData` ScriptableObject, so fuse time and ammo live in data, not in code.
- **Cooking:** the fuse runs while you hold the button. Hold too long and the grenade explodes in your hand.
- **Trajectory line** that previews the throw using the same custom physics as the grenade (`GrenadePhysics`).
- **Line of sight damage:** the frag grenade checks how much of the target is exposed, so cover reduces the damage.
- Flash blinds the screen, smoke blocks the view, fire leaves a damaging zone.
- Explosions push the player, health bars over dummies, death screen with restart.

## Run it
1. Unity **6000.3.25f1** (Universal 3D / URP).
2. Open `Assets/Scenes/SampleScene.unity` and press Play.
