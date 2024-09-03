using System.Collections;
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

    private Color _hatColor;
    private Color _eyesColor;
    private Color _bodyColor;
    private Color _clothesColor;

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

    private void Awake()
    {
        UpdateManager.Instance.RegisterObserver(this);

        UpdateText();
        redSlider.onValueChanged.AddListener(UpdateColor);
        greenSlider.onValueChanged.AddListener(UpdateColor);
        blueSlider.onValueChanged.AddListener(UpdateColor);
        redSlider.value = _redHatColor;
        greenSlider.value = _greenHatColor;
        blueSlider.value = _blueHatColor;
    }

    #region UpdateManager connection
    private void OnEnable()
    {
        UpdateManager.Instance.RegisterObserver(this);
    }
    private void OnDisable()
    {
        UpdateManager.Instance.UnregisterObserver(this);
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
        SaveHatSelection();
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
        SaveHatSelection();
        UpdateText();
        UpdateSlidersWithHatsValue(_hatNumber);
    }

    public void nextEye()
    {
        if (_eyesList.Count == 0) return;
        _eyeNumber = (_eyeNumber + 1) % _eyesList.Count;
        SaveHatSelection();
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
        SaveHatSelection();
        UpdateText();
    }
    public void nextClothes()
    {
        if (_clothesList.Count == 0) return;
        _clothesNumber = (_clothesNumber + 1) % _clothesList.Count;
        SaveHatSelection();
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
        SaveHatSelection();
        UpdateText();
    }


    //tf2 reference
    private void SaveHatSelection()
    {
        Hashtable hash = new Hashtable();
        hash.Add("HatIndex", _hatNumber);
        hash.Add("EyeIndex", _eyeNumber);
        hash.Add("BodyIndex", _bodyNumber);
        hash.Add("ClothesIndex", _clothesNumber);

        //speichere Farben von Slider
        Debug.Log($"ich schreibe in Datenbank, dass rot ist {Mathf.RoundToInt(redSlider.value * 255)}");
        Debug.Log($"ich schreibe in Datenbank, dass green ist {Mathf.RoundToInt(greenSlider.value * 255)}");
        Debug.Log($"ich schreibe in Datenbank, dass blue ist {Mathf.RoundToInt(blueSlider.value * 255)}");
        hash.Add("HatColorR", Mathf.RoundToInt(redSlider.value * 255));
        hash.Add("HatColorG", Mathf.RoundToInt(greenSlider.value * 255));
        hash.Add("HatColorB", Mathf.RoundToInt(blueSlider.value * 255));

        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
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
