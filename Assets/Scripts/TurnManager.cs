using TMPro;
using UnityEngine;

namespace Chess2D
{
    public class TurnManager : MonoBehaviour
    {
        public static TurnManager Instance { get; private set; }

        [SerializeField] private TMP_Text _turnText; // Assign in Inspector
        private bool _isPlayerTurn = true;

        private void Awake()
        {
            Instance = this;
            UpdateTurnUI();
        }

        public void SwitchTurn()
        {
            _isPlayerTurn = !_isPlayerTurn;
            UpdateTurnUI();
        }

        private void UpdateTurnUI()
        {
            _turnText.text = _isPlayerTurn ? "Your Turn" : "AI's Turn";
            _turnText.color = _isPlayerTurn ? Color.green : Color.red;
        }

        public bool IsPlayerTurn() => _isPlayerTurn;
    }
}