using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class Board : MonoBehaviour
{
    // Singleton instance of the Board.
    public static Board Instance;

    // Transforms to separate player's and opponent's pieces.
    [SerializeField] Transform player;
    [SerializeField] Transform opponent;

    // List of all pieces on the board.
    public List<Piece> pieces;

    // List of highlighter game objects.
    public List<GameObject> highLighters;

    // Variable to hold the selected color.
    string color;

    // Default colors for player and opponent.
    [HideInInspector] public string playerColor = "White";
    [HideInInspector] public string opponentColor = "Black";

    // Serialized field to hold prefabs of chess pieces
    [SerializeField] List<GameObject> piecePrefab = new List<GameObject>();

    // Constants for piece names
    const string pawn = "_Pawn";
    const string bishop = "_Bishop";
    const string king = "_King";
    const string knight = "_Knight";
    const string queen = "_Queen";
    const string rook = "_Rook";

    void Awake()
    {
        // Ensure there is only one instance of Board.
        if (Instance == null)
            Instance = this;

        // Retrieve the selected color from PlayerPrefs or default to "White".
        playerColor = PlayerPrefs.GetString("selectedColor", color);

        // Set the opponent color based on the player's color.
        opponentColor = (playerColor == "White") ? "Black" : "White";

        // Place pieces on the board
        PiecePlacement();
    }

    // Start is called before the first frame update.
    void Start()
    {
        // Find all pieces in the scene and add them to the pieces list.
        pieces = FindObjectsOfType<Piece>().ToList();

        // Assign pieces to their respective player's or opponent's parent transform.
        PiecePos();
    }

    // Method to place pieces on the board
    void PiecePlacement()
    {
        // Loop through the rows of the chessboard
        for (int i = 0; i <= 7; i++)
        {
            // Place player's pieces in the first two rows
            for (int j = 0; j <= 1; j++)
            {
                // Determine the type of piece and instantiate it
                if ((i == 0 && j == 0) || (i == 7 && j == 0))
                    InstantiatePieces(playerColor, rook, i, j);
                else if ((i == 1 && j == 0) || (i == 6 && j == 0))
                    InstantiatePieces(playerColor, knight, i, j);
                else if ((i == 2 && j == 0) || (i == 5 && j == 0))
                    InstantiatePieces(playerColor, bishop, i, j);
                else if (i == 3 && j == 0)
                    InstantiatePieces(playerColor, queen, i, j);
                else if (i == 4 && j == 0)
                    InstantiatePieces(playerColor, king, i, j);
                else
                    InstantiatePieces(playerColor, pawn, i, j);
            }

            // Place opponent's pieces in the last two rows
            for (int j = 6; j <= 7; j++)
            {
                // Determine the type of piece and instantiate it
                if ((i == 0 && j == 7) || (i == 7 && j == 7))
                    InstantiatePieces(opponentColor, rook, i, j);
                else if ((i == 1 && j == 7) || (i == 6 && j == 7))
                    InstantiatePieces(opponentColor, knight, i, j);
                else if ((i == 2 && j == 7) || (i == 5 && j == 7))
                    InstantiatePieces(opponentColor, bishop, i, j);
                else if (i == 3 && j == 7)
                    InstantiatePieces(opponentColor, king, i, j);
                else if (i == 4 && j == 7)
                    InstantiatePieces(opponentColor, queen, i, j);
                else
                    InstantiatePieces(opponentColor, pawn, i, j);
            }
        }
    }

    // Method to instantiate pieces on the board
    void InstantiatePieces(string pieceColor, string pieceType, int row, int column)
    {
        // Loop through the list of piece prefabs
        foreach (GameObject g in piecePrefab)
        {
            // Find the matching piece prefab and instantiate it at the specified position
            if (g.name == string.Concat(pieceColor, pieceType))
                Instantiate(g, new Vector2(row, column), Quaternion.identity);
        }
    }

    // Method to assign pieces to their respective player's or opponent's parent transform.
    public void PiecePos()
    {
        foreach (Piece piece in pieces)
        {
            if (piece.name.Contains(playerColor))
                piece.transform.SetParent(player);
            else
                piece.transform.SetParent(opponent);
        }
    }
}
