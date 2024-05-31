using Xunit;
using Match3Maker;

namespace Match3Tests {
    public class SequenceTests {
        [Fact]
        public void Should_Create_Sequence_Correctly() {
            var cells = new List<GridCell>() { new(1, 2, new Piece("triangle")), new(1, 3, new Piece("triangle")), new(1, 4, new Piece("triangle")) };
            var sequence = new Sequence(cells, Sequence.SHAPES.VERTICAL);

            Assert.Equal(sequence.Cells.Count, cells.Count);
            Assert.True(sequence.Cells.All(cell => cell.Column.Equals(1)));
            Assert.Equal(Sequence.SHAPES.VERTICAL, sequence.Shape);
            Assert.False(sequence.IsHorizontal());
            Assert.True(sequence.IsVertical());
            Assert.False(sequence.IsTShape());
            Assert.False(sequence.IsLShape());
        }

        [Fact]
        public void Should_Create_Sequence_Without_Empty_Cells() {
            var cells = new List<GridCell>() {new(1, 2, new Piece("triangle")), new(1, 3, new Piece("triangle")), new(1, 4, new Piece("triangle")) };
            var emptyCell = new GridCell(1, 5);

            var sequence = new Sequence([..cells, emptyCell], Sequence.SHAPES.HORIZONTAL);

            Assert.Equal(cells.Count, sequence.Size());
            Assert.True(sequence.Cells.All(cell => cell.HasPiece()));
        }

        [Fact]
        public void Should_Get_All_The_Pieces_From_Cells() {
            var cells = new List<GridCell>() { new(1, 2, new Piece("triangle")), new(1, 3, new Piece("triangle")), new(1, 4, new Piece("triangle")) };
            var sequence = new Sequence(cells, Sequence.SHAPES.VERTICAL);

            Assert.True(sequence.Pieces().All(piece => piece.Shape.Equals("triangle")));
            Assert.Equal(cells.Count, sequence.Pieces().Count);
        }

        [Fact]
        public void Should_Remove_Pieces_From_Cell_When_Consumed() {
            string shape = "square";

            var cells = new List<GridCell>() { new(2, 1, new Piece(shape)), new(2, 2, new Piece(shape)), new(2, 3, new Piece(shape)) };
            var sequence = new Sequence(cells, Sequence.SHAPES.HORIZONTAL);

            Assert.Equal(sequence.Cells.Count, cells.Count);
            Assert.Equal(cells.Count, sequence.Pieces().Count);
            Assert.True(sequence.Cells.All(cell => cell.HasPiece() && cell.Piece.MatchWith(new Piece(shape))));

            sequence.Consume();

            Assert.True(sequence.Cells.All(cell => cell.IsEmpty()));
            Assert.Empty(sequence.Pieces());
        }

        [Fact]
        public void Sequence_Pieces_Can_Be_Retrieved() {
            string shape = "square";

            var cells = new List<GridCell>() { new(2, 1, new Piece(shape)), new(2, 2, new Piece(shape)), new(2, 3, new Piece(shape)) };
            var sequence = new Sequence(cells, Sequence.SHAPES.VERTICAL);

            Assert.Equal(cells.Count, sequence.Pieces().Count);
            Assert.True(sequence.Pieces().All(piece => piece.MatchWith(new Piece(shape))));
        }

        [Fact]
        public void Should_Retrieve_Correct_Horizontal_Edge_Cells() {
            string shape = "triangle";

            var cells = new List<GridCell>() { new(1, 1, new Piece(shape)), new(2, 1, new Piece(shape)), new(3, 1, new Piece(shape)) };
            var sequence = new Sequence(cells, Sequence.SHAPES.HORIZONTAL);

            Assert.Null(sequence.TopEdgeCell());
            Assert.Null(sequence.BottomEdgeCell());

            var leftEdgeCell = sequence.LeftEdgeCell();

            Assert.NotNull(leftEdgeCell);
            Assert.Equal(1, leftEdgeCell?.Column);
            Assert.Equal(1, leftEdgeCell?.Row);
            Assert.Equal(leftEdgeCell?.Piece?.Shape, shape);

            var rightEdgeCell = sequence.RightEdgeCell();

            Assert.NotNull(rightEdgeCell);
            Assert.Equal(3, rightEdgeCell?.Column);
            Assert.Equal(1, rightEdgeCell?.Row);
            Assert.Equal(rightEdgeCell?.Piece?.Shape, shape);

            var middleCell = sequence.MiddleCell();

            Assert.Equal(2, middleCell?.Column);
            Assert.Equal(1, middleCell?.Row);
            Assert.Equal(middleCell?.Piece?.Shape, shape);
        }

        [Fact]
        public void Should_Retrieve_Correct_Vertical_Edge_Cells() {
            string shape = "triangle";

            var cells = new List<GridCell>() { new(1, 1, new Piece(shape)), new(1, 2, new Piece(shape)), new(1, 3, new Piece(shape)) };
            var sequence = new Sequence(cells, Sequence.SHAPES.VERTICAL);

            Assert.Null(sequence.LeftEdgeCell());
            Assert.Null(sequence.RightEdgeCell());

            var topEdgeCell = sequence.TopEdgeCell();

            Assert.NotNull(topEdgeCell);
            Assert.Equal(1, topEdgeCell?.Column);
            Assert.Equal(1, topEdgeCell?.Row);
            Assert.Equal(topEdgeCell?.Piece?.Shape, shape);

            var bottomEdgeCell = sequence.BottomEdgeCell();

            Assert.NotNull(bottomEdgeCell);
            Assert.Equal(1, bottomEdgeCell?.Column);
            Assert.Equal(3, bottomEdgeCell?.Row);
            Assert.Equal(bottomEdgeCell?.Piece?.Shape, shape);

            var middleCell = sequence.MiddleCell();

            Assert.Equal(1, middleCell?.Column);
            Assert.Equal(2, middleCell?.Row);
            Assert.Equal(middleCell?.Piece?.Shape, shape);
        }
    }
}