using UnityEngine;
using System.Collections.Generic;
using System;

public class PieceController : MonoBehaviour
{
    [Space(5), Header("Red Highlighter")]
    [SerializeField] GameObject captureHighlight; // Highlighter for capturing moves
    [Space(5), Header("Green Highlighter")]
    [SerializeField] GameObject emptyTileHighlight; // Highlighter for non-capturing moves

    // Lists to place captured piece outside the board
    List<Vector2> capturedPlayerPos = new List<Vector2>();
    List<Vector2> capturedOpponentPos = new List<Vector2>();

    // List to store possible moves
    List<Move> moves = new List<Move>();

    MoveFunction moveFunction = new MoveFunction();

    // Selected piece's transform
    Transform selectedPiece;

    int capturedPlayerPieceInd, capturedOpponentPieceInd = 0;

    const string highlighterTag = "Highlighter";

    public event Action OnEndTurn; 
    public event Action<string> OnKingDeath;

    void Start()
    {
        for (int i = 0; i <= 7; i++)
        {
            for (int j = -1; j >= -2; j--) // For player side
                capturedPlayerPos.Add(new Vector2(i, j));
            for (int k = 8; k <= 9; k++) // For opponent side
                capturedOpponentPos.Add(new Vector2(i, k));
        }
    }

    void Update()
    {
        try
        {
            if (Input.GetMouseButtonDown(0) && GameManager.Instance.GameIsActive)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

                ChessPiece chessPiece = hit.transform.GetComponent<ChessPiece>();

                if (chessPiece != null)
                {
                    RemoveHighlighters();

                    if (chessPiece.pieceColor.ToString() == Board.Instance.PlayerColor && GameManager.Instance.PlayerTurn)
                        HighlightPossibleMoves(chessPiece, Board.Instance.PlayerColor);
                    else if (chessPiece.pieceColor.ToString() == Board.Instance.OpponentColor && !GameManager.Instance.PlayerTurn)
                        HighlightPossibleMoves(chessPiece, Board.Instance.OpponentColor);
                }

                if (hit.transform.CompareTag(highlighterTag))
                {
                    selectedPiece.position = hit.transform.position;

                    RemoveHighlighters();

                    // Check if a piece is captured
                    ChessPiece capturedPiece = new OccupiedTileData(selectedPiece.position).occupiedPiece;

                    if (capturedPiece != null)
                    {
                        // Move the captured piece to the appropriate captured position list
                        if (capturedPiece.pieceColor.ToString() == Board.Instance.PlayerColor)
                        {
                            capturedPiece.transform.position = capturedPlayerPos[capturedPlayerPieceInd];
                            capturedPlayerPieceInd++;
                        }
                        else if (capturedPiece.pieceColor.ToString() == Board.Instance.OpponentColor)
                        {
                            capturedPiece.transform.position = capturedOpponentPos[capturedOpponentPieceInd];
                            capturedOpponentPieceInd++;
                        }

                        if (capturedPiece.pieceType == ChessPiece.PieceType.King)
                        {
                            if (capturedPiece.pieceColor.ToString() == Board.Instance.OpponentColor)
                                OnKingDeath?.Invoke(Board.Instance.PlayerColor);
                            else
                                OnKingDeath?.Invoke(Board.Instance.OpponentColor);
                        }
                    }

                    OnEndTurn?.Invoke();
                }
            }
        }
        catch (Exception)
        {
            // Ignore this
        }
        
    }

    /// <summary>
    /// Highlight possible moves for a given chess piece.
    /// </summary>
    /// <param name="chessPiece">The chess piece to highlight moves for.</param>
    /// <param name="color">The color of the player who owns the piece.</param>
    void HighlightPossibleMoves(ChessPiece chessPiece, string color)
    {
        if (chessPiece.pieceColor.ToString() == color)
        {
            Board.Instance.ActivePiecesOnBoard();

            // Calculate possible moves
            moves.Clear();
            moves = moveFunction.GetMoves(chessPiece);

            foreach (Move move in moves)
            {
                if (move.currentPiece != null)
                {
                    selectedPiece = move.currentPiece;

                    if (move.capturablePiece == null)
                        Instantiate(emptyTileHighlight, move.targetPos, Quaternion.identity);
                    else if (move.capturablePiece != null)
                        Instantiate(captureHighlight, move.targetPos, Quaternion.identity);
                }
            }
        }
    }

    void RemoveHighlighters()
    {
        GameObject[] highLighters = GameObject.FindGameObjectsWithTag("Highlighter");

        foreach (var obj in highLighters)
            Destroy(obj);
    }
}
