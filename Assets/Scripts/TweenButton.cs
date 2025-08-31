using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening; 

namespace Chess2D.UI
{
    [RequireComponent(typeof(Image))]
    public class TweenButton : Button
    {
        [SerializeField] private ButtonTweenConfig _buttonTweenConfig;
        private Vector3 _originalScale;

        protected override void Awake()
        {
            base.Awake();
            _originalScale = transform.localScale;
            image.color = _buttonTweenConfig.NormalColor;
        }

        public override void OnPointerClick(PointerEventData eventData)
        {
            base.OnPointerClick(eventData);
            image.color = _buttonTweenConfig.PressedColor;
            _buttonTweenConfig.PlayPressedSound();
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);

            // Cancel previous tweens before starting a new one
            transform.DOKill();

            transform.DOScale(_buttonTweenConfig.PressedScale, _buttonTweenConfig.TweenDuration)
                     .SetEase(_buttonTweenConfig.PressedEase);
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);

            transform.DOKill();

            transform.DOScale(_originalScale, _buttonTweenConfig.TweenDuration)
                     .SetEase(_buttonTweenConfig.PressedEase);
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            base.OnPointerEnter(eventData);
            image.color = _buttonTweenConfig.HoverColor;
            _buttonTweenConfig.PlayHoverSound();
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            base.OnPointerExit(eventData);
            image.color = _buttonTweenConfig.NormalColor;
        }
    }
}