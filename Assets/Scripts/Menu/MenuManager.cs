using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    //benutzte Pattern - singelton. Kein Fan davon, aber folge aktuell tutorial
    public static MenuManager current;

    //die Liste mit allen Menus, die wir haben
    [SerializeField] private List<Menu> _menus;

    public void OpenMenu(string nameOfMenu)
    {
        //god, vorgive me for this 
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


    //damit MenuManager von aussen fuer jeden Script erreichbar ist
    private void Awake()
    {
        current = this;
    }
}
