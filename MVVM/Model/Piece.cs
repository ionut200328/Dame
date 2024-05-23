using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dame.MVVM.Model
{
    public class Piece: INotifyPropertyChanged
    {
        private bool _isQueen;
        public PieceType Type { get; set; }
        public PieceColor Color { get; set; }
        public int Row { get; set; }
        public int Col { get; set; }
        public bool IsQueen { 
            get => _isQueen;
            set
            {
                _isQueen = value;
                OnPropertyChanged(nameof(IsQueen));
            }

        }
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public Piece()
        {
        }
        
        public Piece(Piece pice)
        {
            Type = pice.Type;
            Color = pice.Color;
            Row = pice.Row;
            Col = pice.Col;
            IsQueen = pice.IsQueen;
        }

        public Piece(Piece pice, int row, int col)
        {
            Type = pice.Type;
            Color = pice.Color;
            Row = row;
            Col = col;
            IsQueen = pice.IsQueen;
        }

        public Piece(PieceColor color, int row, int col)
        {
            Type = PieceType.Pawn;
            Color = color;
            Row = row;
            Col = col;
            IsQueen = false;
        }

        public Piece(PieceColor color, int row, int col, bool isQueen)
        {
            Type = PieceType.Queen;
            Color = color;
            Row = row;
            Col = col;
            IsQueen = isQueen;
        }
        public Piece(PieceType type, PieceColor color, int row, int col, bool isQueen)
        {
            Type = type;
            Color = color;
            Row = row;
            Col = col;
            IsQueen = isQueen;
        }
        public override string ToString()
        {
            return $"{Type} {Color} {Row} {Col}";
        }
        public void Promote()
        {
            IsQueen = true;
            Type = PieceType.Queen;
        }

        public static string PositionToString(int row, int col)
        {
            char colLetter = (char)('a' + col);
            int rowNumber = row + 1; // Assuming row index starts at 0
            return $"{colLetter}{rowNumber}";
        }

        public static (int Row, int Col) StringToPosition(string position)
        {
            int col = position[0] - 'a';
            int row = int.Parse(position.Substring(1)) - 1;
            return (row, col);
        }

    }
}
