using Chess2D.Events;
using UnityEngine;

namespace Chess2D.Audio
{
    public class AudioManager : MonoBehaviour
    {
        [SerializeField] private AudioSource _musicAudioSource;
        [SerializeField] private AudioSource _sfxAudioSource;
        [SerializeField] private AudioClip _sfxVolumeChangeFeedbackClip;
        [SerializeField] private GameEvents _gameEvents;

        private float _musicVolume = 0.75f;
        private float _sfxVolume = 0.75f;

        private const float _feedbackDelay = 0.1f;

        private void Awake()
        {
            bool isMusicOn = PlayerPrefs.GetInt("MusicEnabled", 1) == 1;
            bool isSFXOn = PlayerPrefs.GetInt("SFXEnabled", 1) == 1;

            _musicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.75f);
            _sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 0.75f);

            SetMusicVolume(_musicVolume);
            SetSFXVolume(_sfxVolume);

            ToggleMusic(isMusicOn);
            ToggleSFX(isSFXOn);
        }

        private void OnEnable()
        {
            _gameEvents.PlayOneShotAudioEvent.OnEventRaised += PlayOneShotAudio;
            _gameEvents.ToggleMusicEvent.OnEventRaised += ToggleMusic;
            _gameEvents.ToggleSFXEvent.OnEventRaised += ToggleSFX;
            _gameEvents.SFXVolumeChangedEvent.OnEventRaised += SetSFXVolume;
            _gameEvents.MusicVolumeChangedEvent.OnEventRaised += SetMusicVolume;
        }

        private void OnDisable()
        {
            _gameEvents.PlayOneShotAudioEvent.OnEventRaised -= PlayOneShotAudio;
            _gameEvents.ToggleMusicEvent.OnEventRaised -= ToggleMusic;
            _gameEvents.ToggleSFXEvent.OnEventRaised -= ToggleSFX;
            _gameEvents.SFXVolumeChangedEvent.OnEventRaised -= SetSFXVolume;
            _gameEvents.MusicVolumeChangedEvent.OnEventRaised -= SetMusicVolume;
        }

        public void ToggleMusic(bool isOn)
        {
            _musicAudioSource.volume = isOn ? _musicVolume : 0f;
            PlayerPrefs.SetInt("MusicEnabled", isOn ? 1 : 0);
            PlayerPrefs.Save();
        }

        public void ToggleSFX(bool isOn)
        {
            _sfxAudioSource.volume = isOn ? _sfxVolume : 0f;
            PlayerPrefs.SetInt("SFXEnabled", isOn ? 1 : 0);
            PlayerPrefs.Save();
        }

        public void SetMusicVolume(float volume)
        {
            _musicVolume = Mathf.Clamp01(volume);
            if (IsMusicOn())
                _musicAudioSource.volume = _musicVolume;

            PlayerPrefs.SetFloat("MusicVolume", _musicVolume);
            PlayerPrefs.Save();
        }

        public void SetSFXVolume(float volume)
        {
            _sfxVolume = Mathf.Clamp01(volume);
            if (IsSFXOn())
            {
                _sfxAudioSource.volume = _sfxVolume;
                _sfxAudioSource.clip = _sfxVolumeChangeFeedbackClip;
                _sfxAudioSource.PlayDelayed(_feedbackDelay);
            }

            PlayerPrefs.SetFloat("SFXVolume", _sfxVolume);
            PlayerPrefs.Save();
        }

        public bool IsMusicOn() => PlayerPrefs.GetInt("MusicEnabled", 1) == 1;
        public bool IsSFXOn() => PlayerPrefs.GetInt("SFXEnabled", 1) == 1;

        private void PlayOneShotAudio(AudioClip clip) =>
            _sfxAudioSource.PlayOneShot(clip);
    }
}