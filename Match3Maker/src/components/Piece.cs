using System.ComponentModel;

namespace Match3Maker {
    public class Piece(IPieceType type, float weight = 1f) : INotifyPropertyChanged, ICloneable {

        #region Properties
        public Guid Id = Guid.NewGuid();
        public IPieceType Type = type;

        public float Weight = weight;
        public float TotalAccumWeight = 0f;

        public bool Locked {
            get => _locked; set {
                if (_locked != value)
                    OnPropertyChanged(nameof(Locked));

                _locked = value;
            }
        }

        private bool _locked = false;

        public static Piece Create(IPieceType type, float weight = 1f) => new(type, weight);

        #endregion
        public event PropertyChangedEventHandler? PropertyChanged;

        public void ResetAccumWeight() {
            TotalAccumWeight = 0f;
        }

        public bool MatchWith(Piece piece) => Type.MatchWith(piece);

        public bool NotMatchWith(Piece piece) => !MatchWith(piece);

        public void Lock() {
            Locked = true;
        }

        public void Unlock() {
            Locked = false;
        }

        public object Clone() {
            return new Piece(Type, Weight) {
                Locked = Locked,
                TotalAccumWeight = 0f
            };
        }

        #region EventHandlers
        private void OnPropertyChanged(string propertyName) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
