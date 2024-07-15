using Extensionator;
using System.Numerics;
using static Match3Maker.BoardCellUpdate;

namespace Match3Maker {

    #region Virtual board
    public sealed class VirtualBoard {
        public List<GridCell> GridCells = [];
        public List<BoardCellUpdate> Updates = [];

        public VirtualBoard(List<List<GridCell>> gridCells) {
            GridCells = gridCells.SelectMany(cells => cells).Select(cell => new GridCell(cell.Column, cell.Row, cell.Piece, cell.CanContainPiece)).ToList();

            // Only assign neighbours to fall & side down pieces
            GridCells.ForEach(cell => {
                cell.NeighbourBottom = Cell(cell.Column, cell.Row + 1);
                cell.DiagonalNeighbourBottomRight = Cell(cell.Column + 1, cell.Row + 1);
                cell.DiagonalNeighbourBottomLeft = Cell(cell.Column - 1, cell.Row + 1);
            });
        }

        public GridCell? Cell(int column, int row) => GridCells.FirstOrDefault(cell => cell.Column.Equals(column) && cell.Row.Equals(row));
        public List<GridCell> EmptyCells() => GridCells.Where(cell => cell.IsEmpty() && cell.CanContainPiece).ToList();
        public void Update(BoardCellUpdate update) {
            Updates.Add(update);
        }
        public List<BoardCellUpdate> MovementUpdates() => Updates.Where(update => update.CurrentUpdateType.Equals(UPDATE_TYPE.MOVEMENT)).ToList();
        public List<BoardCellUpdate> FillUpdates() => Updates.Where(update => update.CurrentUpdateType.Equals(UPDATE_TYPE.FILL)).ToList();
    }
    public sealed class BoardCellUpdate(UPDATE_TYPE currentUpdateType, CellPieceMovement? cellPieceMovement = null, CellPieceFill? cellPieceFill = null) {
        public enum UPDATE_TYPE {
            MOVEMENT,
            FILL
        }

        public UPDATE_TYPE CurrentUpdateType = currentUpdateType;
        public CellPieceMovement? CellPieceMovement = cellPieceMovement;
        public CellPieceFill? CellPieceFill = cellPieceFill;
        public bool IsMovement() => CurrentUpdateType.Equals(UPDATE_TYPE.MOVEMENT);
        public bool IsFill() => CurrentUpdateType.Equals(UPDATE_TYPE.FILL);
    }

    public record CellPieceMovement(GridCell PreviousCell, GridCell NewCell, Piece Piece);
    public record CellPieceFill(GridCell Cell, Piece Piece);

    #endregion

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
        public event Action? SpentAllMovements;

        public delegate void ConsumedSequenceEventHandler(Sequence sequence);
        public event ConsumedSequenceEventHandler? ConsumedSequence;
        #endregion

        public int GridWidth { get => _gridWidth; set { _gridWidth = Math.Max(MIN_GRID_WIDTH, value); } }
        public int GridHeight { get => _gridHeight; set { _gridHeight = Math.Max(MIN_GRID_HEIGHT, value); } }

        private int _gridWidth;
        private int _gridHeight;

        public Vector2 CellSize = new(48, 48);
        public Vector2 Offset = new(5, 10);

        public int RemainingMoves {
            get => _remainingMoves;
            set {
                if (value != _remainingMoves && value.Equals(0)) {
                    Locked = true;
                    SpentAllMovements?.Invoke();
                }

                _remainingMoves = Math.Max(0, value);
            }
        }

        private int _remainingMoves;

        public IPieceGenerator PieceGenerator;
        public ISequenceFinder SequenceFinder;

        public List<List<GridCell>> GridCells = [];
        public List<Piece> AvailablePieces = [];

        public bool Locked = false;

        #region Constructors
        public Board(
            int gridWidth,
            int gridHeight,
            int initialMoves,
            IPieceGenerator? pieceGenerator = null,
            ISequenceFinder? sequenceFinder = null
        ) {
            GridWidth = gridWidth;
            GridHeight = gridHeight;
            RemainingMoves = initialMoves;
            PieceGenerator = pieceGenerator is not null ? pieceGenerator : new PieceWeightGenerator();
            SequenceFinder = sequenceFinder is not null ? sequenceFinder : new SequenceFinder();
        }

        public Board(Vector2 size, int initialMoves, IPieceGenerator? pieceGenerator = null, ISequenceFinder? sequenceFinder = null) {
            GridWidth = (int)size.X;
            GridHeight = (int)size.Y;
            RemainingMoves = initialMoves;
            PieceGenerator = pieceGenerator is not null ? pieceGenerator : new PieceWeightGenerator();
            SequenceFinder = sequenceFinder is not null ? sequenceFinder : new SequenceFinder();
        }

        public static Board Create(
            int gridWidth,
            int gridHeight,
            int initialMoves,
            IPieceGenerator? pieceGenerator = null,
            ISequenceFinder? sequenceFinder = null
        ) => new(gridWidth, gridHeight, initialMoves, pieceGenerator, sequenceFinder);

        public static Board Create(Vector2 size, int initialMoves, IPieceGenerator? pieceGenerator = null, ISequenceFinder? sequenceFinder = null)
            => Create((int)size.X, (int)size.Y, initialMoves, pieceGenerator, sequenceFinder);

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

        public Board ChangeRemainingMoves(int moves) {
            RemainingMoves = moves;

            return this;
        }

        public Board IncreaseMove() {
            IncreaseMoves(1);

            return this;
        }

        public Board DecreaseMove() {
            DecreaseMoves(1);

            return this;
        }
        public Board IncreaseMoves(int amount) {
            RemainingMoves += amount;

            return this;
        }

        public Board DecreaseMoves(int amount) {
            RemainingMoves -= amount;

            return this;
        }

        public bool IsLocked() => Locked;
        public bool IsFree() => !Locked;

        public void Lock() {
            Locked = true;
        }

        public void Unlock() {
            Locked = false;
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

        public Vector2 CellPosition(GridCell cell) => new(CellSize.X * cell.Column + Offset.X, CellSize.Y * cell.Row + Offset.Y);

        public Board PrepareGridCells(List<Vector2>? disabledCells = null, bool overwrite = false) {
            if (GridCells.IsEmpty() || overwrite) {

                GridCells.Clear();
                disabledCells ??= CellsThatCannotContainPieces().Select(cell => cell.Position()).ToList();

                foreach (var column in Enumerable.Range(0, GridWidth)) {
                    GridCells.Add([]);

                    foreach (var row in Enumerable.Range(0, GridHeight)) {
                        GridCells[column].Add(new GridCell(column, row));
                    }
                }

                UpdateGridCellsNeighbours();

                if (disabledCells.Count > 0)
                    CellsThatCannotContainPieces(disabledCells.Select(Cell));

                PreparedBoard?.Invoke();
            }

            return this;
        }

        public Board CellsThatCanContainPieces(IEnumerable<GridCell?> cells) {
            cells.RemoveNullables().ToList().ForEach(cell => cell.CanContainPiece = true);

            return this;
        }
        public Board CellsThatCannotContainPieces(IEnumerable<GridCell?> cells) {
            cells.RemoveNullables().ToList().ForEach(cell => cell.CanContainPiece = false);

            return this;
        }

        public Board FillInitialBoard(bool allowMatchesOnStart = false, Dictionary<string, Piece>? preSelectedPieces = null) {
            if (AvailablePieces.Count > 2 && GridCells.Count > 0) {

                foreach (var column in Enumerable.Range(0, GridWidth)) {
                    foreach (var row in Enumerable.Range(0, GridHeight)) {

                        if (Cell(column, row) is GridCell currentCell && currentCell.IsEmpty()) {

                            if (preSelectedPieces is not null && preSelectedPieces.TryGetValue(currentCell.Position().ToString(), out Piece piece))
                                currentCell.AssignPiece(piece);
                            else
                                currentCell.AssignPiece(PieceGenerator.Roll(AvailablePieces));

                        }
                    }
                }

                if (EmptyCells().Where(cell => cell.CanContainPiece).Any())
                    throw new InvalidOperationException("Board->FillInitialBoard: After calling the function, some cells are still empty, the operation is not valid");


                if (!allowMatchesOnStart && (preSelectedPieces is null || preSelectedPieces.IsEmpty()))
                    RemoveMatchesFromBoard();

                FilledBoard?.Invoke();
            }

            return this;
        }

        public GridCell? TopRightCornerCell() => Cell(GridWidth - 1, 0);
        public GridCell? TopLeftCornerCell() => Cell(0, 0);
        public GridCell? BottomRightCornerCell() => Cell(GridWidth - 1, GridHeight - 1);
        public GridCell? BottomLeftCornerCell() => Cell(0, GridHeight - 1);

        public List<GridCell> UpperCellsFrom(GridCell cell, int distance) {
            List<GridCell> cells = [];

            distance = Math.Clamp(distance, 0, GridHeight);

            var currentCell = cell.NeighbourUp;

            if (distance > 0 && currentCell is not null)
                cells.AddRange([currentCell, .. UpperCellsFrom(currentCell, distance - 1)]);

            return cells;
        }

        public List<GridCell> BottomCellsFrom(GridCell cell, int distance) {
            List<GridCell> cells = [];

            distance = Math.Clamp(distance, 0, GridHeight);

            var currentCell = cell.NeighbourBottom;

            if (distance > 0 && currentCell is not null)
                cells.AddRange([currentCell, .. BottomCellsFrom(currentCell, distance - 1)]);

            return cells;
        }


        public List<GridCell> RightCellsFrom(GridCell cell, int distance) {
            List<GridCell> cells = [];

            distance = Math.Clamp(distance, 0, GridWidth);

            var currentCell = cell.NeighbourRight;

            if (distance > 0 && currentCell is not null)
                cells.AddRange([currentCell, .. RightCellsFrom(currentCell, distance - 1)]);

            return cells;
        }

        public List<GridCell> LeftCellsFrom(GridCell cell, int distance) {
            List<GridCell> cells = [];

            distance = Math.Clamp(distance, 0, GridWidth);

            var currentCell = cell.NeighbourLeft;

            if (distance > 0 && currentCell is not null)
                cells.AddRange([currentCell, .. LeftCellsFrom(currentCell, distance - 1)]);

            return cells;
        }

        public List<GridCell> DiagonalTopRightCellsFrom(GridCell cell, int distance) {
            List<GridCell> cells = [];

            distance = Math.Clamp(distance, 0, GridWidth);

            var currentCell = cell.DiagonalNeighbourTopRight;

            if (distance > 0 && currentCell is not null)
                cells.AddRange([currentCell, .. DiagonalTopRightCellsFrom(currentCell, distance - 1)]);

            return cells;
        }

        public List<GridCell> DiagonalTopLeftCellsFrom(GridCell cell, int distance) {
            List<GridCell> cells = [];

            distance = Math.Clamp(distance, 0, GridWidth);

            var currentCell = cell.DiagonalNeighbourTopLeft;

            if (distance > 0 && currentCell is not null)
                cells.AddRange([currentCell, .. DiagonalTopLeftCellsFrom(currentCell, distance - 1)]);

            return cells;
        }

        public List<GridCell> DiagonalBottomLeftCellsFrom(GridCell cell, int distance) {
            List<GridCell> cells = [];

            distance = Math.Clamp(distance, 0, GridWidth);

            var currentCell = cell.DiagonalNeighbourBottomLeft;

            if (distance > 0 && currentCell is not null)
                cells.AddRange([currentCell, .. DiagonalBottomLeftCellsFrom(currentCell, distance - 1)]);

            return cells;
        }

        public List<GridCell> DiagonalBottomRightCellsFrom(GridCell cell, int distance) {
            List<GridCell> cells = [];

            distance = Math.Clamp(distance, 0, GridWidth);

            var currentCell = cell.DiagonalNeighbourBottomRight;

            if (distance > 0 && currentCell is not null)
                cells.AddRange([currentCell, .. DiagonalBottomRightCellsFrom(currentCell, distance - 1)]);

            return cells;
        }

        public List<Piece?> UpperCellPiecesFrom(GridCell cell, int distance) =>
         UpperCellsFrom(cell, distance).Select(cell => cell.Piece).ToList();
        public List<Piece?> BottomCellPiecesFrom(GridCell cell, int distance)
            => BottomCellsFrom(cell, distance).Select(cell => cell.Piece).ToList();
        public List<Piece?> RightCellPiecesFrom(GridCell cell, int distance) =>
            RightCellsFrom(cell, distance).Select(cell => cell.Piece).ToList();
        public List<Piece?> LeftCellPiecesFrom(GridCell cell, int distance)
            => LeftCellsFrom(cell, distance).Select(cell => cell.Piece).ToList();

        public List<GridCell> EmptyCells() => GridCells.SelectMany(cells => cells).Where(cell => cell.IsEmpty()).ToList();
        public List<GridCell> EmptyCellsFromRow(int row) => CellsFromRow(row).Where(cell => cell.IsEmpty()).ToList();
        public List<GridCell> EmptyCellsFromColumn(int column) => CellsFromColumn(column).Where(cell => cell.IsEmpty()).ToList();
        public List<GridCell> CellsThatCanContainPieces() => GridCells.SelectMany(cells => cells).Where(cell => cell.CanContainPiece).ToList();
        public List<GridCell> CellsThatCannotContainPieces() => GridCells.SelectMany(cells => cells).Where(cell => !cell.CanContainPiece).ToList();
        public List<GridCell> CellsThatCanContainPiecesFromRow(int row) => CellsFromRow(row).Where(cell => cell.CanContainPiece).ToList();
        public List<GridCell> CellsThatCannotContainPiecesFromRow(int row) => CellsFromRow(row).Where(cell => !cell.CanContainPiece).ToList();
        public List<GridCell> CellsThatCanContainPiecesFromColumn(int column) => CellsFromColumn(column).Where(cell => cell.CanContainPiece).ToList();
        public List<GridCell> CellsThatCannotContainPiecesFromColumn(int column) => CellsFromColumn(column).Where(cell => !cell.CanContainPiece).ToList();
        public Sequence CreateSequenceFromRow(int row) => new(CellsThatCanContainPiecesFromRow(row), Sequence.SHAPES.HORIZONTAL);
        public Sequence CreateSequenceFromColumn(int column) => new(CellsThatCanContainPiecesFromColumn(column), Sequence.SHAPES.VERTICAL);
        public Sequence CreateSequenceFromRowOfPieceType(int row, Type type) => new(CellsFromRowOfPieceType(row, type), Sequence.SHAPES.HORIZONTAL);
        public Sequence CreateSequenceFromColumnOfPieceType(int column, Type type) => new(CellsFromColumnOfPieceType(column, type), Sequence.SHAPES.VERTICAL);
        public Sequence CreateSequenceOfCellsWithPieceType(Type type) => new(CellsWithPieceType(type));
        public Sequence CreateSequenceFromRowOfShape(int row, string shape) => new(CellsFromRowOfShape(row, shape), Sequence.SHAPES.HORIZONTAL);
        public Sequence CreateSequenceFromColumnOfShape(int column, string shape) => new(CellsFromColumnOfShape(column, shape), Sequence.SHAPES.VERTICAL);
        public Sequence CreateSequenceOfCellsWithShape(string shape) => new(CellsWithShape(shape));

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

        public List<GridCell> CellsFromRowOfPieceType(int row, Type type) {
            return CellsFromRow(row)
                .Where(cell => cell.HasPiece() && cell.Piece.Type.GetType().Equals(type))
                .ToList();
        }

        public List<GridCell> CellsFromColumnOfPieceType(int column, Type type) {
            return CellsFromColumn(column)
                .Where(cell => cell.HasPiece() && cell.Piece.Type.GetType().Equals(type))
                .ToList();
        }


        public List<GridCell> CellsWithPieceType(Type type) {
            return GridCells.SelectMany(cells => cells)
                .Where(cell => cell.HasPiece() && cell.Piece.Type.GetType().Equals(type))
                .ToList();
        }

        public List<GridCell> CellsFromRowOfShape(int row, string shape) {
            return CellsFromRow(row)
                .Where(cell => cell.HasPiece() && cell.Piece.Type.Shape.Equals(shape, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        public List<GridCell> CellsFromColumnOfShape(int column, string shape) {
            return CellsFromColumn(column)
                .Where(cell => cell.HasPiece() && cell.Piece.Type.Shape.Equals(shape, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        public List<GridCell> CellsWithShape(string shape) {
            return GridCells.SelectMany(cells => cells)
                .Where(cell => cell.HasPiece() && cell.Piece.Type.Shape.Equals(shape, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        public List<GridCell> AdjacentCellsFrom(GridCell originCell, bool includeDiagonals = false) {
#pragma warning disable 8601
            List<GridCell> adjacentCells = [
                originCell.NeighbourUp,
                originCell.NeighbourRight,
                originCell.NeighbourLeft,
                originCell.NeighbourBottom,
            ];

            if (includeDiagonals)
                adjacentCells.AddRange([originCell.DiagonalNeighbourBottomRight,
                    originCell.DiagonalNeighbourBottomLeft,
                    originCell.DiagonalNeighbourTopRight,
                    originCell.DiagonalNeighbourTopLeft
                ]);

            return adjacentCells.RemoveNullables().RemoveDuplicates().ToList();
        }

        public List<GridCell> CrossCellsFrom(GridCell originCell) {
            return CellsFromRow(originCell.Row)
                    .Concat(CellsFromColumn(originCell.Column))
                    .RemoveDuplicates()
                    .ToList();
        }

        public List<GridCell> CrossDiagonalCellsFrom(GridCell originCell) {
            var distance = GridWidth + GridHeight;

            return DiagonalTopLeftCellsFrom(originCell, distance)
                    .Concat(DiagonalTopRightCellsFrom(originCell, distance))
                    .Concat(DiagonalBottomLeftCellsFrom(originCell, distance))
                    .Concat(DiagonalBottomRightCellsFrom(originCell, distance))
                    .RemoveDuplicates()
                    .RemoveNullables()
                    .ToList();
        }


        public void UpdateGridCellsNeighbours() {
            GridCells.SelectMany(cells => cells).ToList().ForEach(cell => {
                cell.NeighbourUp = Cell(cell.Column, cell.Row - 1);
                cell.NeighbourBottom = Cell(cell.Column, cell.Row + 1);
                cell.NeighbourRight = Cell(cell.Column + 1, cell.Row);
                cell.NeighbourLeft = Cell(cell.Column - 1, cell.Row);
                cell.DiagonalNeighbourTopRight = Cell(cell.Column + 1, cell.Row - 1);
                cell.DiagonalNeighbourTopLeft = Cell(cell.Column - 1, cell.Row - 1);
                cell.DiagonalNeighbourBottomRight = Cell(cell.Column + 1, cell.Row + 1);
                cell.DiagonalNeighbourBottomLeft = Cell(cell.Column - 1, cell.Row + 1);
            });
        }
        public GridCell? FindGridCellWithPiece(Piece piece) => FindGridCellWithPiece(piece.Id);
        public GridCell? FindGridCellWithPiece(Guid id) => FindGridCellWithPiece(id.ToString());
        public GridCell? FindGridCellWithPiece(string id) {
            return GridCells.SelectMany(cells => cells)
                .Where(cell => cell.HasPiece())
                .ToList()
                .Find(cell => cell.Piece.Id.ToString().Equals(id));
        }

        public Dictionary<GridCell, GridCell> Shuffle(IEnumerable<Type>? exceptPieceTypes = null, IEnumerable<GridCell?>? exceptCells = null) {
            exceptPieceTypes ??= [];
            exceptCells ??= [];
            exceptCells.RemoveNullables();

            List<GridCell> snapshot = GridCells.SelectMany(cells => cells)
                .Where(cell => cell.HasPiece()
                    && cell.Piece.Type.CanBeShuffled()
                    && !exceptCells.Contains(cell)
                    && !exceptPieceTypes.Contains(cell.Piece.Type.GetType()))
                .ToList();

            if (GridCells.IsEmpty() || snapshot.IsEmpty())
                throw new ArgumentException("Board::Shuffle: The board is empty, cannot be shuffled.");

            List<GridCell> shuffleCells = [];
            Dictionary<GridCell, GridCell> result = [];

            snapshot.Shuffle();

            snapshot.ForEach(cell => {
                if (!shuffleCells.Contains(cell)) {
                    shuffleCells.Add(cell);

                    var availableCells = snapshot.Where(cell => !shuffleCells.Contains(cell));

                    if (availableCells.Any()) {
                        GridCell targetCell = availableCells.RandomElement();

                        targetCell.SwapPieceWith(cell);
                        result.Add(targetCell, cell);
                        shuffleCells.Add(targetCell);
                    }
                }
            });

            return result;
        }

        public void RemoveMatchesFromBoard() {
            var sequences = SequenceFinder.FindBoardSequences(this);

            while (sequences.Count > 0) {
                foreach (Sequence sequence in sequences) {
                    var cellsToChange = sequence.Cells.Take((sequence.Cells.Count / 3) + 1);
                    var pieceTypesToChange = cellsToChange.Select(cell => cell.Piece.Type);

                    var availablePieces = AvailablePieces.Where(piece => {
                        return !pieceTypesToChange.Contains(piece.Type)
                        && !pieceTypesToChange.Select(pieceType => pieceType.Shape).Contains(piece.Type.Shape);
                    }).ToList();

                    foreach (GridCell currentCell in cellsToChange) {
                        var newPiece = PieceGenerator.Roll(availablePieces, [currentCell.Piece.Type.GetType()]);

                        if (newPiece is not null) {
                            currentCell.RemovePiece();
                            currentCell.AssignPiece(newPiece);
                        }
                    }
                }

                sequences = SequenceFinder.FindBoardSequences(this);
            }
        }

        public VirtualBoard MovePiecesAndFillEmptyCells() {
            if (GridCells.IsEmpty())
                throw new ArgumentException("The board need to have the grid cells prepared, this operation cannot be done");

            VirtualBoard virtualBoard = new(GridCells);

            var pendingMoves = PendingFallMoves(virtualBoard.GridCells);

            while (pendingMoves.Count > 0 && !SelectedFillMode.Equals(FILL_MODES.IN_PLACE)) {
                foreach (var currentCell in pendingMoves) {
                    GridCell? bottomCell = currentCell.NeighbourBottom;

                    if (bottomCell is not null) {

                        if (SelectedFillMode.Equals(FILL_MODES.SIDE_DOWN) && bottomCell.HasPiece()) {
                            bottomCell = currentCell.DiagonalNeighbourBottomLeft is not null && currentCell.DiagonalNeighbourBottomLeft.IsEmpty() ?
                               currentCell.DiagonalNeighbourBottomLeft :
                               currentCell.DiagonalNeighbourBottomRight;
                        }

                        if (bottomCell is not null && bottomCell.IsEmpty()) {
                            bottomCell.AssignPiece(currentCell.Piece);
                            currentCell.RemovePiece();
                        }

                        virtualBoard.Update(new BoardCellUpdate(UPDATE_TYPE.MOVEMENT, new(currentCell, bottomCell, bottomCell.Piece)));
                    }
                }

                pendingMoves = PendingFallMoves(virtualBoard.GridCells);
            }

            foreach (var emptyCell in virtualBoard.EmptyCells()) {
                GenerateRandomPieceOnCell(emptyCell);

                virtualBoard.Update(new BoardCellUpdate(UPDATE_TYPE.FILL, null, new(emptyCell, emptyCell.Piece)));
            }

            return virtualBoard;
        }

        public List<GridCell> PendingFallMoves(IEnumerable<GridCell>? cells = null) {
            cells ??= GridCells.SelectMany(cells => cells).Select(cell => cell).ToList();

            return cells.Where(cell => cell.HasPiece() && cell.Piece.Type.CanBeMoved() && cell.NeighbourBottom is not null)
                .Where(cell => (SelectedFillMode.Equals(FILL_MODES.FALL_DOWN) && cell.NeighbourBottom.IsEmpty()) ||
                 (SelectedFillMode.Equals(FILL_MODES.SIDE_DOWN) && (cell.NeighbourBottom.IsEmpty() || cell.NeighbourBottom.HasPiece())
                 && (cell.DiagonalNeighbourBottomRight is not null && cell.DiagonalNeighbourBottomRight.IsEmpty() || cell.DiagonalNeighbourBottomLeft is not null && cell.DiagonalNeighbourBottomLeft.IsEmpty()))
                )
                .ToList();
        }
        #endregion

        #region Pieces

        public void GenerateRandomPieceOnCells(IEnumerable<GridCell> cells, IEnumerable<Type>? only = null) {
            foreach (GridCell cell in cells)
                GenerateRandomPieceOnCell(cell, only);
        }
        public void GenerateRandomPieceOnCell(GridCell cell, IEnumerable<Type>? only = null) {
            cell.AssignPiece(GenerateRandomPiece(only));
        }
        public Piece GenerateRandomPiece(IEnumerable<Type>? only = null) {
            if (AvailablePieces.IsEmpty())
                throw new InvalidOperationException("The available pieces on this board is empty, piece cannot be generated");

            return PieceGenerator.Roll(AvailablePieces, only);
        }

        public Board AddAvailablePieces(IList<Piece> pieces) {
            AvailablePieces.AddRange(pieces);
            AvailablePieces = AvailablePieces.RemoveDuplicates().ToList();

            return this;
        }

        public Board AddAvailablePiece(Piece piece) {
            AvailablePieces.Add(piece);
            AvailablePieces = AvailablePieces.RemoveDuplicates().ToList();

            return this;
        }
        public Board RemoveAvailablePieces(IList<Piece> pieces) {
            foreach (Piece piece in pieces)
                RemoveAvailablePiece(piece);

            return this;
        }
        public Board RemoveAvailablePiece(Piece piece) {
            AvailablePieces.Remove(piece);

            return this;
        }

        #endregion

        #region Sequences
        public void ConsumeSequence(Sequence sequence) {
            ConsumedSequence?.Invoke(sequence.Clone() as Sequence);
            sequence.Consume();
        }
        #endregion
    }
}