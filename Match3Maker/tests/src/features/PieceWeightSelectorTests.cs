//using Match3Maker;
//using Xunit;

//namespace Match3Tests {
//    public class PieceWeightGeneratorTests {

//        private readonly PieceWeightGenerator _pieceWeightGenerator = new();

//        [Fact]
//        public void Should_Throw_Argument_Exception_When_Pieces_Are_Empty() {
           
//            var exception = Assert.Throws<ArgumentException>(() => _pieceWeightGenerator.Roll([]));

//            Assert.Equal("PieceWeightGenerator: The pieces to roll cannot be empty", exception.Message);
//        }

//        [Fact]
//        public void Should_Roll_Always_At_Least_One_Piece_Of_Type_Provided() {
//            List<Piece> pieces = [
//                new Piece("triangle", Piece.TYPES.NORMAL, 2f),
//                new Piece("square", Piece.TYPES.NORMAL, 1.5f),
//                new Piece("circle", Piece.TYPES.NORMAL, 1f),
//                new Piece("fence", Piece.TYPES.OBSTACLE, 1.3f),
//                new Piece("ice-block", Piece.TYPES.OBSTACLE, 1.6f),
//                new Piece("special-circle", Piece.TYPES.SPECIAL, .9f),
//            ];

//            Piece piece = _pieceWeightGenerator.Roll(pieces, [Piece.TYPES.NORMAL]);

//            Assert.NotNull(piece);
//            Assert.True(piece.Type.Equals(Piece.TYPES.NORMAL));

//            piece = _pieceWeightGenerator.Roll(pieces, [Piece.TYPES.OBSTACLE]);

//            Assert.NotNull(piece);
//            Assert.True(piece.Type.Equals(Piece.TYPES.OBSTACLE));

//            piece = _pieceWeightGenerator.Roll(pieces, [Piece.TYPES.SPECIAL]);

//            Assert.NotNull(piece);
//            Assert.True(piece.Type.Equals(Piece.TYPES.SPECIAL));
//        }
//    }

//}