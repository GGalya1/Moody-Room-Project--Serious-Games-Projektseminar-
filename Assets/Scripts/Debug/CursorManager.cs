using UnityEngine;
using System;
using UnityEngine.SceneManagement;


// this class controls the visibility of the cursor
public class CursorManager : MonoBehaviour
{
    public static Action<bool> OnCursorVisibilityChange;

    private void Start() // Start is called before the first frame update
    {
        if (SceneManager.GetActiveScene().buildIndex == 0) // if we are in the Main Menu Scene
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else // if we are in the Game Scene
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    void OnEnable()
    { // subscribe to the event
        OnCursorVisibilityChange += SetCursorVisibility;
    }

    void OnDisable()
    { // unsubscribe from the event
        OnCursorVisibilityChange -= SetCursorVisibility;
    }

    private void SetCursorVisibility(bool isVisible)
    {
        Cursor.visible = isVisible;
        if (isVisible)
        {
            Cursor.lockState = CursorLockMode.None; // unlocking the cursor
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked; // locking the cursor
        }
    }
}
