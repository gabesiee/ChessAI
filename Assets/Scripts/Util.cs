using System;
using System.Collections.Generic;
using System.Text;

namespace ChessEngine
{
    class Util
    {
        static public String Format64BitBinary(ulong binary)
        {
            return Convert.ToString((long)binary, 2).PadLeft(64, '0');
        }

        static public void PrintBitboard(ulong bitboard)
        {
            String bitboardString = Format64BitBinary(bitboard);
            for (int i = 0; i < 8; i++)
            {
                Console.WriteLine(bitboardString.Substring(i * 8, 8));
            }
        }

        static public ulong GetBitBoardFromSquare(int square)
        {
            return 0x8000000000000000 >> square;
        }


        static public int CountBits(ulong n)
        {
            int count = 0;
            while (n != 0)
            {
                if ((n & 0x1) == 0x1) count++;
                n >>= 1;
            }
            return count;
        }
    }
}
