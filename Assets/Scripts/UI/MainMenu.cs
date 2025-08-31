using UnityEngine;
using UnityEngine.UI;
using Chess2D.Audio;
using UnityEngine.SceneManagement;
using Chess2D.Events;
using System;

namespace Chess2D.UI
{
    public class MainMenu : MonoBehaviour
    {
        [Header("Main Menu UI")]
        [SerializeField] private GameEvents _gameEvents;
        [SerializeField] private Button _playButton;
        [SerializeField] private Slider _difficultySlider;
        [SerializeField] private Toggle _darkColorToggle;
        [SerializeField] private Button _exitButton;

        [SerializeField] private Toggle _musicToggle;
        [SerializeField] private Toggle _sfxToggle;
        [SerializeField] private Slider _musicVolumeSlider;
        [SerializeField] private Slider _sfxVolumeSlider;

        [Header("Audio")]
        [SerializeField] private AudioManager _audioManager;

        private void OnEnable()
        {
            _playButton.onClick.AddListener(StartNewGame);
            _difficultySlider.onValueChanged.AddListener(SetDifficulty);
            _darkColorToggle.onValueChanged.AddListener(SetTeamColor);

            _musicToggle.onValueChanged.AddListener(ToggleMusic);
            _sfxToggle.onValueChanged.AddListener(ToggleSFX);

            _musicVolumeSlider.onValueChanged.AddListener(SetMusicVolume);
            _sfxVolumeSlider.onValueChanged.AddListener(SetSFXVolume);
        }

        private void OnDisable()
        {
            _playButton.onClick.RemoveListener(StartNewGame);
            _difficultySlider.onValueChanged.RemoveListener(SetDifficulty);
            _darkColorToggle.onValueChanged.RemoveListener(SetTeamColor);
        }

        private void SetTeamColor(bool isOn)
        {
            PlayerPrefs.SetInt("PlayerColor", isOn ? 0 : 1);
            PlayerPrefs.Save();
        }

        private void Start()
        {
            SetTeamColor(_darkColorToggle.isOn);
            SetDifficulty(_difficultySlider.value);
        }

        private void SetDifficulty(float difficulty)
        {
            PlayerPrefs.SetInt("Difficulty", (int)difficulty);
            PlayerPrefs.Save();
        }

        private void StartNewGame() => SceneManager.LoadSceneAsync(1);

        private void SetSFXVolume(float volume)
        {
            _sfxToggle.isOn = _sfxVolumeSlider.value > 0f;

            _gameEvents.SFXVolumeChangedEvent.RaiseEvent(volume);
        }

        private void SetMusicVolume(float volume)
        {
            _musicToggle.isOn = _musicVolumeSlider.value > 0f;

            _gameEvents.MusicVolumeChangedEvent.RaiseEvent(volume);
        }

        private void ToggleSFX(bool toggle) => _gameEvents.ToggleSFXEvent.RaiseEvent(toggle);
        private void ToggleMusic(bool toggle) => _gameEvents.ToggleMusicEvent.RaiseEvent(toggle);
    }
}