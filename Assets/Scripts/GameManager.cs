using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Space(5),Header("Controller")]
    [SerializeField] PieceController controller;

    [Space(10), Header("Win text")]
    [SerializeField] TextMeshProUGUI winText;

    const string playerWon = " player has won";

    public bool GameIsActive { get; private set; } = true;
    public bool PlayerTurn { get; private set; } = true;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    void Start()
    {
        if (Board.Instance.PlayerColor == "Black")
            PlayerTurn = false;

        controller.OnEndTurn += Controller_OnEndTurn;
        controller.OnKingDeath += Controller_OnKingDeath;
    }

    void Controller_OnKingDeath(string kingColor)
    {
        PlayerWin(kingColor);
        GameIsActive = false;
    }

    void Controller_OnEndTurn()
    {
        if (PlayerTurn)
            PlayerTurn = false;
        else if (!PlayerTurn)
            PlayerTurn = true;
    }

    void PlayerWin(string playerColor)
    {
        winText.text = playerColor + playerWon;
        winText.gameObject.SetActive(true);
    }
}
