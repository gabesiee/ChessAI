using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ChessEngine;

//Deprecated : model of choice tree for old minimax implementation. should not be used as creating tree slow down treatment
public class MoveTree
{

    public PositionManager board;
    public List<MoveTree> children;
    public (int, int) move;

    public MoveTree(PositionManager boardAfterMove, (int, int) move)
    {
        this.board = boardAfterMove;
        this.move = move;
        this.children = new List<MoveTree>();
    }

    public void AddChild(MoveTree node)
    {
        this.children.Add(node);
    }


}
