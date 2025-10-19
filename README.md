# The Last Commander

The final project of Unity module.

---

## Overview

**The Last Commander** is a tower defend game. You act the last commander of the castle, rally your army and fight the evil skeletons.

## Control

- WASD ---------- Move
- Space ---------- Jump
- Lshift ---------- Sprint
- L Click ---------- Attack/Build Army
- E ---------- Open Menu
- Esc ---------- Cancel

## Game Mechanics

Players can recruit army using gold, and killing enemies will drop potions and gold. Armys can be placed on the map to block enemy paths or deal damage. If the enemy has no path to the castle, they will target the army to attempt to open a path. Players who hold out against enough waves of enemy attacks win; if the enemy captures the castle, player lose.

## Army List

### Dog Knight
- HP: 150
- Damage: 8
- Size: Single
- Cost: 50
- Can block the path
- High attack speed

### Saw Blade
- HP: 0
- Damage: 10
- Size: Double
- Cost: 150
- Will not be attacked

### Cannon
- HP: 80
- Damage: 15
- Size: 2x2
- Cost: 300
- AOE attack, the best damage dealer

## Known Errors
- Sometimes the enemy's material will not return to its original state from hited material.
- There are two conflict bricks which shared same corrdnates.
- When specific brick is blocked, the enemy may be unable to find the path.

## Fulture Plan
- More SE & Particle effects
- Fix bugs, ofcurse
- Better GUI
- More type of enemies / armies
- Bigger map
- Optimize path calculation

## Others
Any possible feedback is welcome!

PS: I really don’t want to write any technical details here (plz check the project), because this is just a prototype, so the code is messy as sh*t... And dev log? Check commit history plz :3.