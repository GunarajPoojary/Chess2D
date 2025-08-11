using UnityEngine;

namespace Chess2D.Board
{
    public interface IBoardUtility
    {
        bool IsTileEmptyAt(Vector2Int boardPosition);
        bool ContainsOpponentPieceAt(Vector2Int boardPosition, bool isPlayer);
        bool ContainsAllyPieceAt(Vector2Int boardPosition, bool isPlayer);
    }
}