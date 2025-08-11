using UnityEngine;

namespace Chess2D
{
    [CreateAssetMenu(fileName = "Theme", menuName = "Custom/Theme")]
    public class ThemeSO : ScriptableObject
    {
        [field: SerializeField] public Color LightPieceColor { get; private set; }
        [field: SerializeField] public Color DarkPieceColor { get; private set; }
        [field: SerializeField] public Color LightTileColor { get; private set; }
        [field: SerializeField] public Color DarkTileColor { get; private set; }
    }
}