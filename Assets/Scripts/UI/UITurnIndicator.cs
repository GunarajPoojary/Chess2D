using Chess2D.Events;
using TMPro;
using UnityEngine;

namespace Chess2D.UI
{
    public class UITurnIndicator : MonoBehaviour
    {
        [SerializeField] private TMP_Text _playerTurnText;
        [SerializeField] private TMP_Text _aiTurnText;
        [SerializeField] private GameEvents _gameEvents;

        private void OnEnable()
        {
            _gameEvents.SwitchTurnToPlayerEvent.OnEventRaised += SetPlayerTurnText;
            _gameEvents.SwitchTurnToAIEvent.OnEventRaised += SetAITurnText;
        }

        private void OnDisable()
        {
            _gameEvents.SwitchTurnToPlayerEvent.OnEventRaised -= SetPlayerTurnText;
            _gameEvents.SwitchTurnToAIEvent.OnEventRaised -= SetAITurnText;
        }

        private void SetPlayerTurnText(Empty e = null)
        {
            _playerTurnText.text = "Your turn";
            _aiTurnText.text = "";
        }

        private void SetAITurnText(Empty e = null)
        {
            _playerTurnText.text = "";
            _aiTurnText.text = "AI thinking";
        }
    }
}