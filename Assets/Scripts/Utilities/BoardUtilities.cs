using UnityEngine;

namespace Chess2D.Utilities
{
    public static class BoardUtilities
    {
        public static bool IsWithinBoard(Vector2Int tile)
        {
            if ((tile.x < 8 && tile.x >= 0) || (tile.y < 8 && tile.y >= 0)) return true;

            return false;
        }
    }
}