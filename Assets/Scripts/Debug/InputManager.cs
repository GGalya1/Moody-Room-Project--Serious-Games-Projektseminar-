using UnityEngine;
using Photon.Pun;

public class InputManager : MonoBehaviour, IUpdateObserver
{
    [SerializeField] private RoomSettingManager roomSettingManager;
    public static bool whiteboardIsOn = true;

    #region UpdateManager connection
    private void Awake()
    {
        UpdateManager.Instance.RegisterObserver(this);
    }
    private void OnEnable()
    {
        UpdateManager.Instance.RegisterObserver(this);
        UpdateManager.Instance.RegisterObserverName("InputManager");
    }
    private void OnDisable()
    {
        UpdateManager.Instance.UnregisterObserver(this);
        UpdateManager.Instance.UnregisterOberverName("InputManager");
    }
    private void OnDestroy()
    {
        UpdateManager.Instance.UnregisterObserver(this);
        UpdateManager.Instance.UnregisterOberverName("InputManager");
    }
    #endregion

    public void ObservedUpdate()
    {
        //Öffnen von PauseMenu
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            IngameMenuManager.OnMenuRequest?.Invoke(MenuType.PauseMenu);
        }
        //Öffnen von Whiteboard
        if (Input.GetKeyDown(KeyCode.RightAlt) && whiteboardIsOn)
        {
            IngameMenuManager.OnMenuRequest?.Invoke(MenuType.WhiteBoardMenu);
        }
        //Öffnen von RoleplayPanel
        if (Input.GetKeyDown(KeyCode.LeftAlt))
        {
            IngameMenuManager.OnMenuRequest?.Invoke(MenuType.RoleplayMenu);
        }
        //Öffnen von AdminPanel
        if (PhotonNetwork.IsMasterClient && Input.GetKeyDown(KeyCode.RightControl))
        {
            IngameMenuManager.OnMenuRequest?.Invoke(MenuType.AdminMenu);
        }
        //Öffnen und Schliessen von "Tab" == PlayerList (aka Overlay)
        if (Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyUp(KeyCode.Tab))
        {
            IngameMenuManager.OnMenuRequest?.Invoke(MenuType.PlayerlistMenu);
        }
        //Öffnen von Chat
        if (Input.GetKeyDown(KeyCode.LeftControl) && roomSettingManager.chatIsOn)
        {
            IngameMenuManager.OnMenuRequest?.Invoke(MenuType.ChatMenu);
        }
        //Interaction durchfuehren. Bzw das Stuhl beitreten
        if (Input.GetKeyDown(KeyCode.E) && IngameMenuManager.GetCurrentMenu() == MenuType.None)
        {
            IngameMenuManager.OnMenuRequest?.Invoke(MenuType.ChairMenu);
        }
        //Das Stuhl verlassen
        if (Input.GetKeyDown(KeyCode.LeftAlt) && IngameMenuManager.GetCurrentMenu() == MenuType.ChairMenu)
        {
            IngameMenuManager.OnMenuRequest?.Invoke(MenuType.ChairMenu);
        }
    }
}
