using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class ButtonSound : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    /*[SerializeField] private Sprite buttonSprite;
    [SerializeField] private TMP_FontAsset buttonFont;
    private Font buttonFontLegacy;*/

    //===================================================BAUARBEITEn
    private const float widthRatio = 921f / 750f;
    private const float heightRatio = 355f / 150f;

    private Sprite shadowImageSprite; // Спрайт, который будет использоваться для тени и света
    private Sprite buttonSprite;

    private TMP_FontAsset buttonFont;
    private Font buttonFontLegacy;
    //===================================================BAUARBEITEn

    //Kustomisieren von jeden Knopf
    void SetFonts()
    {
        buttonFont = Resources.Load<TMP_FontAsset>("UI/Menu/Fonts/ProductSans-Regular SDF (for TMP)");
        buttonFontLegacy = Resources.Load<Font>("UI/Menu/Fonts/ProductSans-Regular");

        //aendern von Font
        TMP_Text buttonText = GetComponentInChildren<TMP_Text>();

        if (buttonText != null)
        {
            buttonText.font = buttonFont;
            //buttonText.color = Color.white;
        }
        else
        {
            Text buttonTextlegacy = GetComponentInChildren<Text>();
            if (buttonTextlegacy != null)
            {
                buttonTextlegacy.font = buttonFontLegacy;
                //buttonTextlegacy.color = Color.white;
            }
            else
            {
                Debug.LogWarning("Text-Komponent war nicht gefunden!");
            }
        }

    }

    void Start()
    {
        //nur in MainMenu kustomisieren wir Buttons als Glassmorphism
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex == 0)
        {
            SetGlassmorphismOnButton();
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        AudioManager.OnButtonClick?.Invoke();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        AudioManager.OnButtonHover?.Invoke();
    }
    public void SetGlassmorphismOnButton()
    {
        RectTransform buttonRectTransform = GetComponent<RectTransform>();
        buttonSprite = Resources.Load<Sprite>("UI/Menu/ButtonPressed");
        shadowImageSprite = Resources.Load<Sprite>("UI/Menu/ButtonUnpressed");

        //neues Object fuer Image mit dem Schaten
        GameObject shadowImageObj = new GameObject("ShadowImage");


        shadowImageObj.transform.SetParent(transform.parent, false);
        shadowImageObj.transform.SetAsFirstSibling(); //Schaten muss hinter dem Knopf sein

        //neues Objekt muss ein Bild-Komponente haben
        Image shadowImage = shadowImageObj.AddComponent<Image>();
        shadowImage.sprite = shadowImageSprite;
        shadowImage.raycastTarget = false; //diese muss keine Ueberlappungen erzeugen

        // Получить RectTransform новой картинки
        RectTransform shadowRectTransform = shadowImageObj.GetComponent<RectTransform>();

        // Устанавливаем размер картинки на основе текущего размера кнопки
        shadowRectTransform.sizeDelta = new Vector2(
            buttonRectTransform.sizeDelta.x * widthRatio,
            buttonRectTransform.sizeDelta.y * heightRatio
        );

        // Устанавливаем позицию картинки в центр кнопки
        Vector3 buttonPosition = buttonRectTransform.localPosition; // Локальная позиция кнопки относительно родителя
        shadowRectTransform.localPosition = buttonPosition; // Установить тень на ту же позицию, что и кнопка

        // Убедиться, что тень находится прямо позади кнопки, а не перекрывает её
        shadowRectTransform.SetAsFirstSibling(); // Перемещаем тень за кнопку

        //ersetzen auch Sprite auf das Button selbst
        GetComponent<Image>().sprite = buttonSprite;
        SetFonts();
    }
}
