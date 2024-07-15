
using Match3Maker;
using Moq;
using System.Numerics;
using Xunit;

namespace Match3MakerTests {



    public class BoardTests {
        private readonly Mock<IPieceGenerator> _mockPieceSelector = new();
        private readonly Mock<ISequenceFinder> _mockSequenceFinder = new();
        private readonly PieceFactory _pieceFactory = new();

        [Fact]
        public void Can_Be_Created_With_Static_Constructor() {
            int width = 5;
            int height = 6;

            var board = Board.Create(width, height, 10, _mockPieceSelector.Object, _mockSequenceFinder.Object);

            Assert.Equal(width, board.GridWidth);
            Assert.Equal(height, board.GridHeight);
            Assert.Equal(width * height, board.Dimensions());

            Vector2 size = new(10, 7);

            board = Board.Create(size, 10, _mockPieceSelector.Object, _mockSequenceFinder.Object);

            Assert.Equal((int)size.X, board.GridWidth);
            Assert.Equal((int)size.Y, board.GridHeight);
            Assert.Equal(size.X * size.Y, board.Dimensions());
        }

        [Fact]
        public void Should_Raise_Spent_All_Movements_Event_Once_When_Remaining_Moves_Reach_Zero() {
            var board = Board.Create(5, 5, 10, _mockPieceSelector.Object, _mockSequenceFinder.Object);

            List<string> receivedEvents = [];

            void listener() {
                receivedEvents.Add("SpentMoves");
            }

            board.SpentAllMovements += listener;

            Assert.False(board.Locked);

            Assert.Equal(10, board.RemainingMoves);
            board.ChangeRemainingMoves(7);

            Assert.Equal(7, board.RemainingMoves);

            board.DecreaseMoves(3);

            Assert.Equal(4, board.RemainingMoves);

            board.IncreaseMoves(3);

            Assert.Equal(7, board.RemainingMoves);

            board.DecreaseMoves(7);

            Assert.Equal(0, board.RemainingMoves);

            board.DecreaseMoves(100);

            // Cannot go below zero and events should be raised once only when previous value was not zero
            Assert.Equal(0, board.RemainingMoves);

            Assert.True(board.Locked);

            Assert.Single(receivedEvents);
            Assert.Equal("SpentMoves", receivedEvents.First());

            board.SpentAllMovements -= listener;

        }

        [Fact]
        public void Grid_Dimensions_Are_Set_Correctly_On_New_Board() {
            int width = 5;
            int height = 6;

            var board = new Board(width, height, 10, _mockPieceSelector.Object, _mockSequenceFinder.Object);

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
            var board = new Board(2, 1, 10, _mockPieceSelector.Object, _mockSequenceFinder.Object);

            Assert.Equal(Board.MIN_GRID_WIDTH, board.GridWidth);
            Assert.Equal(Board.MIN_GRID_HEIGHT, board.GridHeight);
            Assert.Equal(Board.MIN_GRID_HEIGHT * Board.MIN_GRID_WIDTH, board.Dimensions());
        }

        [Fact]
        public void Should_Be_Able_To_Generate_Grid_Cells_Based_On_Size() {
            var board = new Board(8, 7, 10, _mockPieceSelector.Object, _mockSequenceFinder.Object);

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
            var board = new Board(8, 7, 10, _mockPieceSelector.Object, _mockSequenceFinder.Object);

            Assert.Empty(board.GridCells);

            board.PrepareGridCells();

            Assert.Equal(board.GridWidth, board.GridCells.Count);
            Assert.Equal(board.Dimensions(), board.GridCells.SelectMany(cells => cells).Count());

            board.ChangeGridSize(new Vector2(10, 10)).PrepareGridCells(null, true);

            Assert.Equal(board.GridWidth, board.GridCells.Count);
            Assert.Equal(board.Dimensions(), board.GridCells.SelectMany(cells => cells).Count());
        }

        [Fact]
        public void Should_Be_Able_To_Choose_Disabled_Cells_On_Prepare() {
            var board = new Board(8, 7, 10, _mockPieceSelector.Object, _mockSequenceFinder.Object);

            Assert.Empty(board.GridCells);

            board.PrepareGridCells([new Vector2(1, 1), new Vector2(3, 3), new Vector2(4, 2)]);

            Assert.True(new List<GridCell>() { board.Cell(1, 1), board.Cell(3, 3), board.Cell(2, 4) }.All(cell => !cell.CanContainPiece));
        }

        [Fact]
        public void Should_Choose_Grid_Cells_That_Can_Or_Not_Pieces() {
            var board = new Board(8, 7, 10, _mockPieceSelector.Object, _mockSequenceFinder.Object);

            board.PrepareGridCells();


            Assert.True(board.Cell(1, 0).CanContainPiece);
            Assert.True(board.Cell(4, 2).CanContainPiece);
            Assert.True(board.Cell(2, 2).CanContainPiece);

            board.CellsThatCanContainPieces([board.Cell(1, 0), board.Cell(4, 2), board.Cell(2, 2)]);

            Assert.True(board.Cell(1, 0).CanContainPiece);
            Assert.True(board.Cell(4, 2).CanContainPiece);
            Assert.True(board.Cell(2, 2).CanContainPiece);

            board.CellsThatCannotContainPieces([board.Cell(1, 0), board.Cell(4, 2), board.Cell(2, 2)]);

            Assert.False(board.Cell(1, 0).CanContainPiece);
            Assert.False(board.Cell(4, 2).CanContainPiece);
            Assert.False(board.Cell(2, 2).CanContainPiece);
        }

        [Fact]
        public void Should_Assign_Neighbours_On_Prepared_Grid_Cells() {
            var board = new Board(8, 7, 10, _mockPieceSelector.Object, _mockSequenceFinder.Object);

            Assert.Empty(board.GridCells);

            board.PrepareGridCells();

            var topLeftCornerCell = board.Cell(0, 0);
            var topRightCornerCell = board.Cell(7, 0);
            var bottomRightCornerCell = board.Cell(7, 6);
            var bottomLeftCornerCell = board.Cell(0, 6);
            var surroundedCell = board.Cell(2, 2);

            Assert.Equal(board.Cell(2, 1), surroundedCell.NeighbourUp);
            Assert.Equal(board.Cell(2, 3), surroundedCell.NeighbourBottom);
            Assert.Equal(board.Cell(3, 2), surroundedCell.NeighbourRight);
            Assert.Equal(board.Cell(1, 2), surroundedCell.NeighbourLeft);

            Assert.Equal(board.Cell(3, 1), surroundedCell.DiagonalNeighbourTopRight);
            Assert.Equal(board.Cell(1, 1), surroundedCell.DiagonalNeighbourTopLeft);
            Assert.Equal(board.Cell(3, 3), surroundedCell.DiagonalNeighbourBottomRight);
            Assert.Equal(board.Cell(1, 3), surroundedCell.DiagonalNeighbourBottomLeft);

            Assert.Null(topLeftCornerCell.NeighbourUp);
            Assert.Equal(board.Cell(0, 1), topLeftCornerCell.NeighbourBottom);
            Assert.Equal(board.Cell(1, 0), topLeftCornerCell.NeighbourRight);
            Assert.Null(topLeftCornerCell.NeighbourLeft);

            Assert.Null(topLeftCornerCell.DiagonalNeighbourBottomLeft);
            Assert.Null(topLeftCornerCell.DiagonalNeighbourTopLeft);
            Assert.Null(topLeftCornerCell.DiagonalNeighbourTopRight);
            Assert.Equal(board.Cell(1, 1), topLeftCornerCell.DiagonalNeighbourBottomRight);


            Assert.Null(topRightCornerCell.NeighbourUp);
            Assert.Equal(board.Cell(7, 1), topRightCornerCell.NeighbourBottom);
            Assert.Null(topRightCornerCell.NeighbourRight);
            Assert.Equal(board.Cell(6, 0), topRightCornerCell.NeighbourLeft);

            Assert.Equal(board.Cell(6, 1), topRightCornerCell.DiagonalNeighbourBottomLeft);
            Assert.Null(topRightCornerCell.DiagonalNeighbourTopLeft);
            Assert.Null(topRightCornerCell.DiagonalNeighbourTopRight);
            Assert.Null(topRightCornerCell.DiagonalNeighbourBottomRight);

            Assert.Equal(board.Cell(7, 5), bottomRightCornerCell.NeighbourUp);
            Assert.Null(bottomRightCornerCell.NeighbourBottom);
            Assert.Null(bottomRightCornerCell.NeighbourRight);
            Assert.Equal(board.Cell(6, 6), bottomRightCornerCell.NeighbourLeft);

            Assert.Null(bottomRightCornerCell.DiagonalNeighbourBottomLeft);
            Assert.Equal(board.Cell(6, 5), bottomRightCornerCell.DiagonalNeighbourTopLeft);
            Assert.Null(bottomRightCornerCell.DiagonalNeighbourTopRight);
            Assert.Null(bottomRightCornerCell.DiagonalNeighbourBottomRight);

            Assert.Equal(board.Cell(0, 5), bottomLeftCornerCell.NeighbourUp);
            Assert.Null(bottomLeftCornerCell.NeighbourBottom);
            Assert.Equal(board.Cell(1, 6), bottomLeftCornerCell.NeighbourRight);
            Assert.Null(bottomLeftCornerCell.NeighbourLeft);

            Assert.Null(bottomLeftCornerCell.DiagonalNeighbourBottomLeft);
            Assert.Null(bottomLeftCornerCell.DiagonalNeighbourTopLeft);
            Assert.Equal(board.Cell(1, 5), bottomLeftCornerCell.DiagonalNeighbourTopRight);
            Assert.Null(bottomLeftCornerCell.DiagonalNeighbourBottomRight);
        }

        [Fact]
        public void Should_Retrieve_Adjacent_Cells_From_Origin_One_With_Diagonals_As_Option() {
            var board = new Board(5, 6, 10, _mockPieceSelector.Object, _mockSequenceFinder.Object);

            board.PrepareGridCells();

            var originCell = board.TopLeftCornerCell();
            var adjacentCells = board.AdjacentCellsFrom(originCell);

            Assert.Equal(2, adjacentCells.Count);

            foreach (var cell in adjacentCells) {
                Assert.True(cell.IsColumnNeighbourOf(originCell) || cell.IsRowNeighbourOf(originCell));
            }

            adjacentCells = board.AdjacentCellsFrom(originCell, true);

            Assert.Equal(3, adjacentCells.Count);
            Assert.Contains(originCell.DiagonalNeighbourBottomRight, adjacentCells);

            originCell = board.Cell(1, 1);
            adjacentCells = board.AdjacentCellsFrom(originCell);

            Assert.Equal(4, adjacentCells.Count);

            foreach (var cell in adjacentCells) {
                Assert.True(cell.IsColumnNeighbourOf(originCell) || cell.IsRowNeighbourOf(originCell));
            }

            adjacentCells = board.AdjacentCellsFrom(originCell, true);

            Assert.Equal(4 * 2, adjacentCells.Count);
            Assert.Contains(originCell.DiagonalNeighbourBottomLeft, adjacentCells);
            Assert.Contains(originCell.DiagonalNeighbourBottomRight, adjacentCells);
            Assert.Contains(originCell.DiagonalNeighbourTopLeft, adjacentCells);
            Assert.Contains(originCell.DiagonalNeighbourTopRight, adjacentCells);
        }

        [Fact]
        public void Should_Retrieve_Cross_Cells_From_Origin_One() {
            var board = new Board(5, 6, 10, _mockPieceSelector.Object, _mockSequenceFinder.Object);

            board.PrepareGridCells();

            var originCell = board.TopLeftCornerCell();
            var crossCells = board.CrossCellsFrom(originCell);

            // -1 it's because the function removes the duplicated connection cell between row and column
            Assert.Equal((board.GridWidth + board.GridHeight) - 1, crossCells.Count);

            foreach (var cell in crossCells) {
                Assert.True(cell.InSameRowAs(originCell) || cell.InSameColumnAs(originCell));
            }

            originCell = board.Cell(2, 2);
            crossCells = board.CrossCellsFrom(originCell);

            Assert.Equal((board.GridWidth + board.GridHeight) - 1, crossCells.Count);
        }

        [Fact]
        public void Should_Retrieve_Cross_Diagonal_Cells_From_Origin_One() {
            var board = new Board(5, 6, 10, _mockPieceSelector.Object, _mockSequenceFinder.Object);

            board.PrepareGridCells();

            var originCell = board.TopLeftCornerCell();
            var crossDiagonalCells = board.CrossDiagonalCellsFrom(originCell);

            Assert.Equal(board.GridWidth - 1, crossDiagonalCells.Count);

            foreach (var cell in crossDiagonalCells) {
                Assert.False(cell.InSameRowAs(originCell) && cell.InSameColumnAs(originCell));
            }

            originCell = board.Cell(3, 2);
            crossDiagonalCells = board.CrossDiagonalCellsFrom(originCell);

            Assert.Equal(7, crossDiagonalCells.Count);

            foreach (var cell in crossDiagonalCells) {
                Assert.False(cell.InSameRowAs(originCell) && cell.InSameColumnAs(originCell));
            }
        }

        [Fact]
        public void Should_Detect_Border_And_Corners() {
            var board = new Board(8, 7, 10, _mockPieceSelector.Object, _mockSequenceFinder.Object);

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
            var board = new Board(5, 6, 10, _mockPieceSelector.Object, _mockSequenceFinder.Object);

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
            var board = new Board(4, 5, 10, _mockPieceSelector.Object, _mockSequenceFinder.Object);
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
            var board = new Board(4, 5, 10, _mockPieceSelector.Object, _mockSequenceFinder.Object);
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
            var board = new Board(4, 5, 10, _mockPieceSelector.Object, _mockSequenceFinder.Object);
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
            var board = new Board(4, 5, 10, _mockPieceSelector.Object, _mockSequenceFinder.Object);
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
            var board = new Board(4, 5, 10, _mockPieceSelector.Object, _mockSequenceFinder.Object);
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
        public void Should_Return_Diagonal_Top_Right_Cells_From_Selected_Cell() {
            var board = new Board(4, 5, 10, _mockPieceSelector.Object, _mockSequenceFinder.Object);
            board.PrepareGridCells();

            var originCell = board.BottomLeftCornerCell();
            var diagonalTopRightCells = board.DiagonalTopRightCellsFrom(originCell, board.GridWidth);

            Assert.Equal(board.GridWidth - 1, diagonalTopRightCells.Count);
            Assert.True(diagonalTopRightCells.All(cell => !cell.Row.Equals(originCell.Row) && !cell.Column.Equals(originCell.Column)));

            diagonalTopRightCells = board.DiagonalTopRightCellsFrom(originCell, 1);

            Assert.Single(diagonalTopRightCells);
            Assert.Equal(originCell.DiagonalNeighbourTopRight, diagonalTopRightCells[0]);
        }

        [Fact]
        public void Should_Return_Diagonal_Top_Left_Cells_From_Selected_Cell() {
            var board = new Board(4, 5, 10, _mockPieceSelector.Object, _mockSequenceFinder.Object);
            board.PrepareGridCells();

            var originCell = board.BottomRightCornerCell();
            var diagonalTopLeftCells = board.DiagonalTopLeftCellsFrom(originCell, board.GridWidth);

            Assert.Equal(board.GridWidth - 1, diagonalTopLeftCells.Count);
            Assert.True(diagonalTopLeftCells.All(cell => !cell.Row.Equals(originCell.Row) && !cell.Column.Equals(originCell.Column)));

            diagonalTopLeftCells = board.DiagonalTopLeftCellsFrom(originCell, 1);

            Assert.Single(diagonalTopLeftCells);
            Assert.Equal(originCell.DiagonalNeighbourTopLeft, diagonalTopLeftCells[0]);

        }

        [Fact]
        public void Should_Return_Diagonal_Bottom_Left_Cells_From_Selected_Cell() {
            var board = new Board(4, 5, 10, _mockPieceSelector.Object, _mockSequenceFinder.Object);
            board.PrepareGridCells();

            var originCell = board.TopRightCornerCell();
            var diagonalBottomLeftCells = board.DiagonalBottomLeftCellsFrom(originCell, board.GridWidth);

            Assert.Equal(board.GridWidth - 1, diagonalBottomLeftCells.Count);
            Assert.True(diagonalBottomLeftCells.All(cell => !cell.Row.Equals(originCell.Row) && !cell.Column.Equals(originCell.Column)));

            diagonalBottomLeftCells = board.DiagonalBottomLeftCellsFrom(originCell, 1);

            Assert.Single(diagonalBottomLeftCells);
            Assert.Equal(originCell.DiagonalNeighbourBottomLeft, diagonalBottomLeftCells[0]);

        }

        [Fact]
        public void Should_Return_Diagonal_Bottom_Right_Cells_From_Selected_Cell() {
            var board = new Board(4, 5, 10, _mockPieceSelector.Object, _mockSequenceFinder.Object);
            board.PrepareGridCells();

            var originCell = board.TopLeftCornerCell();
            var diagonalBottomRightCells = board.DiagonalBottomRightCellsFrom(originCell, board.GridWidth);

            Assert.Equal(board.GridWidth - 1, diagonalBottomRightCells.Count);
            Assert.True(diagonalBottomRightCells.All(cell => !cell.Row.Equals(originCell.Row) && !cell.Column.Equals(originCell.Column)));

            diagonalBottomRightCells = board.DiagonalBottomRightCellsFrom(originCell, 1);

            Assert.Single(diagonalBottomRightCells);
            Assert.Equal(originCell.DiagonalNeighbourBottomRight, diagonalBottomRightCells[0]);
        }

        [Fact]
        public void Should_Return_Selected_Row_Cells() {
            var board = new Board(8, 7, 10, _mockPieceSelector.Object, _mockSequenceFinder.Object);
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
        public void Should_Return_Cells_That_Cannot_Contain_Pieces() {
            var board = new Board(8, 7, 10, _mockPieceSelector.Object, _mockSequenceFinder.Object);
            board.PrepareGridCells();

            List<GridCell> cells = [board.Cell(1, 0), board.Cell(4, 2), board.Cell(2, 2), board.Cell(1, 3)];

            board.CellsThatCannotContainPieces(cells);

            Assert.True(board.CellsThatCannotContainPieces().All(cell => cells.Contains(cell)));


            var cellsFromColumn = board.CellsThatCannotContainPiecesFromColumn(1);

            Assert.True(cellsFromColumn.All(cell => cells.Contains(cell)));
            Assert.Equal(2, cellsFromColumn.Count);

            var cellsFromRow = board.CellsThatCannotContainPiecesFromRow(2);

            Assert.True(cellsFromRow.All(cell => cells.Contains(cell)));
            Assert.Equal(2, cellsFromRow.Count);
        }

        [Fact]
        public void Should_Return_Selected_Column_Cells() {
            var board = new Board(8, 7, 10, _mockPieceSelector.Object, _mockSequenceFinder.Object);
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
        public void Should_Return_Selected_Pieces_Of_Type() {
            var board = new Board(8, 7, 10);

            Piece square = new(_pieceFactory.CreateNormalPiece("square"));
            Piece circle = new(_pieceFactory.CreateNormalPiece("circle"));
            Piece triangle = new(_pieceFactory.CreateNormalPiece("triangle"));

            List<Piece> pieces = [square, circle, triangle];

            board.AddAvailablePieces(pieces).PrepareGridCells().FillInitialBoard(true);

            var cells = board.CellsWithPieceType(typeof(NormalPieceType));

            Assert.Empty(board.EmptyCells());
            Assert.Equal(board.Dimensions(), cells.Count);
        }

        [Fact]
        public void Should_Return_Selected_Row_Pieces_Of_Type() {
            var board = new Board(8, 7, 10);

            Piece square = new(_pieceFactory.CreateNormalPiece("square"));
            Piece circle = new(_pieceFactory.CreateNormalPiece("circle"));
            Piece triangle = new(_pieceFactory.CreateNormalPiece("triangle"));

            List<Piece> pieces = [square, circle, triangle];

            board.AddAvailablePieces(pieces).PrepareGridCells().FillInitialBoard(true);

            var cells = board.CellsFromRowOfPieceType(1, typeof(NormalPieceType));

            Assert.Equal(board.GridWidth, cells.Count);
            Assert.True(cells.All(cell => cell.Row.Equals(1)));
        }

        [Fact]
        public void Should_Return_Selected_Column_Pieces_Of_Type() {
            var board = new Board(8, 7, 10);

            Piece square = new(_pieceFactory.CreateNormalPiece("square"));
            Piece circle = new(_pieceFactory.CreateNormalPiece("circle"));
            Piece triangle = new(_pieceFactory.CreateNormalPiece("triangle"));

            List<Piece> pieces = [square, circle, triangle];

            board.AddAvailablePieces(pieces).PrepareGridCells().FillInitialBoard(true);

            var cells = board.CellsFromColumnOfPieceType(3, typeof(NormalPieceType));

            Assert.Equal(board.GridHeight, cells.Count);
            Assert.True(cells.All(cell => cell.Column.Equals(3)));
        }

        [Fact]
        public void Should_Return_Empty_Cells_From_Column() {
            var board = new Board(8, 7, 10, _mockPieceSelector.Object, _mockSequenceFinder.Object);
            board.PrepareGridCells();

            Assert.Equal(board.GridHeight, board.EmptyCellsFromColumn(0).Count);

            board.Cell(0, 0)?.AssignPiece(new Piece(_pieceFactory.CreateNormalPiece("circle")));
            board.Cell(0, 1)?.AssignPiece(new Piece(_pieceFactory.CreateNormalPiece("circle")));

            Assert.Equal(board.GridHeight - 2, board.EmptyCellsFromColumn(0).Count);
        }

        [Fact]
        public void Should_Return_Empty_Cells_From_Row() {
            var board = new Board(8, 7, 10, _mockPieceSelector.Object, _mockSequenceFinder.Object);
            board.PrepareGridCells();

            Assert.Equal(board.GridWidth, board.EmptyCellsFromRow(0).Count);

            board.Cell(0, 0)?.AssignPiece(new Piece(_pieceFactory.CreateNormalPiece("circle")));
            board.Cell(1, 0)?.AssignPiece(new Piece(_pieceFactory.CreateNormalPiece("circle")));

            Assert.Equal(board.GridWidth - 2, board.EmptyCellsFromRow(0).Count);
        }

        [Fact]
        public void Should_Find_The_GridCell_Related_To_Piece() {
            var board = new Board(8, 7, 10, _mockPieceSelector.Object, _mockSequenceFinder.Object);
            board.PrepareGridCells();

            var cell = board.Cell(2, 5);
            var piece = new Piece(_pieceFactory.CreateNormalPiece("circle"));

            Assert.Null(board.FindGridCellWithPiece(piece));

            cell?.AssignPiece(piece);

            Assert.Equal(cell, board.FindGridCellWithPiece(piece));
            Assert.Equal(cell, board.FindGridCellWithPiece(piece.Id));
            Assert.Equal(cell, board.FindGridCellWithPiece(piece.Id.ToString()));
        }

        [Fact]
        public void Should_Add_Available_Pieces() {
            var board = new Board(8, 7, 10, _mockPieceSelector.Object);

            List<Piece> pieces = [new Piece(_pieceFactory.CreateNormalPiece("square")), new Piece(_pieceFactory.CreateNormalPiece("circle"))];
            Piece trianglePiece = new(_pieceFactory.CreateNormalPiece("triangle"));

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
            var board = new Board(8, 7, 10, _mockPieceSelector.Object);

            Piece square = new(_pieceFactory.CreateNormalPiece("square"));
            Piece circle = new(_pieceFactory.CreateNormalPiece("circle"));
            Piece triangle = new(_pieceFactory.CreateNormalPiece("triangle"));

            List<Piece> pieces = [square, circle, triangle];

            board.AddAvailablePieces(pieces);

            Assert.Equal(pieces.Count, board.AvailablePieces.Count);

            board.RemoveAvailablePiece(circle);

            Assert.Equal(2, board.AvailablePieces.Count);
            Assert.True(board.AvailablePieces.All(piece => !piece.Type.Shape.Equals("circle")));

            board.RemoveAvailablePieces(pieces);

            Assert.Empty(board.AvailablePieces);
        }

        [Fact]
        public void Should_Not_Fill_The_Board_When_Grid_Cells_Are_Empty() {
            var board = new Board(8, 7, 10, _mockPieceSelector.Object);

            Assert.Empty(board.GridCells);

            board.FillInitialBoard();
            Assert.Empty(board.GridCells);
        }

        [Fact]
        public void Should_Not_Have_Matches_When_Fill_Board_And_No_Matches_Is_True() {
            Piece square = new(_pieceFactory.CreateNormalPiece("square"));
            Piece circle = new(_pieceFactory.CreateNormalPiece("circle"));
            Piece triangle = new(_pieceFactory.CreateNormalPiece("triangle"));
            Piece prism = new(_pieceFactory.CreateNormalPiece("prism"));

            List<Piece> pieces = [square, circle, triangle, prism];

            var board = new Board(8, 7, 10);

            board.AddAvailablePieces(pieces);

            board.PrepareGridCells().FillInitialBoard(false);

            Assert.Empty(board.EmptyCells());
            Assert.Empty(board.SequenceFinder.FindBoardSequences(board));
        }

        [Fact]
        public void Should_Assign_Preselected_Pieces_When_Fill_The_Board() {
            Piece square = new(_pieceFactory.CreateNormalPiece("square"));
            Piece circle = new(_pieceFactory.CreateNormalPiece("circle"));
            Piece triangle = new(_pieceFactory.CreateNormalPiece("triangle"));

            List<Piece> pieces = [square, circle, triangle];

            var board = new Board(8, 7, 10);

            board.AddAvailablePieces(pieces);

            board.PrepareGridCells().FillInitialBoard(
                false,
                new() {
                    { new Vector2(1, 1).ToString(), new Piece(_pieceFactory.CreateSpecialPiece("special")) },
                    { new Vector2(3, 5).ToString(), new Piece(_pieceFactory.CreateSpecialPiece("special2")) },
                }
            );

            Assert.Empty(board.EmptyCells());

            Assert.Equal("special", board.Cell(1, 1).Piece.Type.Shape);
            Assert.Equal(typeof(SpecialPieceType), board.Cell(1, 1).Piece.Type.GetType());

            Assert.Equal("special2", board.Cell(5, 3).Piece.Type.Shape);
            Assert.Equal(typeof(SpecialPieceType), board.Cell(5, 3).Piece.Type.GetType());

        }

        [Fact]
        public void Should_Throw_Exception_When_Try_To_Shuffle_An_Empty_Board() {
            var board = new Board(8, 7, 10);

            Assert.Throws<ArgumentException>(() => board.Shuffle());

            board.PrepareGridCells(); // Grid cells are ready but no pieces assigned

            Assert.Throws<ArgumentException>(() => board.Shuffle());
        }

        [Fact]
        public void Should_Shuffle_All_Valid_Cells() {
            Piece square = new(_pieceFactory.CreateNormalPiece("square"));
            Piece circle = new(_pieceFactory.CreateNormalPiece("circle"));
            Piece triangle = new(_pieceFactory.CreateNormalPiece("triangle"));
            Piece prism = new(_pieceFactory.CreateNormalPiece("prism"));

            List<Piece> pieces = [square, circle, triangle, prism];

            var board = new Board(8, 7, 10);

            board.AddAvailablePieces(pieces).PrepareGridCells().FillInitialBoard(true);

            var result = board.Shuffle();

            foreach (KeyValuePair<GridCell, GridCell> entry in result) {
                Assert.NotEqual(entry.Key, entry.Value);
                Assert.NotEqual(entry.Key.Piece, entry.Value.Piece);
            }
        }

        [Fact]
        public void Should_Shuffle_All_Valid_Cells_Except_Exceptions_Cells() {
            Piece square = new(_pieceFactory.CreateNormalPiece("square"));
            Piece circle = new(_pieceFactory.CreateNormalPiece("circle"));
            Piece triangle = new(_pieceFactory.CreateNormalPiece("triangle"));
            Piece prism = new(_pieceFactory.CreateNormalPiece("prism"));
            Piece special = new(_pieceFactory.CreateSpecialPiece("special"));


            List<Piece> pieces = [square, circle, triangle, prism, special];

            var board = new Board(8, 7, 10);

            board.AddAvailablePieces(pieces).PrepareGridCells().FillInitialBoard(true);

            List<GridCell> exceptCells = [board.Cell(0, 1), board.Cell(2, 0), board.Cell(3, 4)];

            var result = board.Shuffle([typeof(SpecialPieceType)], exceptCells);

            foreach (KeyValuePair<GridCell, GridCell> entry in result) {
                Assert.NotEqual(entry.Key, entry.Value);
                Assert.NotEqual(entry.Key.Piece, entry.Value.Piece);

                Assert.NotEqual(typeof(SpecialPieceType), entry.Key.Piece.GetType());
                Assert.NotEqual(typeof(SpecialPieceType), entry.Value.Piece.GetType());
            }

            exceptCells.ForEach(cell => {
                Assert.DoesNotContain(cell, result.Keys);
                Assert.DoesNotContain(cell, result.Values);
            });
        }

        [Fact]
        public void Should_Raise_Consumed_Sequence_Event_When_Consumed() {
            var board = Board.Create(5, 5, 10, _mockPieceSelector.Object, _mockSequenceFinder.Object);

            List<Sequence> receivedEvents = [];

            void listener(Sequence sequence) {
                receivedEvents.Add(sequence);
            }

            board.ConsumedSequence += listener;

            var sequence = new Sequence(new List<GridCell>() {
                new(1, 1, new Piece(_pieceFactory.CreateNormalPiece("circle"))),
                new(1, 2, new Piece(_pieceFactory.CreateNormalPiece("circle"))),
                new(1, 3, new Piece(_pieceFactory.CreateNormalPiece("circle"))),
            }, Sequence.SHAPES.VERTICAL);

            board.ConsumeSequence(sequence);

            Assert.Single(receivedEvents);
            Assert.True(receivedEvents[0].Pieces().All(piece => piece.Type.GetType().Equals(typeof(NormalPieceType)) && piece.Type.Shape.Equals("circle")));

            board.ConsumedSequence -= listener;
        }

        [Fact]
        public void Should_Be_Able_Create_Sequences_From_Rows_In_Board() {
            Piece square = new(_pieceFactory.CreateNormalPiece("square"));
            Piece circle = new(_pieceFactory.CreateNormalPiece("circle"));
            Piece triangle = new(_pieceFactory.CreateNormalPiece("triangle"));
            Piece prism = new(_pieceFactory.CreateNormalPiece("prism"));
            Piece special = new(_pieceFactory.CreateSpecialPiece("special"));


            List<Piece> pieces = [square, circle, triangle, prism, special];

            var board = new Board(8, 7, 10);

            board.AddAvailablePieces(pieces).PrepareGridCells().FillInitialBoard(true);

            Assert.True(board.CreateSequenceFromRow(1).Cells.All(cell => cell.Row.Equals(1)));
            Assert.True(board.CreateSequenceFromRowOfPieceType(1, typeof(NormalPieceType)).Cells.All(cell => cell.Piece.Type is NormalPieceType));
            Assert.True(board.CreateSequenceFromRowOfShape(1, "triangle").Cells.All(cell => cell.Piece.Type.Shape.Equals("triangle")));
        }

        [Fact]
        public void Should_Be_Able_Create_Sequences_From_Columns_In_Board() {
            Piece square = new(_pieceFactory.CreateNormalPiece("square"));
            Piece circle = new(_pieceFactory.CreateNormalPiece("circle"));
            Piece triangle = new(_pieceFactory.CreateNormalPiece("triangle"));
            Piece prism = new(_pieceFactory.CreateNormalPiece("prism"));
            Piece special = new(_pieceFactory.CreateSpecialPiece("special"));


            List<Piece> pieces = [square, circle, triangle, prism, special];

            var board = new Board(8, 7, 10);

            board.AddAvailablePieces(pieces).PrepareGridCells().FillInitialBoard(true);

            Assert.True(board.CreateSequenceFromColumn(1).Cells.All(cell => cell.Column.Equals(1)));
            Assert.True(board.CreateSequenceFromColumnOfPieceType(1, typeof(NormalPieceType)).Cells.All(cell => cell.Piece.Type is NormalPieceType));
            Assert.True(board.CreateSequenceFromColumnOfShape(1, "triangle").Cells.All(cell => cell.Piece.Type.Shape.Equals("triangle")));
        }

        [Fact]
        public void Should_Be_Able_Create_Sequences_Of_Cells_With_Selected_Type() {
            Piece square = new(_pieceFactory.CreateNormalPiece("square"));
            Piece circle = new(_pieceFactory.CreateNormalPiece("circle"));
            Piece triangle = new(_pieceFactory.CreateNormalPiece("triangle"));
            Piece prism = new(_pieceFactory.CreateNormalPiece("prism"));
            Piece special = new(_pieceFactory.CreateSpecialPiece("special"));

            List<Piece> pieces = [square, circle, triangle, prism, special];

            var board = new Board(8, 7, 10);

            board.AddAvailablePieces(pieces).PrepareGridCells().FillInitialBoard(true);

            Assert.True(board.CreateSequenceOfCellsWithPieceType(typeof(NormalPieceType)).Cells.All(cell => cell.Piece.Type is NormalPieceType));
            Assert.True(board.CreateSequenceOfCellsWithShape("circle").Cells.All(cell => cell.Piece.Type.Shape.Equals("circle")));
        }

        [Fact]
        public void Should_Throw_Exception_When_Try_To_Move_Pieces_And_Fill_When_Grid_Cells_Are_Not_Prepared() {
            var board = new Board(8, 7, 10);

            Assert.Throws<ArgumentException>(() => board.MovePiecesAndFillEmptyCells());

        }

        [Fact]
        public void Should_Be_Able_To_Calculate_The_Pending_Fall_Moves_When_Sequence_Is_Consumed() {
            Piece square = new(_pieceFactory.CreateNormalPiece("square"));
            Piece circle = new(_pieceFactory.CreateNormalPiece("circle"));
            Piece triangle = new(_pieceFactory.CreateNormalPiece("triangle"));
            Piece prism = new(_pieceFactory.CreateNormalPiece("prism"));
            Piece special = new(_pieceFactory.CreateSpecialPiece("special"));

            List<Piece> pieces = [square, circle, triangle, prism, special];

            var board = new Board(8, 7, 10);

            board.AddAvailablePieces(pieces).PrepareGridCells().FillInitialBoard(true);

            Assert.Empty(board.PendingFallMoves());

            board.CreateSequenceFromRow(2).Consume();

            Assert.Equal(board.GridWidth, board.EmptyCells().Count);
            Assert.Equal(board.GridWidth, board.PendingFallMoves().Count);
        }

        [Fact]
        public void Should_Create_A_Virtual_Board_Only_With_Fill_Updates_When_First_Row_Consumed() {
            Piece square = new(_pieceFactory.CreateNormalPiece("square"));
            Piece circle = new(_pieceFactory.CreateNormalPiece("circle"));
            Piece triangle = new(_pieceFactory.CreateNormalPiece("triangle"));
            Piece prism = new(_pieceFactory.CreateNormalPiece("prism"));
            Piece special = new(_pieceFactory.CreateSpecialPiece("special"));

            List<Piece> pieces = [square, circle, triangle, prism, special];

            var board = new Board(8, 7, 10);

            board.AddAvailablePieces(pieces).PrepareGridCells().FillInitialBoard(true);

            board.CreateSequenceFromRow(0).Consume();

            var virtualBoard = board.MovePiecesAndFillEmptyCells();

            //No movements in the first row, only fills
            Assert.True(virtualBoard.Updates.All(update => update.CurrentUpdateType.Equals(BoardCellUpdate.UPDATE_TYPE.FILL) && update.CellPieceFill.Cell.Row.Equals(0)));
            Assert.Equal(board.GridWidth, virtualBoard.Updates.Count);

            // Virtual board has no empty cells after the fill but the original board should keep unaltered.
            Assert.Empty(virtualBoard.EmptyCells());
            Assert.Equal(board.GridWidth, board.EmptyCells().Count);
        }

        [Fact]
        public void Should_Create_A_Virtual_Board_Only_With_Fill_Updates_On_Entire_Column_Consumed() {
            Piece square = new(_pieceFactory.CreateNormalPiece("square"));
            Piece circle = new(_pieceFactory.CreateNormalPiece("circle"));
            Piece triangle = new(_pieceFactory.CreateNormalPiece("triangle"));
            Piece prism = new(_pieceFactory.CreateNormalPiece("prism"));
            Piece special = new(_pieceFactory.CreateSpecialPiece("special"));

            List<Piece> pieces = [square, circle, triangle, prism, special];

            var board = new Board(8, 7, 10);

            board.AddAvailablePieces(pieces).PrepareGridCells().FillInitialBoard(true);

            board.CreateSequenceFromColumn(2).Consume();

            var virtualBoard = board.MovePiecesAndFillEmptyCells();

            //No movements in the first row, only fills
            Assert.True(virtualBoard.Updates.All(update => update.CurrentUpdateType.Equals(BoardCellUpdate.UPDATE_TYPE.FILL) && update.CellPieceFill.Cell.Column.Equals(2)));
            Assert.Equal(board.GridHeight, virtualBoard.Updates.Count);

            // Virtual board has no empty cells after the fill but the original board should keep unaltered.
            Assert.Empty(virtualBoard.EmptyCells());
            Assert.Equal(board.GridHeight, board.EmptyCells().Count);
        }

        [Fact]
        public void Should_Create_A_Virtual_Board_With_Movement_And_Fill() {
            Piece square = new(_pieceFactory.CreateNormalPiece("square"));
            Piece circle = new(_pieceFactory.CreateNormalPiece("circle"));
            Piece triangle = new(_pieceFactory.CreateNormalPiece("triangle"));
            Piece prism = new(_pieceFactory.CreateNormalPiece("prism"));
            Piece special = new(_pieceFactory.CreateSpecialPiece("special"));

            List<Piece> pieces = [square, circle, triangle, prism, special];

            var board = new Board(8, 7, 10);
            board.AddAvailablePieces(pieces).PrepareGridCells().FillInitialBoard(true);

            var sequence = new Sequence([board.Cell(1, 2), board.Cell(2, 2), board.Cell(3, 2)], Sequence.SHAPES.HORIZONTAL);

            //No updates in an initial board
            Assert.Empty(board.MovePiecesAndFillEmptyCells().Updates);

            sequence.Consume();

            Assert.Equal(sequence.Size(), board.EmptyCells().Count);

            var virtualBoard = board.MovePiecesAndFillEmptyCells();

            // 6 Movements that represent moving down the row 0 and 1 after consuming the number 2.
            Assert.Equal(sequence.Size() * 2, virtualBoard.MovementUpdates().Count);
            Assert.Equal(sequence.Size(), virtualBoard.FillUpdates().Count);
            Assert.Empty(virtualBoard.EmptyCells());
        }

        [Fact]
        public void Should_Create_A_Virtual_Board_Only_With_Fill_When_Mode_Is_In_Place() {
            Piece square = new(_pieceFactory.CreateNormalPiece("square"));
            Piece circle = new(_pieceFactory.CreateNormalPiece("circle"));
            Piece triangle = new(_pieceFactory.CreateNormalPiece("triangle"));
            Piece prism = new(_pieceFactory.CreateNormalPiece("prism"));
            Piece special = new(_pieceFactory.CreateSpecialPiece("special"));

            List<Piece> pieces = [square, circle, triangle, prism, special];

            var board = new Board(8, 7, 10);
            board.ChangeFillMode(Board.FILL_MODES.IN_PLACE).AddAvailablePieces(pieces).PrepareGridCells().FillInitialBoard(true);

            var sequence = new Sequence([board.Cell(1, 2), board.Cell(2, 2), board.Cell(3, 2)], Sequence.SHAPES.HORIZONTAL);

            //No updates in an initial board
            Assert.Empty(board.MovePiecesAndFillEmptyCells().Updates);

            sequence.Consume();

            Assert.Equal(sequence.Size(), board.EmptyCells().Count);

            var virtualBoard = board.MovePiecesAndFillEmptyCells();

            // No movements applied when fill it's IN_PLACE
            Assert.Empty(virtualBoard.MovementUpdates());
            Assert.Equal(sequence.Size(), virtualBoard.FillUpdates().Count);
            Assert.Empty(virtualBoard.EmptyCells());
        }
    }

}