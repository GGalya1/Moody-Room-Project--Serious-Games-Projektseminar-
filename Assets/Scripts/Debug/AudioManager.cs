using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

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

    [Header("Toggle customize")]
    [SerializeField] private Sprite toggleSprite;

    [Header("Shadow settings")]
    [SerializeField] private Sprite shadowImageSprite;
    [SerializeField] private Sprite buttonUnpressedSprite;

    private float widthScale = 921f / 750f;
    private float heigthScale = 355f / 150f;
    private bool needShadows;

    //sammeln alle Komponenten in Listen, um das spaeteren Bearbeiten zu ermoeglichen
    private List<Button> buttons = new List<Button>();
    private List<InputField> inputFields = new List<InputField>();
    private List<TMP_InputField> tmpInputFields = new List<TMP_InputField>();
    private List<Text> texts = new List<Text>();
    private List<TMP_Text> tmpTexts = new List<TMP_Text>();
    private List<Slider> sliders = new List<Slider>();
    private List<Dropdown> dropdowns = new List<Dropdown>();
    private List<Toggle> toggles = new List<Toggle>();

    //die Liste fuer Schatten, um diese besser zu managen
    private List<GameObject> shadowObjects = new List<GameObject>();


    private void Start()
    {
        needShadows = SceneManager.GetActiveScene().buildIndex == 0;
        //finden alle Komponenten in Canvas
        FindComponentsInCanvas();

        //kustomisieren alle Komponenten
        CustomizeAllComponents();
    }

    void OnEnable()
    {
        OnButtonHover += PlayHoverSound;
        OnButtonClick += PlayClickSound;
    }

    void OnDisable()
    {
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

    private void FindComponentsInCanvas()
    {
        //schreiben alle Komponenten in Listen
        buttons.AddRange(canvas.GetComponentsInChildren<Button>(true));
        inputFields.AddRange(canvas.GetComponentsInChildren<InputField>(true));
        tmpInputFields.AddRange(canvas.GetComponentsInChildren<TMP_InputField>(true));
        texts.AddRange(canvas.GetComponentsInChildren<Text>(true));
        tmpTexts.AddRange(canvas.GetComponentsInChildren<TMP_Text>(true));
        sliders.AddRange(canvas.GetComponentsInChildren<Slider>(true));
        dropdowns.AddRange(canvas.GetComponentsInChildren<Dropdown>(true));
        toggles.AddRange(canvas.GetComponentsInChildren<Toggle>(true));
    }

    private void CustomizeAllComponents()
    {
        CustomizeButtons();
        CustomizeInputFields();
        CustomizeText();
        CustomizeSliders();
        CustomizeDropdowns();
        CustomizeToggles();
    }

    #region CustomizationRegion
    private void CustomizeButtons()
    {
        foreach (Button button in buttons)
        {
            if (button.gameObject.GetComponent<ButtonSound>() == null)
            {
                button.gameObject.AddComponent<ButtonSound>();
            }
        }
    }

    //Kustomisieren von InputField und TMP_InputField
    private void CustomizeInputFields()
    {
        foreach (InputField inputField in inputFields)
        {
            inputField.GetComponent<Image>().sprite = buttonUnpressedSprite;

            inputField.textComponent.font = buttonFontLegacy;
            AddShadow(inputField.GetComponent<RectTransform>());
        }

        foreach (TMP_InputField tmpInputField in tmpInputFields)
        {
            tmpInputField.GetComponent<Image>().sprite = buttonUnpressedSprite;

            tmpInputField.textComponent.font = buttonFont;
            AddShadow(tmpInputField.GetComponent<RectTransform>());
        }
    }

    private void CustomizeText()
    {
        foreach (Text text in texts)
        {
            text.font = buttonFontLegacy;
        }

        foreach (TMP_Text tmpText in tmpTexts)
        {
            tmpText.font = buttonFont;
        }
    }

    private void CustomizeSliders()
    {
        foreach (Slider slider in sliders)
        {
            //finden das erste Bild in Hierarchy von Slider
            slider.GetComponentInChildren<Image>().sprite = fillSprite;

            if (slider.handleRect != null)
            {
                slider.handleRect.GetComponent<Image>().sprite = knobSprite;
            }

            if (slider.fillRect != null)
            {
                slider.fillRect.GetComponent<Image>().sprite = fillSprite;
            }
        }
    }

    private void CustomizeDropdowns()
    {
        foreach (Dropdown dropdown in dropdowns)
        {
            dropdown.GetComponent<Image>().sprite = buttonUnpressedSprite;
            AddShadow(dropdown.GetComponent<RectTransform>());
        }
    }
    private void CustomizeToggles()
    {
        foreach (Toggle toggle in toggles)
        {
            toggle.GetComponentInChildren<Image>().sprite = toggleSprite;
            //AddShadow(toggle.GetComponent<RectTransform>());
        }
    }
    #endregion

    private void AddShadow(RectTransform targetRectTransform)
    {
        if (!needShadows) return;

        //Erstellen ein Objekt namens ShadowImage
        GameObject shadowImageObj = new GameObject("ShadowImage");
        
        //Fuegen Sprite und setzen auf "nicht interaktiv"
        Image shadowImage = shadowImageObj.AddComponent<Image>();
        shadowImage.sprite = shadowImageSprite;
        shadowImage.raycastTarget = false;

        shadowImageObj.transform.SetParent(targetRectTransform.transform.parent, false);

        //Groesse und Position anpassen
        RectTransform shadowRectTransform = shadowImageObj.GetComponent<RectTransform>();
        shadowRectTransform.sizeDelta = new Vector2(
                targetRectTransform.sizeDelta.x * widthScale,
                targetRectTransform.sizeDelta.y * heigthScale
        );

        shadowRectTransform.localPosition = targetRectTransform.localPosition;
        shadowRectTransform.transform.SetAsFirstSibling();

        //Shadow kommt in die gemeinsame Liste
        shadowObjects.Add(shadowImageObj);
        
    }

    public void DisableShadows()
    {
        for (int i = 0; i < shadowObjects.Count; i++)
        {
            shadowObjects[i].SetActive(false);
        }
    }

}
