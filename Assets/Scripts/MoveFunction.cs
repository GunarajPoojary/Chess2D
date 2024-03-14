using System;
using System.Collections.Generic;
using UnityEngine;

public class MoveFunction
{
    // Dictionary to map chess piece types to their respective move functions
    Dictionary<ChessPiece.PieceType, Action> pieceToFunction = new Dictionary<ChessPiece.PieceType, Action>();

    // List to store generated moves
    List<Move> moves = new List<Move>();

    ChessPiece piece;
    ChessPiece.PieceType pieceType;
    ChessPiece.PieceColor pieceColor;
    Vector2 currentPiecePos;

    public MoveFunction()
    {
        pieceToFunction.Add(ChessPiece.PieceType.Pawn, PawnMoves);
        pieceToFunction.Add(ChessPiece.PieceType.Rook, RookMoves);
        pieceToFunction.Add(ChessPiece.PieceType.Knight, GetKnightMoves);
        pieceToFunction.Add(ChessPiece.PieceType.Bishop, BishopMoves);
        pieceToFunction.Add(ChessPiece.PieceType.Queen, QueenMoves);
        pieceToFunction.Add(ChessPiece.PieceType.King, KingMoves);
    }

    public List<Move> GetMoves(ChessPiece piece)
    {
        this.piece = piece;
        pieceType = piece.pieceType;
        pieceColor = piece.pieceColor;
        currentPiecePos = piece.transform.position;

        foreach (KeyValuePair<ChessPiece.PieceType, Action> kp in pieceToFunction)
        {
            if (piece.pieceType == kp.Key)
                kp.Value.Invoke();
        }

        return moves;
    }

    void GenerateMove(int limit, Vector2 direction)
    {
        for (int i = 1; i < limit; ++i)
        {
            Vector2 move = currentPiecePos + direction * i;
            OccupiedTileData tileData = new OccupiedTileData(move);

            if (IsOnBoard(move) && ContainsPiece(move))
            {
                if (IsEnemy(tileData) && pieceType != ChessPiece.PieceType.Pawn)
                    CheckAndStoreMove(move);
                break;
            }
            CheckAndStoreMove(move);
        }
    }

    void CheckAndStoreMove(Vector2 move)
    {
        OccupiedTileData tileData = new OccupiedTileData(move);

        if (IsOnBoard(move) && !ContainsPiece(move) || IsEnemy(tileData))
        {
            Move moveData = new Move()
            {
                currentPiece = piece.transform,
                targetPos = move
            };

            if (tileData.occupiedPiece != null)
                moveData.capturablePiece = tileData.occupiedPiece;

            moves.Add(moveData);
        }
    }

    bool IsOnBoard(Vector2 tile)
    {
        if (tile.x >= 0 && tile.y >= 0 && tile.x <= 7 && tile.y <= 7) return true;
        else return false;
    }

    bool ContainsPiece(Vector2 tile)
    {
        OccupiedTileData tileData = new OccupiedTileData(tile);

        if (tileData.occupiedPiece != null && tile == tileData.move)
            if (IsOnBoard(tile)) return true;

        return false;
    }

    bool IsEnemy(OccupiedTileData piece)
    {
        if (piece.occupiedPiece != null)
        {
            if (pieceColor != piece.occupiedPiece.pieceColor) return true;
        }
        return false;
    }

    void PawnMoves()
    {
        OccupiedTileData tileData;

        if (pieceColor.ToString() == Board.Instance.PlayerColor)
        {
            // Straight move
            GenerateMove(2, new Vector2(0, 1));

            // Diagonal moves for capturing
            Vector2 diagLeftCapture = new Vector2(currentPiecePos.x - 1, currentPiecePos.y + 1);
            Vector2 diagRightCapture = new Vector2(currentPiecePos.x + 1, currentPiecePos.y + 1);

            tileData = new OccupiedTileData(diagLeftCapture);

            if (ContainsPiece(diagLeftCapture) && IsEnemy(tileData))
                CheckAndStoreMove(diagLeftCapture);

            tileData = new OccupiedTileData(diagRightCapture);

            if (ContainsPiece(diagRightCapture) && IsEnemy(tileData))
                CheckAndStoreMove(diagRightCapture);
        }
        else if (pieceColor.ToString() == Board.Instance.OpponentColor)
        {
            GenerateMove(2, new Vector2(0, -1));

            Vector2 diagLeftCapture = new Vector2(currentPiecePos.x - 1, currentPiecePos.y - 1);
            Vector2 diagRightCapture = new Vector2(currentPiecePos.x + 1, currentPiecePos.y - 1);

            tileData = new OccupiedTileData(diagLeftCapture);

            if (ContainsPiece(diagLeftCapture) && IsEnemy(tileData))
                CheckAndStoreMove(diagLeftCapture);

            tileData = new OccupiedTileData(diagRightCapture);

            if (ContainsPiece(diagRightCapture) && IsEnemy(tileData))
                CheckAndStoreMove(diagRightCapture);
        }
    }

    void RookMoves()
    {
        int[,] rookMoves = new int[,] { {0,1 }, { 0, -1 }, { 1, 0 }, { -1, 0 } };

        for (int i = 0; i < rookMoves.GetLength(0); i++)
            GenerateMove(9, new Vector2(rookMoves[i,0], rookMoves[i, 1]));
    }

    void GetKnightMoves()
    {
        Vector2 move;

        int[,] knightMoves = new int[,] { { 1, -2 }, { 1, 2 }, { -1, 2 }, { -1, -2 }, { 2, 1 }, { 2, -1 }, { -2, 1 }, { -2, -1 } };

        for (int i = 0; i < knightMoves.GetLength(0); i++)
        {
            move = new Vector2(currentPiecePos.x + knightMoves[i,0], currentPiecePos.y + knightMoves[i,1]);
            CheckAndStoreMove(move);
        }
    }

    void BishopMoves()
    {
        int[,] bishopMoves = new int[,] { { 1, 1 }, { 1, -1 }, { -1, 1 }, { -1, -1 } };

        for (int i = 0; i < bishopMoves.GetLength(0); i++)
            GenerateMove(9, new Vector2(bishopMoves[i, 0], bishopMoves[i, 1]));
    }

    void QueenMoves()
    {
        BishopMoves();
        RookMoves();
    }

    void KingMoves()
    {
        for (int x = -1; x <= 1; ++x)
        {
            for (int y = -1; y <= 1; ++y)
            {
                if (x == 0 && y == 0) continue;

                CheckAndStoreMove(new Vector2(currentPiecePos.x + x, currentPiecePos.y + y));
            }
        }
    }
}
