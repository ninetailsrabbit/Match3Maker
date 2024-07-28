using Extensionator;

namespace Match3Maker {

    public class Sequence : ICloneable {
        public enum Shapes {
            Horizontal,
            Vertical,
            TShape,
            LShape,
            Diagonal,
            LineConnected,
            Irregular
        }

        public List<GridCell> Cells;
        public Shapes? Shape;

        public Sequence(List<GridCell> cells, Shapes? shape = null) {
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
            if (Shape.Equals(Shapes.Vertical))
                return Cells.First();

            return null;
        }

        public GridCell? BottomEdgeCell() {
            if (Shape.Equals(Shapes.Vertical))
                return Cells.Last();

            return null;
        }

        public GridCell? LeftEdgeCell() {
            if (Shape.Equals(Shapes.Horizontal))
                return Cells.First();

            return null;
        }

        public GridCell? RightEdgeCell() {
            if (Shape.Equals(Shapes.Horizontal))
                return Cells.Last();

            return null;
        }
        #endregion

        #region Shapes
        public bool IsHorizontal() => Shape.Equals(Shapes.Horizontal);
        public bool IsVertical() => Shape.Equals(Shapes.Vertical);
        public bool IsTShape() => Shape.Equals(Shapes.TShape);
        public bool IsLShape() => Shape.Equals(Shapes.LShape);
        public bool IsDiagonalShape() => Shape.Equals(Shapes.Diagonal);
        public bool IsLineConnectedShape() => Shape.Equals(Shapes.LineConnected);

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

        private static Shapes DetectShape(List<GridCell> cells) {
            var cellsByIndex = cells.Select((value, index) => new { value, index });

            bool isHorizontalShape = cellsByIndex.All(item => item.index == 0 || item.value.InSameRowAs(cells[item.index - 1]));

            if (isHorizontalShape)
                return Shapes.Horizontal;

            bool isVerticalShape = cellsByIndex.All(item => item.index == 0 || item.value.InSameColumnAs(cells[item.index - 1]));

            if (isVerticalShape)
                return Shapes.Vertical;

            bool isDiagonalShape = cellsByIndex.All(item => item.index == 0 || item.value.InDiagonalWith(cells[item.index - 1]));

            if (isDiagonalShape)
                return Shapes.Diagonal;


            return Shapes.Irregular;
        }
    }

}