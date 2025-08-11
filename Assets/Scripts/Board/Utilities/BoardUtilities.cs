using UnityEngine;

namespace Chess2D.Board.Utilities
{
    public static class BoardUtilities
    {
        public static bool IsWithinBoard(Vector2Int tile) => tile.x < 8 && tile.x >= 0 && tile.y < 8 && tile.y >= 0;
    }
}