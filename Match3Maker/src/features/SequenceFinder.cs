using Extensionator;

namespace Match3Maker {
    public class SequenceFinder(
        int minMatch = 3,
        int maxMatch = 5,
        int minSpecialMatch = 2,
        int maxSpecialMatch = 3,
        bool horizontalShape = true,
        bool verticalShape = true,
        bool tShape = false,
        bool lShape = false) : ISequenceFinder {

        #region PublicProperties
        public bool HorizontalShape { get => _horizontalShape; set => _horizontalShape = value; }
        public bool VerticalShape { get => _verticalShape; set => _verticalShape = value; }
        public bool TShape { get => _tShape; set => _tShape = value; }
        public bool LShape { get => _lShape; set => _lShape = value; }
        public int MinMatch { get => _minMatch; set => _minMatch = Math.Min(3, value); }
        public int MaxMatch { get => _maxMatch; set => _maxMatch = Math.Max(MinMatch, value); }
        public int MinSpecialMatch { get => _minSpecialMatch; set => _minSpecialMatch = Math.Min(2, value); }
        public int MaxSpecialMatch { get => _maxSpecialMatch; set => _maxSpecialMatch = Math.Max(MinSpecialMatch, value); }

        #endregion

        #region PrivateProperties
        private bool _horizontalShape = horizontalShape;
        private bool _verticalShape = verticalShape;
        private bool _tShape = tShape;
        private bool _lShape = lShape;
        private int _minMatch = minMatch;
        private int _maxMatch = maxMatch;
        private int _minSpecialMatch = minSpecialMatch;
        private int _maxSpecialMatch = maxSpecialMatch;
        #endregion

        public List<Sequence> FindHorizontalSequences(IEnumerable<GridCell> cells) {
            List<Sequence> sequences = [];
            List<GridCell> currentMatches = [];

            if (HorizontalShape) {

                cells.Where(cell => cell.HasPiece())
                    .Select((value, index) => new { value, index })
                    .ToList()
                    .ForEach(item => {
                        var cell = item.value;
                        var index = item.index;

                        if (currentMatches.IsEmpty() ||
                            (currentMatches.Last() is GridCell previousCell
                            && previousCell.IsRowNeighbourOf(cell)
                            && cell.Piece.MatchWith(previousCell.Piece))
                        ) {
                            currentMatches.Add(cell);

                            if (currentMatches.Count.Equals(MaxMatch)) {
                                AddSequenceFromMatches(sequences, currentMatches, Sequence.Shapes.Horizontal);
                                currentMatches.Clear();
                            }
                        }
                        else {
                            if (currentMatches.Count.IsBetween(MinMatch, MaxMatch)) {
                                AddSequenceFromMatches(sequences, currentMatches, Sequence.Shapes.Horizontal);
                            }

                            currentMatches.Clear();
                            currentMatches.Add(cell);
                        }


                        if (index.Equals(cells.LastIndex())) {
                            if (currentMatches.Count.IsBetween(MinMatch, MaxMatch))
                                AddSequenceFromMatches(sequences, currentMatches, Sequence.Shapes.Horizontal);
                        }

                    });
            }

            return sequences.OrderByDescending(sequence => sequence.Size()).ToList();
        }


        public List<Sequence> FindVerticalSequences(IEnumerable<GridCell> cells) {
            List<Sequence> sequences = [];
            List<GridCell> currentMatches = [];

            if (VerticalShape) {
                cells.Where(cell => cell.HasPiece())
                    .Select((value, index) => new { value, index })
                    .ToList()
                    .ForEach(item => {
                        var cell = item.value;
                        var index = item.index;

                        if (currentMatches.IsEmpty() ||
                            (currentMatches.Last() is GridCell previousCell
                            && previousCell.IsColumnNeighbourOf(cell)
                            && cell.Piece.MatchWith(previousCell.Piece))
                        ) {
                            currentMatches.Add(cell);

                            if (currentMatches.Count.Equals(MaxMatch)) {
                                AddSequenceFromMatches(sequences, currentMatches, Sequence.Shapes.Vertical);
                                currentMatches.Clear();
                            }
                        }
                        else {
                            if (currentMatches.Count.IsBetween(MinMatch, MaxMatch))
                                AddSequenceFromMatches(sequences, currentMatches, Sequence.Shapes.Vertical);

                            currentMatches.Clear();
                            currentMatches.Add(cell);
                        }


                        if (index.Equals(cells.LastIndex()) && currentMatches.Count.IsBetween(MinMatch, MaxMatch))
                            AddSequenceFromMatches(sequences, currentMatches, Sequence.Shapes.Vertical);
                    });
            }

            return sequences.OrderByDescending(sequence => sequence.Size()).ToList();
        }

        public Sequence? FindTShapeSequence(Sequence sequenceA, Sequence sequenceB) {
            if (TShape && sequenceA.IsHorizontalOrVertical() && sequenceB.IsHorizontalOrVertical()) {

                var horizontalSequence = sequenceA.IsHorizontal() ? sequenceA : sequenceB;
                var verticalSequence = sequenceA.IsVertical() ? sequenceA : sequenceB;

                if (horizontalSequence.IsHorizontal() && verticalSequence.IsVertical()) {

                    var leftEdgeCell = horizontalSequence.LeftEdgeCell();
                    var rightEdgeCell = horizontalSequence.RightEdgeCell();
                    var topEdgeCell = verticalSequence.TopEdgeCell();
                    var bottomEdgeCell = verticalSequence.BottomEdgeCell();
                    var horizontalMiddleCell = horizontalSequence.MiddleCell();
                    var verticalMiddleCell = verticalSequence.MiddleCell();

                    if (horizontalMiddleCell.InSamePositionAs(topEdgeCell) || horizontalMiddleCell.InSamePositionAs(bottomEdgeCell)
                            || verticalMiddleCell.InSamePositionAs(leftEdgeCell) || verticalMiddleCell.InSamePositionAs(rightEdgeCell)) {

                        return new Sequence([.. horizontalSequence.Cells, .. verticalSequence.Cells], Sequence.Shapes.TShape);

                    }
                }
            }

            return null;
        }

        public Sequence? FindLShapeSequence(Sequence sequenceA, Sequence sequenceB) {
            if (LShape && sequenceA.IsHorizontalOrVertical() && sequenceB.IsHorizontalOrVertical()) {

                var horizontalSequence = sequenceA.IsHorizontal() ? sequenceA : sequenceB;
                var verticalSequence = sequenceA.IsVertical() ? sequenceA : sequenceB;

                if (horizontalSequence.IsHorizontal() && verticalSequence.IsVertical()) {
                    var leftEdgeCell = horizontalSequence.LeftEdgeCell();
                    var rightEdgeCell = horizontalSequence.RightEdgeCell();
                    var topEdgeCell = verticalSequence.TopEdgeCell();
                    var bottomEdgeCell = verticalSequence.BottomEdgeCell();

                    if (leftEdgeCell.InSamePositionAs(topEdgeCell) || leftEdgeCell.InSamePositionAs(bottomEdgeCell)
                            || rightEdgeCell.InSamePositionAs(topEdgeCell) || rightEdgeCell.InSamePositionAs(bottomEdgeCell)) {

                        return new Sequence([.. horizontalSequence.Cells, .. verticalSequence.Cells], Sequence.Shapes.LShape);
                    }
                }
            }

            return null;
        }

        public Sequence? FindMatchFromCell(Board board, GridCell cell) {

            if (cell.HasPiece()) {
                IEnumerable<Sequence> horizontalSequences = FindHorizontalSequences(board.CellsFromRow(cell.Row));
                IEnumerable<Sequence> verticalSequences = FindVerticalSequences(board.CellsFromColumn(cell.Column));

                Sequence? horizontal = horizontalSequences.Where(sequence => sequence.Cells.Contains(cell)).FirstOrDefault();
                Sequence? vertical = verticalSequences.Where(sequence => sequence.Cells.Contains(cell)).FirstOrDefault();

                if (horizontal is not null && vertical is not null) {
                    Sequence? TShapeSequence = FindTShapeSequence(horizontal, vertical);

                    if (TShapeSequence is not null)
                        return TShapeSequence;

                    Sequence? LShapeSequence = FindLShapeSequence(horizontal, vertical);

                    if (LShapeSequence is not null)
                        return LShapeSequence;

                }

                return horizontal is not null ? horizontal : vertical;

            }

            return null;
        }

        public List<Sequence> FindBoardSequences(Board board) {
            var horizontalSequences = FindHorizontalBoardSequences(board);
            var verticalSequences = FindVerticalBoardSequences(board);

            List<Sequence> validHorizontalSequences = [];
            List<Sequence> validVerticalSequences = [];
            List<Sequence> lShapeSequences = [];
            List<Sequence> tShapeSequences = [];

            foreach (var horizontalSequence in horizontalSequences) {
                bool addHorizontalSequence = true;

                foreach (var verticalSequence in verticalSequences) {

                    if (FindLShapeSequence(horizontalSequence, verticalSequence) is Sequence lShape) {
                        lShapeSequences.Add(lShape);
                        addHorizontalSequence = false;
                    }
                    else if (FindTShapeSequence(horizontalSequence, verticalSequence) is Sequence tShape) {
                        tShapeSequences.Add(tShape);
                        addHorizontalSequence = false;
                    }
                    else {
                        validVerticalSequences.Add(verticalSequence);
                    }
                }

                if (addHorizontalSequence)
                    validHorizontalSequences.Add(horizontalSequence);
            }

            List<Sequence> result = [.. horizontalSequences, .. verticalSequences, .. lShapeSequences, .. tShapeSequences];

            return result;
        }


        public List<Sequence> FindHorizontalBoardSequences(Board board) {
            List<Sequence> horizontalSequences = [];

            foreach (int row in Enumerable.Range(0, board.GridHeight))
                horizontalSequences.AddRange(FindHorizontalSequences(board.CellsFromRow(row)));

            return horizontalSequences;
        }

        public List<Sequence> FindVerticalBoardSequences(Board board) {
            List<Sequence> verticalSequences = [];

            foreach (int column in Enumerable.Range(0, board.GridWidth))
                verticalSequences.AddRange(FindVerticalSequences(board.CellsFromColumn(column)));

            return verticalSequences;
        }

        private static void AddSequenceFromMatches(List<Sequence> sequences, List<GridCell> matches, Sequence.Shapes shape) {
            sequences.Add(new Sequence([.. matches], shape));
        }
    }
}
