using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CharacterStatScript : MonoBehaviour
{
    [SerializeField] private Button decreaseButton;
    [SerializeField] private Button increaseButton;
    [SerializeField] private TMP_Text stat;
    private int value;

    public void SetUp(int _stat)
    {
        value = _stat;
        stat.text = value.ToString();
    }

    public void IncreaseValue()
    {
        value++;
        stat.text = value.ToString();
    }
    public void DecreaseValue()
    {
        value--;
        stat.text = value.ToString();
    }

    public int GetCurrentStat()
    {
        return value;
    }
}
