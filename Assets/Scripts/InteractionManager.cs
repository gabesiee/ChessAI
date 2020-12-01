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

    private void Start()
    {
        gb = this.GetComponent<GenerateBoard>();
    }

    // Update is called once per frame
    void Update()
    {
        GetSelectedSquare();
        Colorize();
    }

    void GetSelectedSquare()
    {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit))
        {

            if (Input.GetMouseButtonDown(0) && isSelected)
            {
                Debug.Log("Second select");
                position = hit.transform.position;

                int targetSquare = (int)((7 - position.z) * 8 + position.x);
                if (gb.pm.GetPossibleMoveList(selectedSquare).Contains(targetSquare))
                {
                    Debug.Log("Moving Piece");
                    gb.pm.MovePiece(selectedSquare, targetSquare);
                    gb.ClearPieces();
                    gb.PlacePieces();
                }
                else
                {
                    Debug.Log("Cancelling Move");
                }
                isSelected = false;
                selectedSquare = -1;
                return;
            }

            if (Input.GetMouseButtonDown(0))
            {
                Debug.Log("First select");
                position = hit.transform.position;
                selectedSquare = (int)((7 - position.z) * 8 + position.x);

                if (gb.pm.IsPlayableSquare(selectedSquare))
                {
                    isSelected = true;
                }
                else
                {
                    hit.collider.gameObject.GetComponent<Renderer>().material.color = Color.red;
                }
            }
        }
    }


    void Colorize()
    {
        ColorizeDefaultBoard();

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
