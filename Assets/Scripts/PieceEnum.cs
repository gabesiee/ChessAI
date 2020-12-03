using System;
using System.Collections.Generic;
using System.Text;

namespace ChessEngine
{
    public enum PieceEnum
    {
        Pawn = 1,
        Knight = 2,
        Bishop = 3,
        Rook = 4,
        Queen = 5,
        King = 6

    }

    static class PieceEnumMethods
    {

        public static int GetValue(this PieceEnum p)
        {
            switch (p)
            {
                case PieceEnum.Pawn:
                    return 10;
                case PieceEnum.Knight:
                    return 30;
                case PieceEnum.Bishop:
                    return 30;
                case PieceEnum.Rook:
                    return 50;
                case PieceEnum.Queen:
                    return 90;
                case PieceEnum.King:
                    return 900;
                default:
                    return 0;
            }
        }
    }
}
