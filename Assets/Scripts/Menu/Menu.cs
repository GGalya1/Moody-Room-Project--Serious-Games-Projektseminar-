using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{ // this class is used to control the activity/visibility of the Menu
    public string menuName; 

    public void Open() // activation of Menu, making it visible
    {
        gameObject.SetActive(true);
    }

    // deactivation of Menu (Alt + Shift + A, if done manually)
    public void Close()
    {
        gameObject.SetActive(false);
    }
}
