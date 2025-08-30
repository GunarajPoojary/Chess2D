using UnityEngine;
using UnityEngine.UI;
using Chess2D.Audio;
using UnityEngine.SceneManagement;

namespace Chess2D.UI
{
    public class MainMenu : MonoBehaviour
    {
        [Header("Main Menu UI")]
        [SerializeField] private Button _newGameButton;
        [SerializeField] private Slider _difficultySlider;
        [SerializeField] private Toggle _darkColorToggle;
        [SerializeField] private Button _exitButton;

        [SerializeField] private Toggle _musicToggle;
        [SerializeField] private Toggle _sfxToggle;

        [Header("Audio")]
        [SerializeField] private AudioManager _audioManager;

        private void OnEnable()
        {
            _newGameButton.onClick.AddListener(StartNewGame);
            _difficultySlider.onValueChanged.AddListener(SetDifficulty);
            _darkColorToggle.onValueChanged.AddListener(SetTeamColor);
        }

        private void SetTeamColor(bool isOn)
        {
            PlayerPrefs.SetInt("PlayerColor", isOn ? 0 : 1);
            PlayerPrefs.Save();
        }

        private void OnDisable()
        {
            _newGameButton.onClick.RemoveListener(StartNewGame);
            _difficultySlider.onValueChanged.RemoveListener(SetDifficulty);
            _darkColorToggle.onValueChanged.RemoveListener(SetTeamColor);
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

        private void StartNewGame()
        {
            SceneManager.LoadSceneAsync(1);
        }
    }
}