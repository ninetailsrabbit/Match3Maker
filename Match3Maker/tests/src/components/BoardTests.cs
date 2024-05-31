
using Match3Maker;
using Moq;
using System.Numerics;
using Xunit;

namespace Match3Tests {

    public class BoardTests {
        private readonly Mock<IPieceSelector> _mockPieceSelector = new();

        [Fact]
        public void Can_Be_Created_With_Static_Method_Create() {
            int width = 5;
            int height = 6;

            var board = Board.Create(width, height, _mockPieceSelector.Object);

            Assert.Equal(width, board.GridWidth);
            Assert.Equal(height, board.GridHeight);
            Assert.Equal(width * height, board.Dimensions());
        }

        [Fact]
        public void Grid_Dimensions_Are_Set_Correctly_On_New_Board() {
            int width = 5;
            int height = 6;

            var board = new Board(width, height, _mockPieceSelector.Object);

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
            var board = new Board(2, 1, _mockPieceSelector.Object);

            Assert.Equal(Board.MIN_GRID_WIDTH, board.GridWidth);
            Assert.Equal(Board.MIN_GRID_HEIGHT, board.GridHeight);
            Assert.Equal(Board.MIN_GRID_HEIGHT * Board.MIN_GRID_WIDTH, board.Dimensions());
        }

        [Fact]
        public void Should_Be_Able_To_Generate_Grid_Cells_Based_On_Size() {
            var board = new Board(8, 7, _mockPieceSelector.Object);

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
            var board = new Board(8, 7, _mockPieceSelector.Object);

            Assert.Empty(board.GridCells);

            board.PrepareGridCells();

            Assert.Equal(board.GridWidth, board.GridCells.Count);
            Assert.Equal(board.Dimensions(), board.GridCells.SelectMany(cells => cells).Count());

            board.ChangeGridSize(new Vector2(10, 10)).PrepareGridCells(true);

            Assert.Equal(board.GridWidth, board.GridCells.Count);
            Assert.Equal(board.Dimensions(), board.GridCells.SelectMany(cells => cells).Count());
        }

        [Fact]
        public void Should_Raise_Prepared_Board_Event_When_Prepared_Grid_Cells() {
            var board = new Board(5, 6, _mockPieceSelector.Object);

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
            var board = new Board(4, 5, _mockPieceSelector.Object);
            board.PrepareGridCells();

            Assert.Null(board.Cell(6, 3));
            Assert.Null(board.Cell(0, 9));
            Assert.Null(board.Cell(8, 8));
            Assert.Null(board.Cell(-1, 0));
            Assert.Null(board.Cell(0, -1));
            Assert.Null(board.Cell(-1, -1));

            GridCell? originCell = board.Cell(1, 2);
            Assert.NotNull(originCell);

            GridCell upperCell = board.UpperCellFrom(originCell);
            Assert.True(originCell.IsColumnNeighbourOf(upperCell));
            Assert.Equal(originCell.Row - 1, upperCell.Row);
            Assert.Equal(upperCell.Column, originCell.Column);

            GridCell bottomCell = board.BottomCellFrom(originCell);
            Assert.True(originCell.IsColumnNeighbourOf(bottomCell));
            Assert.Equal(originCell.Row + 1, bottomCell.Row);
            Assert.Equal(bottomCell.Column, originCell.Column);

            GridCell rightCell = board.RightCellFrom(originCell);
            Assert.True(originCell.IsRowNeighbourOf(rightCell));
            Assert.Equal(originCell.Column + 1, rightCell.Column);
            Assert.Equal(rightCell.Row, originCell.Row);

            GridCell leftCell = board.LeftCellFrom(originCell);
            Assert.True(originCell.IsRowNeighbourOf(leftCell));
            Assert.Equal(originCell.Column - 1, leftCell.Column);
            Assert.Equal(leftCell.Row, originCell.Row);
        }
    }

}