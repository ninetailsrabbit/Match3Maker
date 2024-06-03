using System.Drawing;

namespace Match3Maker {
    public class NormalPieceType : IPieceType {
        public string Shape { get => _shape; set => _shape = value; }
        public Color? Color { get => _color; set => _color = value; }

        private string _shape;
        private Color? _color = null;

        public NormalPieceType(string shape, Color? color = null) {
            Shape = shape;
            Color = color;
        }

        public bool MatchWith(Piece piece) {
            return Shape.Trim().Equals(piece.Type.Shape, StringComparison.OrdinalIgnoreCase) && Color.Equals(piece.Type.Color);
        }

        public bool CanBeShuffled() => true;
        public bool CanBeMoved() => true;
    }
}
