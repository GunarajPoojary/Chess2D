using TMPro;
using UnityEngine;
using System.Collections.Generic;
using Chess2D.Events;

namespace Chess2D.UI
{
    public class UIMoveHistory : MonoBehaviour
    {
        [SerializeField] private TMP_Text _moveTextPrefab;
        [SerializeField] private RectTransform _scrollCeontent;
        [SerializeField] private GameEvents _gameEvents;

        private readonly List<TMP_Text> _moveLines = new();
        private string _playerMove = "";
        private string _aiMove = "";
        private int _currentMoveCount;

        private void OnEnable()
        {
            _gameEvents.AIMadeMoveEvent.OnEventRaised += HandleAIMove;
            _gameEvents.PlayerMadeMoveEvent.OnEventRaised += HandlePlayerMove;
        }

        private void OnDisable()
        {
            _gameEvents.AIMadeMoveEvent.OnEventRaised -= HandleAIMove;
            _gameEvents.PlayerMadeMoveEvent.OnEventRaised -= HandlePlayerMove;
        }

        private void HandlePlayerMove(Vector2Int move)
        {
            // Player moves always start a new turn
            _currentMoveCount++;
            AddMove(_currentMoveCount, move, true);
        }

        private void HandleAIMove(Vector2Int move)
        {
            // AI moves update the current turn's entry
            AddMove(_currentMoveCount, move, false);
        }

        private void AddMove(int moveCount, Vector2Int position, bool isPlayer)
        {
            // If this is the same turn as before, update the last entry
            if (_moveLines.Count > 0 && _currentMoveCount == moveCount && !isPlayer)
            {
                SetMove(moveCount, position, isPlayer);
                return;
            }

            // New turn, create a new entry
            gameObject.SetActive(true);

            TMP_Text newLine = Instantiate(_moveTextPrefab, _scrollCeontent);
            _moveLines.Add(newLine);

            // Reset moves for new turn
            _playerMove = "";
            _aiMove = "";

            SetMove(moveCount, position, isPlayer);
        }

        private void SetMove(int moveCount, Vector2Int position, bool isPlayer)
        {
            // Convert board coordinates to chess notation
            // Assuming 0 = A, 0 = 1
            string square = ((char)(position.x + 65)).ToString() + (position.y + 1);

            if (isPlayer)
                _playerMove = square;
            else
                _aiMove = square;

            // Always update the last line in the list
            TMP_Text lastLine = _moveLines[_moveLines.Count - 1];
            lastLine.text = $"{moveCount}. {_playerMove} {_aiMove}";
        }
    }
}