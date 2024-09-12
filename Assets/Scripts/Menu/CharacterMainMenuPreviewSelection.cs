using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterMainMenuPreviewSelection : MonoBehaviour
{
    public GameObject[] characters;
    public GameObject[] charactersInEditingMenu;
    public Slider angleSlider;
    public Slider secondAngleSlider; //befindet sich in Customization Menu
    public int currCharIndex = 0;
    public int tempCharIndex = 0;

    //um Player modifizieren zu koennen, nur wenn richtigen Avatar gewaehlt ist
    [SerializeField] private TMP_Text notAviableText;
    [SerializeField] private GameObject customizationSettings;

    void Start()
    {
        //sichtbar ist nur "default" Character
        for (int i = 0; i < characters.Length; i++)
        {
            characters[i].SetActive(i == currCharIndex);
            charactersInEditingMenu[i].SetActive(i == currCharIndex);
        }

        //Ab dem Punkt werden alle Veraenderungen von Slider ubernohmen
        angleSlider.onValueChanged.AddListener(OnSliderValueChanged);
        secondAngleSlider.onValueChanged.AddListener(OnCustomizationSliderValueChanged);
    }

    //damit wenn wir die Szene verlassen und danach zurueck kommen, wird unsere Rotation auf default resetet
    private void OnEnable()
    {
        angleSlider.value = 190;
        for (int i = 0; i < characters.Length; i++)
        {
            characters[currCharIndex].transform.rotation = Quaternion.Euler(0, 190, 0);
        }
    }

    //wird aufgerugen jeden Mal, wenn eine Veranderung vorliegt
    void OnSliderValueChanged(float value)
    {
        characters[currCharIndex].transform.rotation = Quaternion.Euler(0, value, 0);
    }
    void OnCustomizationSliderValueChanged(float value)
    {
        charactersInEditingMenu[tempCharIndex].transform.rotation = Quaternion.Euler(0, value, 0);
    }

    void OnDestroy()
    {
        //nach "Destroy" wollen wir nicht mehr ablesen
        angleSlider.onValueChanged.RemoveListener(OnSliderValueChanged);
    }

    //jedes Button bekommt das "OnClick" und Index als int
    public void SelectCharacter(int index)
    {
        if (index < 0 || index >= characters.Length)
        {
            return;
        }

        //alten Character verbergen
        angleSlider.value = 190;
        characters[currCharIndex].SetActive(false);
        charactersInEditingMenu[currCharIndex].SetActive(false);

        //neu gewaehlten zeigen
        currCharIndex = index;
        characters[currCharIndex].SetActive(true);
        charactersInEditingMenu[currCharIndex].SetActive(true);


        //set Angle von Slider to default (bei uns ist das 190)
        angleSlider.value = 190;
    }

    public void SelectinEditMenu(int index)
    {
        if (index < 0 || index >= characters.Length)
        {
            return;
        }

        //alten Character verbergen
        angleSlider.value = 190;
        secondAngleSlider.value = 190;
        charactersInEditingMenu[tempCharIndex].SetActive(false);

        //neu gewaehlten zeigen
        tempCharIndex = index;
        charactersInEditingMenu[tempCharIndex].SetActive(true);


        //set Angle von Slider to default (bei uns ist das 190)
        secondAngleSlider.value = 190;
        angleSlider.value = 190;

        //falls wir Customizable Avatar waehlen
        if (index == 2)
        {
            notAviableText.gameObject.SetActive(false);
            customizationSettings.gameObject.SetActive(true);
        }
        else
        {
            customizationSettings.gameObject.SetActive(false);
            notAviableText.gameObject.SetActive(true);

        }
    }
}
