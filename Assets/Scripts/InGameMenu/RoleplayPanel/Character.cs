using System.Collections.Generic;

// Klasse fuer die Charaktereigenschaften, die in json gespeichert sein koenten
[System.Serializable]
public class Character
{
    //Hauptcharakteristiken
    public int strength;
    public int dexterity;
    public int constitution;
    public int intelligence;
    public int wisdom;
    public int charisma;

    //Zusaetzliche Charakteristiken
    public int currHitPoints;
    public int maxHitPoints;
    public int armorClass;
    public int initiative;
    public int speed;

    //Character Infos
    public string name;
    public string race;
    public string characterClass;
    public string alignment;
    public int level;
    public string background;
}

// Klasse, die eine Liste von allen Charakteren enthaelt (die lokal erstellt waren)
[System.Serializable]
public class CharacterList
{
    public List<Character> characters = new List<Character>();
}

