using System;
using Chess2D.Events;
using UnityEngine;

namespace Chess2D.Audio
{
    public class AudioManager : MonoBehaviour
    {
        [SerializeField] private GameEvents _gameEvents;
        [SerializeField] private AudioSource _sfxAudioSource;
        [SerializeField] private AudioSource _musicAudioSource;

        private void OnEnable()
        {
            _gameEvents.PlayOneShotAudio.OnEventRaised += PlaySFX;
        }

        private void OnDisable()
        {
            _gameEvents.PlayOneShotAudio.OnEventRaised -= PlaySFX;
        }

        public void ToggleMusic(bool isOn)
        {
            if (isOn)
                _musicAudioSource.Play();
            else
                _musicAudioSource.Stop();
        }

        public void ToggleSFX(bool isOn)
        {
            if (isOn)
                _sfxAudioSource.Play();
            else
                _sfxAudioSource.Stop();
        }

        private void PlaySFX(AudioClip sfx) => _sfxAudioSource.PlayOneShot(sfx);
    }
}