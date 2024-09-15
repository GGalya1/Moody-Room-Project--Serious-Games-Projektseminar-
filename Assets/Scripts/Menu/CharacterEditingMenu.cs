using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using UnityEngine.UI;

/// <summary>
/// Manages the character customization interface. 
/// It allows players to customize character, 
/// including selecting hats, eyes, body types, and clothes, 
/// as well as adjusting hat colors using RGB sliders.
/// </summary>
public class CharacterEditingMenu : MonoBehaviour
{
    // UI Elements displaying the current selection
    [SerializeField] TMP_Text hatNumberTxt;
    [SerializeField] TMP_Text eyeNumberTxt;
    [SerializeField] TMP_Text bodyTypeNumberTxt;
    [SerializeField] TMP_Text clothesNumberTxt;

    // Index of the current selected options
    private int _hatNumber = 0;
    private int _eyeNumber = 0;
    private int _bodyNumber = 0;
    private int _clothesNumber = 0;

    // Default hat color
    private float _redHatColor = 100;
    private float _greenHatColor = 37;
    private float _blueHatColor = 30;

    // List of all available options
    [SerializeField] private List<GameObject> _hatsList;
    [SerializeField] private List<GameObject> _eyesList;
    [SerializeField] private List<GameObject> _bodiesList;
    [SerializeField] private List<GameObject> _clothesList;

    // RGB Sliders
    [SerializeField] private Slider redSlider;
    [SerializeField] private Slider greenSlider;
    [SerializeField] private Slider blueSlider;

    private Renderer currentHatRenderer;

    private Hashtable currentCustomize;
    [SerializeField] private PlayerCustomizationManager customizationManager;
    [SerializeField] private PlayerCustomizationManager localCustomizationManager;

    // Reference to the character preview selection in the main menu
    [SerializeField] private CharacterMainMenuPreviewSelection _previewSelection;

    /// <summary>
    /// Initializes default values, sets up listeners for the RGB sliders, 
    /// and populates the currentCustomize hashtable.
    /// </summary>
    private void Awake()
    {
        UpdateText();
        // Set up slider listeners
        redSlider.onValueChanged.AddListener(UpdateColor);
        greenSlider.onValueChanged.AddListener(UpdateColor);
        blueSlider.onValueChanged.AddListener(UpdateColor);
        // Set default slider values
        redSlider.value = _redHatColor / 255f;
        greenSlider.value = _greenHatColor / 255f;
        blueSlider.value = _blueHatColor / 255f;

        // Initialize customization hashtable with default values
        currentCustomize = new Hashtable();
        currentCustomize.Add("HatIndex", 0);
        currentCustomize.Add("EyeIndex", 0);
        currentCustomize.Add("BodyIndex", 0);
        currentCustomize.Add("ClothesIndex", 0);

        currentCustomize.Add("HatColorR", 100);
        currentCustomize.Add("HatColorG", 37);
        currentCustomize.Add("HatColorB", 30);

        SaveHatSelection();
    }

    /// <summary>
    /// Saves the current/local selections to the currentCustomize hashtable.
    /// </summary>
    private void SaveLocalHatSelection()
    {
        currentCustomize["HatIndex"] = _hatNumber;
        currentCustomize["EyeIndex"] = _eyeNumber;
        currentCustomize["BodyIndex"] = _bodyNumber;
        currentCustomize["ClothesIndex"] = _clothesNumber;

        // Save the hat color from the sliders
        currentCustomize["HatColorR"] = Mathf.RoundToInt(redSlider.value * 255);
        currentCustomize["HatColorG"] = Mathf.RoundToInt(greenSlider.value * 255);
        currentCustomize["HatColorB"] = Mathf.RoundToInt(blueSlider.value * 255);

    }

    /// <summary>
    /// Called when the customization menu is enabled. 
    /// Loads the current customization values from the player properties.
    /// </summary>
    private void OnEnable()
    {
        // Clear current customization and load from player properties
        currentCustomize = new Hashtable();
        Hashtable playerProperties = PhotonNetwork.LocalPlayer.CustomProperties;

        currentCustomize["HatIndex"] = (int)playerProperties["HatIndex"];
        currentCustomize["EyeIndex"] = (int)playerProperties["EyeIndex"];
        currentCustomize["BodyIndex"] = (int)playerProperties["BodyIndex"];
        currentCustomize["ClothesIndex"] = (int)playerProperties["ClothesIndex"];

        currentCustomize["HatColorR"] = playerProperties["HatColorR"];
        currentCustomize["HatColorG"] = playerProperties["HatColorG"];
        currentCustomize["HatColorB"] = playerProperties["HatColorB"];

        // Apply customization and update UI. 
        customizationManager.ApplyCustomization(); // Save old values. If no "Save" is pressed, we will reset to this values

        _hatNumber = (int)currentCustomize["HatIndex"];
        _eyeNumber = (int)currentCustomize["EyeIndex"];
        _bodyNumber = (int)currentCustomize["BodyIndex"];
        _clothesNumber = (int)currentCustomize["ClothesIndex"];
        currentHatRenderer = _hatsList[_hatNumber].GetComponent<Renderer>();

        // Sync preview selection with current character. 
        // Main menu and this menu should have the same avatars.
        _previewSelection.SelectinEditMenu(_previewSelection.currCharIndex);
        UpdateSlidersWithHatsValue(_hatNumber);
        UpdateText();
    }

    /// <summary>
    /// Clears the current customization values when the menu is disabled.
    /// </summary>
    private void OnDisable()
    {
        currentCustomize = new Hashtable(); // delete changes (current values)
    }

    /// <summary>
    /// Updates the hat color in real-time based on the RGB sliders' values.
    /// </summary>
    /// <param name="value">???</param>
    private void UpdateColor(float value)
    {
        //vielleicht hier noch Debug.Log hinzufugen
        if (currentHatRenderer == null || !currentHatRenderer.material.HasProperty("_Color"))
        {
            Debug.Log("Farbe kann nicht gesetzt werden, weil Renderer oder Material fehlt");
            return;
        }
        // Get RGB values from sliders
        float red = redSlider.value;
        float green = greenSlider.value;
        float blue = blueSlider.value;

        SaveLocalHatSelection();

        // Create a new color (this method is called every time we change sliders)
        Color newColor = new Color(red, green, blue); 

        // create a new material instance to not change the original material and apply the new color
        Material newMaterial = new Material(currentHatRenderer.material);
        newMaterial.color = newColor;

        currentHatRenderer.material = newMaterial; // Assign the new material to the renderer
    }

    /// <summary>
    /// Updates the RGB sliders with the current color of the hat.
    /// </summary>
    /// <param name="hatIndex">The index of the currently selected hat. </param>
    private void UpdateSlidersWithHatsValue(int hatIndex)
    { 
        Renderer currHatRenderer = _hatsList[hatIndex].GetComponent<Renderer>();

        if (currHatRenderer != null && currHatRenderer.material.HasProperty("_Color"))
        { // if the renderer exits and the material has a color property we set the sliders to the current color of the hat
            Color currentColor = currentHatRenderer.material.color;
            redSlider.value = currentColor.r;
            greenSlider.value = currentColor.g;
            blueSlider.value = currentColor.b;

            /*Debug.Log("Red: " + redSlider.value);
            Debug.Log("Green: " + greenSlider.value);
            Debug.Log("Blue: " + blueSlider.value);*/
        }
    }

    /// <summary>
    /// Update the UI elements to reflect the current selection indicies of the hat, eye, body type, and clothes.
    /// </summary>
    private void UpdateText()
    {
        hatNumberTxt.text = (_hatsList.Count > 0) ? (_hatNumber + 1) + " / " + _hatsList.Count : "es gibt keine";
        eyeNumberTxt.text = (_eyesList.Count > 0) ? (_eyeNumber + 1) + " / " + _eyesList.Count : "es gibt keine";
        bodyTypeNumberTxt.text = (_bodiesList.Count > 0) ? (_bodyNumber + 1) + " / " + _bodiesList.Count : "es gibt keine";
        clothesNumberTxt.text = (_clothesList.Count > 0) ? (_clothesNumber + 1) + " / " + _clothesList.Count : "es gibt keine";
    }

    /// <summary>
    /// Activates the currently selected object from the given list and deactivates all others.
    /// </summary>
    /// <param name="list">List of objects to update. </param>
    /// <param name="selectedIndex">Index of the currently selected object.</param>
    private void UpdateSelection(List<GameObject> list, int selectedIndex)
    {
        if (list.Count == 0) return;

        for (int i = 0; i < list.Count; i++)
        {
            list[i].SetActive(i == selectedIndex);
        }
        UpdateText();
    }

    /// <summary>
    /// Advances to the next hat in the list, looping back to the first hat if the end is reached.
    /// Updates the current hat renderer, saves the current selection, and updates the UI.
    /// </summary>
    public void nextHat()
    {
        if (_hatsList.Count == 0) return;
        _hatNumber = (_hatNumber + 1) % _hatsList.Count; // looping
        currentHatRenderer = _hatsList[_hatNumber].GetComponent<Renderer>();
        SaveLocalHatSelection();
        UpdateSlidersWithHatsValue(_hatNumber);
        UpdateSelection(_hatsList, _hatNumber); 
    }
    /// <summary>
    /// Advances to the previous hat in the list, looping back to the last hat if the beginning is reached.
    /// </summary>
    public void prevHat()
    {
        if (_hatsList.Count == 0) return;
        if (_hatNumber == 0)
        { // looping
            _hatNumber = _hatsList.Count - 1;
        }
        else
        { 
            _hatNumber--;
        }
        currentHatRenderer = _hatsList[_hatNumber].GetComponent<Renderer>();
        SaveLocalHatSelection();
        UpdateSlidersWithHatsValue(_hatNumber);
        UpdateSelection(_hatsList, _hatNumber);
    }

    public void nextEye()
    {
        if (_eyesList.Count == 0) return;
        _eyeNumber = (_eyeNumber + 1) % _eyesList.Count;
        SaveLocalHatSelection();
        UpdateSelection(_eyesList, _eyeNumber);
    }
    public void prevEye()
    {
        if (_eyesList.Count == 0) return;
        if (_eyeNumber == 0)
        {
            _eyeNumber = _eyesList.Count - 1;
        }
        else
        {
            _eyeNumber--;
        }
        SaveLocalHatSelection();
        UpdateSelection(_eyesList, _eyeNumber);
    }
    public void nextClothes()
    {
        if (_clothesList.Count == 0) return;
        _clothesNumber = (_clothesNumber + 1) % _clothesList.Count;
        SaveLocalHatSelection();
        UpdateSelection(_clothesList, _clothesNumber);
    }
    public void prevClothes()
    {
        if (_clothesList.Count == 0) return;
        if (_clothesNumber == 0)
        {
            _clothesNumber = _clothesList.Count - 1;
        }
        else
        {
            _clothesNumber--;
        }
        SaveLocalHatSelection();
        UpdateSelection(_clothesList, _clothesNumber);
    }


    //tf2 reference
    /// <summary>
    /// Saves the current customization values to the player properties and updates the preview selection.
    /// </summary>
    public void SaveHatSelection()
    {
        PhotonNetwork.LocalPlayer.SetCustomProperties(currentCustomize);
        _previewSelection.SelectCharacter(_previewSelection.tempCharIndex);
        _previewSelection.currCharIndex = _previewSelection.tempCharIndex;
        Launch.instance.SelectAvatar(_previewSelection.tempCharIndex);
        
    }

   /// <summary>
   /// Aborts the customization process and returns to the main menu without saving changes.
   /// </summary>
   /// <param name="playerProperties"></param>
    public void ApplyCustomHatFromHash(Hashtable playerProperties)
    {
        _hatNumber = (int)playerProperties["HatIndex"];
        _eyeNumber = (int)playerProperties["EyeIndex"];
        _bodyNumber = (int)playerProperties["BodyIndex"];
        _clothesNumber = (int)playerProperties["ClothesIndex"];

        redSlider.value = (int)playerProperties["HatColorR"] / 255f;
        greenSlider.value = (int)playerProperties["HatColorG"] / 255f;
        blueSlider.value = (int)playerProperties["HatColorB"] / 255f;
        UpdateText();
    }
}
