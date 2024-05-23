using Dame.MVVM.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dame.MVVM.ViewModel
{
    internal class PieceViewModel
    {
        //generate all Pieces on the board
        public static List<Piece> GeneratePieces()
        {
            List<Piece> pieces = new List<Piece>();
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if ((i + j) % 2 == 0)
                    {
                        pieces.Add(new Piece(PieceColor.White, i, j));
                    }
                }
            }
            for (int i = 5; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if ((i + j) % 2 == 0)
                    {
                        pieces.Add(new Piece(PieceColor.Black, i, j));
                    }
                }
            }
            return pieces;
        }
    }
}
