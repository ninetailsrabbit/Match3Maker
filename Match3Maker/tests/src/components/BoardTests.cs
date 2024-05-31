
using Match3Maker;
using System.Numerics;
using Xunit;

namespace Match3Tests {

    public class BoardTests {
        [Fact]
        public void Grid_Dimensions_Are_Set_Correctly_On_New_Board() {
            int width = 5;
            int height = 6;

            var board = new Board(width, height);

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
            var board = new Board(2, 1);

            Assert.Equal(Board.MIN_GRID_WIDTH, board.GridWidth);
            Assert.Equal(Board.MIN_GRID_HEIGHT, board.GridHeight);
            Assert.Equal(Board.MIN_GRID_HEIGHT * Board.MIN_GRID_WIDTH, board.Dimensions());
        }

        [Fact]
        public void Should_Be_Able_To_Generate_Grid_Cells_Based_On_Size() {
            var board = new Board(8, 7);

            Assert.Empty(board.GridCells);

            board.PrepareGridCells();

            Assert.Equal(board.GridWidth, board.GridCells.Count);
            Assert.Equal(board.Dimensions(), board.GridCells.SelectMany(cells => cells).Count());
        }
    }

}