using UnityEngine;

namespace Chess2D.Board
{
    public class BoardRenderer
    {
        private readonly SpriteRenderer[,] _tileRenderers;

        public BoardRenderer(int size, Color darkTileColor, Color lightTileColor, GameObject tilePrefab, Transform parentTransform)
        {
            Transform rowTransform = null;

            _tileRenderers = new SpriteRenderer[size, size];

            for (int row = 0; row < size; row++)
            {
                for (int col = 0; col < size; col++)
                {
                    if (col == 0)
                    {
                        rowTransform = new GameObject($"Row_{row}").transform;
                        rowTransform.SetParent(parentTransform);
                    }

                    bool isDark = (row + col) % 2 == 0;

                    GameObject tileObj = Object.Instantiate(
                        tilePrefab,
                        new Vector3(col, row),
                        Quaternion.identity,
                        rowTransform);

                    tileObj.name = isDark ? $"DarkTile(C{col}_R{row})" : $"LightTile(C{col}_R{row})";

                    SpriteRenderer tileRenderer = tileObj.GetComponentInChildren<SpriteRenderer>();

                    tileRenderer.color = isDark ? darkTileColor : lightTileColor;

                    _tileRenderers[row, col] = tileRenderer;
                }
            }
        }

        public void SetBoardTheme(Color darkTileColor, Color lightTileColor)
        {
            for (int row = 0; row < _tileRenderers.GetLength(0); row++)
            {
                for (int col = 0; col < _tileRenderers.GetLength(1); col++)
                {
                    _tileRenderers[row, col].color = (row + col) % 2 == 0 ? darkTileColor : lightTileColor;
                }
            }
        }
    }
}