using System;
using System.Collections.Generic;
using UnityEngine;

public class GenerateBoard : MonoBehaviour
{
    [SerializeField] private GameObject chessboard;
    [SerializeField] private GameObject lightCube;
    [SerializeField] private GameObject darkCube;

    private void Start()
    {
        generateEmptyBoard();
    }

    private void generateEmptyBoard()
    {
        String squareColor = "light";
        for (int i = 0; i < 64; i++)
        {
            Vector3 pos = new Vector3(i / 8, 0, i % 8);

            if (i % 8 == 0)
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
                GameObject cube = Instantiate(darkCube, pos, Quaternion.identity) as GameObject;
                cube.transform.parent = chessboard.transform;

                squareColor = "light";
            }
            else
            {
                GameObject cube = Instantiate(lightCube, pos, Quaternion.identity) as GameObject;
                cube.transform.parent = chessboard.transform;

                squareColor = "dark";
            }
        }
    }
}
