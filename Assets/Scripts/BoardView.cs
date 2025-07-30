using System.Collections.Generic;
using UnityEngine;

namespace Chess2D
{
    [System.Serializable]
    public class BoardModel
    {
        public Color lightTileColor;
        public Color darkTileColor;
    }

    public class BoardView : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer[] _lightTiles;
        [SerializeField] private SpriteRenderer[] _darkTiles;
        private readonly int _tileCountPerColor = 32;

        public void SetTilesColor(Color darkTileColor, Color lightTileColor)
        {
            for (int i = 0; i < _tileCountPerColor; i++)
            {
                if (_lightTiles[i] != null)
                    _lightTiles[i].color = lightTileColor;

                if (_darkTiles[i] != null)
                    _darkTiles[i].color = darkTileColor;
            }
        }

#if UNITY_EDITOR
        [ContextMenu("Auto assign tiles")]
        private void AutoAssignTiles()
        {
            _lightTiles = new SpriteRenderer[_tileCountPerColor];
            _darkTiles = new SpriteRenderer[_tileCountPerColor];

            GameObject[] darkTileObjects = GameObject.FindGameObjectsWithTag("DarkTile");
            GameObject[] lightTileObjects = GameObject.FindGameObjectsWithTag("LightTile");

            if (darkTileObjects.Length < _tileCountPerColor || lightTileObjects.Length < _tileCountPerColor)
                Debug.LogWarning("Not enough tiles found with the required tags.");

            for (int i = 0; i < _tileCountPerColor && i < darkTileObjects.Length; i++)
                _darkTiles[i] = darkTileObjects[i].GetComponentInChildren<SpriteRenderer>();

            for (int i = 0; i < _tileCountPerColor && i < lightTileObjects.Length; i++)
                _lightTiles[i] = lightTileObjects[i].GetComponentInChildren<SpriteRenderer>();

            Debug.Log($"Auto-assigned {Mathf.Min(_tileCountPerColor, darkTileObjects.Length)} dark tiles and {Mathf.Min(_tileCountPerColor, lightTileObjects.Length)} light tiles.");
        }
#endif
    }

    public class BoardController
    {
        private readonly BoardModel _model;
        private readonly BoardView _view;
        
        private readonly Dictionary<ChessPieceController, Vector3> _lightColoredPieces = new();
        private readonly Dictionary<ChessPieceController, Vector3> _darkColoredPieces = new();

        public BoardController(BoardModel boardModel, BoardView boardView)
        {
            _model = boardModel;
            _view = boardView;

            _view.SetTilesColor(_model.darkTileColor, _model.lightTileColor);
        }

        public void SetTilesColor(Color darkTileColor, Color lightTileColor) => _view.SetTilesColor(darkTileColor, lightTileColor);
    }
}