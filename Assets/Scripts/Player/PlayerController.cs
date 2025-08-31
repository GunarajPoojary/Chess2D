using System.Collections.Generic;
using Chess2D.Board.Utilities;
using Chess2D.Events;
using Chess2D.Highlight;
using Chess2D.Piece;
using UnityEngine;

namespace Chess2D.Player
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private GameEvents _gameEvents;
        [SerializeField] private AudioConfig _audioConfig;
        
        private Camera _mainCamera;
        private ChessPiece _selectedPiece;
        private readonly List<Vector2Int> _selectedPieceLegalMoves = new();
        private Board.IBoard _board;
        private bool _inputEnabled = false;

        private void Awake() => _mainCamera = Camera.main;

        private void Start() => _board = GameManager.Instance.Board;

        private void OnEnable()
        {
            _gameEvents.SwitchTurnToPlayerEvent.OnEventRaised += EnablePlayerTurn;
            _gameEvents.SwitchTurnToAIEvent.OnEventRaised += DisablePlayerTurn;
        }

        private void OnDisable()
        {
            _gameEvents.SwitchTurnToPlayerEvent.OnEventRaised -= EnablePlayerTurn;
            _gameEvents.SwitchTurnToAIEvent.OnEventRaised -= DisablePlayerTurn;
        }

        private void Update()
        {
            if (!_inputEnabled)
                return;

            HandleMouseInput();
        }

        private void HandleMouseInput()
        {
            Vector3 inputPos = Vector3.zero;
            bool inputDown = false;

#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBGL
            inputDown = Input.GetMouseButtonDown(0);
            inputPos = Input.mousePosition;
#else
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                inputPos = touch.position;
                inputDown = touch.phase == TouchPhase.Began;
            }
#endif
            if (inputDown)
            {
                Vector2 mouseWorldPos = _mainCamera.ScreenToWorldPoint(inputPos);
                Vector2Int input = Vector2Int.FloorToInt(mouseWorldPos);

                if (!BoardUtilities.IsWithinBoard(input)) return;

                if (_board.TryGetPlayerPieceAt(input, out ChessPiece piece))
                {
                    // If new piece selected, unhighlight previous
                    if (_selectedPiece != null && _selectedPiece != piece)
                    {
                        _gameEvents.UnHighlightEvent.RaiseEvent(_selectedPiece.BoardPosition);
                        ClearValidMoves();
                    }

                    _selectedPiece = piece;
                    _gameEvents.HighlightEvent.RaiseEvent((input, HighlightType.Select));
                    _selectedPiece.MoveStrategy.CalculateLegalMoves(true, input, OnGetLegalMove);
                }
                else
                {
                    if (_selectedPiece != null && _selectedPieceLegalMoves.Contains(input))
                    {
                        ClearValidMoves();

                        Vector2Int previousPosition = _selectedPiece.BoardPosition;
                        _selectedPiece.SetPiecePosition(input);

                        if (_board.TryCapturePieceAt(input, true, out ChessPiece capturedPiece))
                        {
                            if (capturedPiece != null)
                            {
                                _gameEvents.PieceCaptureEvent.RaiseEvent(capturedPiece);
                                _gameEvents.PlayOneShotAudioEvent.RaiseEvent(_audioConfig.CaptureAudio);
                            }

                            if (capturedPiece.PieceType == PieceType.King)
                                _gameEvents.WinEvent.RaiseEvent(null);
                        }
                        else
                        {
                            _gameEvents.PlayOneShotAudioEvent.RaiseEvent(_audioConfig.MoveSelfAudio);
                        }

                        _board.SetOccupiedPieceAt(null, previousPosition);
                        _board.SetOccupiedPieceAt(_selectedPiece, input);

                        if (_selectedPiece.MoveStrategy is IMoveStrategySwitch postMoveAware)
                            postMoveAware.SwitchStrategy();

                        _gameEvents.PlayerMadeMoveEvent.RaiseEvent(input);

                        DisablePlayerTurn(null); // Lock input after move
                    }
                }
            }
        }

        private void EnablePlayerTurn(Empty e) => _inputEnabled = true;
        private void DisablePlayerTurn(Empty e) => _inputEnabled = false;

        private void ClearValidMoves()
        {
            foreach (Vector2Int position in _selectedPieceLegalMoves)
            {
                _gameEvents.UnHighlightEvent.RaiseEvent(position);
            }

            if (_selectedPiece != null)
                _gameEvents.UnHighlightEvent.RaiseEvent(_selectedPiece.BoardPosition);

            _selectedPieceLegalMoves.Clear();
        }

        private void OnGetLegalMove(Vector2Int position)
        {
            _gameEvents.HighlightEvent.RaiseEvent((
                new Vector2Int(position.x, position.y),
                _board.ContainsOpponentPieceAt(position, true)
                    ? HighlightType.Capture
                    : HighlightType.EmptyTile));

            _selectedPieceLegalMoves.Add(position);
        }
    }
}