using System;

namespace ChessEngine
{
    class Program
    {
        static void Main(string[] args)
        {
            PiecesManager p = new PiecesManager();
            //Util.PrintBitboard(p.RookAttackPattern(0x400));
            //Util.PrintBitboard(p.GetBlockerboard(36, 0b_1011100001));
            //Console.WriteLine(p.GetRookIndexFromBoard(0b_1100000000001000001100010000100001100000000100000000100000100010, 36));
            //Util.PrintBitboard(p.GetMoveboardFromBlockerboard(p.GetBlockerboard(36, 0b_1011100001), 36));
            //Util.PrintBitboard(p.GetRookMoveboard(36, p.GetRookIndexFromBoard(0b_1100000000001000001100010000100001100000000100000000100000100010, 36)));

            //Util.PrintBitboard(p.BishopBlockermask(0x1000000000));
            /*
                        Util.PrintBitboard(p.GetBishopMoveboardFromBlockerboard(0b_0000000000011000000000000000100001100000000001000000100000000000, 18));
                        Console.WriteLine();
                        Util.PrintBitboard(p.GetBishopMoveboard(18, p.GetBishopIndexFromBoard(0b_0000000000011000000000000000100001100000000001000000100000000000, 18)));*/
            //Util.PrintBitboard(p.GetBishopMoveboard(18, 0b_0100010));

            //Console.WriteLine(p.GetIndexFromBoard(0b_0000000000011000000000000000100001100000000001000000100000000000, 18));

            int[] a = p.GetDisplayableBoard();
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    Console.Write(a[i*8+j]);
                }
                Console.WriteLine();
            }
            

            //0100010

            /*00000000
            00011000
            00_00000
            00001000
            01100000
            00000100
            00001000
            00000000*/
        }

        private bool MovePiece(int sourceSquare, int targetSquare, ulong knights, ulong[] knightMoves)
        {
            ulong sourceBitboard = Util.GetBitBoardFromSquare(sourceSquare);
            ulong targetBitboard = Util.GetBitBoardFromSquare(targetSquare);

            if ((knights & sourceBitboard) == 0) return false;
            if ((knightMoves[sourceSquare] & targetBitboard) == 0) return false;

            knights = knights & ~sourceBitboard;
            knights = knights | targetBitboard;

            Console.WriteLine();
            Util.PrintBitboard(knights);

            return true;
        }


    }
}
