namespace Match3Maker {
    public interface ISequenceFinder {

        #region Properties
        bool HorizontalShape { get; set; }
        bool VerticalShape { get; set; }
        bool TShape { get; set; }
        bool LShape { get; set; }
        int MinMatch { get; set; }
        int MaxMatch { get; set; }
        #endregion

        #region Find functions
        public IEnumerable<Sequence> FindHorizontalSequences(IEnumerable<GridCell> cells);
        public IEnumerable<Sequence> FindVerticalSequences(IEnumerable<GridCell> cells);
        public Sequence? FindTShapeSequence(Sequence sequenceA, Sequence sequenceB);
        public Sequence? FindLShapeSequence(Sequence sequenceA, Sequence sequenceB);
        public IEnumerable<Sequence> FindBoardSequences(Board board);
        public IEnumerable<Sequence> FindHorizontalBoardSequences(Board board);
        public IEnumerable<Sequence> FindVerticalBoardSequences(Board board);
        public Sequence? FindMatchFromCell(Board board, GridCell cell);
        #endregion

        #region Options
        public ISequenceFinder ChangeMinMatchTo(int value) {
            MinMatch = value;

            return this;
        }
        public ISequenceFinder ChangeMaxMatchTo(int value) {
            MaxMatch = value;

            return this;
        }

        public ISequenceFinder EnableHorizontalShape() {
            HorizontalShape = true;
            return this;
        }

        public ISequenceFinder DisableHorizontalShape() {
            HorizontalShape = false;
            return this;
        }

        public ISequenceFinder EnableVerticalShape() {
            VerticalShape = true;
            return this;
        }

        public ISequenceFinder DisableVerticalShape() {
            VerticalShape = false;
            return this;
        }

        public ISequenceFinder EnableLShape() {
            LShape = true;
            return this;
        }

        public ISequenceFinder DisableLShape() {
            LShape = false;
            return this;
        }

        public ISequenceFinder EnableTShape() {
            TShape = true;
            return this;
        }

        public ISequenceFinder DisableTShape() {
            TShape = false;
            return this;
        }
        #endregion

    }

}