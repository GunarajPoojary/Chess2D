using Chess2D.Events;
using UnityEngine;

namespace Chess2D
{
    public class TurnManager : MonoBehaviour
    {
        [SerializeField] private GameEvents _gameEvents;

        private void OnEnable()
        {
            _gameEvents.AIMadeMoveEvent.OnEventRaised += SetToPlayerTurn;
            _gameEvents.PlayerMadeMoveEvent.OnEventRaised += SetToAITurn;
        }

        private void OnDisable()
        {
            _gameEvents.AIMadeMoveEvent.OnEventRaised -= SetToPlayerTurn;
            _gameEvents.PlayerMadeMoveEvent.OnEventRaised -= SetToAITurn;
        }

        private void SetToPlayerTurn(Vector2Int e) => SetToPlayerTurn();
        private void SetToAITurn(Vector2Int e) => SetToAITurn();
        
        private void SetToPlayerTurn()
        {
            _gameEvents.SwitchTurnToPlayerEvent.RaiseEvent(null);
        }

        private void SetToAITurn()
        {
            _gameEvents.SwitchTurnToAIEvent.RaiseEvent(null);
        }
    }
}