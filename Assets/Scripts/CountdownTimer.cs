using Chess2D.UI;
using UnityEngine;
using UnityEngine.Events;

namespace Chess2D
{
    public class CountdownTimer : MonoBehaviour
    {
        [SerializeField] private float _totalTime = 60f;
        [SerializeField] private UITimer _uiTimer;

        public UnityEvent OnTimerEnd; // Event triggered when timer hits zero

        private bool _isPaused = true;
        private float _remainingTime;

        private void Start()
        {
            _remainingTime = _totalTime;
            UpdateUITimer();
        }

        private void Update()
        {
            if (_isPaused) return;

            _remainingTime -= Time.deltaTime;
            if (_remainingTime <= 0f)
            {
                _remainingTime = 0f;
                _isPaused = true;
                OnTimerEnd?.Invoke();
            }

            UpdateUITimer();
        }

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