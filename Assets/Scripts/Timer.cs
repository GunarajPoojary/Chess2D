using Chess2D.Events;
using Chess2D.UI;
using UnityEngine;

namespace Chess2D
{
    public class Timer : MonoBehaviour
    {
        [SerializeField] private float _initialTime = 300f; // 5 minutes per side
        [SerializeField] private UITimer _playerUITimer;
        [SerializeField] private UITimer _aiUITimer;
        [SerializeField] private GameEvents _gameEvents;

        private float _playerTime;
        private float _aiTime;
        private bool _isPlayerTurnActive = true;
        private bool _isRunning = false;

        private void OnEnable()
        {
            _gameEvents.SwitchTurnToPlayerEvent.OnEventRaised += StartPlayerTimer;
            _gameEvents.SwitchTurnToAIEvent.OnEventRaised += StartAITimer;
        }

        private void OnDisable()
        {
            _gameEvents.SwitchTurnToPlayerEvent.OnEventRaised -= StartPlayerTimer;
            _gameEvents.SwitchTurnToAIEvent.OnEventRaised -= StartAITimer;
        }

        private void Start()
        {
            _playerTime = _initialTime;
            _aiTime = _initialTime;
            _isRunning = true;
            StartPlayerTimer(null); // Player starts first
        }

        private void Update()
        {
            if (!_isRunning) return;

            if (_isPlayerTurnActive)
            {
                _playerTime -= Time.deltaTime;
                if (_playerTime <= 0)
                {
                    _playerTime = 0;
                    _isRunning = false;
                    _gameEvents.TimeEndEvent.RaiseEvent(true); // Player loses on time
                }
            }
            else
            {
                _aiTime -= Time.deltaTime;
                if (_aiTime <= 0)
                {
                    _aiTime = 0;
                    _isRunning = false;
                    _gameEvents.TimeEndEvent.RaiseEvent(false); // AI loses on time
                }
            }

            UpdateUITimers();
        }

        private void StartPlayerTimer(Empty e)
        {
            _isPlayerTurnActive = true;
        }

        private void StartAITimer(Empty e)
        {
            _isPlayerTurnActive = false;
        }

        private void UpdateUITimers()
        {
            _playerUITimer.UpdateTimer(Mathf.FloorToInt(_playerTime / 60), Mathf.FloorToInt(_playerTime % 60));
            _aiUITimer.UpdateTimer(Mathf.FloorToInt(_aiTime / 60), Mathf.FloorToInt(_aiTime % 60));
        }

        public void PauseAll() => _isRunning = false;
        public void ResumeAll() => _isRunning = true;

        public void RestartAllTimers()
        {
            _playerTime = _initialTime;
            _aiTime = _initialTime;
            _isRunning = true;
            _isPlayerTurnActive = true;
            UpdateUITimers();
        }
    }
}