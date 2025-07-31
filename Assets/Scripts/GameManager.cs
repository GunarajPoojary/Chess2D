using System.Collections.Generic;
using UnityEngine;

namespace Chess2D
{
    public interface IPieceSpriteProvider
    {
        Sprite GetSprite(bool isLight, PieceType type);
    }

    public class PieceSpriteDataBase : IPieceSpriteProvider
    {
        private readonly ColorThemeSO _colorThemeSO;

        public PieceSpriteDataBase(ColorThemeSO colorThemeSO)
        {
            _colorThemeSO = colorThemeSO;
        }

        public Sprite GetSprite(bool isLight, PieceType type)
        {
            Sprite sprite = null;

            if (isLight)
            {
                switch (type)
                {
                    case PieceType.Pawn:
                        sprite = _colorThemeSO.lightPawn;
                        break;
                    case PieceType.Rook:
                        sprite = _colorThemeSO.lightRook;
                        break;
                    case PieceType.Knight:
                        sprite = _colorThemeSO.lightKnight;
                        break;
                    case PieceType.Bishop:
                        sprite = _colorThemeSO.lightBishop;
                        break;
                    case PieceType.Queen:
                        sprite = _colorThemeSO.lightQueen;
                        break;
                    case PieceType.King:
                        sprite = _colorThemeSO.lightKing;
                        break;
                }
            }
            else
            {
                switch (type)
                {
                    case PieceType.Pawn:
                        sprite = _colorThemeSO.darkPawn;
                        break;
                    case PieceType.Rook:
                        sprite = _colorThemeSO.darkRook;
                        break;
                    case PieceType.Knight:
                        sprite = _colorThemeSO.darkKnight;
                        break;
                    case PieceType.Bishop:
                        sprite = _colorThemeSO.darkBishop;
                        break;
                    case PieceType.Queen:
                        sprite = _colorThemeSO.darkQueen;
                        break;
                    case PieceType.King:
                        sprite = _colorThemeSO.darkKing;
                        break;
                }
            }

            return sprite;
        }
    }

    public class GameManager : MonoBehaviour
    {
        [System.Serializable]
        private class PieceDatabase
        {
            public ChessPieceModel model;
            public ChessPieceView view;
        }

        [SerializeField] private BoardView _boardView;
        [SerializeField] private PieceDatabase[] _playerPieceViews;
        [SerializeField] private PieceDatabase[] _aiPieceViews;
        [SerializeField] private ColorThemeSO _colorThemeSO;
        [SerializeField] private PlayerPieceSelectionHandler _playerPieceSelectionHandler;
        [SerializeField] private GameObject _moveHighlighter;
        [SerializeField] private GameObject _captureHighlighter;
        private readonly int _piecesCount = 16;
        private BoardController _boardController;
        private readonly IPieceSpriteProvider _pieceSpriteDataBase;

        private readonly Dictionary<ChessPieceController, Vector3> _playerPieces = new();
        private readonly Dictionary<ChessPieceController, Vector3> _aiPieces = new();

        private void Awake()
        {
            BoardModel model = new()
            {
                lightTileColor = _colorThemeSO.lightTileColor,
                darkTileColor = _colorThemeSO.darkTileColor
            };

            _boardController = new BoardController(model, _boardView);

            SetupPieces();
            _playerPieceSelectionHandler.SetupSelectables(_playerPieces);
        }

        private void SetupPieces()
        {
            MoveStrategyFactory moveStrategyFactory = new();
            IPieceSpriteProvider pieceSpriteDataBase = new PieceSpriteDataBase(_colorThemeSO);

            for (int i = 0; i < _piecesCount; i++)
            {
                _playerPieces.Add(new ChessPieceController(
                    _playerPieceViews[i].view,
                    _playerPieceViews[i].model,
                    new PlayerPieceController(_moveHighlighter, _captureHighlighter),
                    moveStrategyFactory,
                    pieceSpriteDataBase), _playerPieceViews[i].view.transform.position);

                _aiPieces.Add(new ChessPieceController(
                    _aiPieceViews[i].view,
                    _aiPieceViews[i].model,
                    new AIPieceController(),
                    moveStrategyFactory,
                    pieceSpriteDataBase), _aiPieceViews[i].view.transform.position);
            }
        }
    }
}