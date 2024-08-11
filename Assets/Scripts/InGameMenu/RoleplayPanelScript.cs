using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoleplayPanelScript : MonoBehaviour, IUpdateObserver
{
    [SerializeField] private GameObject panel; //Panel hat keine eigene Klasse, darum GameObjekt
    [SerializeField] private GameObject diceRollerPanel;
    public static bool roleplayPanelIsOn;

    #region UpdateManager connection
    private void Awake()
    {
        UpdateManager.Instance.RegisterObserver(this);
    }
    private void OnEnable()
    {
        UpdateManager.Instance.RegisterObserver(this);
    }
    private void OnDisable()
    {
        UpdateManager.Instance.UnregisterObserver(this);
    }
    private void OnDestroy()
    {
        UpdateManager.Instance.UnregisterObserver(this);
    }
    #endregion

    public void ObservedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.LeftAlt) && !Pause.paused)
        {
            roleplayPanelIsOn = !roleplayPanelIsOn;
            panel.gameObject.SetActive(roleplayPanelIsOn);
            Cursor.visible = roleplayPanelIsOn;
            if (roleplayPanelIsOn)
            {
                Cursor.lockState = CursorLockMode.None;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Confined;
            }

        }
    }

    public void OpenDiceRoller()
    {
        if (panel.activeSelf)
        {
            diceRollerPanel.SetActive(true);
        }
    }
}
