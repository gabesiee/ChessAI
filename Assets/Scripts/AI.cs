using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ChessEngine;


public class AI
{

    private PiecesManager board;
    
    public AI(PiecesManager board)
    {
        this.board = board;
    }

    public (int, int) Play()
    {
        return PlayOneMoveAhead();
    }

    private (int, int) PlayRandom()
    {
        List<(int, int)> allPM = GetAllPossibleMoves();
        (int, int) move = allPM[Random.Range(0,allPM.Count)];
        board.MovePiece(move.Item1, move.Item2);
        return move;
    }

    private (int, int) PlayOneMoveAhead()
    {
        List<(int, int)> allPM = GetAllPossibleMoves();
        (int, int) move = (-1, -1);
        int bestValue = -1;
        foreach ((int, int) iMove in allPM)
        {
            int iValue = board.GetValueOfSquare(iMove.Item2);
            if (iValue == bestValue && (Random.Range(0, 2) == 0))
            {
                bestValue = iValue;
                move = iMove;
            } else if (iValue > bestValue)
            {
                bestValue = iValue;
                move = iMove;
            }
        }

        if (move != (-1, -1)) board.MovePiece(move.Item1, move.Item2); //security

        return move;
    }

    private List<(int, int)> GetAllPossibleMoves()
    {
        List<(int, int)> allPossibleMoves = new List<(int, int)>();

        ulong blackPositons = board.GetAllBlack();
        for (int i = 63; i >= 0; i--)
        {
            if ((blackPositons & 1UL) == 1)
            {
                List<int> possibleMoves = board.GetPossibleMoveList(i);
                foreach (int target in possibleMoves)
                {
                    allPossibleMoves.Add((i, target));
                }
            }
            blackPositons = blackPositons >> 1;
        }

        return allPossibleMoves;
    }

}
