using System.Collections.Generic;
using UnityEngine;

public class OccupiedTileData
{
    public ChessPiece occupiedPiece = null;

    public Vector2 tile;

    public OccupiedTileData(Vector2 move)
    {
        tile = move;

        // Iterate through the dictionary of chess pieces and their positions
        foreach (KeyValuePair<ChessPiece, Vector2> pair in Board.Instance.pieceToVec)
        {
            // Check if the specified move position matches the position of any chess piece
            if (move == pair.Value)
            {
                // If a match is found, store the position of the tile and the occupying chess piece
                occupiedPiece = pair.Key;
            }
        }
    }
}
