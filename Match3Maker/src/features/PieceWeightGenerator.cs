using System.Diagnostics;
using SystemExtensions;

namespace Match3Maker {
    public sealed class PieceWeightGenerator : IPieceGenerator {
        private readonly Random _rng = new();

#nullable enable
        public Piece Roll(List<Piece> pieces, IEnumerable<IPieceType>? only = null) {
            if (pieces.IsEmpty())
                throw new ArgumentException("PieceWeightGenerator: The pieces to roll cannot be empty");

            float totalWeight = 0f;
            Piece? selectedPiece = null;

            var selectedPieces = only is not null ?
                pieces.Where(piece => only.Select(pieceType => pieceType.GetType()).ToList().Contains(piece.Type.GetType())) :
                pieces;

            ArgumentOutOfRangeException.ThrowIfZero(selectedPieces.Count());

            do {
                pieces.Shuffle();

                foreach (Piece piece in selectedPieces) {
                    piece.ResetAccumWeight();

                    totalWeight += piece.Weight;
                    piece.TotalAccumWeight = totalWeight;
                }

                float roll = _rng.NextFloat(0f, totalWeight);

                foreach (Piece piece in selectedPieces) {

                    if (roll <= piece.TotalAccumWeight) {
                        selectedPiece = piece.Clone() as Piece;
                        break;
                    }
                }

            } while (selectedPiece == null);


            foreach (Piece piece in selectedPieces)
                piece.ResetAccumWeight();

            Debug.WriteLine(selectedPiece.Type.Shape);
            return selectedPiece;
        }
    }
}

