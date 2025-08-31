using Chess2D.Events;
using Chess2D.Piece;
using Chess2D.ScriptableObjects;
using Chess2D.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Chess2D
{
    [DefaultExecutionOrder(-1)]
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        [SerializeField] private UIManager _uiManager;
        [SerializeField] private HighlightSpritesDatabase _spritesDatabase;
        [SerializeField] private ChessPieceRendererDatabase _pieceDatabase;
        [SerializeField] private GameEvents _gameEvents;
        [SerializeField] private Transform _playerPieceContainer;
        [SerializeField] private Transform _aiPieceContainer;

        public Board.ChessBoard Board { get; private set; }

        private void Awake() => Instance = this;

        private void OnEnable()
        {
            _gameEvents.WinEvent.OnEventRaised += WinGame;
            _gameEvents.LoadSceneEvent.OnEventRaised += LoadScene;
            _gameEvents.QuitGameEvent.OnEventRaised += QuitGame;
        }

        private void OnDisable()
        {
            _gameEvents.WinEvent.OnEventRaised -= WinGame;
            _gameEvents.LoadSceneEvent.OnEventRaised -= LoadScene;
            _gameEvents.QuitGameEvent.OnEventRaised -= QuitGame;
        }

        private void Start()
        {
            bool isPlayerDark = PlayerPrefs.GetInt("PlayerColor", 0) == 0;

            _uiManager.InitUI(isPlayerDark);

            Board = new Board.ChessBoard(_gameEvents.InitializePieceEvent);

            MoveStrategyFactory moveStrategyFactory = new(Board);

            if (isPlayerDark)
            {
                Board.InitializeBoard(
                    moveStrategyFactory,
                    new PieceFactory<PieceRenderer>(_pieceDatabase.BlackPieceSet),
                    new PieceFactory<PieceRenderer>(_pieceDatabase.WhitePieceSet),
                    _playerPieceContainer,
                    _aiPieceContainer
                );
            }
            else
            {
                Board.InitializeBoard(
                    moveStrategyFactory,
                    new PieceFactory<PieceRenderer>(_pieceDatabase.WhitePieceSet),
                    new PieceFactory<PieceRenderer>(_pieceDatabase.BlackPieceSet),
                    _playerPieceContainer,
                    _aiPieceContainer
                );
            }

            if (isPlayerDark)
                _gameEvents.SwitchTurnToAIEvent.RaiseEvent(null);
            else
                _gameEvents.SwitchTurnToPlayerEvent.RaiseEvent(null);

        }

        private void WinGame(Empty empty = null) => _uiManager.ShowWinStats();
        private void QuitGame(Empty e = null) => Application.Quit();
        private void LoadScene(int sceneIndex) => SceneManager.LoadSceneAsync(sceneIndex);
    }
}