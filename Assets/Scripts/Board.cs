using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public static Board Instance { get; private set; }

    [Space(5), Header("Player and Opponents Transform")]
    [SerializeField] Transform player;
    [SerializeField] Transform opponent;

    [Space(20), Header("Chess Piece prefabs")]
    [SerializeField] List<GameObject> piecesPrefab = new List<GameObject>();

    // List of all chess piece components from prefabs.
    List<ChessPiece> prefabsChessPieces = new List<ChessPiece>();

    // List of all chess pieces currently on the board.
    List<ChessPiece> chessPieces = new List<ChessPiece>();

    // Dictionary to map chess pieces to their positions
    public Dictionary<ChessPiece, Vector2> PieceVec { get; private set; } = new Dictionary<ChessPiece, Vector2>();

    public string PlayerColor { get; private set; }
    public string OpponentColor { get; private set; }

    GameObject instantiatedPiece;

    string color;

    void Awake()
    {
        if (Instance == null)
            Instance = this;

        foreach (GameObject gameObject in piecesPrefab)
            prefabsChessPieces.Add(gameObject.GetComponent<ChessPiece>());

        // Set player and opponent colors based on player preference
        PlayerColor = PlayerPrefs.GetString("selectedColor", color);
        OpponentColor = (PlayerColor == "White") ? "Black" : "White";

        // Loop through 8 rows 
        for (int i = 0; i <= 7; i++)
        {
            // Loop through the first two columns
            for (int j = 0; j <= 1; j++)
                PiecePlacement(i, j, player, PlayerColor);

            // Place opponent's pieces in the last two columns
            for (int j = 6; j <= 7; j++)
                PiecePlacement(i, j, opponent, OpponentColor);
        }
    }

    // Assigns current Piece and its position to dictionary PieceVec
    public void ActivePiecesOnBoard()
    {
        PieceVec.Clear();

        foreach (ChessPiece piece in chessPieces)
        {
            if (piece.transform.position.x >= 0 && piece.transform.position.y >= 0 && piece.transform.position.x <= 7 && piece.transform.position.y <= 7)
                PieceVec.Add(piece, piece.transform.position);
        }
    }

    // Places a piece on the board at the specified position
    void PiecePlacement(int i, int j, Transform parent, string color)
    {
        int columnNum = 0;

        if (color == OpponentColor)
            columnNum = 7;

        if ((i == 0 && j == columnNum) || (i == 7 && j == columnNum))
            SettingParent(i, j, color, parent, ChessPiece.PieceType.Rook);
        else if ((i == 1 && j == columnNum) || (i == 6 && j == columnNum))
            SettingParent(i, j, color, parent, ChessPiece.PieceType.Knight);
        else if ((i == 2 && j == columnNum) || (i == 5 && j == columnNum))
            SettingParent(i, j, color, parent, ChessPiece.PieceType.Bishop);
        else if (i == 3 && j == columnNum)
            SettingParent(i, j, color, parent, ChessPiece.PieceType.Queen);
        else if (i == 4 && j == columnNum)
            SettingParent(i, j, color, parent, ChessPiece.PieceType.King);
        else
            SettingParent(i, j, color, parent, ChessPiece.PieceType.Pawn);
    }

    /// <summary>
    /// Sets the parent for the instantiated piece and adds it to the chessPieces list
    /// </summary>
    /// <param name="i">Row index</param>
    /// <param name="j">Column index</param>
    /// <param name="color">Type of Chess Piece color</param>
    /// <param name="parent">Parent transform to be attached</param>
    /// <param name="pieceType">Type of Chess piece</param>
    void SettingParent(int i, int j, string color, Transform parent, ChessPiece.PieceType pieceType)
    {
        instantiatedPiece = InstantiatePieces(color, pieceType, i, j);
        instantiatedPiece.transform.SetParent(parent);
        chessPieces.Add(instantiatedPiece.GetComponent<ChessPiece>());
    }

    /// <summary>
    /// Instantiates pieces on the board
    /// </summary>
    /// <param name="pieceColor">The chess piece's color</param>
    /// <param name="pieceType">The type of chess piece</param>
    /// <param name="row">Row index</param>
    /// <param name="column">Column index</param>
    /// <returns>Returns the instantiated GameObject </returns>
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
