using System;
using System.Collections.Generic;
using System.Text;

namespace ChessEngine
{
    class PiecesManager
    {
        Dictionary<PieceEnum, ulong> whitePositions;
        Dictionary<PieceEnum, ulong> blackPositions;

        Dictionary<PieceEnum, ulong[]> pieceMovements;

        ulong[] rookBlockermasks;
        ulong[][] rookMoveboards;

        ulong[] bishopBlockermasks;
        ulong[][] bishopMoveboards;

        delegate ulong AttackPattern(ulong position);

        #region Initialization
        public PiecesManager()
        {
            positionInitialization();
            moveInitialization();
            
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
        private void moveInitialization()
        {
            pieceMovements = new Dictionary<PieceEnum, ulong[]>();

            pieceMovements.Add(PieceEnum.Knight, GetBitboardMove(KnightAttackPattern));
            pieceMovements.Add(PieceEnum.King, GetBitboardMove(KingAttackPattern));

            GenerateAllRookMoveboard();
            GenerateAllBishopMoveboard();
        }
        #endregion

        #region Moves Management
        private ulong[] GetBitboardMove(AttackPattern attackPatern)
        {
            ulong[] moves = new ulong[64];
            ulong position = 0x8000000000000000;
            for (int i = 0; i < 64; i++)
            {
                moves[i] = attackPatern(position);
                position = position >> 1;
            }
            return moves;
        }

        #region Attack Pattern
        private ulong KnightAttackPattern(ulong knight)
        {
            ulong l1 = (knight >> 1) & 0x7f7f7f7f7f7f7f7f; //case à droite du chevalier + masque qui enlève la colonne la plus à gauche (si cavalier à droite, on n'accepte pas le déplacement)
            ulong l2 = (knight >> 2) & 0x3f3f3f3f3f3f3f3f; //2 case à droite
            ulong r1 = (knight << 1) & 0xfefefefefefefefe; //case à gauche
            ulong r2 = (knight << 2) & 0xfcfcfcfcfcfcfcfc; //2 case à gauche
            ulong h1 = l1 | r1; // couple 1 case écart
            ulong h2 = l2 | r2; // couple 2 case écart
            return (h1 << 16) | (h1 >> 16) | (h2 << 8) | (h2 >> 8); // application des décalage verticaux
        }

        private ulong KingAttackPattern(ulong king)
        {
            ulong l1 = (king >> 1) & 0x7f7f7f7f7f7f7f7f;
            ulong r1 = (king << 1) & 0xfefefefefefefefe;
            ulong h1 = l1 | r1;
            return (h1) | (h1 << 8) | (h1 >> 8) | (king << 8) | (king >> 8);
        }




        #endregion

        #region Rook
        public ulong RookBlockermask(ulong rook)
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

        private void GenerateAllRookMoveboard()
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

        public ulong GetRookMoveboardFromBlockerboard(ulong blockerboard, int square)
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

        private ulong[] GenerateAllRookBlockermask()
        {
            rookBlockermasks = new ulong[64];
            for (int i = 0; i < 64; i++)
            {
                rookBlockermasks[i] = RookBlockermask(Util.GetBitBoardFromSquare(i));
            }
            return rookBlockermasks;
        }

        public int GetRookIndexFromBoard(ulong bitboard, int square)
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
        public ulong BishopBlockermask(ulong bishop)
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

        public ulong[] GenerateAllBishopBlockermask()
        {
            bishopBlockermasks = new ulong[64];
            for (int i = 0; i < 64; i++)
            {
                bishopBlockermasks[i] = BishopBlockermask(Util.GetBitBoardFromSquare(i));
            }
            return bishopBlockermasks;
        }

        private void GenerateAllBishopMoveboard()
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

        public ulong GetBishopMoveboardFromBlockerboard(ulong blockerboard, int square)
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

        public int GetBishopIndexFromBoard(ulong bitboard, int square)
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



        private ulong GenerateBlockerboard(int index, ulong blockermask)
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


        public ulong GetRookMoveboard(int square, int index)
        {
            return rookMoveboards[square][index];
        }

        public ulong GetBishopMoveboard(int square, int index)
        {
            return bishopMoveboards[square][index];
        }

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

        // Fonction reservée au joueur (forcément blanc)
        public bool IsPlayableSquare(int square)
        {
            return (GetAllWhite() & Util.GetBitBoardFromSquare(square)) != 0;
        }
    }
}
