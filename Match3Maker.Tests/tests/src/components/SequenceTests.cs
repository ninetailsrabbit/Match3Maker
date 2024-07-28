using Match3Maker;
using Xunit;

namespace Match3MakerTests {
    public class SequenceTests {
        private readonly PieceFactory _pieceFactory = new();

        [Fact]
        public void Should_Create_Sequence_Correctly() {
            var cells = new List<GridCell>() {
                new(1, 2, new Piece(_pieceFactory.CreateNormalPiece("triangle"))),
                new(1, 3, new Piece(_pieceFactory.CreateNormalPiece("triangle"))),
                new(1, 4, new Piece(_pieceFactory.CreateNormalPiece("triangle")))
            };

            var sequence = new Sequence(cells, Sequence.Shapes.Vertical);

            Assert.Equal(sequence.Cells.Count, cells.Count);
            Assert.True(sequence.Cells.All(cell => cell.Column.Equals(1)));
            Assert.Equal(Sequence.Shapes.Vertical, sequence.Shape);
            Assert.False(sequence.IsHorizontal());
            Assert.True(sequence.IsVertical());
            Assert.False(sequence.IsTShape());
            Assert.False(sequence.IsLShape());
        }

        [Fact]
        public void Should_Create_Sequence_Without_Empty_Cells() {
            var cells = new List<GridCell>() {
                new(1, 2, new Piece(_pieceFactory.CreateNormalPiece("triangle"))),
                new(1, 3, new Piece(_pieceFactory.CreateNormalPiece("triangle"))),
                new(1, 4, new Piece(_pieceFactory.CreateNormalPiece("triangle")))
            };

            var emptyCell = new GridCell(1, 5);

            var sequence = new Sequence([.. cells, emptyCell], Sequence.Shapes.Horizontal);

            Assert.Equal(cells.Count, sequence.Size());
            Assert.True(sequence.Cells.All(cell => cell.HasPiece()));
        }

        [Fact]
        public void Should_Get_All_The_Pieces_From_Cells() {
            var cells = new List<GridCell>() {
                new(1, 2, new Piece(_pieceFactory.CreateNormalPiece("triangle"))),
                new(1, 3, new Piece(_pieceFactory.CreateNormalPiece("triangle"))), new(1, 4,
                new Piece(_pieceFactory.CreateNormalPiece("triangle")))
            };

            var sequence = new Sequence(cells, Sequence.Shapes.Vertical);

            Assert.True(sequence.Pieces().All(piece => piece.Type.Shape.Equals("triangle")));
            Assert.Equal(cells.Count, sequence.Pieces().Count);
        }

        [Fact]
        public void Should_Remove_Pieces_From_Cell_When_Consumed() {
            string shape = "square";

            var cells = new List<GridCell>() {
                new(2, 1, new Piece(_pieceFactory.CreateNormalPiece(shape))),
                new(2, 2, new Piece(_pieceFactory.CreateNormalPiece(shape))),
                new(2, 3, new Piece(_pieceFactory.CreateNormalPiece(shape)))
            };

            var sequence = new Sequence(cells, Sequence.Shapes.Horizontal);

            Assert.Equal(sequence.Cells.Count, cells.Count);
            Assert.Equal(cells.Count, sequence.Pieces().Count);
            Assert.True(sequence.Cells.All(cell => cell.HasPiece() && cell.Piece.MatchWith(new Piece(_pieceFactory.CreateNormalPiece(shape)))));

            sequence.Consume();

            Assert.True(sequence.Cells.All(cell => cell.IsEmpty()));
            Assert.Empty(sequence.Pieces());
        }

        [Fact]
        public void Sequence_Pieces_Can_Be_Retrieved() {
            string shape = "square";

            var cells = new List<GridCell>() {
                new(2, 1, new Piece(_pieceFactory.CreateNormalPiece(shape))),
                new(2, 2, new Piece(_pieceFactory.CreateNormalPiece(shape))),
                new(2, 3, new Piece(_pieceFactory.CreateNormalPiece(shape)))
            };
            var sequence = new Sequence(cells, Sequence.Shapes.Vertical);

            Assert.Equal(cells.Count, sequence.Pieces().Count);
            Assert.True(sequence.Pieces().All(piece => piece.MatchWith(new Piece(_pieceFactory.CreateNormalPiece(shape)))));
        }

        [Fact]
        public void Should_Detect_Shape_Vertical_Automatically_When_Null() {
            string shape = "square";

            var cells = new List<GridCell>() {
                new(2, 1, new Piece(_pieceFactory.CreateNormalPiece(shape))),
                new(2, 2, new Piece(_pieceFactory.CreateNormalPiece(shape))),
                new(2, 3, new Piece(_pieceFactory.CreateNormalPiece(shape)))
            };

            var sequence = new Sequence(cells);

            Assert.Equal(Sequence.Shapes.Vertical, sequence.Shape);


        }

        [Fact]
        public void Should_Detect_Shape_Horizontal_Automatically_When_Null() {
            string shape = "square";

            var cells = new List<GridCell>() {
               new(2, 1, new Piece(_pieceFactory.CreateNormalPiece(shape))),
                new(3, 1, new Piece(_pieceFactory.CreateNormalPiece(shape))),
                new(4, 1, new Piece(_pieceFactory.CreateNormalPiece(shape)))
            };

            var sequence = new Sequence(cells);

            Assert.Equal(Sequence.Shapes.Horizontal, sequence.Shape);
        }

        [Fact]
        public void Should_Detect_Shape_Diagonal_Automatically_When_Null() {
            string shape = "square";

            var cells = new List<GridCell>() {
                new(2, 1, new Piece(_pieceFactory.CreateNormalPiece(shape))),
                new(3, 2, new Piece(_pieceFactory.CreateNormalPiece(shape))),
                new(4, 3, new Piece(_pieceFactory.CreateNormalPiece(shape)))
            };

            var sequence = new Sequence(cells);

            Assert.Equal(Sequence.Shapes.Diagonal, sequence.Shape);
        }


        [Fact]
        public void Should_Detect_Shape_Irregular_Automatically_When_Null() {
            string shape = "square";

            var cells = new List<GridCell>() {
                new(2, 4, new Piece(_pieceFactory.CreateNormalPiece(shape))),
                new(6, 3, new Piece(_pieceFactory.CreateNormalPiece(shape))),
                new(7, 5, new Piece(_pieceFactory.CreateNormalPiece(shape)))
            };

            var sequence = new Sequence(cells);

            Assert.Equal(Sequence.Shapes.Irregular, sequence.Shape);
        }

        [Fact]
        public void Should_Retrieve_Correct_Horizontal_Edge_Cells() {
            string shape = "triangle";

            var cells = new List<GridCell>() {
                new(1, 1, new Piece(_pieceFactory.CreateNormalPiece(shape))),
                new(2, 1, new Piece(_pieceFactory.CreateNormalPiece(shape))),
                new(3, 1, new Piece(_pieceFactory.CreateNormalPiece(shape)))
            };
            var sequence = new Sequence(cells, Sequence.Shapes.Horizontal);

            Assert.Null(sequence.TopEdgeCell());
            Assert.Null(sequence.BottomEdgeCell());

            var leftEdgeCell = sequence.LeftEdgeCell();

            Assert.NotNull(leftEdgeCell);
            Assert.Equal(1, leftEdgeCell?.Column);
            Assert.Equal(1, leftEdgeCell?.Row);
            Assert.Equal(shape, leftEdgeCell?.Piece?.Type.Shape);

            var rightEdgeCell = sequence.RightEdgeCell();

            Assert.NotNull(rightEdgeCell);
            Assert.Equal(3, rightEdgeCell?.Column);
            Assert.Equal(1, rightEdgeCell?.Row);
            Assert.Equal(shape, rightEdgeCell?.Piece?.Type.Shape);

            var middleCell = sequence.MiddleCell();

            Assert.Equal(2, middleCell?.Column);
            Assert.Equal(1, middleCell?.Row);
            Assert.Equal(shape, middleCell?.Piece?.Type.Shape);
        }

        [Fact]
        public void Should_Retrieve_Correct_Vertical_Edge_Cells() {
            string shape = "triangle";

            var cells = new List<GridCell>() {
                new(1, 1, new Piece(_pieceFactory.CreateNormalPiece(shape))),
                new(1, 2, new Piece(_pieceFactory.CreateNormalPiece(shape))),
                new(1, 3, new Piece(_pieceFactory.CreateNormalPiece(shape)))
            };
            var sequence = new Sequence(cells, Sequence.Shapes.Vertical);

            Assert.Null(sequence.LeftEdgeCell());
            Assert.Null(sequence.RightEdgeCell());

            var topEdgeCell = sequence.TopEdgeCell();

            Assert.NotNull(topEdgeCell);
            Assert.Equal(1, topEdgeCell?.Column);
            Assert.Equal(1, topEdgeCell?.Row);
            Assert.Equal(shape, topEdgeCell?.Piece?.Type.Shape);

            var bottomEdgeCell = sequence.BottomEdgeCell();

            Assert.NotNull(bottomEdgeCell);
            Assert.Equal(1, bottomEdgeCell?.Column);
            Assert.Equal(3, bottomEdgeCell?.Row);
            Assert.Equal(shape, bottomEdgeCell?.Piece?.Type.Shape);

            var middleCell = sequence.MiddleCell();

            Assert.Equal(1, middleCell?.Column);
            Assert.Equal(2, middleCell?.Row);
            Assert.Equal(shape, middleCell?.Piece?.Type.Shape);
        }
    }
}