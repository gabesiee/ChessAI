using UnityEngine;
using ChessEngine;

//Main class to instantiate visible board and all the data (AI and model board)
public class GenerateBoard : MonoBehaviour
{
    public PositionManager pm = new PositionManager(); // model board
    public AI ai;

    [SerializeField] private GameObject chessboard;
    [SerializeField] private GameObject cube;

    [SerializeField] private GameObject pawn;
    [SerializeField] private GameObject rook;
    [SerializeField] private GameObject bishop;
    [SerializeField] private GameObject knight;
    [SerializeField] private GameObject queen;
    [SerializeField] private GameObject king;

    public GameObject[] cubesArray = new GameObject[64];

    private void Start()
    {
        ai = new AI(pm);
        generateEmptyBoard();
        PlacePieces();
    }

    private void generateEmptyBoard()
    {
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                Vector3 currentPos = new Vector3(i, 0, j);

                GameObject square = Instantiate(cube, currentPos, Quaternion.identity) as GameObject;
                square.transform.parent = chessboard.transform;

                cubesArray[((7 - j) * 8 + i)] = square;
            }
        }
    }

    public void PlacePieces()
    {
        int[] intBoard = pm.GetDisplayableBoard();

        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                Vector3 currentPos = new Vector3(j, 0, 7 - i);

                switch (intBoard[i * 8 + j])
                {
                    case 1:
                        GameObject whitePawn = Instantiate(pawn, currentPos + Vector3.up * 1.05f, Quaternion.Euler(90, 0, 0)) as GameObject;
                        whitePawn.name = "whitePawn";
                        whitePawn.GetComponent<Renderer>().material.color = Color.white;

                        break;
                    case 2:
                        GameObject whiteKnight = Instantiate(knight, currentPos + Vector3.up * 1.35f, Quaternion.Euler(90, 90, 0)) as GameObject;
                        whiteKnight.name = "whiteKnight";
                        whiteKnight.GetComponent<Renderer>().material.color = Color.white;
                        break;
                    case 3:
                        GameObject whiteBishop = Instantiate(bishop, currentPos + Vector3.up * 1.22f, Quaternion.Euler(90, 0, 0)) as GameObject;
                        whiteBishop.name = "whiteBishop";
                        whiteBishop.GetComponent<Renderer>().material.color = Color.white;
                        break;
                    case 4:
                        GameObject whiteRook = Instantiate(rook, currentPos + Vector3.up * 1.38f, Quaternion.Euler(90, 0, 0)) as GameObject;
                        whiteRook.name = "whiteRook";
                        whiteRook.GetComponent<Renderer>().material.color = Color.white;
                        break;
                    case 5:
                        GameObject whiteQueen = Instantiate(queen, currentPos + Vector3.up * 1.25f, Quaternion.Euler(90, 0, 0)) as GameObject;
                        whiteQueen.name = "whiteQueen";
                        whiteQueen.GetComponent<Renderer>().material.color = Color.white;
                        break;
                    case 6:
                        GameObject whiteKing = Instantiate(king, currentPos + Vector3.up * 1.5f, Quaternion.Euler(90, 0, 0)) as GameObject;
                        whiteKing.name = "whiteKing";
                        whiteKing.GetComponent<Renderer>().material.color = Color.white;
                        break;
                    case -1:
                        GameObject blackPawn = Instantiate(pawn, currentPos + Vector3.up * 1.05f, Quaternion.Euler(90, 0, 0)) as GameObject;
                        blackPawn.name = "blackPawn";
                        blackPawn.GetComponent<Renderer>().material.color = Color.black;
                        break;
                    case -2:
                        GameObject blackKnight = Instantiate(knight, currentPos + Vector3.up * 1.35f, Quaternion.Euler(90, -90, 0)) as GameObject;
                        blackKnight.name = "blackKnight";
                        blackKnight.GetComponent<Renderer>().material.color = Color.black;
                        break;
                    case -3:
                        GameObject blackBishop = Instantiate(bishop, currentPos + Vector3.up * 1.22f, Quaternion.Euler(90, 0, 0)) as GameObject;
                        blackBishop.name = "blackBishop";
                        blackBishop.GetComponent<Renderer>().material.color = Color.black;
                        break;
                    case -4:
                        GameObject blackRook = Instantiate(rook, currentPos + Vector3.up * 1.38f, Quaternion.Euler(90, 0, 0)) as GameObject;
                        blackRook.name = "blackRook";
                        blackRook.GetComponent<Renderer>().material.color = Color.black;
                        break;
                    case -5:
                        GameObject blackQueen = Instantiate(queen, currentPos + Vector3.up * 1.25f, Quaternion.Euler(90, 0, 0)) as GameObject;
                        blackQueen.name = "blackQueen";
                        blackQueen.GetComponent<Renderer>().material.color = Color.black;
                        break;
                    case -6:
                        GameObject blackKing = Instantiate(king, currentPos + Vector3.up * 1.5f, Quaternion.Euler(90, 0, 0)) as GameObject;
                        blackKing.name = "blackKing";
                        blackKing.GetComponent<Renderer>().material.color = Color.black;
                        break;
                }
            }
        }
    }

    public void ClearPieces()
    {
        GameObject[] piecesGO = GameObject.FindGameObjectsWithTag("Piece");

        foreach(GameObject go in piecesGO)
        {
            Destroy(go);
        }
    }
}
