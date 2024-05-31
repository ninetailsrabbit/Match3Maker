using Match3Maker;
using System.ComponentModel;
using Xunit;

namespace Match3Tests {
    public class PieceTests {

        [Fact]
        public void Should_Be_Normal_And_Unlocked_On_Default_Initialization() {
            var piece = new Piece("square", Piece.TYPES.NORMAL);

            Assert.Equal(Piece.TYPES.NORMAL, piece.Type);
            Assert.False(piece.Locked);
        }

        [Fact]
        public void Should_Be_Created_From_Static_Constructor() {
            var pieceWeight = new PieceWeight("circle", Piece.TYPES.NORMAL, 2.5f);
            var piece = Piece.From(pieceWeight);

            Assert.Equal(Piece.TYPES.NORMAL, piece.Type);
            Assert.Equal("circle", piece.Shape);
            Assert.False(piece.Locked);
        }

        [Fact]
        public void Should_Be_Initialized_Correctly() {
            var pieceA = new Piece("square", Piece.TYPES.OBSTACLE);

            Assert.Equal("square", pieceA.Shape);
            Assert.Equal(Piece.TYPES.OBSTACLE, pieceA.Type);
            Assert.True(pieceA.Locked);

            var pieceB = new Piece("square", Piece.TYPES.NORMAL);

            Assert.Equal("square", pieceB.Shape);
            Assert.Equal(Piece.TYPES.NORMAL, pieceB.Type);
            Assert.False(pieceB.Locked);
        }

        [Fact]
        public void Should_Be_Notify_When_Locked_Property_Changes() {
            List<string?> receivedEvents = [];

            var piece = new Piece("square");

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
            var piece = new Piece("square");
            var piece2 = new Piece("SquAre");
            var piece3 = new Piece("circle");
            var piece4 = new Piece("square", Piece.TYPES.SPECIAL);


            Assert.True(piece.MatchWith(piece2));
            Assert.False(piece.MatchWith(piece3));
            Assert.False(piece.MatchWith(piece4));
        }

    }

}