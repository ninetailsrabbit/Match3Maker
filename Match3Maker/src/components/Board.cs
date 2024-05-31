using System.Numerics;

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

        public int GridWidth { get => _gridWidth; set { _gridWidth = Math.Max(MIN_GRID_WIDTH, value); } }
        public int GridHeight { get => _gridHeight; set { _gridHeight = Math.Max(MIN_GRID_HEIGHT, value); } }

        private int _gridWidth;
        private int _gridHeight;

        public Vector2 CellSize = new(48, 48);
        public Vector2 Offset = new(5, 10);

        public List<List<GridCell>> GridCells = [];


        public Board(int gridWidth, int gridHeight) {
            GridWidth = gridWidth;
            GridHeight = gridHeight;
        }

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
        public Board PrepareGridCells() {
            foreach (var column in Enumerable.Range(0, GridWidth)) {
                GridCells.Add([]);

                foreach (var row in Enumerable.Range(0, GridHeight)) {
                    GridCells[column].Add(new GridCell(column, row));
                }
            }

            return this;
        }
        #endregion
    }
}