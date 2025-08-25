using Chess2D.Piece;
using Chess2D.ScriptableObjects;
using UnityEngine;
using UnityEngine.UI;

namespace Chess2D.UI
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private GameObject _winStats;
        [SerializeField] private UIMoveHistory _uiMoveHistory;
        [SerializeField] private UIChessPieceDatabase _uiPieceDatabase;
        [SerializeField] private UICapturedPieces _capturedPiecesUI;

        public void InitUI(bool isPlayerDark)
        {
            PieceFactory<Image> playerPieceFactory;
            PieceFactory<Image> aiPieceFactory;

            if (isPlayerDark)
            {
                playerPieceFactory = new PieceFactory<Image>(_uiPieceDatabase.BlackPieceSet);
                aiPieceFactory = new PieceFactory<Image>(_uiPieceDatabase.WhitePieceSet);
            }
            else
            {
                playerPieceFactory = new PieceFactory<Image>(_uiPieceDatabase.WhitePieceSet);
                aiPieceFactory = new PieceFactory<Image>(_uiPieceDatabase.BlackPieceSet);
            }

            _capturedPiecesUI.Initialize(playerPieceFactory, aiPieceFactory);
        }

        public void ShowWinStats()
        {
            _winStats.SetActive(true);
        }
    }
}