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
      - [Creating new piece types](#creating-new-piece-types)
      - [Creating one piece](#creating-one-piece)
    - [Sequence](#sequence)
    - [Board](#board)
      - [Creating a new board](#creating-a-new-board)

## Getting started

This library empowers you to bring your match-3 game concepts to life quickly and efficiently. Don't get bogged down in the technical details - focus on what makes your game stand out!

**_It not handle any visual components, it contains the logic for you to use it in your project and link the corresponding UI components_**

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

**_You never actually create them yourself, it is recommended to initialise them from the Board class._**

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

A core Piece that manage most of the logic when it comes to interact with. It can be easily extended providing your own types so you're free to decide which one match and which one not. Are intended to handle the internal logic and not its display as a UI.

#### Creating new piece types

To start adding valid pieces into the board, you need to implement the `IPieceType` interface in your class

```csharp
 public interface IPieceType {
     public string Shape {  get; set; }
     public Color? Color { get; set; }
     public bool MatchWith(Piece piece);
     public bool CanBeShuffled();
     public bool CanBeMoved();
 }

```

This library provides three types by default _(Normal, Special, Obstacle)_ that you can use in your match-3 game but nothing stops you to create your own custom ones.

```csharp
  public class NormalPieceType : IPieceType {
      public string Shape { get => _shape; set => _shape = value; }
      public Color? Color { get => _color; set => _color = value; }

      private string _shape;
      private Color? _color = null;

      public NormalPieceType(string shape, Color? color = null) {
          Shape = shape;
          Color = color;
      }

      public bool MatchWith(Piece piece) {
        return piece.Type is not ObstaclePieceType
            && Shape.Trim().Equals(piece.Type.Shape, StringComparison.OrdinalIgnoreCase)
            && Color.Equals(piece.Type.Color);
      }

      public bool CanBeShuffled() => true;
      public bool CanBeMoved() => true;
  }
```

```csharp

    public class SpecialPieceType : IPieceType {
        public string Shape { get => _shape; set => _shape = value; }
        public Color? Color { get => _color; set => _color = value; }

        private string _shape;
        private Color? _color = null;

        public SpecialPieceType(string shape, Color? color = null) {
            Shape = shape;
            Color = color;
        }

        public bool MatchWith(Piece piece) {

            return typeof(SpecialPieceType).IsAssignableFrom(piece.Type.GetType())
                && Shape.Trim().Equals(piece.Type.Shape, StringComparison.OrdinalIgnoreCase)
                && Color.Equals(piece.Type.Color);
        }

        public bool CanBeShuffled() => true;
        public bool CanBeMoved() => true;
    }
```

```csharp

using System.Drawing;

namespace Match3Maker {
    public class ObstaclePieceType : IPieceType {
        public string Shape { get => _shape; set => _shape = value; }
        public Color? Color { get => _color; set => _color = value; }

        private string _shape;
        private Color? _color = null;

        public ObstaclePieceType(string shape, Color? color = null) {
            Shape = shape;
            Color = color;
        }

        public bool MatchWith(Piece piece) => false;
        public bool CanBeShuffled() => false;
        public bool CanBeMoved() => false;

    }

}
```

#### Creating one piece

```csharp
// Only the shape
var piece = new Piece(new NormalPieceType("circle"));
// Or
var piece = new Piece(new NormalPieceType("circle", Color.Green));

// Static constructor
var piece = Piece.create(new NormalPieceType("square", Color.Red));


// Properties
piece.Id
piece.Type // IPieceType
piece.Locked

// Probability properties to use with IPieceGenerator
piece.Weight
piece.TotalAccumWeight

// Lock methods, just use as information for you to decide whether to move the piece, mix it or match it
piece.Lock()
piece.Unlock();

// When you initialize the board the pieces are already instanciated with the proper type, this clone works when it comes to generate new pieces in the board.
piece.Clone();
```

### Sequence

A sequence by definition does not need to follow any rules, the cells from this sequence are considered a match. This provides flexibility to for example remove an entire row in the board without the pieces having to match each other or live in the same row & column.

**The constructor validate the cells passed as parameter and only assign the ones that has a piece.**

The calculations to retrieve the list of cells are done before create the sequence. That's what an `ISequenceFinder` implementation does since it knows how to allocate the proper `SHAPE`.

```csharp
// Available shapes

 public enum SHAPES {
     HORIZONTAL,
     VERTICAL,
     T_SHAPE,
     L_SHAPE
 }
```

```csharp
var cells = new List<GridCell>() {new GridCell(1, 0), new GridCell(2, 0), new GridCell(3, 0)}
var sequence = new Sequence(cells, Sequence.SHAPES.HORIZONTAL)

// Get the current sequence cells
sequence.Cells();

// Give the size of the sequence
sequence.Size()

//Remove all the pieces from the cells
sequence.Consume();

// Get all the pieces from this sequence
sequence.Pieces();

//Position related, useful for ISequenceFinder to help find shapes.
sequence.MiddleCell();
sequence.TopEdgeCell();
sequence.BottomEdgeCell();
sequence.LeftEdgeCell();
sequence.RightEdgeCell();

// Shape shorcuts
sequence.IsHorizontal();
sequence.IsVertical();
sequence.IsTShape();
sequence.IsLShape();

// To clone sequences before consuming and allow to pass the cells that were affected through events
sequence.Clone();


```

### Board

This is the main class that you will actually use, you can manipulate any aspect of the dashboard and extract the necessary information for the UI.

#### Creating a new board

```csharp

// PieceWeightGenerator from library is provided when null
// SequenceFinder from library is provided when null
public Board(
    int gridWidth,
    int gridHeight,
    int initialMoves,
    IPieceGenerator? pieceGenerator = null,
    ISequenceFinder? sequenceFinder = null)


// Grid size can be passed as integer or Vector2.
var board = new Board(5, 7, 25);
var board = new Board(new Vector2(7, 5), 25);
// Or
var board = Board.Create(5, 7, 25);


// Add available pieces to roll, they need to be defined to fill the board correctly
 Piece square = new(new NormalPieceType("square"));
 Piece circle = new(new NormalPieceType("circle"));
 Piece triangle = new(new NormalPieceType("triangle"));

 List<Piece> pieces = [square, circle, triangle];

// Further methods are provided to add or remove pieces on the board any time
board.AddAvailablePieces(pieces);
board.AddAvailablePiece(pieces.First());
board.RemoveAvailablePieces(pieces);
board.RemoveAvailablePiece(pieces.Last());


board.AddAvailablePieces(pieces)
     .PrepareGridCells() // Initialize the GridCell classes with the width & height provided, overwrite parameter needs to be true if called a second time
     .FillInitialBoard(false); // public Board FillInitialBoard(bool allowMatchesOnStart = false, Dictionary<string, Piece>? preSelectedPieces = null)



// You can change properties any time
board.ChangeGridWidth(7)
     .ChangeGridHeight(8)
     .ChangeGridSize(new Vector2(8, 7)) //Shorcut alternative
     .ChangeFillMode(Board.FILL_MODES.SIDE_DOWN)
     .ChangeCellSize(new Vector2(32, 32)) // Size information from the cell to display on your UI
     .ChangeOffset(new Vector2(5, 10)) // Offset separation between cells to display on your UI
     .ChangeRemainingMoves(20);

// Increase or decrease board remaining moves which represents the remaining moves the player has left to use on this board.
// EVENT: When remaining moves reachs zero raises the event "SpentAllMoves" and lock the board.
board.IncreaseMove();
board.DecreaseMove();

//Plural
board.IncreaseMoves(2);
board.DecreaseMoves(3);

// Lock or unlock this board, has no immediate effect on the board, it is information to be used externally.
board.Lock();
board.Unlock();

//Access current cells with
board.GridCells;

// Cell from column 0 and row 1, null is returned if it does not exists.
board.Cell(0, 1)
board.Cell(new Vector2(1, 0))

//Returns the calculated cell position with the offset for a grid cell
board.CellPosition(board.Cell(2, 5)); // Vector2(250, 50) random vector for example purposes

// Get the cells of selected column & row
board.CellsFromColumn(1);
board.CellsFromRow(3);

// Get empty cells from board
board.EmptyCells();
board.EmptyCellsFromRow(3);
board.EmptyCellsFromColumn(2);

// Get cells that contains a piece of selected type
board.CellsFromRowOfPieceType(2, typeof(SpecialPieceType));
board.CellsFromColumnOfPieceType(1, typeof(NormalPieceType));
board.CellsOfPieceType(typeof(NormalPieceType));

// Returns the 2 upper cells from the origin one provided, only returns the valid cells,
// it will never go out of bounds even if you use large numbers or out of range of that cell.
board.UpperCellsFrom(board.Cell(3, 3), 2); // GridCell(3, 2), GridCell(3, 1)
board.BottomCellsFrom(board.Cell(3, 3), 2); // GridCell(3, 4), GridCell(3, 5)
board.RightCellsFrom(board.Cell(3, 3), 2); // GridCell(4, 3), GridCell(5, 3)
board.LeftCellsFrom(board.Cell(3, 3), 2); // GridCell(1, 3), GridCell(2, 3)

// Same syntax to retrieve the pieces instead of the cells
board.UpperCellsPiecesFrom(board.Cell(3, 3), 2);
board.BottomCellsPiecesFrom(board.Cell(2, 1), 1);
board.RightCellsPiecesFrom(board.Cell(4, 1), 5);
board.LeftCellsPiecesFrom(board.Cell(7, 2), 3);


// Find a grid cell that contains the piece provided, it uses the Id property internally for the search.
var piece = new Piece(new NormalPieceType("circle"));
board.FindGridCellWithPiece(piece);
//Or
board.FindGridCellWithPiece(piece.Id);
board.FindGridCellWithPiece(piece.Id.ToString());



```
