using Match3Maker;
using Xunit;

namespace Match3Tests {
    public class PieceWeightSelectorTests {

        private readonly PieceWeightSelector _pieceWeightSelector = new();

        [Fact]
        public void Should_Throw_Argument_Exception_When_Pieces_Are_Empty() {
           
            var exception = Assert.Throws<ArgumentException>(() => _pieceWeightSelector.Roll([]));

            Assert.Equal("PieceWeightSelector: The pieces to roll cannot be empty", exception.Message);
        }

        [Fact]
        public void Should_Roll_Always_At_Least_One_Piece_Of_Type_Provided() {
            List<PieceWeight> pieces = [
                new PieceWeight("triangle", Piece.TYPES.NORMAL, 2f),
                new PieceWeight("square", Piece.TYPES.NORMAL, 1.5f),
                new PieceWeight("circle", Piece.TYPES.NORMAL, 1f),
                new PieceWeight("fence", Piece.TYPES.OBSTACLE, 1.3f),
                new PieceWeight("ice-block", Piece.TYPES.OBSTACLE, 1.6f),
                new PieceWeight("special-circle", Piece.TYPES.SPECIAL, .9f),
            ];

            Piece piece = _pieceWeightSelector.Roll(pieces, [Piece.TYPES.NORMAL]);

            Assert.NotNull(piece);
            Assert.True(piece.Type.Equals(Piece.TYPES.NORMAL));

            piece = _pieceWeightSelector.Roll(pieces, [Piece.TYPES.OBSTACLE]);

            Assert.NotNull(piece);
            Assert.True(piece.Type.Equals(Piece.TYPES.OBSTACLE));

            piece = _pieceWeightSelector.Roll(pieces, [Piece.TYPES.SPECIAL]);

            Assert.NotNull(piece);
            Assert.True(piece.Type.Equals(Piece.TYPES.SPECIAL));
        }
    }

}