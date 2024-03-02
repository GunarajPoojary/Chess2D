using UnityEngine;
using System;
using System.Linq;

public class PieceController : MonoBehaviour
{
    // Singleton instance of the ChessPieceController.
    public static PieceController instance;

    // Selected chess piece.
    Transform selectedPiece;

    // Main camera reference.
    Camera mainCam;

    // Counter for pieces taken out of play by the player.
    int outPlayerPiece = 9;

    // Counter for pieces taken out of play by the opponent.
    int outOpponentPiece = 9;

    // Enum to track the current turn state.
    public enum TurnStates { PlayerTurn, OpponentTurn };
    public TurnStates state;

    // Awake is called when the script instance is being loaded.
    private void Awake()
    {
        // Ensure there is only one instance of ChessPieceController.
        if (instance == null)
            instance = this;

        // Get the main camera component.
        mainCam = GetComponent<Camera>();
    }

    // Start is called before the first frame update.
    private void Start()
    {
        // Set the initial turn state to player's turn.
        state = TurnStates.PlayerTurn;
    }

    // Update is called once per frame.
    private void Update()
    {
        // If the game hasn't ended
        if (GameManager.Instance.gameIsActive)
        {
            // Check for mouse click or touch input.
            if (Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
            {
                // Get the input position.
                Vector3 inputPosition = Input.GetMouseButtonDown(0) ? Input.mousePosition : (Vector3)Input.GetTouch(0).position;

                // Cast a ray from the main camera.
                Ray ray = mainCam.ScreenPointToRay(inputPosition);
                RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

                try
                {
                    // Check if the hit object has a Piece component.
                    if (hit.transform.gameObject.GetComponent<Piece>())
                    {
                        // Clear existing highlights.
                        ClearHighlights();

                        // Check the turn state and select the appropriate piece.
                        if (hit.transform.name.Contains(Board.Instance.playerColor) && state == TurnStates.PlayerTurn)
                        {
                            selectedPiece = hit.transform;
                            IHighlight highlight = hit.transform.GetComponent<IHighlight>();
                            highlight.Highlight();
                        }
                        else if (hit.transform.name.Contains(Board.Instance.opponentColor) && state == TurnStates.OpponentTurn)
                        {
                            selectedPiece = hit.transform;
                            IHighlight highlight = hit.transform.GetComponent<IHighlight>();
                            highlight.Highlight();
                        }
                    }

                    // Check if the hit object is a Highlighter.
                    if (hit.transform.CompareTag("Highlighter"))
                    {
                        // Move the selected piece to the highlighted position.
                        if (selectedPiece.name.Contains(Board.Instance.playerColor))
                        {
                            selectedPiece.position = hit.transform.position;

                            // Handle capturing opponent's piece.
                            foreach (Piece c in Board.Instance.pieces)
                            {
                                if (selectedPiece.position == c.transform.position && c.name.Contains(Board.Instance.opponentColor))
                                {
                                    outPlayerPiece++;
                                    c.transform.position = new Vector2(outPlayerPiece, 6);
                                }
                            }
                            state = TurnStates.OpponentTurn;
                        }
                        else if (selectedPiece.name.Contains(Board.Instance.opponentColor))
                        {
                            selectedPiece.position = hit.transform.position;

                            // Handle capturing player's piece.
                            foreach (Piece c in Board.Instance.pieces)
                            {
                                if (selectedPiece.position == c.transform.position && c.name.Contains(Board.Instance.playerColor))
                                {
                                    outOpponentPiece++;
                                    c.transform.position = new Vector2(outOpponentPiece, 1);
                                }
                            }
                            state = TurnStates.PlayerTurn;
                        }
                        ClearHighlights();
                    }
                }
                catch (Exception)
                {
                    Debug.Log("Invalid Tile");
                }
            }
        }
    }

    // Method to clear all highlighters from the board.
    void ClearHighlights()
    {
        Board.Instance.highLighters = GameObject.FindGameObjectsWithTag("Highlighter").ToList();
        foreach (GameObject g in Board.Instance.highLighters)
            Destroy(g);
    }
}
