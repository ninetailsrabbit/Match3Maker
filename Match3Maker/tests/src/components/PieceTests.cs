using Match3Maker;
using System.ComponentModel;
using Xunit;

namespace Match3Tests {
    public class PieceTests {

        private readonly PieceFactory _pieceFactory = new();

        [Fact]
        public void Should_Be_Normal_And_Unlocked_On_Default_Initialization() {
            var piece = new Piece(_pieceFactory.CreateNormalPiece("square"));

            Assert.Equal(typeof(NormalPieceType), piece.Type.GetType());
            Assert.False(piece.Locked);
        }

        [Fact]
        public void Should_Be_Created_From_Static_Constructor() {
            var piece = new Piece(_pieceFactory.CreateNormalPiece("circle"), 2.5f);

            Assert.Equal(typeof(NormalPieceType), piece.Type.GetType());

            Assert.Equal("circle", piece.Type.Shape);
            Assert.False(piece.Locked);
        }

        [Fact]
        public void Should_Be_Notify_When_Locked_Property_Changes() {
            List<string?> receivedEvents = [];

            var piece = new Piece(_pieceFactory.CreateNormalPiece("square"));

            piece.PropertyChanged += delegate (object? sender, PropertyChangedEventArgs e) {
                receivedEvents.Add(e.PropertyName);
            };

            piece.Lock();

            Assert.Contains(nameof(piece.Locked), receivedEvents);
            Assert.Single(receivedEvents);

            piece.Lock();
            // Should still single, no new events it's received when value did not change
            Assert.Single(receivedEvents);
        }

        [Fact]
        public void Should_Match_With_Similar_Pieces() {
            var piece = new Piece(_pieceFactory.CreateNormalPiece("square"));
            var piece2 = new Piece(_pieceFactory.CreateNormalPiece("SquAre"));
            var piece3 = new Piece(_pieceFactory.CreateNormalPiece("circle"));
            var piece4 = new Piece(_pieceFactory.CreateSpecialPiece("square"));

            Assert.True(piece.MatchWith(piece2));
            Assert.False(piece.MatchWith(piece3));
            Assert.True(piece.MatchWith(piece4));
        }

    }

}