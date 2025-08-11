using UnityEngine;

namespace Chess2D
{
    [CreateAssetMenu(fileName = "SpritesDatabase", menuName = "Custom/Sprites Database")]
    public class HighlightSpritesDatabase : ScriptableObject
    {
        [Header("Highlighters Sprite")]
        [field: SerializeField] public Sprite SelectionHighlightSprite { get; private set; }
        [field: SerializeField] public Sprite CaptureHighlightSprite { get; private set; }
        [field: SerializeField] public Sprite EmptyTileHighlightSprite { get; private set; }
        //[field: SerializeField] public Sprite SpecialHighlightSprite { get; private set; }
    }
}