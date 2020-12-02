using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ChessEngine;

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
                    if (gb.pm.GetPossibleMoveList(selectedSquare).Contains(targetSquare)) //Moving piece
                    {
                        gb.pm.MovePiece(selectedSquare, targetSquare);
                        if (gb.pm.IsGameOver())
                        {
                            gameOver = true;
                            Debug.Log("Player Won !");
                        }
                        else
                        {
                            lastAIMove = gb.ai.Play();
                            if (gb.pm.IsGameOver())
                            {
                                gameOver = true;
                                Debug.Log("AI Won !");
                            }
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

        if (lastAIMove != (-1, -1))
        {
            ColorizeSquare(lastAIMove.Item1, Color.yellow);
            ColorizeSquare(lastAIMove.Item2, Color.yellow);
        }

        if (isSelected)
        {
            ColorizeSquare(selectedSquare, Color.green);
            foreach (int square in gb.pm.GetPossibleMoveList(selectedSquare))
            {
                ColorizeSquare(square, Color.blue);
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
