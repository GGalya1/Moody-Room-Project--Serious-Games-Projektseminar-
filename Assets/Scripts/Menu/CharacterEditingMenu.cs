using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using UnityEngine.UI;

public class CharacterEditingMenu : MonoBehaviour, IUpdateObserver
{
    [SerializeField] TMP_Text hatNumberTxt;
    [SerializeField] TMP_Text eyeNumberTxt;
    [SerializeField] TMP_Text bodyTypeNumberTxt;
    [SerializeField] TMP_Text clothesNumberTxt;

    private int _hatNumber = 0;
    private int _eyeNumber = 0;
    private int _bodyNumber = 0;
    private int _clothesNumber = 0;

    private float _redHatColor = 100;
    private float _greenHatColor = 37;
    private float _blueHatColor = 30;

    [SerializeField] private List<GameObject> _hatsList;
    [SerializeField] private List<GameObject> _eyesList;
    [SerializeField] private List<GameObject> _bodiesList;
    [SerializeField] private List<GameObject> _clothesList;

    [SerializeField] private Slider redSlider;
    [SerializeField] private Slider greenSlider;
    [SerializeField] private Slider blueSlider;

    private Renderer currentHatRenderer;

    private Hashtable currentCustomize;
    //fraglich, ob ich diese zwei Variablen ueberhaupt brauche
    [SerializeField] private PlayerCustomizationManager customizationManager;
    [SerializeField] private PlayerCustomizationManager localCustomizationManager;

    [SerializeField] private CharacterMainMenuPreviewSelection _previewSelection;

    private void Awake()
    {
        UpdateManager.Instance.RegisterObserver(this);

        UpdateText();
        redSlider.onValueChanged.AddListener(UpdateColor);
        greenSlider.onValueChanged.AddListener(UpdateColor);
        blueSlider.onValueChanged.AddListener(UpdateColor);
        redSlider.value = _redHatColor / 255f;
        greenSlider.value = _greenHatColor / 255f;
        blueSlider.value = _blueHatColor / 255f;

        //erstellen Default-Werte fuer Customization Menu
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

    private void SaveLocalHatSelection()
    {
        currentCustomize["HatIndex"] = _hatNumber;
        currentCustomize["EyeIndex"] = _eyeNumber;
        currentCustomize["BodyIndex"] = _bodyNumber;
        currentCustomize["ClothesIndex"] = _clothesNumber;

        //speichere Farben von Slider
        currentCustomize["HatColorR"] = Mathf.RoundToInt(redSlider.value * 255);
        currentCustomize["HatColorG"] = Mathf.RoundToInt(greenSlider.value * 255);
        currentCustomize["HatColorB"] = Mathf.RoundToInt(blueSlider.value * 255);

        //default Werte fuer Customization Menu

    }

    #region UpdateManager connection
    private void OnEnable()
    {
        UpdateManager.Instance.RegisterObserver(this);

        //loeschen von alten Werten (zur Sichercheit) und schreiben von aktuellen Werten aus PlayerPropertiess
        //und alle nicht gespeicherte Veraenderungen in diesem Menu werden hier gespeichert
        currentCustomize = new Hashtable();
        Hashtable playerProperties = PhotonNetwork.LocalPlayer.CustomProperties;

        currentCustomize["HatIndex"] = (int)playerProperties["HatIndex"];
        currentCustomize["EyeIndex"] = (int)playerProperties["EyeIndex"];
        currentCustomize["BodyIndex"] = (int)playerProperties["BodyIndex"];
        currentCustomize["ClothesIndex"] = (int)playerProperties["ClothesIndex"];

        //speichere Farben von Slider
        currentCustomize["HatColorR"] = playerProperties["HatColorR"];
        currentCustomize["HatColorG"] = playerProperties["HatColorG"];
        currentCustomize["HatColorB"] = playerProperties["HatColorB"];

        customizationManager.ApplyCustomization(); //???
        //Speichern den alten Wert. Falls kein Save gedruckt wird, werden wir diesen Wert zurucksetzen

        //anpassen zu Customization aus dem MainMenu
        _hatNumber = (int)currentCustomize["HatIndex"];
        _eyeNumber = (int)currentCustomize["EyeIndex"];
        _bodyNumber = (int)currentCustomize["BodyIndex"];
        _clothesNumber = (int)currentCustomize["ClothesIndex"];
        currentHatRenderer = _hatsList[_hatNumber].GetComponent<Renderer>();

        //damit wir in MainMenu und in diesem Menu gleiche Avatars beim Beitreten haben
        _previewSelection.SelectinEditMenu(_previewSelection.currCharIndex);
        UpdateSlidersWithHatsValue(_hatNumber);
        UpdateText();
    }
    private void OnDisable()
    {
        UpdateManager.Instance.UnregisterObserver(this);

        //loeschen von allen gespeicherten Veraenderungen
        currentCustomize = new Hashtable();
    }
    private void OnDestroy()
    {
        UpdateManager.Instance.UnregisterObserver(this);
    }
    #endregion

    private void UpdateColor(float value)
    {
        //vielleicht hier noch Debug.Log hinzufugen
        if (currentHatRenderer == null || !currentHatRenderer.material.HasProperty("_Color"))
        {
            Debug.Log("Farbe kann nicht gesetzt werden, weil Renderer oder Material fehlt");
            return;
        }
        //holt die aktuellen Werte des Sliders
        float red = redSlider.value;
        float green = greenSlider.value;
        float blue = blueSlider.value;

        SaveLocalHatSelection();

        //erstellt eine neue Farbe basierend auf diesen (da diese Methode aufgerufen wird jedes Mal, wenn wir Slider aendern)
        Color newColor = new Color(red, green, blue);

        //erstellt eine neue Material-Instanz, um das Originalmaterial nicht zu aendern
        Material newMaterial = new Material(currentHatRenderer.material);
        newMaterial.color = newColor;

        //setzt das neue Material auf den Renderer
        currentHatRenderer.material = newMaterial;
    }

    private void UpdateSlidersWithHatsValue(int hatIndex)
    {
        //holen Renderer, um Material und Farbe zu holen
        Renderer currHatRenderer = _hatsList[hatIndex].GetComponent<Renderer>();

        if (currHatRenderer != null && currHatRenderer.material.HasProperty("_Color"))
        {
            //setzt Sliders auf der aktuellen Farbe des Hutes
            Color currentColor = currentHatRenderer.material.color;
            redSlider.value = currentColor.r;
            greenSlider.value = currentColor.g;
            blueSlider.value = currentColor.b;

            /*Debug.Log("Red: " + redSlider.value);
            Debug.Log("Green: " + greenSlider.value);
            Debug.Log("Blue: " + blueSlider.value);*/
        }
    }

    private void UpdateText()
    {
        hatNumberTxt.text = (_hatsList.Count > 0) ? (_hatNumber + 1) + " / " + _hatsList.Count : "es gibt keine";
        eyeNumberTxt.text = (_eyesList.Count > 0) ? (_eyeNumber + 1) + " / " + _eyesList.Count : "es gibt keine";
        bodyTypeNumberTxt.text = (_bodiesList.Count > 0) ? (_bodyNumber + 1) + " / " + _bodiesList.Count : "es gibt keine";
        clothesNumberTxt.text = (_clothesList.Count > 0) ? (_clothesNumber + 1) + " / " + _clothesList.Count : "es gibt keine";
    }

    private void UpdateSelection(List<GameObject> list, int selectedIndex)
    {
        if (list.Count == 0) return;
        for (int i = 0; i < list.Count; i++)
        {
            list[i].SetActive(i == selectedIndex);
        }
    }

    public void ObservedUpdate()
    {
        UpdateSelection(_hatsList, _hatNumber);
        UpdateSelection(_eyesList, _eyeNumber);
        UpdateSelection(_clothesList, _clothesNumber);
        UpdateSelection(_bodiesList, _bodyNumber);
    }

    public void nextHat()
    {
        if (_hatsList.Count == 0) return;
        _hatNumber = (_hatNumber + 1) % _hatsList.Count;
        currentHatRenderer = _hatsList[_hatNumber].GetComponent<Renderer>();
        SaveLocalHatSelection();
        UpdateText();
        UpdateSlidersWithHatsValue(_hatNumber);
    }
    public void prevHat()
    {
        if (_hatsList.Count == 0) return;
        if (_hatNumber == 0)
        {
            _hatNumber = _hatsList.Count - 1;
        }
        else
        {
            _hatNumber--;
        }
        currentHatRenderer = _hatsList[_hatNumber].GetComponent<Renderer>();
        SaveLocalHatSelection();
        UpdateText();
        UpdateSlidersWithHatsValue(_hatNumber);
    }

    public void nextEye()
    {
        if (_eyesList.Count == 0) return;
        _eyeNumber = (_eyeNumber + 1) % _eyesList.Count;
        SaveLocalHatSelection();
        UpdateText();
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
        UpdateText();
    }
    public void nextClothes()
    {
        if (_clothesList.Count == 0) return;
        _clothesNumber = (_clothesNumber + 1) % _clothesList.Count;
        SaveLocalHatSelection();
        UpdateText();
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
        UpdateText();
    }


    //tf2 reference
    public void SaveHatSelection()
    {
        PhotonNetwork.LocalPlayer.SetCustomProperties(currentCustomize);
        _previewSelection.SelectCharacter(_previewSelection.tempCharIndex);
        _previewSelection.currCharIndex = _previewSelection.tempCharIndex;
        Launch.instance.SelectAvatar(_previewSelection.tempCharIndex);
        
    }

    //damit nachdem wir "nicht speichern und in Main Menu" Button drucken, unsere Variablen aktualisiert werden
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
