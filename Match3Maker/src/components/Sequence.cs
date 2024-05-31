using SystemExtensions;

namespace Match3Maker {

    public class Sequence(List<GridCell> cells, Sequence.SHAPES shape) : ICloneable {
        public enum SHAPES {
            HORIZONTAL,
            VERTICAL,
            T_SHAPE,
            L_SHAPE
        }

        public List<GridCell> Cells = [.. ValidCells(cells)];
        public SHAPES Shape = shape;

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
    }

}