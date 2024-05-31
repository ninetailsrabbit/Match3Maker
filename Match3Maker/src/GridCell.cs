﻿using System.Numerics;

namespace Match3Maker {
    public sealed class GridCell {

        #region Properties
        public Guid Id = Guid.NewGuid();
        public int Column;
        public int Row;
        public bool CanContainPiece = true;
        public Piece? Piece;
        #endregion

        #region Events
        public delegate void SwappedPieceEventHandler(GridCell from, GridCell to);
        public event SwappedPieceEventHandler? SwappedPiece;

        public delegate void SwapRejectedEventHandler(GridCell from, GridCell to);
        public event SwapRejectedEventHandler? SwapRejected;
        #endregion

        public GridCell(int column, int row, Piece? piece = null, bool canContainPiece = true) {
            if (column < 0 || row < 0)
                throw new ArgumentException($"GridCell cannot have negative column {column} & row {row}");

            Column = column;
            Row = row;
            CanContainPiece = canContainPiece;
            Piece = piece;
        }

        public Vector2 Position() => new(Row, Column);
        public bool IsEmpty() => Piece is null;
        public bool HasPiece() => Piece is not null;
        public void AssignPiece(Piece piece) {
            if (CanContainPiece && IsEmpty()) {
                Piece = piece;
            }
        }
        public void RemovePiece() {
            Piece = null;
        }

        public bool SwapPieceWith(GridCell otherCell) {
            if (CanSwapPieceWith(otherCell)) {

                Piece? currentPiece = Piece;
                Piece? newPiece = otherCell.Piece;

                RemovePiece();
                AssignPiece(newPiece);

                otherCell.RemovePiece();
                otherCell.AssignPiece(currentPiece);

                SwappedPiece?.Invoke(this, otherCell);

                return true;
            }

            SwapRejected?.Invoke(this, otherCell);

            return false;
        }

        public bool CanSwapPieceWith(GridCell otherCell) {
            return otherCell.HasPiece()
                && HasPiece()
                && CanContainPiece
                && otherCell.CanContainPiece
                && !Piece.Locked
                && !otherCell.Piece.Locked
                && !Equals(otherCell)
                && !Piece.Equals(otherCell.Piece);
        }

        public bool InSameRowAs(GridCell cell) => cell.Row.Equals(Row);
        public bool InSameColumnAs(GridCell cell) => cell.Column.Equals(Column);
        public bool InSamePositionAs(GridCell cell) => InSameColumnAs(cell) && InSameRowAs(cell);
        public bool IsRowNeighbourOf(GridCell cell) {
            int leftColumn = Column - 1;
            int rightColumn = Column + 1;

            return InSameRowAs(cell) && new List<int>() { leftColumn, rightColumn }.Any((column) => column.Equals(cell.Column));
        }

        public bool IsColumnNeighbourOf(GridCell cell) {
            int upperRow = Row - 1;
            int bottomRow = Row + 1;

            return InSameColumnAs(cell) && new List<int>() { upperRow, bottomRow }.Any((row) => row.Equals(cell.Row));
        }

        public bool IsAdjacentTo(GridCell cell) => IsRowNeighbourOf(cell) || IsColumnNeighbourOf(cell);

    }
}