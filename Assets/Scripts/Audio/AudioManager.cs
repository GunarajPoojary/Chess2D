using Chess2D.Events;
using UnityEngine;

namespace Chess2D.Audio
{
    public class AudioManager : MonoBehaviour
    {
        [SerializeField] private AudioSource _musicAudioSource;
        [SerializeField] private AudioSource _sfxAudioSource;
        [SerializeField] private GameEvents _gameEvents;

        private float _musicVolume = 0.75f;
        private float _sfxVolume = 0.75f;

        private void Awake()
        {
            bool isMusicOn = PlayerPrefs.GetInt("MusicEnabled", 1) == 1;
            bool isSFXOn = PlayerPrefs.GetInt("SFXEnabled", 1) == 1;

            _musicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.75f);
            _sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 0.75f);

            ToggleMusic(isMusicOn);
            ToggleSFX(isSFXOn);
        }

        private void OnEnable()
        {
            _gameEvents.PlayOneShotAudioEvent.OnEventRaised += PlayOneShotAudio;
            _gameEvents.ToggleMusicEvent.OnEventRaised += ToggleMusic;
            _gameEvents.ToggleSFXEvent.OnEventRaised += ToggleSFX;
        }

        private void OnDisable()
        {
            _gameEvents.PlayOneShotAudioEvent.OnEventRaised -= PlayOneShotAudio;
            _gameEvents.ToggleMusicEvent.OnEventRaised -= ToggleMusic;
            _gameEvents.ToggleSFXEvent.OnEventRaised -= ToggleSFX;
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
            _musicVolume = volume;
            if (IsMusicOn())
                _musicAudioSource.volume = _musicVolume;

            PlayerPrefs.SetFloat("MusicVolume", _musicVolume);
            PlayerPrefs.Save();
        }

        public void SetSFXVolume(float volume)
        {
            _sfxVolume = volume;
            if (IsSFXOn())
                _sfxAudioSource.volume = _sfxVolume;

            PlayerPrefs.SetFloat("SFXVolume", _sfxVolume);
            PlayerPrefs.Save();
        }

        public float GetMusicVolume() => _musicVolume;
        public float GetSFXVolume() => _sfxVolume;
        public bool IsMusicOn() => PlayerPrefs.GetInt("MusicEnabled", 1) == 1;
        public bool IsSFXOn() => PlayerPrefs.GetInt("SFXEnabled", 1) == 1;

        private void PlayOneShotAudio(AudioClip clip) =>
            _sfxAudioSource.PlayOneShot(clip);
    }
}