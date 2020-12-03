using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ChessEngine;
using System;

using Random = UnityEngine.Random;

public class AI
{

    private PositionManager realBoard;
    
    public AI(PositionManager board)
    {
        this.realBoard = board;
    }

    public (int, int) Play()
    {
        return PlayNMoveAhead(0);
    }

    private (int, int) PlayRandom()
    {
        List<(int, int)> allPM = GetAllPossibleMoves(realBoard);
        (int, int) move = allPM[Random.Range(0,allPM.Count)];
        realBoard.MovePiece(move.Item1, move.Item2);
        return move;
    }

    /*private (int, int) PlayOneMoveAhead()
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
    }*/

    private (int, int) PlayNMoveAhead(int n)
    {
        MoveTree moveTree = CreateMoveTree(realBoard, (-1,-1), n+1);
        (int, int) move = MiniMax(moveTree, n);
        realBoard.MovePiece(move.Item1, move.Item2);
        return move;
    }


    private MoveTree CreateMoveTree(PositionManager board, (int, int) move, int depth)
    {

        MoveTree moveTree = new MoveTree(board, move);

        if (depth == 0) return moveTree;

        List <(int, int)> allPM = GetAllPossibleMoves(board);

        foreach ((int, int) iMove in allPM)
        {
            PositionManager iBoard = new PositionManager(board);
            iBoard.MovePiece(iMove.Item1, iMove.Item2);
            moveTree.AddChild(CreateMoveTree(iBoard, iMove, depth - 1));
        }

        return moveTree;
    }

    private (int, int) MiniMax(MoveTree moveTree, int n)
    {
        int min = int.MaxValue;
        (int, int) move = (-1, -1);
        foreach (MoveTree child in moveTree.children)
        {
            int score = Maxi(child, n - 1);
            if (score == min && (Random.Range(0, 2) == 0))
            {
                move = child.move;
            }
            if (score < min)
            {
                min = score;
                move = child.move;
            }
        }

        return move;
    }

    private int Maxi(MoveTree moveTree, int depth)
    {
        if (depth <= 0) return moveTree.board.GetValueOfBoard();

        int max = int.MinValue;
        foreach (MoveTree child in moveTree.children)
        {
            int score = Mini(child, depth - 1);
            if (score > max) max = score;
        }

        return max;
    }

    private int Mini(MoveTree moveTree, int depth)
    {
        if (depth <= 0) return moveTree.board.GetValueOfBoard();

        int min = int.MaxValue;
        foreach (MoveTree child in moveTree.children)
        {
            int score = Maxi(child, depth - 1);
            if (score < min) min = score;
        }

        return min;
    }

    private List<(int, int)> GetAllPossibleMoves(PositionManager board)
    {
        List<(int, int)> allPossibleMoves = new List<(int, int)>();

        ulong blackPositons = board.GetAllBlack();
        for (int i = 63; i >= 0; i--)
        {
            if ((blackPositons & 1UL) == 1)
            {
                List<int> possibleMoves = MoveManager.GetPossibleMoveList(board, i);
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
