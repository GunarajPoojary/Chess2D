using UnityEngine;
using UnityEngine.SceneManagement;

public class ColorSelection : MonoBehaviour
{
    public void SetWhiteColor()
    {
        // Save the selected color to Player Preferences as "White".
        PlayerPrefs.SetString("selectedColor", "White");

        SceneManager.LoadScene("Gameplay", LoadSceneMode.Single);
    }

    public void SetBlackColor()
    {
        // Save the selected color to Player Preferences as "Black".
        PlayerPrefs.SetString("selectedColor", "Black");

        SceneManager.LoadScene("Gameplay", LoadSceneMode.Single);
    }
}
