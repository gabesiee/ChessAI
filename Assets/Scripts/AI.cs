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
        var watch = System.Diagnostics.Stopwatch.StartNew();
        (int, int) move = PlayNMoveAhead(2);
        watch.Stop();
        Debug.Log(watch.ElapsedMilliseconds);
        return move;
    }

    private (int, int) PlayRandom()
    {
        List<(int, int)> allPM = MoveManager.GetAllPossibleMoves(realBoard, false, false);
        (int, int) move = allPM[Random.Range(0,allPM.Count)];
        realBoard.MovePiece(move.Item1, move.Item2);
        return move;
    }

    private (int, int) PlayNMoveAhead(int n)
    {
        MoveTree moveTree = CreateMoveTree(realBoard, (-1,-1), n+1, false);
        //(int, int) move = MiniMax(moveTree, n);
        (int, int) move = MiniMaxABPruning(moveTree, n);
        realBoard.MovePiece(move.Item1, move.Item2);
        return move;
    }


    private MoveTree CreateMoveTree(PositionManager board, (int, int) move, int depth, bool isFalsePlayer)
    {

        MoveTree moveTree = new MoveTree(board, move);

        if (depth == 0) return moveTree;


        List <(int, int)> allPM = MoveManager.GetAllPossibleMoves(board, false, isFalsePlayer);

        foreach ((int, int) iMove in allPM)
        {
            PositionManager iBoard = new PositionManager(board);
            iBoard.MovePiece(iMove.Item1, iMove.Item2);
            moveTree.AddChild(CreateMoveTree(iBoard, iMove, depth - 1, !isFalsePlayer));
        }

        return moveTree;
    }
    #region MiniMax without pruning
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
    #endregion


    #region MiniMax with Alpha Beta Pruning
    private (int, int) MiniMaxABPruning(MoveTree moveTree, int n)
    {
        int min = int.MaxValue;
        (int, int) move = (-1, -1);
        foreach (MoveTree child in moveTree.children)
        {
            int score = MaxiABPruning(child, n, int.MinValue, int.MaxValue);
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

    private int MaxiABPruning(MoveTree moveTree, int depth, int a, int b)
    {
        if (depth <= 0 || moveTree.children.Count == 0) return moveTree.board.GetValueOfBoard();

        int max = int.MinValue;
        foreach (MoveTree child in moveTree.children)
        {
            max = Math.Max(max, MiniABPruning(child, depth - 1, a, b));
            a = Math.Max(a, max);
            
            if (a >= b) break;
        }

        return max;
    }

    private int MiniABPruning(MoveTree moveTree, int depth, int a, int b)
    {
        if (depth <= 0 || moveTree.children.Count == 0) return moveTree.board.GetValueOfBoard();

        int min = int.MaxValue;
        foreach (MoveTree child in moveTree.children)
        {
            min = Math.Min(min, Maxi(child, depth - 1));
            b = Math.Min(b, min);
            if (b <= a) break;
        }

        return min;
    }
    #endregion



}
