using UnityEngine;
using System.Collections.Generic;
using System;

public class PieceController : MonoBehaviour
{
    Transform selectedPiece;

    [SerializeField] GameObject captureHighlight;
    [SerializeField] GameObject emptyTileHighlight;

    const string highlighterTag = "Highlighter";

    List<Move> moves = new List<Move>();

    MoveFunction moveFunction = new MoveFunction();

    public event Action OnEndTurn;

    public event Action<string> OnKingDeath;

    void Update()
    {

        if (GameManager.Instance.gameIsActive)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
                ChessPiece chessPiece = hit.transform.GetComponent<ChessPiece>();

                if (chessPiece != null)
                {
                    RemoveHighlights();

                    if (chessPiece.pieceColor.ToString() == Board.Instance.playerColor && GameManager.Instance.playerTurn)
                        HighlightPossibleMoves(chessPiece, Board.Instance.playerColor);
                    else if (chessPiece.pieceColor.ToString() == Board.Instance.opponentColor && !GameManager.Instance.playerTurn)
                        HighlightPossibleMoves(chessPiece, Board.Instance.opponentColor);
                }

                if (hit.transform.CompareTag(highlighterTag))
                {
                    selectedPiece.position = hit.transform.position;

                    ChessPiece opponentPiece = new OccupiedTileData(selectedPiece.position).occupiedPiece;

                    if (opponentPiece != null)
                    {
                        opponentPiece.transform.position = new Vector2(9, 5);

                        if (opponentPiece.pieceType == ChessPiece.PieceType.King)
                        {
                            if (opponentPiece.pieceColor.ToString() == Board.Instance.opponentColor)
                                OnKingDeath?.Invoke(Board.Instance.playerColor);
                            else
                                OnKingDeath?.Invoke(Board.Instance.opponentColor);
                        }
                    }

                    OnEndTurn?.Invoke();

                    RemoveHighlights();
                }
            }
        }
    }

    void HighlightPossibleMoves(ChessPiece chessPiece, string color)
    {
        if (chessPiece.pieceColor.ToString() == color)
        {
            Board.Instance.ActivePiecesOnBoard();

            moves.Clear();
            moves = moveFunction.GetMoves(chessPiece);

            foreach (Move move in moves)
            {
                if (move.currentPos != null)
                {
                    selectedPiece = move.currentPos;

                    if (move.capturablePiece == null)
                        Instantiate(emptyTileHighlight, move.targetPos, Quaternion.identity);
                    else if (move.capturablePiece != null)
                        Instantiate(captureHighlight, move.targetPos, Quaternion.identity);
                }
            }
        }
    }

    void RemoveHighlights()
    {
        GameObject[] highLighters = GameObject.FindGameObjectsWithTag("Highlighter");

        foreach (var obj in highLighters)
            Destroy(obj);
    }
}
