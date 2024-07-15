using Extensionator;

namespace Match3Maker {
    public sealed class PieceWeightGenerator : IPieceGenerator {
        private readonly Random _rng = new();

#nullable enable
        public Piece Roll(List<Piece> pieces, IEnumerable<Type>? only = null) {
            if (pieces.IsEmpty())
                throw new ArgumentException("PieceWeightGenerator: The pieces to roll cannot be empty");

            float totalWeight = 0f;
            Piece? selectedPiece = null;

            var selectedPieces = only is not null ? pieces.Where(piece => only.Contains(piece.Type.GetType())).ToList() : [.. pieces];

            ArgumentOutOfRangeException.ThrowIfZero(selectedPieces.Count);

            do {
                selectedPieces.ToList().Shuffle();
                totalWeight = 0f;

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

            return selectedPiece;
        }
    }
}

