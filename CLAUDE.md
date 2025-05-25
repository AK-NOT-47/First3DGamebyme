# First3DGamebyme - Project Context

## Project Overview
- **Name**: First3DGamebyme
- **Type**: 3D game built with MonoGame/C#
- **Repository**: https://github.com/AK-NOT-47/First3DGamebyme.git
- **Location**: /home/ashraf/First3DGamebyme/
- **Description**: My first 3D game using MonoGame framework

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

## Development Setup
- **Framework**: MonoGame 3.8.3
- **.NET SDK**: 8.0 (installed at ~/.dotnet)
- **Platform**: Cross-platform desktop (Windows/Linux/macOS)
- **Build**: `dotnet build`
- **Run**: `dotnet run`

## 3D Game Development Notes
- MonoGame uses XNA-style APIs for 3D rendering
- Key classes for 3D: BasicEffect, Model, Matrix, Vector3
- Content pipeline processes 3D models (.fbx, .x, .obj)
- Uses right-handed coordinate system

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

## TODO for 3D Setup
- [ ] Set up basic 3D camera system
- [ ] Implement input handling for camera controls
- [ ] Add basic 3D primitives (cube, sphere)
- [ ] Set up lighting (ambient, directional)
- [ ] Implement basic physics/collision
- [ ] Add 3D model loading support

This file will be automatically read by Claude in new sessions when working in this project directory.