using Chess2D.Events;
using DG.Tweening;
using UnityEngine;

namespace Chess2D
{
    [CreateAssetMenu(menuName = "Custom/Custom Button Tween")]
    public class ButtonTweenConfig : ScriptableObject
    {
        [SerializeField] private GameEvents _gameEvents;
        [SerializeField] private AudioClip _hoverEnterSound;
        [SerializeField] private AudioClip _pressSound;

        [field: SerializeField] public Color NormalColor { get; private set; } = Color.white;
        [field: SerializeField] public Color PressedColor { get; private set; } = new(0.8f, 0.8f, 0.8f);
        [field: SerializeField] public Color HoverColor { get; private set; } = new(0.9f, 0.9f, 0.9f);

        [field: SerializeField] public float PressedScale { get; private set; } = 0.9f;
        [field: SerializeField] public Ease PressedEase { get; private set; } = Ease.Linear;
        [field: SerializeField] public float TweenDuration { get; private set; } = 0.15f;

        public void PlayHoverSound()
        {
            _gameEvents.PlayOneShotAudioEvent.RaiseEvent(_hoverEnterSound);
        }

        public void PlayPressedSound()
        {
            _gameEvents.PlayOneShotAudioEvent.RaiseEvent(_pressSound);
        }
    }
}