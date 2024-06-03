using Match3Maker;
using System.Numerics;
using Xunit;

namespace Match3Tests {
    public class GridCellTests {
        private readonly PieceFactory _pieceFactory = new();

        [Fact]
        public void Should_Be_Created_With_Correct_Properties() {
            var cell = new GridCell(0, 5);
            var expectedPosition = new Vector2(5, 0); // X represents row, Y Represents column

            Assert.Equal(expectedPosition, cell.Position());
            Assert.True(cell.Row.Equals(5) && cell.Column.Equals(0));
        }

        [Fact]
        public void ThrowException_When_Coordinates_Are_Negative() {
            ArgumentException exception = Assert.Throws<ArgumentException>(() => new GridCell(-1, 6));
            Assert.Equal($"GridCell cannot have a negative column {-1} or row {6}", exception.Message);
            exception = Assert.Throws<ArgumentException>(() => new GridCell(1, -6));
            Assert.Equal($"GridCell cannot have a negative column {1} or row {-6}", exception.Message);
        }

        [Fact]
        public void Should_Detect_Cells_OnSameColumn() {
            var cell = new GridCell(3, 2);

            Assert.True(cell.InSameColumnAs(new GridCell(3, 7)));
            Assert.True(cell.InSameColumnAs(new GridCell(3, 0)));
            Assert.False(cell.InSameColumnAs(new GridCell(0, 3)));
        }

        [Fact]
        public void Should_Detect_Cells_OnSameRow() {
            var cell = new GridCell(3, 2);

            Assert.True(cell.InSameRowAs(new GridCell(7, 2)));
            Assert.True(cell.InSameRowAs(new GridCell(5, 2)));
            Assert.False(cell.InSameRowAs(new GridCell(3, 1)));
        }

        [Fact]
        public void Should_Detect_Row_Neighbours() {
            var cell = new GridCell(1, 1);
            var leftNeighbour = new GridCell(0, 1);
            var rightNeighbour = new GridCell(2, 1);
            var nonNeighbour = new GridCell(3, 1);


            Assert.True(cell.IsRowNeighbourOf(leftNeighbour));
            Assert.True(cell.IsRowNeighbourOf(rightNeighbour));
            Assert.False(cell.IsRowNeighbourOf(nonNeighbour));
        }

        [Fact]
        public void Should_Detect_Column_Neighbours() {
            var cell = new GridCell(1, 1);
            var upperNeighbour = new GridCell(1, 0);
            var bottomNeighbour = new GridCell(1, 2);
            var nonNeighbour = new GridCell(1, 3);

            Assert.True(cell.IsColumnNeighbourOf(upperNeighbour));
            Assert.True(cell.IsColumnNeighbourOf(bottomNeighbour));
            Assert.False(cell.IsColumnNeighbourOf(nonNeighbour));
        }

        [Fact]
        public void Should_Detect_Adjacent_Cells() {
            var cell = new GridCell(1, 1);

            var leftNeighbour = new GridCell(0, 1);
            var rightNeighbour = new GridCell(2, 1);
            var upperNeighbour = new GridCell(1, 0);
            var bottomNeighbour = new GridCell(1, 2);
            var nonNeighbourRow = new GridCell(3, 1);

            var nonNeighbourColumn = new GridCell(1, 3);

            Assert.True(cell.IsAdjacentTo(leftNeighbour));
            Assert.True(cell.IsAdjacentTo(rightNeighbour));
            Assert.True(cell.IsAdjacentTo(upperNeighbour));
            Assert.True(cell.IsAdjacentTo(bottomNeighbour));

            Assert.False(cell.IsAdjacentTo(nonNeighbourRow));
            Assert.False(cell.IsAdjacentTo(nonNeighbourColumn));
        }

        [Fact]
        public void Should_Detect_Piece() {
            var cell = new GridCell(1, 1);
            var piece = new Piece(_pieceFactory.CreateNormalPiece("square"));

            Assert.True(cell.IsEmpty());

            cell.AssignPiece(piece);

            Assert.True(cell.HasPiece());
            Assert.Equal(piece, cell.Piece);
        }

        [Fact]
        public void Should_Not_Swap_Piece_When_Conditions_Are_Not_Met() {
            var squarePiece = new Piece(_pieceFactory.CreateNormalPiece("square"));
            var circlePiece = new Piece(_pieceFactory.CreateNormalPiece("circle"));

            var cell = new GridCell(1, 1);
            var otherCell = new GridCell(1, 2);

            Assert.False(cell.SwapPieceWith(otherCell));

            cell.AssignPiece(squarePiece);
            Assert.False(cell.SwapPieceWith(otherCell));

            otherCell.AssignPiece(circlePiece);
            otherCell.Piece.Lock();

            Assert.False(cell.SwapPieceWith(otherCell));

            otherCell.AssignPiece(cell.Piece);
            Assert.False(cell.SwapPieceWith(otherCell));

        }

        [Fact]
        public void Should_Swap_Piece_When_Both_Cells_Has_One_And_Met_Conditions() {
            var squarePiece = new Piece(_pieceFactory.CreateNormalPiece("square"));
            var circlePiece = new Piece(_pieceFactory.CreateNormalPiece("circle"));

            var cell = new GridCell(1, 1, squarePiece);
            var otherCell = new GridCell(1, 2, circlePiece);

            Assert.True(cell.SwapPieceWith(otherCell));

            Assert.Equal(circlePiece, cell.Piece);
            Assert.Equal(squarePiece, otherCell.Piece);
        }

        [Fact]
        public void Should_Raise_Swapped_Piece_Event_When_Swaps() {
            var squarePiece = new Piece(_pieceFactory.CreateNormalPiece("square"));
            var circlePiece = new Piece(_pieceFactory.CreateNormalPiece("circle"));

            var cell = new GridCell(1, 1, squarePiece);
            var otherCell = new GridCell(1, 2, circlePiece);

            List<GridCell> cellEvent = [];
            List<GridCell> otherCellEvent = [];

            GridCell.SwappedPieceEventHandler cellListener = delegate (GridCell to, GridCell from) { cellEvent.AddRange([to, from]); };
            GridCell.SwappedPieceEventHandler otherCellListener = delegate (GridCell to, GridCell from) { otherCellEvent.AddRange([to, from]); };

            cell.SwappedPiece += cellListener;
            otherCell.SwappedPiece += otherCellListener;

            Assert.True(cell.SwapPieceWith(otherCell));

            Assert.Equal(cellEvent.First(), cell);
            Assert.Equal(cellEvent.Last(), otherCell);

            Assert.True(otherCell.SwapPieceWith(cell));

            Assert.Equal(otherCellEvent.First(), otherCell);
            Assert.Equal(otherCellEvent.Last(), cell);

            cell.SwappedPiece -= cellListener;
            otherCell.SwappedPiece -= otherCellListener;

        }

        [Fact]
        public void Should_Raise_Swap_Rejected_Event_When_Swap_Cannot_Be_Done() {

            var cell = new GridCell(1, 1, new Piece(_pieceFactory.CreateNormalPiece("square")));
            var otherCell = new GridCell(1, 2);

            List<GridCell> cellEvent = [];

            GridCell.SwapRejectedEventHandler listener = delegate (GridCell to, GridCell from) { cellEvent.AddRange([to, from]); };
            cell.SwapRejected += listener;

            Assert.False(cell.SwapPieceWith(otherCell));
            Assert.Equal(cellEvent.First(), cell);
            Assert.Equal(cellEvent.Last(), otherCell);

            cell.SwapRejected -= listener;
        }

        [Fact]
        public void Should_Detect_Adjacent_Diagonal_Cells() {
            var cell = new GridCell(3, 4, new Piece(_pieceFactory.CreateNormalPiece("square")));

            var diagonalTopRightCell = new GridCell(4, 3);
            var diagonalTopLeftCell = new GridCell(2, 3);
            var diagonalBottomRightCell = new GridCell(4, 5);
            var diagonalBottomLeftCell = new GridCell(4, 3);

            Assert.True(cell.InDiagonalWith(diagonalTopRightCell));
            Assert.True(cell.InDiagonalWith(diagonalTopLeftCell));
            Assert.True(cell.InDiagonalWith(diagonalBottomRightCell));
            Assert.True(cell.InDiagonalWith(diagonalBottomLeftCell));

            var nonDiagonalCell = new GridCell(1, 1);
            var nonDiagonalCell2 = new GridCell(5, 2);
            var nonDiagonalCell3 = new GridCell(0, 0);

            Assert.False(cell.InDiagonalWith(nonDiagonalCell));
            Assert.False(cell.InDiagonalWith(nonDiagonalCell2));
            Assert.False(cell.InDiagonalWith(nonDiagonalCell3));
        }

        [Fact]
        public void Should_Remove_Piece_And_Return_It() {
            var cell = new GridCell(3, 4);
            var piece = new Piece(_pieceFactory.CreateNormalPiece("square"));

            Assert.True(cell.IsEmpty());
            Assert.Null(cell.RemovePiece());

            cell.AssignPiece(piece);

            Assert.True(cell.HasPiece());
            Assert.Equal(piece, cell.RemovePiece());

        }
    }



}
