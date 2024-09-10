using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Dice : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private TMP_Text dicedNumber;
    [SerializeField] private TMP_Text typeOfDice;
    private int sides;
    private Image colorOfDice;
    private RollDice rollDice;
    public int number; 

    //implementing Drag&Drop
    private Canvas canvas;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        canvas = GetComponentInParent<Canvas>();
    }

    public void Initialize(int sides)
    {
        rollDice = GetComponentInParent<RollDice>();
        this.sides = sides;
        colorOfDice = GetComponent<Image>();
        typeOfDice.text = $"1d{sides}";
        Reroll();
        RecolorDice(sides);
    }

    public void Reroll()
    {
        rollDice.sum -= number;

        number = Random.Range(1, sides + 1);
        dicedNumber.text = number.ToString();
        rollDice.sum += number;
        rollDice.sumOfDices.text = $"Sum: {rollDice.sum}";
    }

    private void RecolorDice(int sides)
    {
        if (sides == 20)
        {
            colorOfDice.color = Color.cyan;
        }
        else if(sides == 12)
        {
            colorOfDice.color = Color.blue;
        }
        else if (sides == 10)
        {
            colorOfDice.color = Color.green;
        }
        else if (sides == 8)
        {
            colorOfDice.color = Color.yellow;
        }
        else if (sides == 6)
        {
            colorOfDice.color = Color.white;
        }
        else if(sides == 4)
        {
            colorOfDice.color = Color.green;
        }
        else
        {
            colorOfDice.color = Color.red;
        }
    }


    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
    }
}
