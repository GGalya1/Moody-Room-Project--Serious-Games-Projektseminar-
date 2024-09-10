using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{
    //speichert Menu, das jetzt ein-/ ausgeschaltet wird
    public string menuName;

    //Menu wird visible
    public void Open()
    {
        gameObject.SetActive(true);
    }

    //dekativierung von Menu (Alt + Shift + A, falls man das per Hand macht)
    public void Close()
    {
        gameObject.SetActive(false);
    }
}
