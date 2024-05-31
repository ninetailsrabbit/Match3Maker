namespace Match3Maker {
    public interface IPieceSelector {
#nullable enable
        public Piece Roll(List<PieceWeight> pieces, IEnumerable<Piece.TYPES>? only = null);
    }
}


