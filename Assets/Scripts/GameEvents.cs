using Chess2D.Piece;
using UnityEngine;
using UnityEngine.Events;

namespace Chess2D.Events
{
    [CreateAssetMenu(menuName = "Custom/GameEvents")]
    public class GameEvents : ScriptableObject
    {
        public EventChannel<ChessPiece> PieceCaptureEvent = new();
        public EventChannel<Vector2Int> PlayerMadeMoveEvent = new();
        public EventChannel<Vector2Int> AIMadeMoveEvent = new();
        public EventChannel<bool> TimeEndEvent = new(); // if true player lost else AI lost
        public EventChannel<Vector2Int> UnHighlightEvent = new();
        public EventChannel<(Vector2Int, Highlight.HighlightType)> HighlightEvent = new();
        public EventChannel<Empty> WinEvent = new();
        public EventChannel<bool> SwitchTurnEvent = new();
        public EventChannel<Empty> SwitchTurnToPlayerEvent = new();
        public EventChannel<Empty> SwitchTurnToAIEvent = new();
        public EventChannel<ChessPiece> InitializePieceEvent = new();
        public EventChannel<AudioClip> PlayOneShotAudioEvent = new();

        public EventChannel<bool> ToggleSFXEvent = new();
        public EventChannel<float> SFXVolumeChangedEvent = new();
        public EventChannel<float> MusicVolumeChangedEvent = new();
        public EventChannel<bool> ToggleMusicEvent = new();
    }

    public class Empty { }

    public class EventChannel<T>
    {
        public event UnityAction<T> OnEventRaised;
        public void RaiseEvent(T value) => OnEventRaised?.Invoke(value);
    }
}