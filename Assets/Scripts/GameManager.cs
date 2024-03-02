using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Singleton instance of the GameManager.
    public static GameManager Instance;

    // TextMeshProUGUI objects for displaying win and lose messages.
    public TextMeshProUGUI winText;
    public TextMeshProUGUI loseText;

    // To check whether game has ended or not
    [HideInInspector]public bool gameIsActive = true;

    // String to display that the player has won.
    string playerWon = " Player Won!";

    // GameObjects representing the white and black kings.
    [SerializeField] GameObject whiteKing, blackKing;

    // Awake is called when the script instance is being loaded.
    void Awake()
    {
        // Ensure that there is only one instance of GameManager.
        if (Instance == null)
            Instance = this;
    }

    // Method to display win message for Player 1.
    public void Player_1Win(string playerName)
    {
        // Activate the winText object.
        winText.gameObject.SetActive(true);

        // Set the winText to display the winning player's name concatenated with the "Player Won!" message.
        winText.text = string.Concat(playerName, playerWon);
    }
}
