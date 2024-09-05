using UnityEngine;
using UnityEngine.UI;

public class RoleplayPanelScript : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject panel; //Panel hat keine eigene Klasse, darum GameObjekt
    [SerializeField] private GameObject diceRollerPanel;
    [SerializeField] private GameObject characterSheetPanel;
    [SerializeField] private GameObject createCharacterPanel;

    [Header("Character Sheet")]
    [SerializeField] private Dropdown characterDropdown;
    private int[] characterStats = new int[6];
    [SerializeField] private RollDice rollScript;
    /*
     * Indizen fuer Characteristiken:
     * STR - 0 (strength)
     * DEX - 1 (dexterity)
     * CON - 2 (constitution)
     * INT - 3 (intellect)
     * WIS - 4 (wisdom)
     * CHA - 5 (charisma)
     */

    void Start()
    {
        characterDropdown.onValueChanged.AddListener(OnDropdownValueChanged);
    }

    public void OpenDiceRoller()
    {
        if (panel.activeSelf)
        {
            characterSheetPanel.SetActive(false);
            createCharacterPanel.SetActive(false);
            diceRollerPanel.SetActive(true);
        }
    }
    public void OpenCharacterSheet()
    {
        diceRollerPanel.SetActive(false);
        createCharacterPanel.SetActive(false);
        characterSheetPanel.SetActive(true);
    }
    public void OpenCreateCharacter()
    {
        diceRollerPanel.SetActive(false);
        characterSheetPanel.SetActive(false);
        createCharacterPanel.SetActive(true);
    }

    public void OnDropdownValueChanged(int selectedIndex)
    {
        // Liste der möglichen Stat-Werte des Charakters
        int selectedStatValue = characterStats[selectedIndex];

        // Berechne den Modifikator
        int modifikator = CalculateModifier(selectedStatValue);

        // Zeige den Modifikator im UI-Textfeld an
        rollScript.modificator = modifikator;
        rollScript.modificatorText.text = "Modifikator is: " + modifikator;

        rollScript.RecalculateSum();
    }

    // Hilfsfunktion zur Berechnung des Modifikators
    int CalculateModifier(int statValue)
    {
        return (statValue - 10) / 2;
    }

    public void SetStats(int str, int dex, int con, int _int, int wis, int cha)
    {
        characterStats[0] = str;
        characterStats[1] = dex;
        characterStats[2] = con;
        characterStats[3] = _int;
        characterStats[4] = wis;
        characterStats[5] = cha;
    }
}
