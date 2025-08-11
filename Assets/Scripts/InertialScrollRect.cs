using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Chess2D.UI
{
    public class InertialScrollRect : ScrollRect
    {
        private bool _draggingByTouch;
        private bool _isDragging;
        private Vector2 _prevPosition = Vector2.zero;

        protected override void OnDestroy()
        {
            base.OnDestroy();

            StopAllCoroutines();
        }

        public override void OnBeginDrag(PointerEventData eventData)
        {
            base.OnBeginDrag(eventData);

            _draggingByTouch = eventData.pointerId != -1;
            _isDragging = true;
        }

        public override void OnEndDrag(PointerEventData eventData)
        {
            base.OnEndDrag(eventData);

            _isDragging = false;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            _isDragging = false;
        }

        public override void Rebuild(CanvasUpdate executing)
        {
            base.Rebuild(executing);

            if (executing == CanvasUpdate.PostLayout)
            {
                _prevPosition = content.anchoredPosition;
            }
        }

        protected override void LateUpdate()
        {
            base.LateUpdate();

            float deltaTime = Time.unscaledDeltaTime;
            if (deltaTime > 0.0f && _isDragging && inertia)
            {
                Vector3 newVelocity = (content.anchoredPosition - _prevPosition) / deltaTime;
                velocity = _draggingByTouch ? newVelocity : Vector3.Lerp(velocity, newVelocity, deltaTime * 10f);
            }

            _prevPosition = content.anchoredPosition;
        }
    }
}