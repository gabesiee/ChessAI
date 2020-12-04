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
        return PlayNMoveAhead(2);
    }

    private (int, int) PlayRandom()
    {
        List<(int, int)> allPM = GetAllPossibleMoves(realBoard, realBoard.GetAllBlack());
        (int, int) move = allPM[Random.Range(0,allPM.Count)];
        realBoard.MovePiece(move.Item1, move.Item2);
        return move;
    }

    private (int, int) PlayNMoveAhead(int n)
    {
        MoveTree moveTree = CreateMoveTree(realBoard, (-1,-1), n+1, true);
        (int, int) move = MiniMax(moveTree, n);
        realBoard.MovePiece(move.Item1, move.Item2);
        return move;
    }


    private MoveTree CreateMoveTree(PositionManager board, (int, int) move, int depth, bool isBlack)
    {

        MoveTree moveTree = new MoveTree(board, move);

        if (depth == 0) return moveTree;

        ulong alliedPositions = (isBlack) ? board.GetAllBlack() : board.GetAllWhite();


        List <(int, int)> allPM = GetAllPossibleMoves(board, alliedPositions);

        foreach ((int, int) iMove in allPM)
        {
            PositionManager iBoard = new PositionManager(board);
            iBoard.MovePiece(iMove.Item1, iMove.Item2);
            moveTree.AddChild(CreateMoveTree(iBoard, iMove, depth - 1, !isBlack));
        }

        return moveTree;
    }

    private (int, int) MiniMax(MoveTree moveTree, int n)
    {
        int min = int.MaxValue;
        (int, int) move = (-1, -1);
        foreach (MoveTree child in moveTree.children)
        {
            int score = Maxi(child, n);
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

    private List<(int, int)> GetAllPossibleMoves(PositionManager board, ulong alliedPositions)
    {
        List<(int, int)> allPossibleMoves = new List<(int, int)>();

        for (int i = 63; i >= 0; i--)
        {
            if ((alliedPositions & 1UL) == 1)
            {
                List<int> possibleMoves = MoveManager.GetPossibleMoveList(board, i);
                foreach (int target in possibleMoves)
                {
                    allPossibleMoves.Add((i, target));
                }
            }
            alliedPositions = alliedPositions >> 1;
        }

        return allPossibleMoves;
    }

}
