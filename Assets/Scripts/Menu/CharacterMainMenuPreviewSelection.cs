using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Manages the preview and selection of characters in the main menu and editing menu.
/// Allows users to rotate the character model and switch between available characters.
/// Also controls visibility of customization settings for specific characters.
/// </summary>
public class CharacterMainMenuPreviewSelection : MonoBehaviour
{
    public GameObject[] characters;
    public GameObject[] charactersInEditingMenu;
    public Slider angleSlider;
    public int currCharIndex = 0; // Index of the currently selected character
    public int tempCharIndex = 0; // Index of the currently selected character in the editing menu

    // Only one avatar type is available for customization
    [SerializeField] private TMP_Text notAviableText;
    [SerializeField] private GameObject customizationSettings;

    /// <summary>
    /// Called at the start of the scene (before the first frame). Sets up character visibility and initializes slider listeners.
    /// </summary>
    void Start()
    {
        // Show only the default character (index 0) at the start
        for (int i = 0; i < characters.Length; i++)
        {
            characters[i].SetActive(i == currCharIndex);
            charactersInEditingMenu[i].SetActive(i == currCharIndex);
        }

        // Set up the slider to rotate the character when its value changes
        angleSlider.onValueChanged.AddListener(OnSliderValueChanged);
    }

    /// <summary>
    /// Called when the GameObject is enabled. Resets the character's rotation to the default value.
    /// </summary>
    private void OnEnable() 
    {
        angleSlider.value = 190;
        for (int i = 0; i < characters.Length; i++)
        {
            characters[currCharIndex].transform.rotation = Quaternion.Euler(0, 190, 0); 
        }
    }

    // Called whenever the slider value changes.
    void OnSliderValueChanged(float value)
    {
        characters[currCharIndex].transform.rotation = Quaternion.Euler(0, value, 0);
    }

    // Cleans up the slider listener when this object is destroyed.
    void OnDestroy()
    {
        angleSlider.onValueChanged.RemoveListener(OnSliderValueChanged);
    }

    // Each button for the character selection has an index and "OnClick"
    /// <summary>
    /// Called when a character selection button is pressed. Updates the currently active character.
    /// </summary>
    /// <param name="index">Index of the selected character in the array.</param>
    public void SelectCharacter(int index)
    {
        if (index < 0 || index >= characters.Length)
        {
            return;
        }

        // old character is hidden
        angleSlider.value = 190;
        characters[currCharIndex].SetActive(false);
        charactersInEditingMenu[currCharIndex].SetActive(false);

        // new character is shown
        currCharIndex = index;
        characters[currCharIndex].SetActive(true);
        charactersInEditingMenu[currCharIndex].SetActive(true);

        angleSlider.value = 190; // set slider's angle to default 
    }

    /// <summary>
    /// Called by buttons in the editing menu to select a character for editing.
    /// Also manages the visibility of customization settings.
    /// </summary>
    /// <param name="index">Index of the selected character in the editing menu array.</param>
    public void SelectinEditMenu(int index) 
    {
        if (index < 0 || index >= characters.Length)
        {
            return;
        }

        // Hide the previous character in the editing menu and reset the slider
        angleSlider.value = 190;
        charactersInEditingMenu[tempCharIndex].SetActive(false);

        // Show the newly selected character
        tempCharIndex = index;
        charactersInEditingMenu[tempCharIndex].SetActive(true); 

        // Set slider's rotation to default 
        angleSlider.value = 190;

        // Show customization settings if the selected character is customizable
        if (index == 2)
        {
            notAviableText.gameObject.SetActive(false);
            customizationSettings.gameObject.SetActive(true);
        }
        else
        {
            customizationSettings.gameObject.SetActive(false);
            notAviableText.gameObject.SetActive(true); // Show the "Not Available" text, if the character is not customizable

        }
    }
}
