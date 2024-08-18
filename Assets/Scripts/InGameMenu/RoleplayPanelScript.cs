using UnityEngine;

public class RoleplayPanelScript : MonoBehaviour, IUpdateObserver
{
    [SerializeField] private GameObject panel; //Panel hat keine eigene Klasse, darum GameObjekt
    [SerializeField] private GameObject diceRollerPanel;

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
        if (Input.GetKeyDown(KeyCode.LeftAlt))
        {
            IngameMenuManager.OnMenuRequest?.Invoke(MenuType.RoleplayMenu);
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
