# First3DGamebyme - Project Context

## IMPORTANT: Update this file after every development session!
This file should be updated after each chat/development session to track progress and maintain context for future sessions.

## Project Overview
- **Name**: First3DGamebyme (Cult of the Lamb Replica)
- **Type**: 2D action roguelike + cult management game built with MonoGame/C#
- **Repository**: https://github.com/AK-NOT-47/First3DGamebyme.git
- **Location**: /home/ashraf/First3DGamebyme/
- **Description**: A replica of Cult of the Lamb featuring combat, procedural dungeons, and cult management

## Git Configuration
- **Username**: AK-NOT-47
- **Email**: Ashraf.mh.khalifa@gmail.com
- **Authentication**: Personal Access Token (already configured in ~/.git-credentials)
- **Default Branch**: main

## Project Structure
- Main game code: `First3DGamebyme/`
- Entry point: `First3DGamebyme/Program.cs`
- Game logic: `First3DGamebyme/Game1.cs`
- Content pipeline: `First3DGamebyme/Content/`
- Engine systems: `First3DGamebyme/Engine/`
  - Camera: 2D camera with zoom and follow
  - Input: Unified input handling for keyboard/mouse/gamepad
  - Animation: Sprite animation system (ready for use)
- Entities: `First3DGamebyme/Entities/`
  - Entity: Base class for all game objects
  - Player: Player character with combat and movement
  - Enemy: Base enemy class with AI states
  - BasicEnemy: Simple enemy implementation
- Combat: `First3DGamebyme/Systems/Combat/`
  - Weapon: Weapon definitions and properties
  - Attack: Attack hitbox and collision detection

## Development Setup
- **Framework**: MonoGame 3.8.3
- **.NET SDK**: 8.0 (installed at ~/.dotnet)
- **Platform**: Cross-platform desktop (Windows/Linux/macOS)
- **Build**: `dotnet build`
- **Run**: `dotnet run`

## Game Controls
- **Movement**: WASD or Arrow keys
- **Attack**: Left mouse click
- **Dodge**: Space bar
- **Camera Zoom**: Q (zoom in), E (zoom out)
- **Exit**: Escape

## Current Features (Implemented)
- ✅ 2D rendering system with SpriteBatch
- ✅ Camera system with smooth follow and zoom
- ✅ Input management (keyboard, mouse, gamepad support)
- ✅ Player character with movement and rotation
- ✅ Dodge roll mechanic with cooldown
- ✅ Advanced combat system:
  - ✅ Combo attack system with timing windows
  - ✅ Weapon weight and swing momentum
  - ✅ Critical hits with damage variation
  - ✅ Enemy knockback and hitstun
  - ✅ Screen shake and hit pause effects
  - ✅ Damage numbers with visual feedback
  - ✅ Hit particles and effects
  - ✅ Perfect dodge mechanics with slow motion
  - ✅ Extended attack range (150 units)
  - ✅ Special visual effects for attacks:
    - ✅ Multi-layered swing trails
    - ✅ Energy particles on combos/crits
    - ✅ Impact waves on final combo hits
- ✅ Attack hitboxes with visual feedback
- ✅ Enemy AI with states (Idle, Chase, Attack, Hurt)
- ✅ Health and damage systems
- ✅ Enemy spawning and targeting
- ✅ UI system:
  - ✅ Combo counter display
  - ✅ Player health bar
  - ✅ Pixelated damage numbers
- ✅ Game Over system:
  - ✅ Death animation with particles
  - ✅ Game over screen with restart
  - ✅ Press R to restart, ESC to exit

## Git Workflow
- Always pull before starting work: `git pull`
- Check status frequently: `git status`
- Stage changes: `git add .`
- Commit with descriptive messages: `git commit -m "Add feature: ..."`
- Push to GitHub: `git push origin main`

## Important Commands
```bash
# Navigate to project
cd /home/ashraf/First3DGamebyme

# Build the project
dotnet build

# Run the game (from Windows for graphics)
dotnet run

# Git operations
git status
git add .
git commit -m "message"
git push
```

## TODO - Next Features to Implement

### High Priority
- [ ] Procedural dungeon generation system
  - Room templates and connections
  - Door/exit system
  - Room types (combat, treasure, boss)
- [ ] More enemy types
  - Ranged enemies
  - Heavy/tank enemies
  - Flying enemies
- [ ] Weapon variety and upgrades
  - Different weapon types (axe, hammer, dagger)
  - Weapon stats and modifiers
  - Combo attacks

### Medium Priority
- [ ] Cult base/hub area
  - Base layout and buildings
  - Building placement system
  - Decorations and customization
- [ ] Follower system
  - Follower NPCs with names and traits
  - Needs system (hunger, faith, sleep)
  - Devotion mechanics
  - Work assignments
- [ ] Resource gathering and crafting
  - Resource nodes in dungeons
  - Crafting stations
  - Recipe system
- [ ] UI/HUD system
  - Health bar
  - Resource counters
  - Minimap
  - Inventory

### Low Priority
- [ ] Ritual and sermon mechanics
  - Ritual circle and animations
  - Sermon podium
  - Faith generation
- [ ] Save/load system
  - Game state serialization
  - Multiple save slots
- [ ] Sound and music
  - Sound effects for actions
  - Background music
  - Ambient sounds

## Development Notes
- Using placeholder colored squares for sprites currently
- Attack visualization shows swing arc with weapon weight effects
- Combat features:
  - 3-hit combo system for swords, 4-hit for daggers
  - Different weapon types have unique combo patterns
  - Critical hits show golden damage numbers
  - Perfect dodge window triggers slow motion
  - Hit particles spawn on impact
  - Enemies flash red when hurt
  - Screen shake scales with damage
  - Attack range significantly extended (150 units)
  - Special effects include trails, particles, and impact waves
- Enemy AI uses state machine with hurt state for knockback
- Camera follows player with lerp and screen shake offset
- All entity collision uses Rectangle bounds
- UI rendered with pixel art style numbers
- Death system:
  - Player spins and shrinks during death animation
  - Red particles emit during death
  - Game over screen with pixelated text
  - Full game restart on R press

## Known Issues
- Build may fail due to MonoGame content pipeline on Linux (can ignore for now)
- Need to implement proper sprite loading system
- Attack hit detection could be more precise with polygon collisions

## Recent Session Updates (25/5/2025)
- Implemented comprehensive combat system with game feel improvements
- Added combo attacks, critical hits, and special visual effects
- Created game over screen with restart functionality
- Extended attack range and added particle effects
- Improved enemy reactions with knockback and hitstun

This file will be automatically read by Claude in new sessions when working in this project directory.