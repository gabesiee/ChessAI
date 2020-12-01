using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ChessEngine;

public class InteractionManager : MonoBehaviour
{
    Ray ray;
    RaycastHit hit;
    Renderer objRenderer;
    Material oldMaterial;
    Material newMaterial;
    GameObject obj;
    Vector3 position;

    PiecesManager pm = new PiecesManager();

    bool isSelected = false;
    int selectedSquare = -1;

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
                if (pm.GetPossibleMoveList(selectedSquare).Contains(targetSquare))
                {
                    Debug.Log("Moving Piece");
                    pm.MovePiece(selectedSquare, targetSquare);
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

                if (pm.IsPlayableSquare(selectedSquare))
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
            foreach (int square in pm.GetPossibleMoveList(selectedSquare))
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
        this.GetComponent<GenerateBoard>().cubesArray[square].GetComponent<Renderer>().material.color = color;
    }

}
