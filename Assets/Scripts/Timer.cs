using Chess2D.Events;
using Chess2D.UI;
using UnityEngine;

namespace Chess2D
{
    public class Timer : MonoBehaviour
    {
        [SerializeField] private float _totalTime = 60f;
        [SerializeField] private UITimer _uiTimer;
        [SerializeField] private GameEvents _gameEvents;

        private bool _isPaused = true;
        private float _remainingTime;

        private void OnEnable()
        {
            _gameEvents.AIMadeMoveEvent.OnEventRaised += ResetTimer;
            _gameEvents.PlayerMadeMoveEvent.OnEventRaised += ResetTimer;
            _gameEvents.SwitchTurnToAIEvent.OnEventRaised += ResetTimer;
            _gameEvents.SwitchTurnToPlayerEvent.OnEventRaised += ResetTimer;
        }

        private void OnDisable()
        {
            _gameEvents.AIMadeMoveEvent.OnEventRaised -= ResetTimer;
            _gameEvents.PlayerMadeMoveEvent.OnEventRaised -= ResetTimer;
            _gameEvents.SwitchTurnToAIEvent.OnEventRaised -= ResetTimer;
            _gameEvents.SwitchTurnToPlayerEvent.OnEventRaised -= ResetTimer;
        }

        private void Start()
        {
            RestartCountdown();
        }

        private void Update()
        {
            if (_isPaused) return;

            _remainingTime -= Time.deltaTime;
            if (_remainingTime <= 0f)
            {
                _remainingTime = 0f;
                _isPaused = true;
                _gameEvents.TimeEndEvent.RaiseEvent(null);
            }

            UpdateUITimer();
        }

        private void ResetTimer(Empty empty) => RestartCountdown();
        private void ResetTimer(Vector2Int empty) => RestartCountdown();

        public void RestartCountdown(float? newTime = null)
        {
            _remainingTime = newTime ?? _totalTime;
            _isPaused = false;
            UpdateUITimer();
        }

        public void PauseCountdown() => _isPaused = true;
        public void ResumeCountdown() => _isPaused = false;

        private void UpdateUITimer()
        {
            int min = Mathf.FloorToInt(_remainingTime / 60);
            int sec = Mathf.FloorToInt(_remainingTime % 60);
            _uiTimer.UpdateTimer(min, sec);
        }
    }
}