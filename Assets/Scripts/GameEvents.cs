using UnityEngine;
using UnityEngine.Events;

namespace Chess2D.Events
{
    [CreateAssetMenu(menuName = "Custom/GameEvents")]
    public class GameEvents : ScriptableObject
    {
        public EventChannel<Empty> LoseGameEvent = new();
        public EventChannel<Empty> GameCompleteEvent = new();
        public EventChannel<Empty> RestartGameEvent = new();
        public EventChannel<Empty> ExitGameEvent = new();
        public EventChannel<bool> GameActiveEvent = new();
        public EventChannel<Vector2Int> OngetLegalMoveEvent = new();
        public EventChannel<Piece.PieceRenderer> PieceCaptureEvent = new();
        public EventChannel<AudioClip> PlayOneShotAudioEvent = new();
        public EventChannel<Empty> PlayerMadeMove = new();
        public EventChannel<Empty> AIMadeMove = new();
        public EventChannel<Vector2Int> UnHighlightEvent = new();
        public EventChannel<(Vector2Int, Highlight.HighlightType)> HighlightEvent = new();
        public EventChannel<Empty> KingCapturedEvent = new();
        public EventChannel<Empty> WinEvent = new();
    }

    public class Empty { }

    public class EventChannel<T>
    {
        public event UnityAction<T> OnEventRaised;
        public void RaiseEvent(T value) => OnEventRaised?.Invoke(value);
    }
}