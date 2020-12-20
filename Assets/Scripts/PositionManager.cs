using System;
using System.Collections.Generic;

namespace ChessEngine
{
    public class PositionManager
    {
        public Dictionary<PieceEnum, ulong> whitePositions;
        public Dictionary<PieceEnum, ulong> blackPositions;

        #region Initialization
        public PositionManager(PositionManager pm) //create a copy
        {
            this.blackPositions = new Dictionary<PieceEnum, ulong>(pm.blackPositions);
            this.whitePositions = new Dictionary<PieceEnum, ulong>(pm.whitePositions);
        }

        public PositionManager()
        {
            positionInitialization();
        }

        private void positionInitialization()
        {
            whitePositions = new Dictionary<PieceEnum, ulong>();
            whitePositions.Add(PieceEnum.Knight, 0x42);
            whitePositions.Add(PieceEnum.King, 0x08);
            whitePositions.Add(PieceEnum.Queen, 0x10);
            whitePositions.Add(PieceEnum.Rook, 0x81);
            whitePositions.Add(PieceEnum.Bishop, 0x24);
            whitePositions.Add(PieceEnum.Pawn, 0xFF00);

            blackPositions = new Dictionary<PieceEnum, ulong>();
            blackPositions.Add(PieceEnum.Knight, 0x4200000000000000);
            blackPositions.Add(PieceEnum.King, 0x0800000000000000);
            blackPositions.Add(PieceEnum.Queen, 0x1000000000000000);
            blackPositions.Add(PieceEnum.Rook, 0x8100000000000000);
            blackPositions.Add(PieceEnum.Bishop, 0x2400000000000000);
            blackPositions.Add(PieceEnum.Pawn, 0x00FF000000000000);
        }
        #endregion

        #region Positions Union
        public ulong GetAllWhite()
        {
            ulong allWhiteBitboard = 0x0;
            foreach (ulong positions in whitePositions.Values)
            {
                allWhiteBitboard = allWhiteBitboard | positions;
            }
            return allWhiteBitboard;
        }

        public ulong GetAllBlack()
        {
            ulong allBlackBitboard = 0x0;
            foreach (ulong positions in blackPositions.Values)
            {
                allBlackBitboard = allBlackBitboard | positions;
            }
            return allBlackBitboard;
        }

        public ulong GetAll()
        {
            return GetAllWhite() | GetAllBlack();
        }
        #endregion

        public int[] GetDisplayableBoard()
        {
            int[] displayableBoard = new int[64];
            foreach (KeyValuePair<PieceEnum, ulong> bitboard in whitePositions)
            {
                for (int i = 0; i < 64; i++)
                {
                    if ((bitboard.Value & Util.GetBitBoardFromSquare(i)) != 0)
                    {
                        displayableBoard[i] = (int)bitboard.Key;
                    }
                }
            }
            foreach (KeyValuePair<PieceEnum, ulong> bitboard in blackPositions)
            {
                for (int i = 0; i < 64; i++)
                {
                    if ((bitboard.Value & Util.GetBitBoardFromSquare(i)) != 0)
                    {
                        displayableBoard[i] = -(int)bitboard.Key;
                    }
                }
            }
            return displayableBoard;
        }

        // Only for Player (white positions)
        public bool IsPlayableSquare(int square)
        {
            return (GetAllWhite() & Util.GetBitBoardFromSquare(square)) != 0;
        }

        public bool IsGameOver()
        {
            return whitePositions[PieceEnum.King] == 0 || blackPositions[PieceEnum.King] == 0;
        }

        public void MovePiece(int sourceSquare, int targetSquare)
        {
            ulong sourceBitboard = Util.GetBitBoardFromSquare(sourceSquare);
            ulong targetBitboard = Util.GetBitBoardFromSquare(targetSquare);

            foreach (PieceEnum type in (PieceEnum[])Enum.GetValues(typeof(PieceEnum)))
            {
                if ((whitePositions[type] & sourceBitboard) != 0)
                {
                    whitePositions[type] = whitePositions[type] & ~sourceBitboard;
                    foreach (PieceEnum sourceType in (PieceEnum[])Enum.GetValues(typeof(PieceEnum)))
                    {
                        blackPositions[sourceType] = blackPositions[sourceType] & ~targetBitboard;
                    }
                    whitePositions[type] = whitePositions[type] | targetBitboard;
                    return;
                }

                if ((blackPositions[type] & sourceBitboard) != 0)
                {
                    blackPositions[type] = blackPositions[type] & ~sourceBitboard;
                    foreach (PieceEnum sourceType in (PieceEnum[])Enum.GetValues(typeof(PieceEnum)))
                    {
                        whitePositions[sourceType] = whitePositions[sourceType] & ~targetBitboard;
                    }
                    blackPositions[type] = blackPositions[type] | targetBitboard;
                    return;
                }
            }
        }

        /*public int GetValueOfSquare(int square)
        {
            ulong bitboard = Util.GetBitBoardFromSquare(square);
            foreach (PieceEnum type in (PieceEnum[])Enum.GetValues(typeof(PieceEnum)))
            {
                if ((whitePositions[type] & bitboard) != 0)
                {
                    return type.GetValue();
                }
                if ((blackPositions[type] & bitboard) != 0)
                {
                    return -type.GetValue();
                }
            }
            return 0;
        }*/

        public int GetValueOfBoard()
        {
            int boardValue = 0;
            foreach (PieceEnum type in (PieceEnum[])Enum.GetValues(typeof(PieceEnum)))
            {
                boardValue += Util.CountBits(whitePositions[type]) * type.GetValue();
                boardValue -= Util.CountBits(blackPositions[type]) * type.GetValue();
            }

            return boardValue;
        }

        // Only for Player (white positions) as it would slow AI treatment
        public bool IsCheck()
        {
            int kingSquare = Util.GetSquareFromBitboard(whitePositions[PieceEnum.King]);

            ulong blackPositions = GetAllBlack();
            for (int i = 63; i >= 0; i--)
            {
                if ((blackPositions & 1UL) == 1)
                {
                    if (MoveManager.GetPossibleMoveList(this, i, false).Contains(kingSquare)) return true;
                }
                blackPositions = blackPositions >> 1;

            }

            return false;
        }
    }
}
