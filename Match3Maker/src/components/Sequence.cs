using Extensionator;

namespace Match3Maker {

    public class Sequence : ICloneable {
        public enum SHAPES {
            HORIZONTAL,
            VERTICAL,
            T_SHAPE,
            L_SHAPE,
            DIAGONAL,
            LINE_CONNECTED,
            IRREGULAR
        }

        public List<GridCell> Cells;
        public SHAPES? Shape;


        public Sequence(List<GridCell> cells, SHAPES? shape = null) {
            Cells = [.. ValidCells(cells)];

            shape ??= DetectShape(Cells);

            Shape = shape;
        }

        public int Size() => Cells.Count;

        public void Consume() {
            Cells.ForEach(cell => cell.RemovePiece());
        }

        public List<Piece?> Pieces() => Cells.Where(cell => cell.HasPiece()).Select(cell => cell.Piece).ToList();

        #region CellPositions
        public GridCell? MiddleCell() => Cells.Count >= 3 ? Cells.MiddleElement() : null;

        public GridCell? TopEdgeCell() {
            if (Shape.Equals(SHAPES.VERTICAL))
                return Cells.First();

            return null;
        }

        public GridCell? BottomEdgeCell() {
            if (Shape.Equals(SHAPES.VERTICAL))
                return Cells.Last();

            return null;
        }

        public GridCell? LeftEdgeCell() {
            if (Shape.Equals(SHAPES.HORIZONTAL))
                return Cells.First();

            return null;
        }

        public GridCell? RightEdgeCell() {
            if (Shape.Equals(SHAPES.HORIZONTAL))
                return Cells.Last();

            return null;
        }
        #endregion

        #region Shapes
        public bool IsHorizontal() => Shape.Equals(SHAPES.HORIZONTAL);
        public bool IsVertical() => Shape.Equals(SHAPES.VERTICAL);
        public bool IsTShape() => Shape.Equals(SHAPES.T_SHAPE);
        public bool IsLShape() => Shape.Equals(SHAPES.L_SHAPE);
        public bool IsDiagonalShape() => Shape.Equals(SHAPES.DIAGONAL);
        public bool IsLineConnectedShape() => Shape.Equals(SHAPES.LINE_CONNECTED);

        public bool IsHorizontalOrVertical() => IsHorizontal() || IsVertical();
        #endregion

        public object Clone() {

            List<GridCell> clonedCells = Cells.Select(cell => new GridCell(cell.Column, cell.Row, cell.HasPiece() ? (Piece)cell.Piece?.Clone() : null)).ToList();

            return new Sequence(clonedCells, Shape);
        }

        private static List<GridCell> ValidCells(List<GridCell> cells) {
            HashSet<GridCell> validCells = [];

            cells.Where(cell => cell.HasPiece())
                .ToList()
                .ForEach(cell => {
                    if (validCells.IsEmpty() || !validCells.Any(validCell => validCell.InSamePositionAs(cell)))
                        validCells.Add(cell);
                });

            return [.. validCells];
        }

        private static SHAPES DetectShape(List<GridCell> cells) {
            var cellsByIndex = cells.Select((value, index) => new { value, index });

            bool isHorizontalShape = cellsByIndex.All(item => item.index == 0 || item.value.InSameRowAs(cells[item.index - 1]));

            if (isHorizontalShape)
                return SHAPES.HORIZONTAL;

            bool isVerticalShape = cellsByIndex.All(item => item.index == 0 || item.value.InSameColumnAs(cells[item.index - 1]));

            if (isVerticalShape)
                return SHAPES.VERTICAL;

            bool isDiagonalShape = cellsByIndex.All(item => item.index == 0 || item.value.InDiagonalWith(cells[item.index - 1]));

            if (isDiagonalShape)
                return SHAPES.DIAGONAL;


            return SHAPES.IRREGULAR;
        }
    }

}