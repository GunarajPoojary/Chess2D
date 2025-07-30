using UnityEditor;
using UnityEngine;

namespace Chess2D
{
    public class BoardGeneratorWindow : EditorWindow
    {
        private GameObject _darkTilePrefab;
        private GameObject _lightTilePrefab;
        private Transform _boardTransform;

        private readonly int _size = 8;
        private Transform _rowTransform;

        [MenuItem("Tools/Board Generator")]
        public static void ShowWindow() => GetWindow<BoardGeneratorWindow>("Board Generator");

        private void OnGUI()
        {
            GUILayout.Space(10);
            
            _darkTilePrefab = (GameObject)EditorGUILayout.ObjectField("Dark tile prefab", _darkTilePrefab, typeof(GameObject), false);
            _lightTilePrefab = (GameObject)EditorGUILayout.ObjectField("Light tile prefab", _lightTilePrefab, typeof(GameObject), false);
            _boardTransform = (Transform)EditorGUILayout.ObjectField("Board transform", _boardTransform, typeof(Transform), true);

            if (GUILayout.Button("Generate"))
            {
                if (_boardTransform && _darkTilePrefab && _lightTilePrefab)
                    GenerateBoard();
            }
        }

        private void GenerateBoard()
        {
            for (int i = 0; i < _size * _size; i++)
            {
                int row = i / _size;
                int col = i % _size;

                if (col == 0)
                {
                    _rowTransform = new GameObject($"Row_{row}").transform;
                    _rowTransform.SetParent(_boardTransform);
                }

                bool isDark = (row + col) % 2 == 0;

                GameObject tile = Instantiate(
                    isDark ? _darkTilePrefab : _lightTilePrefab,
                    _rowTransform);

                tile.name = isDark ? $"DarkTile(C{col},R{row})" : $"LightTile(C{col},R{row})";

                tile.transform.position = new Vector3(col, row, 0);
            }
        }
    }
}