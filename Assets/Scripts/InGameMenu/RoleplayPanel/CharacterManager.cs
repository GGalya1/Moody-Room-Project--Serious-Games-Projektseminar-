using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class CharacterManager : MonoBehaviour
{
    private CharacterList characterList = new CharacterList();
    [SerializeField] private TMP_Dropdown charactersListDropdown;
    [SerializeField] private RoleplayPanelScript roleplayPanel;

    [Header("Player Info Input Field")]
    [SerializeField] private TMP_InputField charNameIF;
    [SerializeField] private TMP_InputField charRaceIF;
    [SerializeField] private TMP_InputField charClassIF;
    [SerializeField] private TMP_InputField charBackgroundIF;
    [SerializeField] private TMP_InputField charAligmentIF;

    [Header("Player Stats Input Field")]
    [SerializeField] private TMP_InputField strIF;
    [SerializeField] private TMP_InputField dexIF;
    [SerializeField] private TMP_InputField conIF;
    [SerializeField] private TMP_InputField intIF;
    [SerializeField] private TMP_InputField wisIF;
    [SerializeField] private TMP_InputField chaIF;

    [Header("Player secondary Info Input Field")]
    [SerializeField] private TMP_InputField hitpointsIF;
    [SerializeField] private TMP_InputField armorClassIF;
    [SerializeField] private TMP_InputField initiativeIF;
    [SerializeField] private TMP_InputField speedIF;

    [Header("Player Info Display")]
    [SerializeField] private TMP_Text characterName;
    [SerializeField] private TMP_Text characterRace;
    [SerializeField] private TMP_Text characterClass;
    [SerializeField] private TMP_Text characterBackground;
    [SerializeField] private TMP_Text characterAligment;

    [Header("Player Stats Display")]
    [SerializeField] private GameObject strDisplay;
    [SerializeField] private GameObject dexDisplay;
    [SerializeField] private GameObject conDisplay;
    [SerializeField] private GameObject intDisplay;
    [SerializeField] private GameObject wisDisplay;
    [SerializeField] private GameObject chaDisplay;
    [SerializeField] private GameObject maxHpDisplay;
    [SerializeField] private GameObject currentHpDisplay;
    [SerializeField] private GameObject initiativeDisplay;
    [SerializeField] private GameObject armorClassDisplay;
    [SerializeField] private GameObject speedDisplay;



    void Start()
    {
        //Lade bereits erstellte Characteren
        LoadCharacterList();

        // Setze die Input Fields auf numerischen Eingabetyp, damit man dort nur Zahlen schreiben kann
        SetNumericInputFields();

        //Schreibe in Dropdown alle Characteren oder schalte das aus, falls keine Characteren erstellt sind
        FillDropdown();

        //da am Anfang nichts gewaehlt ist. Auch wenn in Dropdown etwas steht, werden Daten in Felder nicht uebernohmen
        ClearCharacterDisplay();
    }
    void SetNumericInputFields()
    {
        strIF.contentType = TMP_InputField.ContentType.IntegerNumber;
        dexIF.contentType = TMP_InputField.ContentType.IntegerNumber;
        conIF.contentType = TMP_InputField.ContentType.IntegerNumber;
        intIF.contentType = TMP_InputField.ContentType.IntegerNumber;
        wisIF.contentType = TMP_InputField.ContentType.IntegerNumber;
        chaIF.contentType = TMP_InputField.ContentType.IntegerNumber;

        hitpointsIF.contentType = TMP_InputField.ContentType.IntegerNumber;
        armorClassIF.contentType = TMP_InputField.ContentType.IntegerNumber;
        initiativeIF.contentType = TMP_InputField.ContentType.IntegerNumber;
        speedIF.contentType = TMP_InputField.ContentType.IntegerNumber;
    }
    void FillDropdown()
    {
        charactersListDropdown.ClearOptions();

        //als erstes kommt Element, um ein Bug zu fixen. Das ist einfach ein Platzhalter
        List<string> characterNames = new List<string> { "None" };

        if (characterList.characters.Count > 0)
        {
            Debug.Log("Dropdown ist sichtbar");
            
            foreach (var character in characterList.characters)
            {
                characterNames.Add(character.name);
            }

            charactersListDropdown.AddOptions(characterNames);
            charactersListDropdown.gameObject.SetActive(true);
        }
        else
        {
            Debug.Log("Habe keine Charackteren gefunden, darum Dropdown nicht sichtbar ist");
            charactersListDropdown.gameObject.SetActive(false);
        }
    }

    //Wird aufgerufen, um Daten ueber Character in JSON zu speichern
    public void CreateCharacter()
    {
        // Neuen Charakter erstellen
        Character newCharacter = new Character();

        // Werte aus den Input Fields in den neuen Charakter übertragen
        newCharacter.name = charNameIF.text;
        newCharacter.race = charRaceIF.text;
        newCharacter.characterClass = charClassIF.text;
        newCharacter.background = charBackgroundIF.text;
        newCharacter.alignment = charAligmentIF.text;

        newCharacter.strength = int.Parse(strIF.text);
        newCharacter.dexterity = int.Parse(dexIF.text);
        newCharacter.constitution = int.Parse(conIF.text);
        newCharacter.intelligence = int.Parse(intIF.text);
        newCharacter.wisdom = int.Parse(wisIF.text);
        newCharacter.charisma = int.Parse(chaIF.text);

        newCharacter.maxHitPoints = int.Parse(hitpointsIF.text);
        newCharacter.currHitPoints = newCharacter.maxHitPoints;
        newCharacter.armorClass = int.Parse(armorClassIF.text);
        newCharacter.initiative = int.Parse(initiativeIF.text);
        newCharacter.speed = int.Parse(speedIF.text);

        // Charakter zur Liste aller lokalen Charakteren hinzufügen
        characterList.characters.Add(newCharacter);

        // Liste der Charaktere in JSON speichern
        SaveCharacterList();

        // Speichere den einzelnen Charakter in PlayerPrefs, um spaeter Daten darueber zu ziehen
        SaveCharacter(newCharacter);

        // Aktualisiere Dropdown
        FillDropdown();
    }

    void SaveCharacterList()
    {
        // Namen der Charaktere in JSON-Format konvertieren
        string json = JsonUtility.ToJson(characterList);
        PlayerPrefs.SetString("Characters", json);
        PlayerPrefs.Save();
    }

    void SaveCharacter(Character character)
    {
        // Einzelnen Charakter in JSON-Format konvertieren und speichern
        string json = JsonUtility.ToJson(character);
        PlayerPrefs.SetString(character.name, json);
        PlayerPrefs.Save();
    }

    void LoadCharacterList()
    {
        // Ueberpruefen, ob die Charakterliste bereits in PlayerPrefs gespeichert ist
        if (PlayerPrefs.HasKey("Characters"))
        {
            string json = PlayerPrefs.GetString("Characters");
            characterList = JsonUtility.FromJson<CharacterList>(json);
        }
        else
        {
            Debug.Log("keinen CharacterList gefunden");
            characterList = new CharacterList(); // Leere Liste, wenn nichts vorhanden ist
        }
    }

    public Character LoadCharacter(string characterName)
    {
        //Checken, ob es bereits ein Character vorhanden ist
        if (PlayerPrefs.HasKey(characterName))
        {
            string json = PlayerPrefs.GetString(characterName);
            return JsonUtility.FromJson<Character>(json);
        }
        else
        {
            Debug.LogWarning("Charakter nicht gefunden: " + characterName);
            return null;
        }
    }

    public void OnCharacterSelected()
    {
        string selectedName = charactersListDropdown.options[charactersListDropdown.value].text;

        if (selectedName == "None")
        {
            //default-Wert, wenn nichts ausgewaehlt ist oder alles geloscht ist
            ClearCharacterDisplay();
            return;
        }
        Debug.Log("SelectedName ist: " + selectedName);
        Character selectedCharacter = LoadCharacter(selectedName);

        if (selectedCharacter != null)
        {
            characterName.text = "Name: " + selectedCharacter.name;
            characterRace.text = "Race: " + selectedCharacter.race;
            characterClass.text = "Class: " + selectedCharacter.characterClass;
            characterBackground.text = "Background: " + selectedCharacter.background;
            characterAligment.text = "Aligment: " + selectedCharacter.alignment;

            //machen Sachen sichtbar
            strDisplay.SetActive(true);
            dexDisplay.SetActive(true);
            conDisplay.SetActive(true);
            intDisplay.SetActive(true);
            wisDisplay.SetActive(true);
            chaDisplay.SetActive(true);
            maxHpDisplay.SetActive(true);
            currentHpDisplay.SetActive(true);
            initiativeDisplay.SetActive(true);
            armorClassDisplay.SetActive(true);
            speedDisplay.SetActive(true);

            //setzen numerische Werte in Prefabs
            strDisplay.GetComponent<CharacterStatScript>().SetUp(selectedCharacter.strength);
            dexDisplay.GetComponent<CharacterStatScript>().SetUp(selectedCharacter.dexterity);
            conDisplay.GetComponent<CharacterStatScript>().SetUp(selectedCharacter.constitution);
            intDisplay.GetComponent<CharacterStatScript>().SetUp(selectedCharacter.intelligence);
            wisDisplay.GetComponent<CharacterStatScript>().SetUp(selectedCharacter.wisdom);
            chaDisplay.GetComponent<CharacterStatScript>().SetUp(selectedCharacter.charisma);
            maxHpDisplay.GetComponent<CharacterStatScript>().SetUp(selectedCharacter.maxHitPoints);
            currentHpDisplay.GetComponent<CharacterStatScript>().SetUp(selectedCharacter.currHitPoints);
            initiativeDisplay.GetComponent<CharacterStatScript>().SetUp(selectedCharacter.initiative);
            armorClassDisplay.GetComponent<CharacterStatScript>().SetUp(selectedCharacter.armorClass);
            speedDisplay.GetComponent<CharacterStatScript>().SetUp(selectedCharacter.speed);

            //Setzen von Werten in RolePlayPanelScript, um das Wuerfeln zu erleichtern
            roleplayPanel.SetStats(selectedCharacter.strength, selectedCharacter.dexterity, selectedCharacter.constitution, selectedCharacter.intelligence, selectedCharacter.wisdom, selectedCharacter.maxHitPoints);

        }
    }

    public void SaveChanges()
    {
        //da "Name:" 6 Zeichen hat und wir loeschen diese
        Character temp = LoadCharacter(characterName.text.Substring(6));
        if (temp == null)
        {
            Debug.Log("Name von Character ist falsch, war nicht gefunden: " + characterName.text);
        }
        else
        {
            Debug.Log("Habe gespeichert alle Changes");
            //lesen aus Textfelder aus
            temp.name = characterName.text.Substring(6);
            temp.race = characterRace.text.Substring(6);
            temp.characterClass = characterClass.text.Substring(7);
            temp.background = characterBackground.text.Substring(12);
            temp.alignment = characterAligment.text.Substring(10);

            //lesen aus Prefabs aus
            Debug.Log("Str: " + strDisplay.GetComponent<CharacterStatScript>().GetCurrentStat());
            temp.strength = strDisplay.GetComponent<CharacterStatScript>().GetCurrentStat();
            temp.dexterity = dexDisplay.GetComponent<CharacterStatScript>().GetCurrentStat();
            temp.constitution = conDisplay.GetComponent<CharacterStatScript>().GetCurrentStat();
            temp.intelligence = intDisplay.GetComponent<CharacterStatScript>().GetCurrentStat();
            temp.wisdom = wisDisplay.GetComponent<CharacterStatScript>().GetCurrentStat();
            temp.charisma = chaDisplay.GetComponent<CharacterStatScript>().GetCurrentStat();
            temp.maxHitPoints = maxHpDisplay.GetComponent<CharacterStatScript>().GetCurrentStat();
            temp.currHitPoints = currentHpDisplay.GetComponent<CharacterStatScript>().GetCurrentStat();
            temp.initiative = initiativeDisplay.GetComponent<CharacterStatScript>().GetCurrentStat();
            temp.armorClass = armorClassDisplay.GetComponent<CharacterStatScript>().GetCurrentStat();
            temp.speed = speedDisplay.GetComponent<CharacterStatScript>().GetCurrentStat();

            SaveCharacter(temp);
            SaveCharacterList();
        }
    }

    public void DeleteCharacter()
    {
        if (charactersListDropdown.options.Count == 0)
        {
            Debug.LogWarning("Keine Charaktere vorhanden, die gelöscht werden können.");
            return;
        }

        // Den aktuell ausgewählten Charakter aus dem Dropdown erhalten
        string selectedCharacterName = charactersListDropdown.options[charactersListDropdown.value].text;

        if (selectedCharacterName == "None")
        {
            //None koenen wir nicht aus der Liste entfernen
            return;
        }

        // Charakter aus der Liste entfernen
        Character characterToRemove = characterList.characters.Find(c => c.name == selectedCharacterName);
        if (characterToRemove != null)
        {
            characterList.characters.Remove(characterToRemove);
        }

        // Charakter aus PlayerPrefs entfernen
        if (PlayerPrefs.HasKey(selectedCharacterName))
        {
            PlayerPrefs.DeleteKey(selectedCharacterName);
            PlayerPrefs.Save();
        }
        else
        {
            Debug.LogWarning("Character war in Dropbox, aber nicht in PlayerPrefs! Name: " + selectedCharacterName);
        }

        // Aktualisiere die Liste in PlayerPrefs
        SaveCharacterList();

        // Dropdown-Menü aktualisieren
        FillDropdown();

        // Optionale Aktion: Leere die Textfelder oder setze sie auf einen Standardwert zurück
        ClearCharacterDisplay();
    }

    void ClearCharacterDisplay()
    {
        characterName.text = "Name: Max Musterman";
        characterRace.text = "Race: none";
        characterClass.text = "Class: universal";
        characterBackground.text = "Background: all";
        characterAligment.text = "Aligment: chaotic good";

        strDisplay.SetActive(false);
        dexDisplay.SetActive(false);
        conDisplay.SetActive(false);
        intDisplay.SetActive(false);
        wisDisplay.SetActive(false);
        chaDisplay.SetActive(false);
        maxHpDisplay.SetActive(false);
        currentHpDisplay.SetActive(false);
        initiativeDisplay.SetActive(false);
        armorClassDisplay.SetActive(false);
        speedDisplay.SetActive(false);
    }
}

