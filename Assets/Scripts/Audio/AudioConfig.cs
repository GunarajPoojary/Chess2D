using UnityEngine;

namespace Chess2D
{
    [CreateAssetMenu(fileName = "AudioConfig", menuName = "Custom/Audio/AudioConfig")]
    public class AudioConfig : ScriptableObject
    {
        [field: SerializeField] public AudioClip CaptureAudio { get; private set; }
        [field: SerializeField] public AudioClip MoveSelfAudio { get; private set; }
        [field: SerializeField] public AudioClip CheckmateAudio { get; private set; }
        [field: SerializeField] public AudioClip CheckAudio { get; private set; }
    }
}