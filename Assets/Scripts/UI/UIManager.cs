using Chess2D.Events;
using Chess2D.Piece;
using Chess2D.ScriptableObjects;
using UnityEngine;
using UnityEngine.UI;

namespace Chess2D.UI
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private GameObject _winStats;
        [SerializeField] private GameEvents _gameEvents;

        [SerializeField] private Button _replayButton;
        [SerializeField] private Button _goToMainMenuButton;
        [SerializeField] private Button _quitGameButton;

        [SerializeField] private UIMoveHistory _uiMoveHistory;
        [SerializeField] private UIChessPieceDatabase _uiPieceDatabase;
        [SerializeField] private UICapturedPieces _capturedPiecesUI;
        [SerializeField] private Color _defaultColor;
        [SerializeField] private Color _highlightColor;
        [SerializeField] private Toggle _musicToggle;
        [SerializeField] private Toggle _sfxToggle;
        [SerializeField] private Slider _musicVolumeSlider;
        [SerializeField] private Slider _sfxVolumeSlider;

        private void OnEnable()
        {
            _musicToggle.onValueChanged.AddListener(ToggleMusic);
            _sfxToggle.onValueChanged.AddListener(ToggleSFX);

            _musicVolumeSlider.onValueChanged.AddListener(SetMusicVolume);
            _sfxVolumeSlider.onValueChanged.AddListener(SetSFXVolume);

            _replayButton.onClick.AddListener(ReplayGame);
            _goToMainMenuButton.onClick.AddListener(GoToMainMenu);
            _quitGameButton.onClick.AddListener(QuitGame);
        }

        private void OnDisable()
        {
            _musicToggle.onValueChanged.RemoveListener(ToggleMusic);
            _sfxToggle.onValueChanged.RemoveListener(ToggleSFX);

            _musicVolumeSlider.onValueChanged.RemoveListener(SetMusicVolume);
            _sfxVolumeSlider.onValueChanged.RemoveListener(SetSFXVolume);

            _replayButton.onClick.RemoveListener(ReplayGame);
            _goToMainMenuButton.onClick.RemoveListener(GoToMainMenu);
            _quitGameButton.onClick.RemoveListener(QuitGame);
        }

        public void InitUI(bool isPlayerDark)
        {
            PieceFactory<Image> playerPieceFactory;
            PieceFactory<Image> aiPieceFactory;

            if (isPlayerDark)
            {
                playerPieceFactory = new PieceFactory<Image>(_uiPieceDatabase.BlackPieceSet);
                aiPieceFactory = new PieceFactory<Image>(_uiPieceDatabase.WhitePieceSet);
            }
            else
            {
                playerPieceFactory = new PieceFactory<Image>(_uiPieceDatabase.WhitePieceSet);
                aiPieceFactory = new PieceFactory<Image>(_uiPieceDatabase.BlackPieceSet);
            }

            _capturedPiecesUI.Initialize(playerPieceFactory, aiPieceFactory);
        }

        public void ShowWinStats()
        {
            _winStats.SetActive(true);
        }

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

        private void QuitGame() => _gameEvents.QuitGameEvent.RaiseEvent(null);
        private void GoToMainMenu() => _gameEvents.LoadSceneEvent.RaiseEvent(0);
        private void ReplayGame() => _gameEvents.LoadSceneEvent.RaiseEvent(1);
    }
}