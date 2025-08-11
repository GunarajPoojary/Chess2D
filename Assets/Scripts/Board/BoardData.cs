using UnityEngine;

namespace Chess2D.Board
{
    public class BoardData
    {
        private readonly TileData[,] _tileDatas;

        public BoardData(int size)
        {
            _tileDatas = new TileData[size, size];

            for (int row = 0; row < size; row++)
            {
                for (int col = 0; col < size; col++)
                {
                    _tileDatas[row, col] = new TileData();
                }
            }
        }

        public bool IsMoveValidAt(Vector2Int boardPosition) => _tileDatas[boardPosition.y, boardPosition.x].IsValidForMove;
        public void UnMarkMoveValidAt(Vector2Int boardPosition) => _tileDatas[boardPosition.y, boardPosition.x].UnMarkValidForMove();
        public void MarkMoveValidAt(Vector2Int boardPosition) => _tileDatas[boardPosition.y, boardPosition.x].MarkValidForMove();
    }
}