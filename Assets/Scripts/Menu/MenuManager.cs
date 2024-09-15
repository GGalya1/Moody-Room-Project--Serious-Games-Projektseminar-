using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    // singelton pattern - access to the MenuManager from any script
    public static MenuManager current;

    // list of all menus
    [SerializeField] private List<Menu> _menus;

    public void OpenMenu(string nameOfMenu)
    {
        foreach (Menu menu in _menus)
        {
            if (menu.menuName == nameOfMenu)
            {
                menu.Open();
            }
            else
            {
                menu.Close();
            }
        }
    }
    public Menu GetActiveMenu()
    {
        foreach (Menu menu in _menus)
        {
            if (menu.gameObject.activeSelf) return menu;
        }
        return null; // if no menu is active
    }


    // singleton pattern allows to access the MenuManager from any script
    private void Awake() // Awake is called when the script instance is being loaded
    {
        current = this;
    }
}
