using System;
using System.Collections.Generic;
using UnityEngine;
using ChessEngine;

public class GenerateBoard : MonoBehaviour
{
    [SerializeField] private GameObject chessboard;
    [SerializeField] private GameObject lightCube;
    [SerializeField] private GameObject darkCube;

    [SerializeField] private GameObject pawn;
    [SerializeField] private GameObject rook;
    [SerializeField] private GameObject bishop;
    [SerializeField] private GameObject knight;
    [SerializeField] private GameObject queen;
    [SerializeField] private GameObject king;

    public GameObject[] cubesArray = new GameObject[64];

    private void Start()
    {
        generateEmptyBoard();
        initializeTheBoard();
    }

    private void generateEmptyBoard()
    {
        String squareColor = "light";
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                Vector3 currentPos = new Vector3(i, 0, j);

                if (j % 8 == 0)
                {
                    if (squareColor == "light")
                    {
                        squareColor = "dark";
                    }
                    else
                    {
                        squareColor = "light";
                    }
                }

                if (squareColor == "dark")
                {
                    GameObject cube = Instantiate(darkCube, currentPos, Quaternion.identity) as GameObject;
                    cube.transform.parent = chessboard.transform;

                    cubesArray[((7 - j) * 8 + i)] = cube;

                    squareColor = "light";
                }
                else
                {
                    GameObject cube = Instantiate(lightCube, currentPos, Quaternion.identity) as GameObject;
                    cube.transform.parent = chessboard.transform;

                    cubesArray[((7 - j) * 8 + i)] = cube;

                    squareColor = "dark";
                }

            }
        }
    }

    private void initializeTheBoard()
    {
        PiecesManager pos = new PiecesManager();

        int[] intBoard = pos.GetDisplayableBoard();
        //Array.Reverse(intBoard);

        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                Vector3 currentPos = new Vector3(j, 1, 7 - i);

                switch (intBoard[i * 8 + j])
                {
                    case 1:
                        GameObject whitePawn = Instantiate(pawn, currentPos, Quaternion.identity) as GameObject;
                        whitePawn.name = "whitePawn";
                        whitePawn.GetComponent<Renderer>().material.color = Color.white;

                        break;
                    case 2:
                        GameObject whiteKnight = Instantiate(knight, currentPos, Quaternion.identity) as GameObject;
                        whiteKnight.name = "whiteKnight";
                        whiteKnight.GetComponent<Renderer>().material.color = Color.white;
                        break;
                    case 3:
                        GameObject whiteBishop = Instantiate(bishop, currentPos, Quaternion.identity) as GameObject;
                        whiteBishop.name = "whiteBishop";
                        whiteBishop.GetComponent<Renderer>().material.color = Color.white;
                        break;
                    case 4:
                        GameObject whiteRook = Instantiate(rook, currentPos, Quaternion.identity) as GameObject;
                        whiteRook.name = "whiteRook";
                        whiteRook.GetComponent<Renderer>().material.color = Color.white;
                        break;
                    case 5:
                        GameObject whiteQueen = Instantiate(queen, currentPos, Quaternion.identity) as GameObject;
                        whiteQueen.name = "whiteQueen";
                        whiteQueen.GetComponent<Renderer>().material.color = Color.white;
                        break;
                    case 6:
                        GameObject whiteKing = Instantiate(king, currentPos, Quaternion.identity) as GameObject;
                        whiteKing.name = "whiteKing";
                        whiteKing.GetComponent<Renderer>().material.color = Color.white;
                        break;
                    case -1:
                        GameObject blackPawn = Instantiate(pawn, currentPos, Quaternion.identity) as GameObject;
                        blackPawn.name = "blackPawn";
                        blackPawn.GetComponent<Renderer>().material.color = Color.black;
                        break;
                    case -2:
                        GameObject blackKnight = Instantiate(knight, currentPos, Quaternion.identity) as GameObject;
                        blackKnight.name = "blackKnight";
                        blackKnight.GetComponent<Renderer>().material.color = Color.black;
                        break;
                    case -3:
                        GameObject blackBishop = Instantiate(bishop, currentPos, Quaternion.identity) as GameObject;
                        blackBishop.name = "blackBishop";
                        blackBishop.GetComponent<Renderer>().material.color = Color.black;
                        break;
                    case -4:
                        GameObject blackRook = Instantiate(rook, currentPos, Quaternion.identity) as GameObject;
                        blackRook.name = "blackRook";
                        blackRook.GetComponent<Renderer>().material.color = Color.black;
                        break;
                    case -5:
                        GameObject blackQueen = Instantiate(queen, currentPos, Quaternion.identity) as GameObject;
                        blackQueen.name = "blackQueen";
                        blackQueen.GetComponent<Renderer>().material.color = Color.black;
                        break;
                    case -6:
                        GameObject blackKing = Instantiate(king, currentPos, Quaternion.identity) as GameObject;
                        blackKing.name = "blackKing";
                        blackKing.GetComponent<Renderer>().material.color = Color.black;
                        break;
                }
            }
        }
    }
}
