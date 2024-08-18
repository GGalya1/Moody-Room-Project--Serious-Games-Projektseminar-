using UnityEngine;
using System;

public class IngameMenuManager : MonoBehaviour
{
    public static Action<MenuType> OnMenuRequest; // Event fuer MenuAnfragen

    private static MenuType currentOpenedMenu = MenuType.None;

    [SerializeField] private GameObject _adminMenu;
    [SerializeField] private GameObject _whiteboardMenu;
    [SerializeField] private GameObject _pauseMenu;
    [SerializeField] private GameObject _roleplayMenu;
    [SerializeField] private GameObject _chatMenu;
    [SerializeField] private GameObject _playerlistMenu;

    private void OnEnable()
    {
        OnMenuRequest += HandleMenuRequest;
    }

    private void OnDisable()
    {
        OnMenuRequest -= HandleMenuRequest;
    }

    //checkt Bedingungen, um ein Menu zu oeffnen
    private void HandleMenuRequest(MenuType requestedMenu)
    {
        // Wenn kein Menü geöffnet ist
        if (currentOpenedMenu == MenuType.None)
        {
            CloseMenu(currentOpenedMenu);
            currentOpenedMenu = requestedMenu;
            OpenMenu(currentOpenedMenu);
        }
        //schliessen von bereits geoffneten Menus
        else if (currentOpenedMenu == requestedMenu)
        {
            CloseMenu(currentOpenedMenu);
            currentOpenedMenu = MenuType.None;
        }
    }

    //oefnet angefragtes Menu
    private void OpenMenu(MenuType requestedMenu)
    {
        CursorManager.OnCursorVisibilityChange?.Invoke(true);
        if (requestedMenu == MenuType.AdminMenu)
        {
            _adminMenu.SetActive(true);
        }
        else if (requestedMenu == MenuType.ChatMenu)
        {
            _chatMenu.SetActive(true);
        }
        else if (requestedMenu == MenuType.PauseMenu)
        {
            _pauseMenu.SetActive(true);
        }
        else if (requestedMenu == MenuType.RoleplayMenu)
        {
            _roleplayMenu.SetActive(true);
        }
        else if (requestedMenu == MenuType.WhiteBoardMenu)
        {
            _whiteboardMenu.SetActive(true);
        }
        else if (requestedMenu == MenuType.PlayerlistMenu)
        {
            _playerlistMenu.SetActive(true);
        }
    }

    //schliest Menu
    private void CloseMenu(MenuType requestedMenu)
    {
        CursorManager.OnCursorVisibilityChange?.Invoke(false);
        if (requestedMenu == MenuType.AdminMenu)
        {
            _adminMenu.SetActive(false);
        }
        else if (requestedMenu == MenuType.ChatMenu)
        {
            _chatMenu.SetActive(false);
        }
        else if (requestedMenu == MenuType.PauseMenu)
        {
            _pauseMenu.SetActive(false);
        }
        else if (requestedMenu == MenuType.RoleplayMenu)
        {
            _roleplayMenu.SetActive(false);
        }
        else if (requestedMenu == MenuType.WhiteBoardMenu)
        {
            _whiteboardMenu.SetActive(false);
        }
        else if (requestedMenu == MenuType.PlayerlistMenu)
        {
            _playerlistMenu.SetActive(false);
        }
    }

    public static MenuType GetCurrentMenu()
    {
        return currentOpenedMenu;
    }

}

public enum MenuType
{
    None,
    AdminMenu,
    WhiteBoardMenu,
    PauseMenu,
    RoleplayMenu,
    ChatMenu,
    PlayerlistMenu
}
