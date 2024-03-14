using System.Collections.Generic;
using UnityEngine;

public class OccupiedTileData
{
    public ChessPiece occupiedPiece = null;

    public Vector2 move;

    public OccupiedTileData(Vector2 move)
    {
        this.move = move;

        foreach (KeyValuePair<ChessPiece, Vector2> pair in Board.Instance.PieceVec)
        {
            if (move == pair.Value)
                occupiedPiece = pair.Key; // If a match is found, store the occupying chess piece
        }
    }
}
