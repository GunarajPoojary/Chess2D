using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public static Board Instance;

    [SerializeField] Transform player, opponent;

    // List of all chess piece components from prefabs to get type and color for instantiation.
    List<ChessPiece> prefabsChessPieces = new List<ChessPiece>();

    // List of all pieces currently on the board.
    [HideInInspector] public List<ChessPiece> chessPieces = new List<ChessPiece>();

    [SerializeField] List<GameObject> piecesPrefab = new List<GameObject>();

    public Dictionary<ChessPiece, Vector2> pieceToVec = new Dictionary<ChessPiece, Vector2>();

    GameObject instantiatedPiece;

    string color;

    [HideInInspector] public string playerColor, opponentColor;

    void Awake()
    {
        if (Instance == null)
            Instance = this;

        foreach (GameObject gameObject in piecesPrefab)
            prefabsChessPieces.Add(gameObject.GetComponent<ChessPiece>());

        playerColor = PlayerPrefs.GetString("selectedColor", color);

        opponentColor = (playerColor == "White") ? "Black" : "White";

        for (int i = 0; i <= 7; i++)
        {
            // Loop through the first two columns
            for (int j = 0; j <= 1; j++)
                PiecePlacement(i, j, player, playerColor);

            // Place opponent's pieces in the last two columns
            for (int j = 6; j <= 7; j++)
                PiecePlacement(i, j, opponent, opponentColor);
        }
    }

    // Assigns current Piece and it's position to dictionary piecToVec
    public void ActivePiecesOnBoard()
    {
        pieceToVec.Clear();

        foreach (ChessPiece piece in chessPieces)
        {
            if (piece.transform.position.x >= 0 && piece.transform.position.y >= 0 && piece.transform.position.x <= 7 && piece.transform.position.y <= 7)
                pieceToVec.Add(piece, piece.transform.position);
        }
    }

    void PiecePlacement(int i , int j, Transform parent, string color)
    {
        int columnNum = 0;

        if (color == opponentColor)
            columnNum = 7;

        if ((i == 0 && j == columnNum) || (i == 7 && j == columnNum))
        {
            instantiatedPiece = InstantiatePieces(color, ChessPiece.PieceType.Rook, i, j);
            instantiatedPiece.transform.SetParent(parent);
            chessPieces.Add(instantiatedPiece.GetComponent<ChessPiece>());
        }
        else if ((i == 1 && j == columnNum) || (i == 6 && j == columnNum))
        {
            instantiatedPiece = InstantiatePieces(color, ChessPiece.PieceType.Knight, i, j);
            instantiatedPiece.transform.SetParent(parent);
            chessPieces.Add(instantiatedPiece.GetComponent<ChessPiece>());
        }
        else if ((i == 2 && j == columnNum) || (i == 5 && j == columnNum))
        {
            instantiatedPiece = InstantiatePieces(color, ChessPiece.PieceType.Bishop, i, j);
            instantiatedPiece.transform.SetParent(parent);
            chessPieces.Add(instantiatedPiece.GetComponent<ChessPiece>());
        }
        else if (i == 3 && j == columnNum)
        {
            instantiatedPiece = InstantiatePieces(color, ChessPiece.PieceType.Queen, i, j);
            instantiatedPiece.transform.SetParent(parent);
            chessPieces.Add(instantiatedPiece.GetComponent<ChessPiece>());
        }
        else if (i == 4 && j == columnNum)
        {
            instantiatedPiece = InstantiatePieces(color, ChessPiece.PieceType.King, i, j);
            instantiatedPiece.transform.SetParent(parent);
            chessPieces.Add(instantiatedPiece.GetComponent<ChessPiece>());
        }
        else
        {
            instantiatedPiece = InstantiatePieces(color, ChessPiece.PieceType.Pawn, i, j);
            instantiatedPiece.transform.SetParent(parent);
            chessPieces.Add(instantiatedPiece.GetComponent<ChessPiece>());
        }
    }

    /// <summary>
    /// This method instantiates pieces on the board
    /// </summary>
    /// <param name="pieceColor">The chess piece's color</param>
    /// <param name="pieceType">The type of chess piece</param>
    /// <param name="row">Row index</param>
    /// <param name="column">Column index</param>
    /// <returns>Returns list of Gameobjects </returns>
    GameObject InstantiatePieces(string pieceColor, ChessPiece.PieceType pieceType, int row, int column)
    {
        GameObject obj = null;

        foreach (ChessPiece chessPiece in prefabsChessPieces)
        {
            // Find the matching piece prefab and instantiate it at the specified position
            if (chessPiece.pieceType == pieceType && chessPiece.pieceColor.ToString() == pieceColor)
                obj = Instantiate(chessPiece.gameObject, new Vector2(row, column), Quaternion.identity);
        }
        return obj;
    }
}
