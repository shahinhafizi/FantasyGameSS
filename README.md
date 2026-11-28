# FantasyGameSS – 2D Action Game in Godot

A 2D action game developed for the Advanced C# course using Godot Engine 4.7.2 (.NET) and .NET 8.0. The project features a state-machine-driven player controller (KnightController), an autonomous boss AI, and decoupled health logic (HealthLogic) verified with automated xUnit tests.

## Prerequisites
- Godot Engine 4.7.2 (.NET version): [https://godotengine.org/](https://godotengine.org/)
- .NET 8.0 SDK: [https://dotnet.microsoft.com/](https://dotnet.microsoft.com/)

## How to Build and Run the Game
1. Clone the repository or extract the project ZIP file:
   git clone [https://github.com/shahinhafizi/FantasyGameSS.git](https://github.com/shahinhafizi/FantasyGameSS.git)
2. Open Godot Engine 4.7.2 (.NET) and import project.godot from the root directory.
3. Click Build (the hammer icon in the top-right corner) to compile the C# solution.
4. Press F5 (or click the Play button) to run the game.

## How to Run Unit Tests
The core health and damage logic (HealthLogic) is decoupled from Godot and tested independently using xUnit in the FantasyGame.Tests project.

To run all 9 unit tests, open a terminal in the project root directory and run:
dotnet test

## Project Structure
- Scripts/ – C# scripts for the player state machine, boss AI, HealthLogic, and HealthComponent.
- FantasyGame.Tests/ – xUnit test project (HealthLogicTests.cs).
- level.tscn, player.tscn, boss.tscn – Godot scene files.