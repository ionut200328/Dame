using System.ComponentModel;
using System.Windows.Media;
using System.Windows.Input;
using Dame.Core; // Assuming RelayCommand is in Dame.Core
using Dame.MVVM.Model;
using System;

namespace Dame.MVVM.ViewModel
{
    public class ChessCellViewModel : INotifyPropertyChanged
    {
        private Piece _piece;

        private ChessCellViewModel _selectedCell;
        private GameViewModel _gameViewModel; // Store reference to GameViewModel

        public SolidColorBrush CellColor { get; private set; }
        public ICommand MovePieceCommand { get; }
        private bool _isMoveTarget;
        public bool IsMoveTarget
        {
            get => _isMoveTarget;
            set
            {
                _isMoveTarget = value;
                OnPropertyChanged(nameof(IsMoveTarget));
            }
        }

        private bool _isJumpTarget;
        public bool IsJumpTarget
        {
            get => _isJumpTarget;
            set
            {
                _isJumpTarget = value;
                OnPropertyChanged(nameof(IsJumpTarget));
            }
        }

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                OnPropertyChanged(nameof(IsSelected));
            }
        }

        public Piece Piece
        {
            get => _piece;
            set
            {
                if (_piece != value)
                {
                    _piece = value;
                    OnPropertyChanged(nameof(Piece));
                }
            }
        }
        public ChessCellViewModel SelectedCell
        {
            get => _selectedCell;
            set
            {
                _selectedCell = value;
                OnPropertyChanged(nameof(SelectedCell));
            }
        }

        // Modified constructor to accept GameViewModel
        public ChessCellViewModel(Color cellColor, GameViewModel gameViewModel)
        {
            CellColor = new SolidColorBrush(cellColor);
            _gameViewModel = gameViewModel; // Store the passed-in GameViewModel reference

            // Initialize the command with a lambda that calls MovePiece on the GameViewModel
            MovePieceCommand = new RelayCommand(param =>
            {
                if (Piece != null && !IsMoveTarget && !IsJumpTarget)
                {
                    // If the cell has a piece and it's not currently marked as a move target, select it.
                    
                    //if(Piece.Type == PieceType.Queen)
                    //    Piece.IsQueen = true;
                    _gameViewModel.SelectPiece(this);
                }
                else if (Piece == null && (IsMoveTarget || IsJumpTarget))
                {
                    // If the cell is marked as a move target, move the selected piece here.
                    _gameViewModel.SelectMoveTarget(this);
                    // New logic for identifying jump opportunities...
                   OnPropertyChanged(nameof(Piece));

                }
                // New logic for identifying jump opportunities...
               

            }, param => Piece != null || IsMoveTarget || IsJumpTarget);
        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
