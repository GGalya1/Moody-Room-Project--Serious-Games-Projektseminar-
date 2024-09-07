using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AudioManager : MonoBehaviour
{
    public static Action OnButtonHover;
    public static Action OnButtonClick;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private Canvas canvas;

    [Header("Sound clips")]
    [SerializeField] private AudioClip hoverSound;
    [SerializeField] private AudioClip clickSound;

    [Header("Fonts")]
    [SerializeField] private TMP_FontAsset buttonFont;
    [SerializeField] private Font buttonFontLegacy;

    [Header("Slider customize")]
    [SerializeField] private Sprite knobSprite;
    [SerializeField] private Sprite fillSprite;

    private void Start()
    {
        AddSoundToButtons(canvas.transform);
    }

    void OnEnable()
    {
        // Abonniere das Event
        OnButtonHover += PlayHoverSound;
        OnButtonClick += PlayClickSound;
    }

    void OnDisable()
    {
        // Trenne das Event
        OnButtonHover -= PlayHoverSound;
        OnButtonClick -= PlayClickSound;
    }

    private void PlayHoverSound()
    {
        if (hoverSound != null)
        {
            audioSource.PlayOneShot(hoverSound);
        }
    }
    private void PlayClickSound()
    {
        if (clickSound != null)
        {
            audioSource.PlayOneShot(clickSound);
        }
    }

    //methode, die rekursiv alle Kinder durchgeht und findet GameObjects mit dem Komponent "Button"
    //fuegt zu diesem Objekt neuen Komponent, der OnClick eine Anfrage an diesem Script senden wird
    void AddSoundToButtons(Transform parent)
    {
        foreach (Transform child in parent)
        {
            //ueberprüfe, ob das Kind eine Button-Komponente hat
            Button button = child.GetComponent<Button>();
            if (button != null)
            {
                //fuege das ButtonSound-Komponent hinzu, falls es nicht bereits vorhanden ist
                if (child.GetComponent<ButtonSound>() == null)
                {
                    child.gameObject.AddComponent<ButtonSound>();
                }

                //wenn ein Button gefunden wurde, nicht weiter tiefer in diese Hierarchie gehen (da kein Sinn macht, falls Button in sich weitere Buttons enthaelt)
                continue;
            }

            //erstetzen Font in allen Komponenten, wo es moeglich ist
            //checken nach InputField-Komponent
            InputField inputField = child.GetComponent<InputField>();
            if (inputField != null)
            {
                inputField.textComponent.font = buttonFontLegacy;
                continue;
            }

            //nach TMP_InputField
            TMP_InputField tmpInputField = child.GetComponent<TMP_InputField>();
            if (tmpInputField != null)
            {
                tmpInputField.textComponent.font = buttonFont;
                continue;
            }

            //nach (Text)
            Text text = child.GetComponent<Text>();
            if (text != null)
            {
                text.font = buttonFontLegacy;
                continue;
            }

            //nach TMP_Text
            TMP_Text tmpText = child.GetComponent<TMP_Text>();
            if (tmpText != null)
            {
                tmpText.font = buttonFont;
                continue;
            }

            //Kustomizieren Sliders
            Slider slide = child.GetComponent<Slider>();
            if (slide != null)
            {
                //diese Zeile ersetzt "Background"-Image. Kann instabil sein
                slide.GetComponentInChildren<Image>().sprite = fillSprite;

                slide.handleRect.GetComponent<Image>().sprite = knobSprite;
                slide.fillRect.GetComponent<Image>().sprite = fillSprite;
                continue;
            }

            //Kustomizieren Dropdowns
            Dropdown drop = child.GetComponent<Dropdown>();
            if (drop != null)
            {
                SetNeumorphismOnDropdown(drop);
            }
            //rekursiven Aufruf der Methode, um auch die Kinder des aktuellen Kindes zu ueberpruefen
            if (child.childCount > 0)
            {
                AddSoundToButtons(child);
            }
        }
    }

    private void SetNeumorphismOnDropdown(Dropdown dropdown)
    {
        RectTransform dropdownRectTransform = dropdown.GetComponent<RectTransform>();

        //Sprites fuer das Dropdown und die Schatten
        Sprite dropdownSprite = Resources.Load<Sprite>("UI/Menu/ButtonPressed");
        Sprite shadowImageSprite = Resources.Load<Sprite>("UI/Menu/ButtonUnpressed");

        //erstelle das Schattenbild
        GameObject shadowImageObj = new GameObject("ShadowImage");
        shadowImageObj.transform.SetParent(dropdown.transform.parent, false);
        shadowImageObj.transform.SetAsFirstSibling();

        //fuege das Bild-Component hinzu und setze das Schatten-Sprite
        Image shadowImage = shadowImageObj.AddComponent<Image>();
        shadowImage.sprite = shadowImageSprite;
        shadowImage.raycastTarget = false;

        // Setze die Größe des Schattens basierend auf der Dropdown-Größe
        RectTransform shadowRectTransform = shadowImageObj.GetComponent<RectTransform>();
        shadowRectTransform.sizeDelta = new Vector2(
            dropdownRectTransform.sizeDelta.x * (921f / 750f),
            dropdownRectTransform.sizeDelta.y * (355f / 150f)
        );

        // Setze die Position des Schattens
        shadowRectTransform.localPosition = dropdownRectTransform.localPosition;

        // Der Schatten muss hinter dem Dropdown sein
        shadowRectTransform.SetAsFirstSibling();

        // Ersetze das Dropdown-Sprite
        Image dropdownImage = dropdown.GetComponent<Image>();
        dropdownImage.sprite = dropdownSprite;
    }
}
