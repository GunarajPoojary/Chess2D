using System;
using System.Collections.Generic;
using UnityEngine;

public class MoveFunction
{
    Dictionary<ChessPiece.PieceType, Action> pieceToFunction = new Dictionary<ChessPiece.PieceType, Action>();

    List<Move> moves = new List<Move>();

    ChessPiece piece;
    ChessPiece.PieceType pieceType;
    ChessPiece.PieceColor pieceColor;
    Vector2 currentPiecePos;

    // Initialize the dictionary with mappings between chess piece types and their move functions
    public MoveFunction()
    {
        pieceToFunction.Add(ChessPiece.PieceType.Pawn,PawnMoves);
        pieceToFunction.Add(ChessPiece.PieceType.Rook, RookMoves);
        pieceToFunction.Add(ChessPiece.PieceType.Knight, GetKnightMoves);
        pieceToFunction.Add(ChessPiece.PieceType.Bishop, BishopMoves);
        pieceToFunction.Add(ChessPiece.PieceType.Queen, QueenMoves);
        pieceToFunction.Add(ChessPiece.PieceType.King, KingMoves);
    }

    // Generates legal moves for the given chess piece
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

    // Generates moves in a certain direction up to a specified limit.
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
                currentPos = piece.transform,
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

        if (tileData.occupiedPiece != null && tile == tileData.tile)
        {
            if (IsOnBoard(tile)) return true;
        }
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

        if (pieceColor.ToString() == Board.Instance.playerColor)
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
        else if (pieceColor.ToString() == Board.Instance.opponentColor)
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
        GenerateMove(9, new Vector2(0, 1));
        GenerateMove(9, new Vector2(0, -1));
        GenerateMove(9, new Vector2(1, 0));
        GenerateMove(9, new Vector2(-1, 0));
    }

    void GetKnightMoves()
    {
        // Possible knight moves
        Vector2 move;

        move = new Vector2(currentPiecePos.x + 1, currentPiecePos.y - 2);
        CheckAndStoreMove(move);
        move = new Vector2(currentPiecePos.x + 1, currentPiecePos.y + 2);
        CheckAndStoreMove(move);
        move = new Vector2(currentPiecePos.x - 1, currentPiecePos.y + 2);
        CheckAndStoreMove(move);
        move = new Vector2(currentPiecePos.x - 1, currentPiecePos.y - 2);
        CheckAndStoreMove(move);

        move = new Vector2(currentPiecePos.x + 2, currentPiecePos.y + 1);
        CheckAndStoreMove(move);
        move = new Vector2(currentPiecePos.x + 2, currentPiecePos.y - 1);
        CheckAndStoreMove(move);
        move = new Vector2(currentPiecePos.x - 2, currentPiecePos.y + 1);
        CheckAndStoreMove(move);
        move = new Vector2(currentPiecePos.x - 2, currentPiecePos.y - 1);
        CheckAndStoreMove(move);
    }

    void BishopMoves()
    {
        GenerateMove(9, new Vector2(1, 1));
        GenerateMove(9, new Vector2(1, -1));
        GenerateMove(9, new Vector2(-1, 1));
        GenerateMove(9, new Vector2(-1, -1));
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
