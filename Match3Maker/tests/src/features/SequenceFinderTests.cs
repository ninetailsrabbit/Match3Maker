//using Match3Maker;
//using Xunit;

//namespace Match3Tests {

//    public class SequenceFinderTests {
//        private readonly ISequenceFinder _sequenceMatcher = new SequenceFinder(3, 5, 2, 3, true, true, true, true);

//        [Fact]
//        public void Should_Return_Empty_When_No_Horizontal_Matches_Found() {
//            var sequences = _sequenceMatcher.EnableHorizontalShape().FindHorizontalSequences([]);

//            Assert.Empty(sequences);
//        }

//        [Fact]
//        public void Should_Return_Empty_When_Horizontal_Matches_Are_Disabled() {
//            var cells = new List<GridCell>() { new(2, 2), new(2, 3), new(2, 4), };

//            var sequences = _sequenceMatcher.DisableHorizontalShape().FindHorizontalSequences(cells);

//            Assert.Empty(sequences);
//        }

//        [Fact]
//        public void Should_Return_Empty_When_No_Vertical_Matches_Found() {
//            var sequences = _sequenceMatcher.EnableVerticalShape().FindVerticalSequences([]);

//            Assert.Empty(sequences);
//        }

//        [Fact]
//        public void Should_Return_Empty_When_Vertical_Matches_Are_Disabled() {
//            var cells = new List<GridCell>() { new(2, 1), new(2, 2), new(2, 3), };

//            var sequences = _sequenceMatcher.DisableVerticalShape().FindVerticalSequences(cells);

//            Assert.Empty(sequences);
//        }

//        [Fact]
//        public void Should_Find_Horizontal_Matches_When_Enabled() {
//            var cells = new List<GridCell>() { new(3, 2), new(4, 2), new(5, 2), }
//            .Select(cell => {
//                cell.AssignPiece(new Piece("square"));
//                return cell;
//            });

//            var sequences = _sequenceMatcher.EnableHorizontalShape().FindHorizontalSequences(cells);

//            Assert.Single(sequences);
//            Assert.Equal(cells.Count(), sequences.First().Cells.Count);
//            Assert.True(sequences.First().Cells.All(cell => cell.Piece.Shape.Equals("square")));
//        }

//        [Fact]
//        public void Should_Find_Horizontal_Matches_On_Min_Max_Match_Range_Order_Desc_By_Size() {
//            var match3Cells = new List<GridCell>() { new(0, 2), new(1, 2), new(2, 2), }
//            .Select(cell => {
//                cell.AssignPiece(new Piece("square"));
//                return cell;
//            });

//            var match4Cells = new List<GridCell>() { new(3, 2), new(4, 2), new(5, 2), new(6, 2) }
//           .Select(cell => {
//               cell.AssignPiece(new Piece("circle"));
//               return cell;
//           });

//            var sequences = _sequenceMatcher.ChangeMinMatchTo(3)
//                .ChangeMaxMatchTo(4)
//                .EnableHorizontalShape()
//                .FindHorizontalSequences([.. match3Cells, .. match4Cells]);

//            Assert.Equal(2, sequences.Count());

//            Assert.Equal(match4Cells.Count(), sequences.First().Cells.Count);
//            Assert.Equal(match3Cells.Count(), sequences.ToArray()[1].Cells.Count);

//            Assert.True(sequences.First().Cells.All(cell => cell.Piece.Shape.Equals("circle")));
//            Assert.True(sequences.ToArray()[1].Cells.All(cell => cell.Piece.Shape.Equals("square")));
//        }

//        [Fact]
//        public void Should_Find_Horizontal_Matches_Up_To_Max_Match() {
//            var match5Cells = new List<GridCell>() { new(3, 2), new(4, 2), new(5, 2), new(6, 2), new(7, 2) }
//          .Select(cell => {
//              cell.AssignPiece(new Piece("circle"));
//              return cell;
//          });

//            var sequences = _sequenceMatcher.ChangeMaxMatchTo(4)
//               .EnableHorizontalShape()
//               .FindHorizontalSequences(match5Cells);

//            Assert.Single(sequences);
//            Assert.Equal(match5Cells.Count() - 1, sequences.First().Cells.Count);
//        }

//        [Fact]
//        public void Should_Find_Horizontal_Matches_From_Min_Match() {
//            var match2Cells = new List<GridCell>() { new(3, 2), new(4, 2) }
//        .Select(cell => {
//            cell.AssignPiece(new Piece("circle"));
//            return cell;
//        });
//            var match4Cells = new List<GridCell>() { new(5, 2), new(6, 2), new(7, 2), new(8, 2) }
//          .Select(cell => {
//              cell.AssignPiece(new Piece("circle"));
//              return cell;
//          });

//            var sequences = _sequenceMatcher.ChangeMinMatchTo(3)
//                .ChangeMaxMatchTo(3)
//               .EnableHorizontalShape()
//               .FindHorizontalSequences([.. match2Cells, .. match4Cells]);

//            Assert.Equal(2, sequences.Count());

//            Assert.Equal(3, sequences.First().Cells.Count);
//            Assert.Equal(3, sequences.ToArray()[1].Cells.Count);

//            Assert.True(sequences.First().Cells.All(cell => cell.Piece.Shape.Equals("circle")));
//            Assert.True(sequences.ToArray()[1].Cells.All(cell => cell.Piece.Shape.Equals("circle")));
//        }

//        [Fact]
//        public void Should_Find_Vertical_Matches_When_Enabled() {
//            var cells = new List<GridCell>() { new(2, 3), new(2, 4), new(2, 5), }
//            .Select(cell => {
//                cell.AssignPiece(new Piece("square"));
//                return cell;
//            });

//            var sequences = _sequenceMatcher.EnableVerticalShape().FindVerticalSequences(cells);

//            Assert.Single(sequences);
//            Assert.Equal(cells.Count(), sequences.First().Cells.Count);
//            Assert.True(sequences.First().Cells.All(cell => cell.Piece.Shape.Equals("square")));
//        }

//        [Fact]
//        public void Should_Find_Vertical_Matches_On_Min_Max_Match_Range_Order_Desc_By_Size() {
//            var match3Cells = new List<GridCell>() { new(0, 0), new(0, 1), new(0, 2), }
//            .Select(cell => {
//                cell.AssignPiece(new Piece("square"));
//                return cell;
//            });

//            var match4Cells = new List<GridCell>() { new(0, 3), new(0, 4), new(0, 5), new(0, 6) }
//           .Select(cell => {
//               cell.AssignPiece(new Piece("circle"));
//               return cell;
//           });

//            var sequences = _sequenceMatcher.ChangeMinMatchTo(3)
//                .ChangeMaxMatchTo(4)
//                .EnableVerticalShape()
//                .FindVerticalSequences([.. match3Cells, .. match4Cells]);

//            Assert.Equal(2, sequences.Count());

//            Assert.Equal(match4Cells.Count(), sequences.First().Cells.Count);
//            Assert.Equal(match3Cells.Count(), sequences.ToArray()[1].Cells.Count);

//            Assert.True(sequences.First().Cells.All(cell => cell.Piece.Shape.Equals("circle")));
//            Assert.True(sequences.ToArray()[1].Cells.All(cell => cell.Piece.Shape.Equals("square")));
//        }

//        [Fact]
//        public void Should_Find_Vertical_Matches_Up_To_Max_Match() {
//            var match5Cells = new List<GridCell>() { new(0, 3), new(0, 4), new(0, 5), new(0, 6), new(0, 7) }
//          .Select(cell => {
//              cell.AssignPiece(new Piece("circle"));
//              return cell;
//          });

//            var sequences = _sequenceMatcher.ChangeMaxMatchTo(4)
//               .EnableVerticalShape()
//               .FindVerticalSequences(match5Cells);

//            Assert.Single(sequences);
//            Assert.Equal(match5Cells.Count() - 1, sequences.First().Cells.Count);
//        }

//        [Fact]
//        public void Should_Find_Vertical_Matches_From_Min_Match() {
//            var match2Cells = new List<GridCell>() { new(2, 3), new(2, 4) }
//        .Select(cell => {
//            cell.AssignPiece(new Piece("circle"));
//            return cell;
//        });
//            var match4Cells = new List<GridCell>() { new(2, 5), new(2, 6), new(2, 7), new(2, 8) }
//          .Select(cell => {
//              cell.AssignPiece(new Piece("circle"));
//              return cell;
//          });

//            var sequences = _sequenceMatcher.ChangeMinMatchTo(3)
//                .ChangeMaxMatchTo(3)
//               .EnableVerticalShape()
//               .FindVerticalSequences([.. match2Cells, .. match4Cells]);

//            Assert.Equal(2, sequences.Count());

//            Assert.Equal(3, sequences.First().Cells.Count);
//            Assert.Equal(3, sequences.ToArray()[1].Cells.Count);

//            Assert.True(sequences.First().Cells.All(cell => cell.Piece.Shape.Equals("circle")));
//            Assert.True(sequences.ToArray()[1].Cells.All(cell => cell.Piece.Shape.Equals("circle")));
//        }

//        [Fact]
//        public void Should_Return_Null_When_Sequences_To_Find_TShape_Are_Not_Horizontal_Or_Vertical() {
//            var horizontalCells = new List<GridCell>() { new(3, 3), new(3, 4), new(3, 5), new(3, 6), new(3, 7) };
//            var verticalCells = new List<GridCell>() { new(2, 5), new(3, 5), new(4, 5) };

//            var horizontalSequence = new Sequence(horizontalCells, Sequence.SHAPES.T_SHAPE);
//            var verticalSequence = new Sequence(verticalCells, Sequence.SHAPES.L_SHAPE);

//            Assert.Null(_sequenceMatcher.FindTShapeSequence(horizontalSequence, verticalSequence));
//            Assert.Null(_sequenceMatcher.FindTShapeSequence(verticalSequence, horizontalSequence));

//            verticalSequence.Shape = Sequence.SHAPES.HORIZONTAL;

//            Assert.Null(_sequenceMatcher.FindTShapeSequence(horizontalSequence, verticalSequence));
//            Assert.Null(_sequenceMatcher.FindTShapeSequence(verticalSequence, horizontalSequence));

//            horizontalSequence.Shape = Sequence.SHAPES.L_SHAPE;
//            verticalSequence.Shape = Sequence.SHAPES.VERTICAL;

//            Assert.Null(_sequenceMatcher.FindTShapeSequence(horizontalSequence, verticalSequence));
//            Assert.Null(_sequenceMatcher.FindTShapeSequence(verticalSequence, horizontalSequence));
//        }

//        [Fact]
//        public void Should_Return_Null_When_Sequences_To_Find_LShape_Are_Not_Horizontal_Or_Vertical() {
//            var horizontalCells = new List<GridCell>() { new(3, 3), new(3, 4), new(3, 5), new(3, 6), new(3, 7) };
//            var verticalCells = new List<GridCell>() { new(2, 5), new(3, 5), new(4, 5) };

//            var horizontalSequence = new Sequence(horizontalCells, Sequence.SHAPES.T_SHAPE);
//            var verticalSequence = new Sequence(verticalCells, Sequence.SHAPES.L_SHAPE);

//            Assert.Null(_sequenceMatcher.FindLShapeSequence(horizontalSequence, verticalSequence));
//            Assert.Null(_sequenceMatcher.FindLShapeSequence(verticalSequence, horizontalSequence));

//            verticalSequence.Shape = Sequence.SHAPES.HORIZONTAL;

//            Assert.Null(_sequenceMatcher.FindLShapeSequence(horizontalSequence, verticalSequence));
//            Assert.Null(_sequenceMatcher.FindLShapeSequence(verticalSequence, horizontalSequence));

//            horizontalSequence.Shape = Sequence.SHAPES.L_SHAPE;
//            verticalSequence.Shape = Sequence.SHAPES.VERTICAL;

//            Assert.Null(_sequenceMatcher.FindLShapeSequence(horizontalSequence, verticalSequence));
//            Assert.Null(_sequenceMatcher.FindLShapeSequence(verticalSequence, horizontalSequence));
//        }

//        [Fact]
//        public void Should_Find_TSHapes() {
//            var horizontalCells = new List<GridCell>() {
//                new(3, 3, new Piece("triangle")),
//                new(4, 3, new Piece("triangle")),
//                new(5, 3, new Piece("square")),
//                new(6, 3, new Piece("triangle")),
//                new(7, 3, new Piece("triangle"))
//            };
//            var verticalCells = new List<GridCell>() { new(5, 3, new Piece("square")), new(5, 4, new Piece("square")), new(5, 5, new Piece("square")) };

//            var horizontalSequence = new Sequence(horizontalCells, Sequence.SHAPES.HORIZONTAL);
//            var verticalSequence = new Sequence(verticalCells, Sequence.SHAPES.VERTICAL);

//            // Horizontal - Vertical column bottom
//            var tShapeSequence = _sequenceMatcher.FindTShapeSequence(horizontalSequence, verticalSequence);

//            Assert.Equal((horizontalCells.Count + verticalCells.Count) - 1, tShapeSequence?.Cells.Count);
//            Assert.Equal(Sequence.SHAPES.T_SHAPE, tShapeSequence?.Shape);

//            // Horizontal - Vertical column top
//            verticalCells = [new(5, 3, new Piece("square")), new(5, 2, new Piece("square")), new(5, 1, new Piece("square"))];

//            tShapeSequence = _sequenceMatcher.FindTShapeSequence(horizontalSequence, verticalSequence);

//            Assert.Equal((horizontalCells.Count + verticalCells.Count) - 1, tShapeSequence?.Cells.Count);
//            Assert.Equal(Sequence.SHAPES.T_SHAPE, tShapeSequence?.Shape);

//            // Vertical - Horizontal line right
//            horizontalCells = [new(5, 4, new Piece("circle")), new(6, 4, new Piece("square")), new(7, 4, new Piece("square"))];
//            verticalCells = [new(5, 2, new Piece("circle")), new(5, 3, new Piece("circle")), new(5, 4, new Piece("circle")), new(5, 5, new Piece("circle")), new(5, 6, new Piece("circle"))];

//            tShapeSequence = _sequenceMatcher.FindTShapeSequence(horizontalSequence, verticalSequence);

//            Assert.Equal((horizontalCells.Count + verticalCells.Count) - 1, tShapeSequence?.Cells.Count);
//            Assert.Equal(Sequence.SHAPES.T_SHAPE, tShapeSequence?.Shape);

//            //Vertical - Horizontal line left
//            horizontalCells = [new(3, 4, new Piece("circle")), new(4, 4, new Piece("circle")), new(5, 4, new Piece("circle"))];

//            tShapeSequence = _sequenceMatcher.FindTShapeSequence(horizontalSequence, verticalSequence);

//            Assert.Equal((horizontalCells.Count + verticalCells.Count) - 1, tShapeSequence?.Cells.Count);
//            Assert.Equal(Sequence.SHAPES.T_SHAPE, tShapeSequence?.Shape);

//        }

//        [Fact]
//        public void Should_Find_LSHapes() {
//            var horizontalCells = new List<GridCell>() {
//                new(3, 3, new Piece("triangle")),
//                new(4, 3, new Piece("square")),
//                new(5, 3, new Piece("square")),
//                new(6, 3, new Piece("circle")),
//                new(7, 3, new Piece("square"))};

//            var verticalCells = new List<GridCell>() { new(3, 3, new Piece("triangle")), new(3, 4, new Piece("triangle")), new(3, 5, new Piece("triangle")) };

//            var horizontalSequence = new Sequence(horizontalCells, Sequence.SHAPES.HORIZONTAL);
//            var verticalSequence = new Sequence(verticalCells, Sequence.SHAPES.VERTICAL);

//            var lShapeSequence = _sequenceMatcher.FindLShapeSequence(horizontalSequence, verticalSequence);

//            Assert.Equal((horizontalCells.Count + verticalCells.Count) - 1, lShapeSequence?.Cells.Count);
//            Assert.Equal(Sequence.SHAPES.L_SHAPE, lShapeSequence?.Shape);

//            verticalCells = [new(3, 3, new Piece("triangle")), new(3, 2, new Piece("triangle")), new(3, 1, new Piece("triangle"))];

//            lShapeSequence = _sequenceMatcher.FindLShapeSequence(horizontalSequence, verticalSequence);

//            Assert.Equal((horizontalCells.Count + verticalCells.Count) - 1, lShapeSequence?.Cells.Count);
//            Assert.Equal(Sequence.SHAPES.L_SHAPE, lShapeSequence?.Shape);

//            verticalCells = [new(7, 3, new Piece("triangle")), new(7, 4, new Piece("triangle")), new(7, 5, new Piece("triangle"))];

//            lShapeSequence = _sequenceMatcher.FindLShapeSequence(horizontalSequence, verticalSequence);

//            Assert.Equal((horizontalCells.Count + verticalCells.Count) - 1, lShapeSequence?.Cells.Count);
//            Assert.Equal(Sequence.SHAPES.L_SHAPE, lShapeSequence?.Shape);

//            verticalCells = [new(7, 3, new Piece("triangle")), new(7, 2, new Piece("triangle")), new(7, 1, new Piece("triangle"))];

//            lShapeSequence = _sequenceMatcher.FindLShapeSequence(horizontalSequence, verticalSequence);

//            Assert.Equal((horizontalCells.Count + verticalCells.Count) - 1, lShapeSequence?.Cells.Count);
//            Assert.Equal(Sequence.SHAPES.L_SHAPE, lShapeSequence?.Shape);

//        }
//    }


//}