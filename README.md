# Match3 Maker

![license](https://badgen.net/static/License/MIT/yellow)
[![readme](https://badgen.net/static/README/📃/yellow)](https://github.com/ninetailsrabbit/Match3Maker/README.md)
![csharp](https://img.shields.io/badge/C%23-239120?style//for-the-badge&logo//c-sharp&logoColor//white)

This lightweight library provides the core logic and functionality you need to build engaging match-3 games. Focus on game design and mechanics while leaving the complex logic to this library.

---

<p align="center">
<img alt="Godot-XTension-Pack" src="Match3Maker/icon.png" width="250">
</p>

---

- [Match3 Maker](#match3-maker)
  - [Getting started](#getting-started)
    - [Requirements](#requirements)
    - [.csproj](#csproj)
    - [Installation via CLI](#installation-via-cli)
  - [Components](#components)
    - [GridCell](#gridcell)
    - [Piece](#piece)
    - [Sequence](#sequence)
    - [Board](#board)

## Getting started

This library empowers you to bring your match-3 game concepts to life quickly and efficiently. Don't get bogged down in the technical details - focus on what makes your game stand out!

### Requirements

**This package uses standard csharp library code and does not use third party libraries.**

- Net 8.0

### .csproj

Add the package directly into your .csproj

```xml


<ItemGroup>
## Latest stable release
  <PackageReference Include="Ninetailsrabbit.Match3Maker"/>

## Manual version
  <PackageReference Include="Ninetailsrabbit.Match3Maker" Version="1.0.0" />
</ItemGroup>
```

### Installation via CLI

Further information can be found on the [official microsoft documentation](https://learn.microsoft.com/en-us/nuget/consume-packages/install-use-packages-nuget-cli)

```sh
nuget install Ninetailsrabbit.Match3Maker

# Or choosing version

nuget install Ninetailsrabbit.Match3Maker -Version 1.0.0

# Using dotnet
dotnet add package Ninetailsrabbit.Match3Maker --version 1.0.0
```

## Components

The main logic has been simplified in few classes that represents each with a different functionality.

### GridCell

Each cell of the board can be accessed through this class. The position of the cells are immutable once the board is initialised so you can avoid common errors.

**You never actually create them yourself, it is recommended to initialise them from the Board class.**

```csharp
// public GridCell(int column, int row, Piece? piece = null, bool canContainPiece = true)
GridCell cell = new GridCell(2, 3);

// You have available the following properties
cell.Id // A unique GUID created for each new instance
cell.column // The column position
cell.Row // The row position
cell.CanContainPiece // Defines if can contain a piece
cell.Piece // The current Piece assigned to this cell

// Neighbours, you can access easily the nearest GridCells after initialize the board to the corresponding neighbour in the board. This improves performance as it only needs to be calculated once.

//Null is returned if the cell does not exist (for example, a bottom border cell does not have a neighbour bottom)
cell.NeighbourUp;
cell.NeighbourBottom;
cell.NeighbourRight;
cell.NeighbourLeft;
cell.DiagonalNeighbourTopRight;
cell.DiagonalNeighbourTopLeft;
cell.DiagonalNeighbourBottomRight;
cell.DiagonalNeighbourBottomLeft;


cell.Position(); // Vector2(3, 2) The position in the board where X is the row and Y is the column

// Shortcuts to detect the assigned piece
cell.IsEmpty();
cell.HasPiece();

// Assign a piece only if it's IsEmpty() and CanContainPiece is true
cell.AssignPiece(new Piece("triangle"));

// Remove and return the current assigned piece if exists or null.
cell.RemovePiece();

// Manual check if the pieces can be swapped between this cells.
cell.CanSwapPieceWith(GridCell otherCell);

// Swap the piece with the other cell if both have pieces and they are not locked (CanSwapPieceWith is called internally).
// SwappedPiece or SwapRejected events are raised whether it has been successful or not.
cell.SwapPieceWith(GridCell otherCell);

// Position detection
cell.InSameRowAs(GridCell otherCell);
cell.InSameColumnAs(GridCell otherCell);

// Can be use with other cell or a Vector2 position
cell.InSamePositionAs(GridCell otherCell);
cell.InSamePositionAs(Vector2 position);

cell.IsRowNeighbourOf(GridCell otherCell);
cell.IsColumnNeighbourOf(GridCell otherCell);

cell.IsAdjacentTo(GridCell otherCell);
cell.InDiagonalWith(GridCell otherCell);

// The IEquatable is implemented and two GridCells are equals if they are on the same position
public bool Equals(GridCell? other) {
    return InSamePositionAs(other);
}
```

### Piece

### Sequence

### Board
