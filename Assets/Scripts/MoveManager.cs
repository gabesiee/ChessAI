using System;
using System.Collections.Generic;
using System.Text;

namespace ChessEngine
{
    public static class MoveManager
    {
        static Dictionary<PieceEnum, ulong[]> pieceMovements;

        static ulong[] rookBlockermasks;
        static ulong[][] rookMoveboards;

        static ulong[] bishopBlockermasks;
        static ulong[][] bishopMoveboards;

        static ulong[] whitePawnBlockermasks;
        static ulong[][] whitePawnMoveboards;

        static ulong[] blackPawnBlockermasks;
        static ulong[][] blackPawnMoveboards;

        #region Initialization
        static MoveManager()
        {
            moveInitialization();
        }
        static private void moveInitialization()
        {
            pieceMovements = new Dictionary<PieceEnum, ulong[]>();

            pieceMovements.Add(PieceEnum.Knight, GetKnightMoveboard());
            pieceMovements.Add(PieceEnum.King, GetKingMoveboard());

            GenerateAllRookMoveboard();
            GenerateAllBishopMoveboard();
            GenerateAllWhitePawnMoveboard();
            GenerateAllBlackPawnMoveboard();
        }
        #endregion

        #region Moves Management

        static private ulong[] GetKnightMoveboard()
        {
            ulong[] moves = new ulong[64];
            ulong position = 0x8000000000000000;
            for (int i = 0; i < 64; i++)
            {
                moves[i] = KnightAttackPattern(position);
                position = position >> 1;
            }
            return moves;
        }

        static private ulong[] GetKingMoveboard()
        {
            ulong[] moves = new ulong[64];
            ulong position = 0x8000000000000000;
            for (int i = 0; i < 64; i++)
            {
                moves[i] = KingAttackPattern(position);
                position = position >> 1;
            }
            return moves;
        }

        #region Attack Pattern
        static private ulong KnightAttackPattern(ulong knight)
        {
            ulong l1 = (knight >> 1) & 0x7f7f7f7f7f7f7f7f; //case à droite du chevalier + masque qui enlève la colonne la plus à gauche (si cavalier à droite, on n'accepte pas le déplacement)
            ulong l2 = (knight >> 2) & 0x3f3f3f3f3f3f3f3f; //2 case à droite
            ulong r1 = (knight << 1) & 0xfefefefefefefefe; //case à gauche
            ulong r2 = (knight << 2) & 0xfcfcfcfcfcfcfcfc; //2 case à gauche
            ulong h1 = l1 | r1; // couple 1 case écart
            ulong h2 = l2 | r2; // couple 2 case écart
            return (h1 << 16) | (h1 >> 16) | (h2 << 8) | (h2 >> 8); // application des décalage verticaux
        }

        static private ulong KingAttackPattern(ulong king)
        {
            ulong l1 = (king >> 1) & 0x7f7f7f7f7f7f7f7f;
            ulong r1 = (king << 1) & 0xfefefefefefefefe;
            ulong h1 = l1 | r1;
            return (h1) | (h1 << 8) | (h1 >> 8) | (king << 8) | (king >> 8);
        }




        #endregion

        #region Rook
        static public ulong RookBlockermask(ulong rook)
        {
            ulong l1 = (rook >> 1) & 0x7E7E7E7E7E7E7E7E;
            ulong l2 = (rook >> 2) & 0x3E3E3E3E3E3E3E3E;
            ulong l3 = (rook >> 3) & 0x1E1E1E1E1E1E1E1E;
            ulong l4 = (rook >> 4) & 0xE0E0E0E0E0E0E0E;
            ulong l5 = (rook >> 5) & 0x606060606060606;
            ulong l6 = (rook >> 6) & 0x202020202020202;

            ulong l = l1 | l2 | l3 | l4 | l5 | l6;

            ulong r1 = (rook << 1) & 0x7E7E7E7E7E7E7E7E;
            ulong r2 = (rook << 2) & 0x7C7C7C7C7C7C7C7C;
            ulong r3 = (rook << 3) & 0x7878787878787878;
            ulong r4 = (rook << 4) & 0x7070707070707070;
            ulong r5 = (rook << 5) & 0x6060606060606060;
            ulong r6 = (rook << 6) & 0x4040404040404040;

            ulong r = r1 | r2 | r3 | r4 | r5 | r6;

            ulong u = ((rook << 8) | (rook << 16) | (rook << 24) | (rook << 32) | (rook << 40) | (rook << 48)) & 0xFFFFFFFFFFFFFF;

            ulong b = ((rook >> 8) | (rook >> 16) | (rook >> 24) | (rook >> 32) | (rook >> 40) | (rook >> 48)) & 0xFFFFFFFFFFFFFF00;

            return l | r | u | b;
        }

        static private void GenerateAllRookMoveboard()
        {
            GenerateAllRookBlockermask();

            rookMoveboards = new ulong[64][];
            for (int i = 0; i < 64; i++)
            {
                int bits = Util.CountBits(rookBlockermasks[i]);
                rookMoveboards[i] = new ulong[1 << bits];
                for (int index = 0; index < (1 << bits); index++)
                {
                    rookMoveboards[i][index] = GetRookMoveboardFromBlockerboard(GenerateBlockerboard(index, rookBlockermasks[i]), i);
                }
            }
        }

        static public ulong GetRookMoveboardFromBlockerboard(ulong blockerboard, int square)
        {
            ulong moveboard = 0;

            //gauche
            ulong bit = Util.GetBitBoardFromSquare(square);
            do
            {
                bit <<= 1;
                if ((bit & 0xfefefefefefefefe) == 0) break; //trop à gauche
                moveboard |= bit;
            } while ((blockerboard & bit) == 0);

            //droite
            bit = Util.GetBitBoardFromSquare(square);
            do
            {
                bit >>= 1;
                if ((bit & 0x7f7f7f7f7f7f7f7f) == 0) break; //trop à droite
                moveboard |= bit;
            } while ((blockerboard & bit) == 0);

            //haut
            bit = Util.GetBitBoardFromSquare(square);
            do
            {
                bit <<= 8;
                if (bit == 0) break; //trop en haut
                moveboard |= bit;
            } while ((blockerboard & bit) == 0);

            //bas
            bit = Util.GetBitBoardFromSquare(square);
            do
            {
                bit >>= 8;
                if (bit == 0) break; //trop en bas
                moveboard |= bit;
            } while ((blockerboard & bit) == 0);

            return moveboard;


        }

        static private ulong[] GenerateAllRookBlockermask()
        {
            rookBlockermasks = new ulong[64];
            for (int i = 0; i < 64; i++)
            {
                rookBlockermasks[i] = RookBlockermask(Util.GetBitBoardFromSquare(i));
            }
            return rookBlockermasks;
        }

        static public int GetRookIndexFromBoard(ulong bitboard, int square)
        {
            ulong blockermask = rookBlockermasks[square];
            ulong index = 0;
            int count = 0;
            while (blockermask != 0)
            {
                if ((blockermask & 0x1) == 0x1) index = ((bitboard & 0x1) << count++) | index;
                blockermask >>= 1;
                bitboard >>= 1;
            }
            return (int)index;
        }

        #endregion

        #region Bishop
        static public ulong BishopBlockermask(ulong bishop)
        {
            ulong l1 = (bishop >> 1) & 0x7E7E7E7E7E7E7E7E;
            ulong l2 = (bishop >> 2) & 0x3E3E3E3E3E3E3E3E;
            ulong l3 = (bishop >> 3) & 0x1E1E1E1E1E1E1E1E;
            ulong l4 = (bishop >> 4) & 0xE0E0E0E0E0E0E0E;
            ulong l5 = (bishop >> 5) & 0x606060606060606;
            ulong l6 = (bishop >> 6) & 0x202020202020202;

            ulong r1 = (bishop << 1) & 0x7E7E7E7E7E7E7E7E;
            ulong r2 = (bishop << 2) & 0x7C7C7C7C7C7C7C7C;
            ulong r3 = (bishop << 3) & 0x7878787878787878;
            ulong r4 = (bishop << 4) & 0x7070707070707070;
            ulong r5 = (bishop << 5) & 0x6060606060606060;
            ulong r6 = (bishop << 6) & 0x4040404040404040;

            ulong h1 = l1 | r1;
            ulong h2 = l2 | r2;
            ulong h3 = l3 | r3;
            ulong h4 = l4 | r4;
            ulong h5 = l5 | r5;
            ulong h6 = l6 | r6;

            ulong u = ((h1 << 8) | (h2 << 16) | (h3 << 24) | (h4 << 32) | (h5 << 40) | (h6 << 48)) & 0xFFFFFFFFFFFFFF;
            ulong b = ((h1 >> 8) | (h2 >> 16) | (h3 >> 24) | (h4 >> 32) | (h5 >> 40) | (h6 >> 48)) & 0xFFFFFFFFFFFFFF00;

            return u | b;

        }

        static public ulong[] GenerateAllBishopBlockermask()
        {
            bishopBlockermasks = new ulong[64];
            for (int i = 0; i < 64; i++)
            {
                bishopBlockermasks[i] = BishopBlockermask(Util.GetBitBoardFromSquare(i));
            }
            return bishopBlockermasks;
        }

        static private void GenerateAllBishopMoveboard()
        {
            GenerateAllBishopBlockermask();

            bishopMoveboards = new ulong[64][];
            for (int i = 0; i < 64; i++)
            {
                int bits = Util.CountBits(bishopBlockermasks[i]);
                bishopMoveboards[i] = new ulong[1 << bits];
                for (int index = 0; index < (1 << bits); index++)
                {
                    bishopMoveboards[i][index] = GetBishopMoveboardFromBlockerboard(GenerateBlockerboard(index, bishopBlockermasks[i]), i);
                }
            }
        }

        static public ulong GetBishopMoveboardFromBlockerboard(ulong blockerboard, int square)
        {
            ulong moveboard = 0;

            //gauche-haut
            ulong bit = Util.GetBitBoardFromSquare(square);
            do
            {
                bit <<= 9;
                if ((bit & 0xfefefefefefefefe) == 0) break; //trop à gauche
                moveboard |= bit;
            } while ((blockerboard & bit) == 0);

            //droite-haut
            bit = Util.GetBitBoardFromSquare(square);
            do
            {
                bit <<= 7;
                if ((bit & 0x7f7f7f7f7f7f7f7f) == 0) break; //trop à droite
                moveboard |= bit;
            } while ((blockerboard & bit) == 0);

            //gauche-bas
            bit = Util.GetBitBoardFromSquare(square);
            do
            {
                bit >>= 7;
                if ((bit & 0xfefefefefefefefe) == 0) break; //trop à gauche
                moveboard |= bit;
            } while ((blockerboard & bit) == 0);

            //droite-bas
            bit = Util.GetBitBoardFromSquare(square);
            do
            {
                bit >>= 9;
                if ((bit & 0x7f7f7f7f7f7f7f7f) == 0) break; //trop à droite
                moveboard |= bit;
            } while ((blockerboard & bit) == 0);

            return moveboard;


        }

        static public int GetBishopIndexFromBoard(ulong bitboard, int square)
        {
            ulong blockermask = bishopBlockermasks[square];
            ulong index = 0;
            int count = 0;
            while (blockermask != 0)
            {
                if ((blockermask & 0x1) == 0x1) index = ((bitboard & 0x1) << count++) | index;
                blockermask >>= 1;
                bitboard >>= 1;
            }
            return (int)index;
        }

        #endregion

        #region Pawn
        static public ulong WhitePawnBlockermask(ulong pawn)
        {
            ulong l1 = (pawn >> 1) & 0x7f7f7f7f7f7f7f7f;
            ulong r1 = (pawn << 1) & 0xfefefefefefefefe;
            ulong h1 = l1 | r1 | pawn;
            ulong u2 = 0;

            if ((pawn & 0xFF00) != 0)
            {
                u2 = pawn << 16;
            }

            return (h1 << 8) | u2;
        }

        static public ulong[] GenerateAllWhitePawnBlockermask()
        {
            whitePawnBlockermasks = new ulong[64];
            for (int i = 0; i < 64; i++)
            {
                whitePawnBlockermasks[i] = WhitePawnBlockermask(Util.GetBitBoardFromSquare(i));
            }
            return whitePawnBlockermasks;
        }

        static private void GenerateAllWhitePawnMoveboard()
        {
            GenerateAllWhitePawnBlockermask();

            whitePawnMoveboards = new ulong[64][];
            for (int i = 0; i < 64; i++)
            {
                int bits = Util.CountBits(whitePawnBlockermasks[i]);
                whitePawnMoveboards[i] = new ulong[1 << bits];
                for (int index = 0; index < (1 << bits); index++)
                {
                    whitePawnMoveboards[i][index] = GetWhitePawnMoveboardFromBlockerboard(GenerateBlockerboard(index, whitePawnBlockermasks[i]), i);
                }
            }
        }

        static private ulong GetWhitePawnMoveboardFromBlockerboard(ulong blockerboard, int square)
        {
            ulong u1 = Util.GetBitBoardFromSquare(square) << 8;
            ulong u2 = u1 << 8;
            ulong moveBoard = blockerboard & ~u1 & ~u2;

            if ((blockerboard & u1) == 0)
            {
                moveBoard |= u1;

                if (((Util.GetBitBoardFromSquare(square) & 0xFF00) != 0) && ((blockerboard & u2) == 0))
                {
                    moveBoard |= u2;
                }
            }

            return moveBoard;
        }

        static public int GetWhitePawnIndexFromBoard(ulong bitboard, int square)
        {
            ulong blockermask = whitePawnBlockermasks[square];
            ulong index = 0;
            int count = 0;
            while (blockermask != 0)
            {
                if ((blockermask & 0x1) == 0x1) index = ((bitboard & 0x1) << count++) | index;
                blockermask >>= 1;
                bitboard >>= 1;
            }
            return (int)index;
        }

        static public ulong BlackPawnBlockermask(ulong pawn)
        {
            ulong l1 = (pawn >> 1) & 0x7f7f7f7f7f7f7f7f;
            ulong r1 = (pawn << 1) & 0xfefefefefefefefe;
            ulong h1 = l1 | r1 | pawn;
            ulong d2 = 0;

            if ((pawn & 0xFF000000000000) != 0)
            {
                d2 = pawn >> 16;
            }

            return (h1 >> 8) | d2;
        }

        static public ulong[] GenerateAllBlackPawnBlockermask()
        {
            blackPawnBlockermasks = new ulong[64];
            for (int i = 0; i < 64; i++)
            {
                blackPawnBlockermasks[i] = BlackPawnBlockermask(Util.GetBitBoardFromSquare(i));
            }
            return blackPawnBlockermasks;
        }

        static private void GenerateAllBlackPawnMoveboard()
        {
            GenerateAllBlackPawnBlockermask();

            blackPawnMoveboards = new ulong[64][];
            for (int i = 0; i < 64; i++)
            {
                int bits = Util.CountBits(blackPawnBlockermasks[i]);
                blackPawnMoveboards[i] = new ulong[1 << bits];
                for (int index = 0; index < (1 << bits); index++)
                {
                    blackPawnMoveboards[i][index] = GetBlackPawnMoveboardFromBlockerboard(GenerateBlockerboard(index, blackPawnBlockermasks[i]), i);
                }
            }
        }

        static private ulong GetBlackPawnMoveboardFromBlockerboard(ulong blockerboard, int square)
        {
            ulong d1 = Util.GetBitBoardFromSquare(square) >> 8;
            ulong d2 = d1 >> 8;
            ulong moveBoard = blockerboard & ~d1 & ~d2;

            if ((blockerboard & d1) == 0)
            {
                moveBoard |= d1;

                if (((Util.GetBitBoardFromSquare(square) & 0xFF000000000000) != 0) && ((blockerboard & d2) == 0))
                {
                    moveBoard |= d2;
                }
            }

            return moveBoard;
        }

        static public int GetBlackPawnIndexFromBoard(ulong bitboard, int square)
        {
            ulong blockermask = blackPawnBlockermasks[square];
            ulong index = 0;
            int count = 0;
            while (blockermask != 0)
            {
                if ((blockermask & 0x1) == 0x1) index = ((bitboard & 0x1) << count++) | index;
                blockermask >>= 1;
                bitboard >>= 1;
            }
            return (int)index;
        }

        #endregion

        static private ulong GenerateBlockerboard(int index, ulong blockermask)
        {
            ulong blockerboard = blockermask;

            int bitindex = 0;
            for (int i = 0; i < 64; i++)
            {
                if ((blockermask & (1UL << i)) != 0)
                {
                    if ((index & (1 << bitindex)) == 0)
                    {
                        blockerboard = blockerboard & ~(1UL << i);
                    }
                    bitindex++;
                }
            }
            return blockerboard;
        }

        #endregion

        static public ulong GetPossibleMoveBitboard(PositionManager board, int square)
        {
            ulong positionBitboard = Util.GetBitBoardFromSquare(square);
            foreach (PieceEnum type in (PieceEnum[])Enum.GetValues(typeof(PieceEnum)))
            {
                if ((board.whitePositions[type] & positionBitboard) != 0 || (board.blackPositions[type] & positionBitboard) != 0)
                {
                    ulong alliedPositions;
                    if ((board.whitePositions[type] & positionBitboard) != 0) alliedPositions = board.GetAllWhite();
                    else alliedPositions = board.GetAllBlack();

                    switch (type)
                    {
                        case PieceEnum.Pawn:
                            if ((board.whitePositions[type] & positionBitboard) != 0)
                            {
                                return whitePawnMoveboards[square][GetWhitePawnIndexFromBoard(board.GetAll(), square)] & ~alliedPositions;
                            }
                            else
                            {
                                return blackPawnMoveboards[square][GetBlackPawnIndexFromBoard(board.GetAll(), square)] & ~alliedPositions;
                            }

                        case PieceEnum.Knight:
                            return pieceMovements[PieceEnum.Knight][square] & ~alliedPositions;
                        case PieceEnum.Bishop:
                            return bishopMoveboards[square][GetBishopIndexFromBoard(board.GetAll(), square)] & ~alliedPositions;
                        case PieceEnum.Rook:
                            return rookMoveboards[square][GetRookIndexFromBoard(board.GetAll(), square)] & ~alliedPositions;
                        case PieceEnum.Queen:
                            return (bishopMoveboards[square][GetBishopIndexFromBoard(board.GetAll(), square)] | rookMoveboards[square][GetRookIndexFromBoard(board.GetAll(), square)]) & ~alliedPositions;
                        case PieceEnum.King:
                            return pieceMovements[PieceEnum.King][square] & ~alliedPositions;
                        default:
                            break;
                    }
                }
            }

            return 0;
        }

        static public List<int> GetPossibleMoveList(PositionManager board, int square)
        {
            List<int> moveList = new List<int>();
            ulong bitboard = GetPossibleMoveBitboard(board, square);
            for (int i = 0; i < 64; i++)
            {
                if ((bitboard & Util.GetBitBoardFromSquare(i)) != 0)
                {
                    moveList.Add(i);
                }
            }
            return moveList;

        }


    }
}
