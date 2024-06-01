﻿
using Match3Maker;
using Moq;
using System.Numerics;
using Xunit;

namespace Match3Tests {

    public class BoardTests {
        private readonly Mock<IPieceSelector> _mockPieceSelector = new();
        private readonly Mock<ISequenceFinder> _mockSequenceFinder = new();

        [Fact]
        public void Can_Be_Created_With_Static_Constructor() {
            int width = 5;
            int height = 6;

            var board = Board.Create(width, height, _mockPieceSelector.Object, _mockSequenceFinder.Object);

            Assert.Equal(width, board.GridWidth);
            Assert.Equal(height, board.GridHeight);
            Assert.Equal(width * height, board.Dimensions());

            Vector2 size = new(10, 7);

            board = Board.Create(size, _mockPieceSelector.Object, _mockSequenceFinder.Object);

            Assert.Equal((int)size.X, board.GridWidth);
            Assert.Equal((int)size.Y, board.GridHeight);
            Assert.Equal(size.X * size.Y, board.Dimensions());
        }

        [Fact]
        public void Grid_Dimensions_Are_Set_Correctly_On_New_Board() {
            int width = 5;
            int height = 6;

            var board = new Board(width, height, _mockPieceSelector.Object, _mockSequenceFinder.Object);

            Assert.Equal(width, board.GridWidth);
            Assert.Equal(height, board.GridHeight);
            Assert.Equal(width * height, board.Dimensions());

            board.ChangeGridWidth(width + 3).ChangeGridHeight(height + 1);

            Assert.Equal(width + 3, board.GridWidth);
            Assert.Equal(height + 1, board.GridHeight);
            Assert.Equal((width + 3) * (height + 1), board.Dimensions());

            var newSize = new Vector2(8, 10);

            board.ChangeGridSize(newSize);
            Assert.Equal(newSize.X, board.GridWidth);
            Assert.Equal(newSize.Y, board.GridHeight);
            Assert.Equal(newSize.X * newSize.Y, board.Dimensions());
        }

        [Fact]
        public void Grid_Dimensions_Are_Clamped_ToMin() {
            var board = new Board(2, 1, _mockPieceSelector.Object, _mockSequenceFinder.Object);

            Assert.Equal(Board.MIN_GRID_WIDTH, board.GridWidth);
            Assert.Equal(Board.MIN_GRID_HEIGHT, board.GridHeight);
            Assert.Equal(Board.MIN_GRID_HEIGHT * Board.MIN_GRID_WIDTH, board.Dimensions());
        }

        [Fact]
        public void Should_Be_Able_To_Generate_Grid_Cells_Based_On_Size() {
            var board = new Board(8, 7, _mockPieceSelector.Object, _mockSequenceFinder.Object);

            Assert.Empty(board.GridCells);

            board.PrepareGridCells();

            Assert.Equal(board.GridWidth, board.GridCells.Count);
            Assert.Equal(board.Dimensions(), board.GridCells.SelectMany(cells => cells).Count());

            //Should not change current cells if overwrite is false
            board.ChangeGridSize(new Vector2(10, 10)).PrepareGridCells();

            Assert.Equal(8, board.GridCells.Count);
            Assert.Equal(8 * 7, board.GridCells.SelectMany(cells => cells).Count());
        }

        [Fact]
        public void Should_Be_Able_To_Overwrite_Grid_Cells_When_Size_Is_Changed_And_Overwrite_Is_True() {
            var board = new Board(8, 7, _mockPieceSelector.Object, _mockSequenceFinder.Object);

            Assert.Empty(board.GridCells);

            board.PrepareGridCells();

            Assert.Equal(board.GridWidth, board.GridCells.Count);
            Assert.Equal(board.Dimensions(), board.GridCells.SelectMany(cells => cells).Count());

            board.ChangeGridSize(new Vector2(10, 10)).PrepareGridCells(true);

            Assert.Equal(board.GridWidth, board.GridCells.Count);
            Assert.Equal(board.Dimensions(), board.GridCells.SelectMany(cells => cells).Count());
        }

        [Fact]
        public void Should_Assign_Neighbours_On_Prepared_Grid_Cells() {
            var board = new Board(8, 7, _mockPieceSelector.Object, _mockSequenceFinder.Object);

            Assert.Empty(board.GridCells);

            board.PrepareGridCells();

            var topLeftCell = board.Cell(0, 0);
            var topRightCell = board.Cell(7, 0);
            var bottomRightCell = board.Cell(7, 6);
            var bottomLeftCell = board.Cell(0, 6);
            var surroundedCell = board.Cell(2, 2);

            Assert.Equal(board.Cell(2, 1), surroundedCell.NeighbourUp);
            Assert.Equal(board.Cell(2, 3), surroundedCell.NeighbourBottom);
            Assert.Equal(board.Cell(3, 2), surroundedCell.NeighbourRight);
            Assert.Equal(board.Cell(1, 2), surroundedCell.NeighbourLeft);

            Assert.Null(topLeftCell.NeighbourUp);
            Assert.Equal(board.Cell(0, 1), topLeftCell.NeighbourBottom);
            Assert.Equal(board.Cell(1, 0), topLeftCell.NeighbourRight);
            Assert.Null(topLeftCell.NeighbourLeft);

            Assert.Null(topRightCell.NeighbourUp);
            Assert.Equal(board.Cell(7, 1), topRightCell.NeighbourBottom);
            Assert.Null(topRightCell.NeighbourRight);
            Assert.Equal(board.Cell(6, 0), topRightCell.NeighbourLeft);

            Assert.Equal(board.Cell(0, 5), bottomLeftCell.NeighbourUp);
            Assert.Null(bottomLeftCell.NeighbourBottom);
            Assert.Equal(board.Cell(1, 6), bottomLeftCell.NeighbourRight);
            Assert.Null(bottomLeftCell.NeighbourLeft);

            Assert.Equal(board.Cell(7, 5), bottomRightCell.NeighbourUp);
            Assert.Null(bottomRightCell.NeighbourBottom);
            Assert.Null(bottomRightCell.NeighbourRight);
            Assert.Equal(board.Cell(6, 6), bottomRightCell.NeighbourLeft);
        }

        [Fact]
        public void Should_Detect_Border_And_Corners() {
            var board = new Board(8, 7, _mockPieceSelector.Object, _mockSequenceFinder.Object);

            Assert.Empty(board.GridCells);

            board.PrepareGridCells();

            var topLeftCell = board.Cell(0, 0);
            var topRightCell = board.Cell(7, 0);
            var bottomRightCell = board.Cell(7, 6);
            var bottomLeftCell = board.Cell(0, 6);
            var surroundedCell = board.Cell(2, 2);

            var topBorderCell = board.Cell(3, 0);
            var bottomBorderCell = board.Cell(2, 6);
            var leftBorderCell = board.Cell(0, 3);
            var rightBorderCell = board.Cell(7, 4);

            Assert.True(topLeftCell.IsTopLeftCorner());
            Assert.True(topRightCell.IsTopRightCorner());
            Assert.True(bottomLeftCell.IsBottomLeftCorner());
            Assert.True(bottomRightCell.IsBottomRightCorner());

            Assert.True(topBorderCell.IsTopBorder());
            Assert.True(bottomBorderCell.IsBottomBorder());
            Assert.True(leftBorderCell.IsLeftBorder());
            Assert.True(rightBorderCell.IsRightBorder());

            Assert.False(surroundedCell.IsTopLeftCorner());
            Assert.False(surroundedCell.IsTopRightCorner());
            Assert.False(surroundedCell.IsBottomLeftCorner());
            Assert.False(surroundedCell.IsBottomRightCorner());
            Assert.False(surroundedCell.IsTopBorder());
            Assert.False(surroundedCell.IsBottomBorder());
            Assert.False(surroundedCell.IsRightBorder());
            Assert.False(surroundedCell.IsLeftBorder());
            Assert.False(surroundedCell.IsTopBorder());
            Assert.False(surroundedCell.IsBottomBorder());
            Assert.False(surroundedCell.IsLeftBorder());
            Assert.False(surroundedCell.IsRightBorder());
        }


        [Fact]
        public void Should_Raise_Prepared_Board_Event_When_Prepared_Grid_Cells() {
            var board = new Board(5, 6, _mockPieceSelector.Object, _mockSequenceFinder.Object);

            List<string> receivedEvents = [];

            void listener() {
                receivedEvents.Add("Prepared");
            }

            board.PreparedBoard += listener;

            board.PrepareGridCells();

            Assert.Single(receivedEvents);
            Assert.Equal("Prepared", receivedEvents.First());

            board.PreparedBoard -= listener;
        }

        [Fact]
        public void Should_Return_Adjacent_Cells_Or_Null_When_Requested() {
            var board = new Board(4, 5, _mockPieceSelector.Object, _mockSequenceFinder.Object);
            board.PrepareGridCells();

            Assert.Null(board.Cell(6, 3));
            Assert.Null(board.Cell(0, 9));
            Assert.Null(board.Cell(8, 8));
            Assert.Null(board.Cell(-1, 0));
            Assert.Null(board.Cell(0, -1));
            Assert.Null(board.Cell(-1, -1));

            GridCell? originCell = board.Cell(1, 2);
            Assert.NotNull(originCell);

            GridCell upperCell = originCell.NeighbourUp;
            Assert.True(originCell.IsColumnNeighbourOf(upperCell));
            Assert.Equal(originCell.Row - 1, upperCell.Row);
            Assert.Equal(upperCell.Column, originCell.Column);

            GridCell bottomCell = originCell.NeighbourBottom;
            Assert.True(originCell.IsColumnNeighbourOf(bottomCell));
            Assert.Equal(originCell.Row + 1, bottomCell.Row);
            Assert.Equal(bottomCell.Column, originCell.Column);

            GridCell rightCell = originCell.NeighbourRight;
            Assert.True(originCell.IsRowNeighbourOf(rightCell));
            Assert.Equal(originCell.Column + 1, rightCell.Column);
            Assert.Equal(rightCell.Row, originCell.Row);

            GridCell leftCell = originCell.NeighbourLeft;
            Assert.True(originCell.IsRowNeighbourOf(leftCell));
            Assert.Equal(originCell.Column - 1, leftCell.Column);
            Assert.Equal(leftCell.Row, originCell.Row);
        }

        [Fact]
        public void Should_Return_Upper_Cells_From_Selected_Cell() {
            var board = new Board(4, 5, _mockPieceSelector.Object, _mockSequenceFinder.Object);
            board.PrepareGridCells();

            var originCell = board.Cell(2, 4);
            var upperCells = board.UpperCellsFrom(originCell, board.GridHeight);

            Assert.Equal(board.GridHeight - 1, upperCells.Count);
            Assert.True(upperCells.All(cell => !cell.Row.Equals(originCell.Row) && cell.Column.Equals(originCell.Column)));

            upperCells = board.UpperCellsFrom(originCell, 1);

            Assert.Single(upperCells);
            Assert.Equal(upperCells[0].Column, originCell.Column);
            Assert.Equal(upperCells[0].Row, originCell.Row - 1);
        }

        [Fact]
        public void Should_Return_Right_Cells_From_Selected_Cell() {
            var board = new Board(4, 5, _mockPieceSelector.Object, _mockSequenceFinder.Object);
            board.PrepareGridCells();

            var originCell = board.Cell(0, 2);
            var rightCells = board.RightCellsFrom(originCell, board.GridWidth);

            Assert.Equal(board.GridWidth - 1, rightCells.Count);
            Assert.True(rightCells.All(cell => cell.Row.Equals(originCell.Row) && !cell.Column.Equals(originCell.Column)));

            rightCells = board.RightCellsFrom(originCell, 1);

            Assert.Single(rightCells);
            Assert.Equal(rightCells[0].Row, originCell.Row);
            Assert.Equal(rightCells[0].Column, originCell.Column + 1);
        }

        [Fact]
        public void Should_Return_Left_Cells_From_Selected_Cell() {
            var board = new Board(4, 5, _mockPieceSelector.Object, _mockSequenceFinder.Object);
            board.PrepareGridCells();

            var originCell = board.Cell(3, 1);
            var leftCells = board.LeftCellsFrom(originCell, board.GridWidth);

            Assert.Equal(board.GridWidth - 1, leftCells.Count);
            Assert.True(leftCells.All(cell => cell.Row.Equals(originCell.Row) && !cell.Column.Equals(originCell.Column)));

            leftCells = board.LeftCellsFrom(originCell, 1);

            Assert.Single(leftCells);
            Assert.Equal(leftCells[0].Row, originCell.Row);
            Assert.Equal(leftCells[0].Column, originCell.Column - 1);
        }


        [Fact]
        public void Should_Return_Bottoms_Cells_From_Selected_Cell() {
            var board = new Board(4, 5, _mockPieceSelector.Object, _mockSequenceFinder.Object);
            board.PrepareGridCells();

            var originCell = board.Cell(1, 0);
            var bottomCells = board.BottomCellsFrom(originCell, board.GridHeight);

            Assert.Equal(board.GridHeight - 1, bottomCells.Count);
            Assert.True(bottomCells.All(cell => !cell.Row.Equals(originCell.Row) && cell.Column.Equals(originCell.Column)));

            bottomCells = board.BottomCellsFrom(originCell, 1);

            Assert.Single(bottomCells);
            Assert.Equal(bottomCells[0].Column, originCell.Column);
            Assert.Equal(bottomCells[0].Row, originCell.Row + 1);
        }

        [Fact]
        public void Should_Return_Selected_Row_Cells() {
            var board = new Board(8, 7, _mockPieceSelector.Object, _mockSequenceFinder.Object);
            board.PrepareGridCells();

            var rowCells = board.CellsFromRow(1);

            Assert.Equal(board.GridWidth, rowCells.Count);
            Assert.True(rowCells.All(cell => cell.Row.Equals(1)));

            rowCells = board.CellsFromRow(5);

            Assert.Equal(board.GridWidth, rowCells.Count);
            Assert.True(rowCells.All(cell => cell.Row.Equals(5)));

            Assert.Empty(board.CellsFromRow(-1));
            Assert.Empty(board.CellsFromRow(9));

        }

        [Fact]
        public void Should_Return_Selected_Column_Cells() {
            var board = new Board(8, 7, _mockPieceSelector.Object, _mockSequenceFinder.Object);
            board.PrepareGridCells();

            var columnCells = board.CellsFromColumn(1);

            Assert.Equal(board.GridHeight, columnCells.Count);
            Assert.True(columnCells.All(cell => cell.Column.Equals(1)));

            columnCells = board.CellsFromColumn(5);

            Assert.Equal(board.GridHeight, columnCells.Count);
            Assert.True(columnCells.All(cell => cell.Column.Equals(5)));

            Assert.Empty(board.CellsFromColumn(-1));
            Assert.Empty(board.CellsFromColumn(8));
        }

        [Fact]
        public void Should_Return_Empty_Cells_From_Column() {
            var board = new Board(8, 7, _mockPieceSelector.Object, _mockSequenceFinder.Object);
            board.PrepareGridCells();

            Assert.Equal(board.GridHeight, board.EmptyCellsFromColumn(0).Count);

            board.Cell(0, 0)?.AssignPiece(new Piece("circle"));
            board.Cell(0, 1)?.AssignPiece(new Piece("circle"));

            Assert.Equal(board.GridHeight - 2, board.EmptyCellsFromColumn(0).Count);
        }

        [Fact]
        public void Should_Return_Empty_Cells_From_Row() {
            var board = new Board(8, 7, _mockPieceSelector.Object, _mockSequenceFinder.Object);
            board.PrepareGridCells();

            Assert.Equal(board.GridWidth, board.EmptyCellsFromRow(0).Count);

            board.Cell(0, 0)?.AssignPiece(new Piece("circle"));
            board.Cell(1, 0)?.AssignPiece(new Piece("circle"));

            Assert.Equal(board.GridWidth - 2, board.EmptyCellsFromRow(0).Count);
        }

        [Fact]
        public void Should_Find_The_GridCell_Related_To_Piece() {
            var board = new Board(8, 7, _mockPieceSelector.Object, _mockSequenceFinder.Object);
            board.PrepareGridCells();

            var cell = board.Cell(2, 5);
            var piece = new Piece("circle");

            Assert.Null(board.FindGridCellWithPiece(piece));

            cell?.AssignPiece(piece);

            Assert.Equal(cell, board.FindGridCellWithPiece(piece));
        }

        [Fact]
        public void Should_Add_Available_Pieces() {
            var board = new Board(8, 7, _mockPieceSelector.Object);

            List<PieceWeight> pieces = [new PieceWeight("square"), new PieceWeight("circle")];
            PieceWeight trianglePiece = new("triangle");

            Assert.Empty(board.AvailablePieces);

            board.AddAvailablePieces(pieces);

            Assert.Equal(pieces.Count, board.AvailablePieces.Count);

            board.AddAvailablePiece(trianglePiece);

            Assert.Equal(pieces.Count + 1, board.AvailablePieces.Count);

            board.AddAvailablePiece(trianglePiece);

            // Duplicates are removed
            Assert.Equal(pieces.Count + 1, board.AvailablePieces.Count);
        }


        [Fact]
        public void Should_Remove_Available_Pieces() {
            var board = new Board(8, 7, _mockPieceSelector.Object);

            PieceWeight square = new("square");
            PieceWeight circle = new("circle");
            PieceWeight triangle = new("triangle");

            List<PieceWeight> pieces = [square, circle, triangle];

            board.AddAvailablePieces(pieces);

            Assert.Equal(pieces.Count, board.AvailablePieces.Count);

            board.RemoveAvailablePiece(circle);

            Assert.Equal(2, board.AvailablePieces.Count);
            Assert.True(board.AvailablePieces.All(piece => !piece.Shape.Equals("circle")));

            board.RemoveAvailablePieces(pieces);

            Assert.Empty(board.AvailablePieces);
        }

    }

}