﻿using System.Collections.Generic;
using UnityEngine;
using ChessEngine;
using System;

using Random = UnityEngine.Random;

public class AI
{

    private PositionManager realBoard; //actual board of the game
    
    public AI(PositionManager board)
    {
        this.realBoard = board;
    }

    public (int, int) Play()
    {
        var watch = System.Diagnostics.Stopwatch.StartNew();
        (int, int) move = PlayNMoveAhead(MenuManager.difficulty);
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

    private (int, int) PlayNMoveAhead(int n) //0 move ahead is depth of 1 as it is considering only one move
    {
        //MoveTree moveTree = CreateMoveTree(realBoard, (-1,-1), n+1, false);
        //(int, int) move = MiniMax(moveTree, n);
        (int, int) move = MiniMaxABPruning(n);
        realBoard.MovePiece(move.Item1, move.Item2);
        return move;
    }

    //Deprecated : Used to physically create tree for minimax
    //Removed as not building the actual tree make the execution much faster
    /*private MoveTree CreateMoveTree(PositionManager board, (int, int) move, int depth, bool isFalsePlayer)
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
    }*/

    // Deprecated : Even for test purpose, this method should not be used as it used method CreateMoveTree
    #region MiniMax without pruning
    private (int, int) MiniMax(MoveTree moveTree, int n)
    {
        int min = int.MaxValue;
        (int, int) move = (-1, -1);
        foreach (MoveTree child in moveTree.children)
        {
            int score = Max(child, n);
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

    private int Max(MoveTree moveTree, int depth)
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
            int score = Max(child, depth - 1);
            if (score < min) min = score;
        }

        return min;
    }
    #endregion


    #region MiniMax with Alpha Beta Pruning
    // Minimax main function, should be the one to call in all cases
    // Essentialy the same as MinABPruning but store the actual move
    private (int, int) MiniMaxABPruning(int n)
    {
        (int, int) move = (-1, -1);

        int min = int.MaxValue;
        List<(int, int)> allPM = MoveManager.GetAllPossibleMoves(realBoard, false, false);
        foreach ((int, int) iMove in allPM)
        {
            PositionManager childBoard = new PositionManager(realBoard);
            childBoard.MovePiece(iMove.Item1, iMove.Item2);
            int score = MaxABPruning(childBoard, n, int.MinValue, int.MaxValue);
            if (score == min && (Random.Range(0, 2) == 0))  //add random to make the AI less predictable
            {
                move = iMove;
            }
            if (score < min)
            {
                min = score;
                move = iMove;
            }
        }

        if (min > 300) return (-1, -1);

        return move;
    }

    // Max part of Minimax with AB pruning
    private int MaxABPruning(PositionManager board, int depth, int a, int b)
    {
        if (depth <= 0) return board.GetValueOfBoard();

        int max = int.MinValue;
        List<(int, int)> allPM = MoveManager.GetAllPossibleMoves(board, false, true);

        if (allPM.Count == 0) return board.GetValueOfBoard();

        foreach ((int, int) iMove in allPM)
        {
            PositionManager childBoard = new PositionManager(board);
            childBoard.MovePiece(iMove.Item1, iMove.Item2);
            max = Math.Max(max, MiniABPruning(childBoard, depth - 1, a, b));
            a = Math.Max(a, max);
            
            if (a >= b) break;
        }

        return max;
    }

    // Min part of Minimax with AB pruning
    private int MiniABPruning(PositionManager board, int depth, int a, int b)
    {
        if (depth <= 0) return board.GetValueOfBoard();

        int min = int.MaxValue;
        List<(int, int)> allPM = MoveManager.GetAllPossibleMoves(board, false, false);

        if (allPM.Count == 0) return board.GetValueOfBoard();

        foreach ((int, int) iMove in allPM)
        {
            PositionManager childBoard = new PositionManager(board);
            childBoard.MovePiece(iMove.Item1, iMove.Item2);
            min = Math.Min(min, MaxABPruning(childBoard, depth - 1, a, b));
            b = Math.Min(b, min);
            if (b <= a) break;
        }

        return min;
    }
    #endregion



}
