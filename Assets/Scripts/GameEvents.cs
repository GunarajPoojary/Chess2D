using UnityEngine;
using UnityEngine.Events;

namespace Chess2D.Events
{
     [CreateAssetMenu(menuName = "Custom/GameEvents")]
    public class GameEvents : ScriptableObject
    {
        public EventChannel BounceEvent = new();
        public EventChannel ScoreEvent = new();
        public EventChannel LoseGameEvent = new();
        public EventChannel GameCompleteEvent = new();
        public EventChannel RestartGameEvent = new();
        public EventChannel ExitGameEvent = new();
        public EventChannel<bool> GameActiveEvent = new();
        public EventChannel<AudioClip> PlayOneShotAudioEvent = new();
    }

    public class EventChannel<T>
    {
        public event UnityAction<T> OnEventRaised;
        public void RaiseEvent(T value) => OnEventRaised?.Invoke(value);
    }
    
    public class EventChannel
    {
        public event UnityAction OnEventRaised;
        public void RaiseEvent() => OnEventRaised?.Invoke();
    }
}