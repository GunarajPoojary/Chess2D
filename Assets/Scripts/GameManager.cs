using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Singleton instance of the GameManager.
    public static GameManager Instance;
    public PieceController controller;

    // TextMeshProUGUI objects for displaying win and lose messages.
    public TextMeshProUGUI winText;
    public TextMeshProUGUI loseText;

    // To check whether game has ended or not
    [HideInInspector]public bool gameIsActive = true;

    public bool playerTurn = true;

    // String to display that the player has won.
    string playerWon = " Player Won!";

    // GameObjects representing the white and black kings.
    [SerializeField] GameObject whiteKing, blackKing;

    // Awake is called when the script instance is being loaded.
    void Awake()
    {
        controller.OnEndTurn += Controller_OnEndTurn;
        controller.OnKingDeath += Controller_OnKingDeath;

        // Ensure that there is only one instance of GameManager.
        if (Instance == null)
            Instance = this;
    }

    private void Controller_OnKingDeath(string obj)
    {
        PlayerWin(obj);
    }

    private void Controller_OnEndTurn()
    {
        if (playerTurn)
            playerTurn = false;
        else if (!playerTurn)
            playerTurn = true;
    }

    // Method to display win message for Player 1.
    public void PlayerWin(string playerColor)
    {
        winText.text = playerColor + " player has won";
        winText.gameObject.SetActive(true);
    }
}
