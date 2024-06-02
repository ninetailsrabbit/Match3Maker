namespace Match3Maker {
    public interface ISequenceFinder {

        #region Properties
        bool HorizontalShape { get; set; }
        bool VerticalShape { get; set; }
        bool TShape { get; set; }
        bool LShape { get; set; }
        int MinMatch { get; set; }
        int MaxMatch { get; set; }
        int MinSpecialMatch { get; set; }
        int MaxSpecialMatch { get; set; }
        #endregion

        #region Find functions
        public List<Sequence> FindHorizontalSequences(IEnumerable<GridCell> cells);
        public List<Sequence> FindVerticalSequences(IEnumerable<GridCell> cells);
        public Sequence? FindTShapeSequence(Sequence sequenceA, Sequence sequenceB);
        public Sequence? FindLShapeSequence(Sequence sequenceA, Sequence sequenceB);
        public List<Sequence> FindBoardSequences(Board board);
        public List<Sequence> FindHorizontalBoardSequences(Board board);
        public List<Sequence> FindVerticalBoardSequences(Board board);
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