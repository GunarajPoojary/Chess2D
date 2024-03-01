using UnityEngine;
using UnityEngine.SceneManagement;

public class ColorSelection : MonoBehaviour
{
    // This method is called when the player selects the white color.
    public void SetWhiteColor()
    {
        // Save the selected color to Player Preferences as "White".
        PlayerPrefs.SetString("selectedColor", "White");

        // Load the "Gameplay" scene.
        SceneManager.LoadScene("Gameplay", LoadSceneMode.Single);
    }

    // This method is called when the player selects the black color.
    public void SetBlackColor()
    {
        // Save the selected color to Player Preferences as "Black".
        PlayerPrefs.SetString("selectedColor", "Black");

        // Load the "Gameplay" scene.
        SceneManager.LoadScene("Gameplay", LoadSceneMode.Single);
    }
}
