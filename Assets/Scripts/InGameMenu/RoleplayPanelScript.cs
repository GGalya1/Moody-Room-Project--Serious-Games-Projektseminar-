using UnityEngine;

public class RoleplayPanelScript : MonoBehaviour
{
    [SerializeField] private GameObject panel; //Panel hat keine eigene Klasse, darum GameObjekt
    [SerializeField] private GameObject diceRollerPanel;

    public void OpenDiceRoller()
    {
        if (panel.activeSelf)
        {
            diceRollerPanel.SetActive(true);
        }
    }
}
