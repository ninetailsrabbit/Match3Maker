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

        public List<List<GridCell>> GridCells = [];
        public List<PieceWeight> AvailablePieces = [];


        #region Constructors
        public Board(int gridWidth, int gridHeight) {
            GridWidth = gridWidth;
            GridHeight = gridHeight;
        }

        public static Board Create(int gridWidth, int gridHeight) => new(gridWidth, gridHeight);

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

        public GridCell? BottomCellFrom(GridCell cell) {
            var bottomRow = cell.Row + 1;

            return Cell(cell.Column, bottomRow);
        }

        public GridCell? RightCellFrom(GridCell cell) {
            var rightColumn = cell.Column + 1;

            return Cell(rightColumn, cell.Row);
        }

        public GridCell? LeftCellFrom(GridCell cell) {
            var leftColumn = cell.Column - 1;

            return Cell(leftColumn, cell.Row);
        }
        public List<GridCell> EmptyCells() {
            return GridCells.SelectMany(cells => cells).Where(cell => cell.IsEmpty()).ToList();
        }
        #endregion
    }
}