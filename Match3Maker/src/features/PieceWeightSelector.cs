using SystemExtensions;

namespace Match3Maker {
    public sealed class PieceWeightSelector : IPieceSelector {
        private readonly Random _rng = new();

#nullable enable
        public Piece Roll(List<PieceWeight> pieces, IEnumerable<Piece.TYPES>? only = null) {


            if (pieces.IsEmpty())
                throw new ArgumentException("PieceWeightSelector: The pieces to roll cannot be empty");

            float totalWeight = 0f;
            Piece? selectedPiece = null;
            2.In(3, 4, 5, 6, 2);
            var selectedPieces = only is not null ? pieces.Where(piece => piece.Type.In(only)) : pieces;

            do {
                pieces.Shuffle();

                foreach (PieceWeight pieceWeight in selectedPieces) {
                    pieceWeight.ResetAccumWeight();

                    totalWeight += pieceWeight.Weight;
                    pieceWeight.TotalAccumWeight = totalWeight;
                }

                float roll = _rng.NextFloat(0f, totalWeight);

                foreach (PieceWeight pieceWeight in selectedPieces) {

                    if (roll <= pieceWeight.TotalAccumWeight) {
                        selectedPiece = Piece.From(pieceWeight);
                        break;
                    }
                }

            } while (selectedPiece == null);


            return selectedPiece;
        }
    }
}

