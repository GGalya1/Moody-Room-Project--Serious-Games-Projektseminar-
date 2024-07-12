using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RollDice : MonoBehaviour
{
    [SerializeField] private RectTransform table;
    [SerializeField] private GameObject dicePrefab;

    [SerializeField] public TMP_Text sumOfDices;
    [SerializeField] public TMP_Text modificatorText;

    [SerializeField] TMP_InputField modificatorInputField;

    List<GameObject> dices = new List<GameObject>();
    public int sum;
    public int modificator;
    //public int oldModificator;

    private void Start()
    {
        sum = 0;
        sumOfDices.text = "Sum: 0";
        modificatorText.text = "Modificator: 0";

        modificatorInputField.onEndEdit.AddListener(OnInputFieldChanged);
    }

    public void CreateDice(int sides)
    {
        GameObject temp = Instantiate(dicePrefab, table);
        dices.Add(temp);
        Dice dice = temp.GetComponent<Dice>();
        dice.Initialize(sides);
    }

    public void RerollAll()
    {
        if (dices == null)
        {
            Debug.LogWarning("List of dices is null !");
            return;
        }
        for (int i = 0; i < dices.Count; i++)
        {
            dices[i].GetComponent<Dice>().Reroll();
        }
        RecalculateSum();
    }

    public void RemoveAll()
    {
        if (dices == null)
        {
            Debug.LogWarning("List of dices is null !");
            return;
        }
        for (int i = 0; i < dices.Count; i++)
        {
            Destroy(dices[i]);
        }
        dices = new List<GameObject>();

        sum = 0;
        sumOfDices.text = "Sum: 0";
    }

    public void RecalculateSum()
    {
        sum = 0;
        for (int i = 0; i < dices.Count; i++)
        {
            sum += dices[i].GetComponent<Dice>().number;
        }
        sumOfDices.text = $"Sum: {sum}";
    }

    private void OnInputFieldChanged(string input)
    {
        sum -= modificator;
        if (int.TryParse(input, out modificator))
        {
            Debug.Log("Valid number entered: " + modificator);
            modificatorText.text = $"Modificator is: {modificator}";
            sum += modificator;
            sumOfDices.text = $"Sum: {sum}";
        }
        else
        {
            Debug.LogError("Invalid number entered.");
        }
    }
}
