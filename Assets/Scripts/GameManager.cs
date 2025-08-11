using Chess2D.Events;
using Chess2D.Piece;
using Chess2D.ScriptableObjects;
using Chess2D.UI;
using UnityEngine;

namespace Chess2D
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        [SerializeField] private UIManager _uiManager;
        [SerializeField] private HighlightSpritesDatabase _spritesDatabase;
        [SerializeField] private PieceRendererDatabase _pieceDatabase;
        [SerializeField] private GameEvents _gameEvents;
        [SerializeField] private PieceRenderer _chessPiecePrefab;
        [SerializeField] private bool _isPlayerDark = false;
        [SerializeField] private Transform _playerPieceContainer;
        [SerializeField] private Transform _aiPieceContainer;
        [SerializeField] private GameObject _tilePrefab;
        [SerializeField] private Transform _boardTransform;
        [SerializeField] private UIMoveHistory _uiMoveHistory;
        private PieceFactory _pieceFactory;

        public Board.ChessBoard Board { get; private set; }

        public UIMoveHistory MoveHistory => _uiMoveHistory;
        private int _moveCount = 0;

        private void Awake()
        {
            Instance = this;

            Board = new Board.ChessBoard(
                _tilePrefab,
                Color.grey,
                Color.white,
                _boardTransform);

            MoveStrategyFactory moveStrategyFactory = new(Board);
            _pieceFactory = new PieceFactory(_pieceDatabase, moveStrategyFactory);

            Board.InitializePieces(_isPlayerDark, _pieceFactory, _playerPieceContainer, _aiPieceContainer);
        }

        private void OnEnable()
        {
            _gameEvents.WinEvent.OnEventRaised += WinGame;
        }

        private void OnDisable()
        {
            _gameEvents.WinEvent.OnEventRaised -= WinGame;
        }

        private void WinGame(Empty empty = null)
        {
            _uiManager.ShowWinStats();
        }

        public void RecordMove(Vector2Int position, bool isPlayer)
        {
            if (isPlayer)
                _moveCount++;

            _uiMoveHistory.AddMove(_moveCount, position.y + 1, position.x + 1, isPlayer);
        }
    }
}