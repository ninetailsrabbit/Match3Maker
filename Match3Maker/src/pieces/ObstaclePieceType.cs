using System.Drawing;

namespace Match3Maker {
    public class ObstaclePieceType : IPieceType {
        public string Shape { get => _shape; set => _shape = value; }
        public Color? Color { get => _color; set => _color = value; }

        private string _shape;
        private Color? _color = null;

        public ObstaclePieceType(string shape, Color? color = null) {
            Shape = shape;
            Color = color;
        }

        public bool MatchWith(Piece piece) => false;
        public bool CanBeShuffled() => false;
        public bool CanBeMoved() => false;

    }

}