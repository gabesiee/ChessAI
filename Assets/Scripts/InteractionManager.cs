﻿using UnityEngine;
using ChessEngine;
using UnityEngine.SceneManagement;

//Manage interactions from player and game cycle
public class InteractionManager : MonoBehaviour
{
    Ray ray;
    RaycastHit hit;
    Vector3 position;

    GenerateBoard gb;

    bool isSelected = false;
    int selectedSquare = -1;
    (int, int) lastAIMove = (-1, -1);

    bool gameOver = false;

    private void Start()
    {
        gb = this.GetComponent<GenerateBoard>();
        Colorize();
    }

    // Update is called once per frame
    void Update()
    {
        if (!gameOver) GetSelectedSquare();
    }

    void GetSelectedSquare()
    {
        if (Input.GetMouseButtonDown(0))
        {
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                if (isSelected) //Second Select
                {
                    position = hit.transform.position;

                    int targetSquare = (int)((7 - position.z) * 8 + position.x);
                    if (MoveManager.GetPossibleMoveList(gb.pm, selectedSquare, true).Contains(targetSquare)) //Moving piece
                    {
                        gb.pm.MovePiece(selectedSquare, targetSquare);
                        
                        lastAIMove = gb.ai.Play(); // Make the AI play
                        if (lastAIMove == (-1, -1)) // if AI can't play
                        {
                            gameOver = true;
                            Debug.Log("Player Won !");
                            SceneManager.LoadScene("VictoryScene");
                        }
                        else if (gb.pm.IsCheck()) {
                            if (MoveManager.GetAllPossibleMoves(gb.pm, true, false).Count == 0)
                            {
                                gameOver = true;
                                Debug.Log("CHECKMATE");
                                Debug.Log("AI Won !");
                                SceneManager.LoadScene("DefeatScene");
                            }
                            else Debug.Log("CHECK");
                        }
                        
                        gb.ClearPieces();
                        gb.PlacePieces();
                    }
                    isSelected = false;
                    selectedSquare = -1;
                }
                else //First select
                {
                    position = hit.transform.position;
                    selectedSquare = (int)((7 - position.z) * 8 + position.x);

                    if (gb.pm.IsPlayableSquare(selectedSquare))
                    {
                        isSelected = true;
                    }
                }
                Colorize();
            }
        }
    }


    void Colorize()
    {
        ColorizeDefaultBoard();

        if (lastAIMove != (-1, -1)) // colorize the last AI move
        {
            ColorizeSquare(lastAIMove.Item1, Color.yellow);
            ColorizeSquare(lastAIMove.Item2, Color.yellow);
        }

        if (isSelected)
        {
            ColorizeSquare(selectedSquare, Color.green); //colorize the selected piece
            foreach (int square in MoveManager.GetPossibleMoveList(gb.pm, selectedSquare, true))
            {
                ColorizeSquare(square, Color.blue); //colorize the possible move
            }
        } else if (selectedSquare >= 0)
        {
            ColorizeSquare(selectedSquare, Color.red);
        }
        
    }

    void ColorizeDefaultBoard()
    {
        bool isWhite = false;
        for (int i = 0; i < 64; i ++)
        {
            if (i % 8 == 0) isWhite = !isWhite;
            if (isWhite) ColorizeSquare(i, Color.white);
            else ColorizeSquare(i, Color.black);
            isWhite = !isWhite;
        }
    }

    void ColorizeSquare(int square, Color color)
    {
        gb.cubesArray[square].GetComponent<Renderer>().material.color = color;
    }

}
