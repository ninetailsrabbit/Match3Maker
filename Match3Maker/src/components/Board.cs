using System.Numerics;
using SystemExtensions;

namespace Match3Maker {

    public class Board {
        public static readonly int MIN_GRID_WIDTH = 3;
        public static readonly int MIN_GRID_HEIGHT = 3;

        public enum FILL_MODES {
            FALL_DOWN,
            IN_PLACE,
            SIDE_DOWN
        }

        public FILL_MODES SelectedFillMode = FILL_MODES.FALL_DOWN;

        #region Events
        public event Action? PreparedBoard;
        public event Action? FilledBoard;

        public delegate void ConsumedSequenceEventHandler(Sequence sequence);
        public event ConsumedSequenceEventHandler? ConsumedSequence;
        #endregion

        public int GridWidth { get => _gridWidth; set { _gridWidth = Math.Max(MIN_GRID_WIDTH, value); } }
        public int GridHeight { get => _gridHeight; set { _gridHeight = Math.Max(MIN_GRID_HEIGHT, value); } }

        private int _gridWidth;
        private int _gridHeight;

        public Vector2 CellSize = new(48, 48);
        public Vector2 Offset = new(5, 10);

        public IPieceSelector PieceSelector;
        public ISequenceFinder SequenceFinder;

        public List<List<GridCell>> GridCells = [];
        public List<PieceWeight> AvailablePieces = [];


        #region Constructors
        public Board(int gridWidth, int gridHeight, IPieceSelector? pieceSelector = null, ISequenceFinder? sequenceFinder = null) {
            GridWidth = gridWidth;
            GridHeight = gridHeight;
            PieceSelector = pieceSelector is not null ? pieceSelector : new PieceWeightSelector();
            SequenceFinder = sequenceFinder is not null ? sequenceFinder : new SequenceFinder();
        }

        public Board(Vector2 size, IPieceSelector? pieceSelector = null, ISequenceFinder? sequenceFinder = null) {
            GridWidth = (int)size.X;
            GridHeight = (int)size.Y;
            PieceSelector = pieceSelector is not null ? pieceSelector : new PieceWeightSelector();
            SequenceFinder = sequenceFinder is not null ? sequenceFinder : new SequenceFinder();
        }

        public static Board Create(int gridWidth, int gridHeight, IPieceSelector? pieceSelector = null, ISequenceFinder? sequenceFinder = null)
            => new(gridWidth, gridHeight, pieceSelector, sequenceFinder);

        public static Board Create(Vector2 size, IPieceSelector? pieceSelector = null, ISequenceFinder? sequenceFinder = null)
            => Create((int)size.X, (int)size.Y, pieceSelector, sequenceFinder);

        #endregion

        #region Information
        public int Dimensions() => GridWidth * GridHeight;
        public Vector2 Size() => new(GridWidth, GridHeight);
        #endregion

        #region Options
        public Board ChangeGridWidth(int width) {
            GridWidth = width;

            return this;
        }
        public Board ChangeGridHeight(int height) {
            GridHeight = height;

            return this;
        }

        public Board ChangeGridSize(Vector2 size) {
            GridWidth = (int)size.X;
            GridHeight = (int)size.Y;

            return this;
        }

        public Board ChangeFillMode(FILL_MODES mode) {
            SelectedFillMode = mode;

            return this;
        }

        public Board ChangeCellSize(Vector2 size) {
            CellSize = size;

            return this;
        }

        public Board ChangeOffset(Vector2 offset) {
            Offset = offset;

            return this;
        }
        #endregion

        #region Cells

        public GridCell? Cell(Vector2 position) => Cell((int)position.Y, ((int)position.X));
        public GridCell? Cell(int column, int row) {
            if (GridCells.Count > 0
                && column < GridWidth
                && row < GridHeight
                && (column & int.MinValue) == 0 && (row & int.MinValue) == 0 // Bitwise operation to check if is not negative
            )
                return GridCells[column][row];

            return null;
        }

        public Vector2 CellPosition(GridCell cell)
            => new(CellSize.X * cell.Column + (Offset.X * cell.Column), CellSize.Y * cell.Row + (Offset.Y * cell.Row));

        public Board PrepareGridCells(bool overwrite = false) {

            if (GridCells.IsEmpty() || overwrite) {
                GridCells.Clear();

                foreach (var column in Enumerable.Range(0, GridWidth)) {
                    GridCells.Add([]);

                    foreach (var row in Enumerable.Range(0, GridHeight)) {
                        GridCells[column].Add(new GridCell(column, row));
                    }
                }

                PreparedBoard?.Invoke();
            }

            return this;
        }
        public GridCell? UpperCellFrom(GridCell cell) {
            var upperRow = cell.Row - 1;

            return Cell(cell.Column, upperRow);
        }

        public List<GridCell> UpperCellsFrom(GridCell cell, int distance) {
            List<GridCell> cells = [];

            distance = Math.Clamp(distance, 0, GridHeight);

            var currentCell = UpperCellFrom(cell);

            if (distance > 0 && currentCell is not null)
                cells.AddRange([currentCell, .. UpperCellsFrom(currentCell, distance - 1)]);

            return cells;
        }

        public GridCell? BottomCellFrom(GridCell cell) {
            var bottomRow = cell.Row + 1;

            return Cell(cell.Column, bottomRow);
        }
        public List<GridCell> BottomCellsFrom(GridCell cell, int distance) {
            List<GridCell> cells = [];

            distance = Math.Clamp(distance, 0, GridHeight);

            var currentCell = BottomCellFrom(cell);

            if (distance > 0 && currentCell is not null)
                cells.AddRange([currentCell, .. BottomCellsFrom(currentCell, distance - 1)]);

            return cells;
        }

        public GridCell? RightCellFrom(GridCell cell) {
            var rightColumn = cell.Column + 1;

            return Cell(rightColumn, cell.Row);
        }

        public List<GridCell> RightCellsFrom(GridCell cell, int distance) {
            List<GridCell> cells = [];

            distance = Math.Clamp(distance, 0, GridWidth);

            var currentCell = RightCellFrom(cell);

            if (distance > 0 && currentCell is not null)
                cells.AddRange([currentCell, .. RightCellsFrom(currentCell, distance - 1)]);

            return cells;
        }

        public GridCell? LeftCellFrom(GridCell cell) {
            var leftColumn = cell.Column - 1;

            return Cell(leftColumn, cell.Row);
        }

        public List<GridCell> LeftCellsFrom(GridCell cell, int distance) {
            List<GridCell> cells = [];

            distance = Math.Clamp(distance, 0, GridWidth);

            var currentCell = LeftCellFrom(cell);

            if (distance > 0 && currentCell is not null)
                cells.AddRange([currentCell, .. LeftCellsFrom(currentCell, distance - 1)]);

            return cells;
        }


        public List<GridCell> EmptyCells() {
            return GridCells.SelectMany(cells => cells).Where(cell => cell.IsEmpty()).ToList();
        }

        public List<GridCell> EmptyCellsFromRow(int row) => CellsFromRow(row).Where(cell => cell.IsEmpty()).ToList();
        public List<GridCell> EmptyCellsFromColumn(int column) => CellsFromColumn(column).Where(cell => cell.IsEmpty()).ToList();

        public List<GridCell> CellsFromColumn(int column) {
            List<GridCell> result = [];


            if (GridCells.Count > 0 && column < GridWidth && (column & int.MinValue) == 0) {
                foreach (int row in Enumerable.Range(0, GridHeight))
                    result.Add(GridCells[column][row]);
            }

            return result;
        }

        public List<GridCell> CellsFromRow(int row) {
            List<GridCell> result = [];

            if (GridCells.Count > 0 && row < GridHeight && (row & int.MinValue) == 0) {
                foreach (int column in Enumerable.Range(0, GridWidth))
                    result.Add(GridCells[column][row]);
            }

            return result;
        }

        public GridCell? FindGridCellWithPiece(Piece piece) {
            return GridCells.SelectMany(cells => cells)
                .Where(cell => cell.HasPiece())
                .ToList()
                .Find(cell => cell.Piece.Equals(piece));
        }

        #endregion

        #region Pieces
        public Board AddAvailablePieces(IList<PieceWeight> pieces) {
            AvailablePieces.AddRange(pieces);
            AvailablePieces = AvailablePieces.RemoveDuplicates().ToList();

            return this;
        }

        public Board AddAvailablePiece(PieceWeight piece) {
            AvailablePieces.Add(piece);
            AvailablePieces = AvailablePieces.RemoveDuplicates().ToList();

            return this;
        }
        public Board RemoveAvailablePieces(IList<PieceWeight> pieces) {
            foreach (PieceWeight piece in pieces)
                RemoveAvailablePiece(piece);

            return this;
        }
        public Board RemoveAvailablePiece(PieceWeight piece) {
            AvailablePieces.Remove(piece);

            return this;
        }

        #endregion
    }
}