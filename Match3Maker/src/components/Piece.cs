using System.ComponentModel;

namespace Match3Maker {

    /// <summary>
    /// Represents a configuration for a game piece used in determining roll probabilities within a match-3 or similar game.
    /// </summary>
    public sealed class PieceWeight(string shape, Piece.TYPES type = Piece.TYPES.NORMAL, float weight = 1f) {
        public Piece.TYPES Type = type;
        public string Shape = shape;

        #region RollProperties
        public float Weight = weight;
        public float TotalAccumWeight = 0f;
        #endregion
        public void ResetAccumWeight() {
            TotalAccumWeight = 0f;
        }
    }

    public class Piece : INotifyPropertyChanged, ICloneable {
        public enum TYPES {
            NORMAL, SPECIAL, OBSTACLE
        }

        #region Properties
        public Guid Id = Guid.NewGuid();
        public TYPES Type;
        public string Shape;
        public bool Locked {
            get => _locked; set {
                if (_locked != value)
                    OnPropertyChanged(nameof(Locked));

                _locked = value;
            }
        }

        private bool _locked = false;

        public Piece(string shape, TYPES type = TYPES.NORMAL) {
            Shape = shape;
            Type = type;

            if (type.Equals(TYPES.OBSTACLE))
                Lock();

        }

        public static Piece From(PieceWeight pieceWeight) => new(pieceWeight.Shape, pieceWeight.Type);

        #endregion
        public event PropertyChangedEventHandler? PropertyChanged;

        public bool IsNormal() => Type.Equals(TYPES.NORMAL);
        public bool IsSpecial() => Type.Equals(TYPES.SPECIAL);
        public bool IsObstacle() => Type.Equals(TYPES.OBSTACLE);


        public bool MatchWith(Piece piece)
            => Type.Equals(piece.Type) && Shape.Trim().Equals(piece.Shape, StringComparison.OrdinalIgnoreCase);

        public bool NotMatchWith(Piece piece) => !MatchWith(piece);

        public void Lock() {
            Locked = true;
        }

        public void Unlock() {
            Locked = false;
        }

        public object Clone() {
            return MemberwiseClone();
        }

        #region EventHandlers
        private void OnPropertyChanged(string propertyName) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

     
        #endregion
    }
}
