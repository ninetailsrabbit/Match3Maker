using Match3Maker;
using System.Drawing;

namespace Match3MakerTests {
    public class PieceFactory {
        public IPieceType CreateNormalPiece(string shape, Color? color = null) {
            return new NormalPieceType(shape, color);
        }

        public IPieceType CreateSpecialPiece(string shape, Color? color = null) {
            return new SpecialPieceType(shape, color);
        }

        public IPieceType CreateObstaclePiece(string shape, Color? color = null) {
            return new ObstaclePieceType(shape, color);
        }
    }

}