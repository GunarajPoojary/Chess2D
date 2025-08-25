using Chess2D.Events;
using TMPro;
using UnityEngine;

namespace Chess2D
{
    public class TurnManager : MonoBehaviour
    {
        [SerializeField] private TMP_Text _turnText;
        [SerializeField] private GameEvents _gameEvents;
        private bool _isPlayerTurn = true;

        private void OnEnable()
        {
            _gameEvents.AIMadeMoveEvent.OnEventRaised += SetToPlayerTurn;
            _gameEvents.PlayerMadeMoveEvent.OnEventRaised += SetToAITurn;
            _gameEvents.TimeEndEvent.OnEventRaised += SwitchTurn;
        }

        private void OnDisable()
        {
            _gameEvents.AIMadeMoveEvent.OnEventRaised -= SetToPlayerTurn;
            _gameEvents.PlayerMadeMoveEvent.OnEventRaised -= SetToAITurn;
            _gameEvents.TimeEndEvent.OnEventRaised -= SwitchTurn;
        }

        /// <summary>
        /// Called by timer timeout to swap turns.
        /// </summary>
        private void SwitchTurn(Empty e)
        {
            if (_isPlayerTurn)
                SetToAITurn();
            else
                SetToPlayerTurn();
        }

        private void SetToPlayerTurn(Vector2Int e) => SetToPlayerTurn();
        private void SetToAITurn(Vector2Int e) => SetToAITurn();
        
        private void SetToPlayerTurn()
        {
            _isPlayerTurn = true;
            _gameEvents.SwitchTurnToPlayerEvent.RaiseEvent(null);
            _gameEvents.SwitchTurnEvent.RaiseEvent(true); // true = player's turn
            UpdateTurnUI();
        }

        private void SetToAITurn()
        {
            _isPlayerTurn = false;
            _gameEvents.SwitchTurnToAIEvent.RaiseEvent(null);
            _gameEvents.SwitchTurnEvent.RaiseEvent(false); // false = AI's turn
            UpdateTurnUI();
        }

        private void UpdateTurnUI()
        {
            if (_turnText != null)
                _turnText.text = _isPlayerTurn ? "Player Turn" : "AI Turn";
        }
    }
}