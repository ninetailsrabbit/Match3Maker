using System.Drawing;

namespace Match3Maker {
    public interface IPieceType {
        public string Shape {  get; set; }
        public Color? Color { get; set; }
        public bool MatchWith(Piece piece);
        public bool CanBeShuffled();
        public bool CanBeMoved();
    }
}
