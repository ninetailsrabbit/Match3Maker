using System.ComponentModel;
using System.Drawing;

namespace Match3Maker {


    public class Piece : INotifyPropertyChanged, ICloneable, IEquatable<Piece> {
        public enum TYPES {
            NORMAL, SPECIAL, OBSTACLE
        }

        #region Properties
        public Guid Id = Guid.NewGuid();
        public TYPES Type;
        public string Shape;
        public Color? Color;

        public float Weight = 1f;
        public float TotalAccumWeight = 1f;

        public bool Locked {
            get => _locked; set {
                if (_locked != value)
                    OnPropertyChanged(nameof(Locked));

                _locked = value;
            }
        }

        private bool _locked = false;

        public Piece(string shape, TYPES type = TYPES.NORMAL, float weight = 1f) {
            Shape = shape;
            Type = type;
            Weight = weight;

            if (type.Equals(TYPES.OBSTACLE))
                Lock();
        }

        #endregion
        public event PropertyChangedEventHandler? PropertyChanged;

        public void ResetAccumWeight() {
            TotalAccumWeight = 0f;
        }
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
            return new Piece(Shape, Type, Weight) {
                Color = Color
            };
        }

        public bool Equals(Piece? other) {
            return MatchWith(other);
        }

        #region EventHandlers
        private void OnPropertyChanged(string propertyName) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
