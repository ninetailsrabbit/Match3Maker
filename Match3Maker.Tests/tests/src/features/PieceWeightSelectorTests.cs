using Match3Maker;
using Xunit;

namespace Match3MakerTests {
    public class PieceWeightGeneratorTests {

        private readonly PieceWeightGenerator _pieceWeightGenerator = new();
        private readonly PieceFactory _pieceFactory = new();

        [Fact]
        public void Should_Throw_Argument_Exception_When_Pieces_Are_Empty() {

            var exception = Assert.Throws<ArgumentException>(() => _pieceWeightGenerator.Roll([]));

            Assert.Equal("PieceWeightGenerator: The pieces to roll cannot be empty", exception.Message);
        }

        [Fact]
        public void Should_Roll_Always_At_Least_One_Piece_Of_Type_Provided() {
            List<Piece> pieces = [
                new Piece(_pieceFactory.CreateNormalPiece("triangle"), 2f),
                        new Piece(_pieceFactory.CreateNormalPiece("square"), 1.5f),
                        new Piece(_pieceFactory.CreateNormalPiece("circle"), 1f),
                        new Piece(_pieceFactory.CreateObstaclePiece("block"), 1.3f),
                        new Piece(_pieceFactory.CreateObstaclePiece("ice-block"), 1.6f),
                        new Piece(_pieceFactory.CreateSpecialPiece("special"), .9f),
                    ];

            Piece piece = _pieceWeightGenerator.Roll(pieces, [typeof(NormalPieceType)]);

            Assert.NotNull(piece);
            Assert.True(piece.Type.GetType().Equals(typeof(NormalPieceType)));

            piece = _pieceWeightGenerator.Roll(pieces, [typeof(ObstaclePieceType)]);
            Assert.NotNull(piece);
            Assert.True(piece.Type.GetType().Equals(typeof(ObstaclePieceType)));

            piece = _pieceWeightGenerator.Roll(pieces, [typeof(SpecialPieceType)]);
            Assert.NotNull(piece);
            Assert.True(piece.Type.GetType().Equals(typeof(SpecialPieceType)));
        }
    }

}