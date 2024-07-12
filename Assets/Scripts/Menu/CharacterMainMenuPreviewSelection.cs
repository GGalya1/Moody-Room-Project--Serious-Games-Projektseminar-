using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterMainMenuPreviewSelection : MonoBehaviour
{
    public GameObject[] characters;
    public Slider angleSlider;
    private int currCharIndex = 0;

    void Start()
    {
        //sichtbar ist nur "default" Character
        for (int i = 0; i < characters.Length; i++)
        {
            characters[i].SetActive(i == currCharIndex);
        }

        //Ab dem Punkt werden alle Veraenderungen von Slider ubernohmen
        angleSlider.onValueChanged.AddListener(OnSliderValueChanged);
    }

    //wird aufgerugen jeden Mal, wenn eine Veranderung vorliegt
    void OnSliderValueChanged(float value)
    {
        characters[currCharIndex].transform.rotation = Quaternion.Euler(0, value, 0);
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

        //neu gewaehlten zeigen
        currCharIndex = index;
        characters[currCharIndex].SetActive(true);

        //set Angle von Slider to default (bei uns ist das 190)
        angleSlider.value = 190;
    }
}
