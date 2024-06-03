namespace Match3Maker {
    public interface IPieceGenerator {
#nullable enable
        public Piece Roll(List<Piece> pieces, IEnumerable<IPieceType>? only = null);
    }
}


