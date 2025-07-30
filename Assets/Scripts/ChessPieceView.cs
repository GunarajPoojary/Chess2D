using UnityEngine;

namespace Chess2D
{
    public enum PieceType
    {
        Pawn,
        Rook,
        Knight,
        Bishop,
        Queen,
        King
    }

    public interface ISelectable
    {
        void Select();
        Vector3 GetPosition();
    }

    [System.Serializable]
    public class ChessPieceModel
    {
        public bool isPlayer;
        public PieceType pieceType;
    }

    public class ChessPieceView : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _renderer;

        public void SetSprite(Sprite sprite) => _renderer.sprite = sprite;
        public void SetPosition(Vector3 position) => transform.position = position;
    }

    public class ChessPieceController : ISelectable
    {
        private readonly ChessPieceView _view;
        private readonly ChessPieceModel _model;

        private readonly ISelectStrategy _selectStrategy;
        private readonly IMoveStrategy _moveStrategy;
        private readonly IPieceSpriteProvider _pieceSpriteProvider;

        public ChessPieceController(ChessPieceView view,
                                    ChessPieceModel model,
                                    ISelectStrategy selectStrategy,
                                    MoveStrategyFactory moveStrategyFactory,
                                    IPieceSpriteProvider pieceSpriteProvider)
        {
            _model = model;
            _view = view;
            _selectStrategy = selectStrategy;
            _pieceSpriteProvider = pieceSpriteProvider;
            SetSprite();
            _moveStrategy = moveStrategyFactory.Create(_model.pieceType);
        }

        public void Select() => _selectStrategy.HandleSelection();
        public void SetSprite() => _view.SetSprite(_pieceSpriteProvider.GetSprite(_model.isPlayer, _model.pieceType));
        public void MakeMove(Vector3 pos) => _view.SetPosition(pos);

        public Vector3 GetPosition() => _view.transform.position;
    }
}