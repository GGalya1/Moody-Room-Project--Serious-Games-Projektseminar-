using UnityEngine;
using System;
using UnityEngine.SceneManagement;

//diese Classe steuert die Sichtbarkeit von Cursor
public class CursorManager : MonoBehaviour
{
    public static Action<bool> OnCursorVisibilityChange;

    //damit wir am Anfang der Scene Cursor sehen/nicht sehen
    private void Start()
    {
        //falls wir in MainMenu sind
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        //falls wir in GameScene sind
        else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    void OnEnable()
    {
        // Abonniere das Event
        OnCursorVisibilityChange += SetCursorVisibility;
    }

    void OnDisable()
    {
        // Trenne das Event
        OnCursorVisibilityChange -= SetCursorVisibility;
    }

    private void SetCursorVisibility(bool isVisible)
    {
        Cursor.visible = isVisible;
        if (isVisible)
        {
            Cursor.lockState = CursorLockMode.None; // Entsperrt den Cursor
            Debug.Log("Cursor visibility: " + Cursor.visible + "  Cursor lock: " + Cursor.lockState);
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked; // Sperrt den Cursor
            Debug.Log("Cursor visibility: " + Cursor.visible + "  Cursor lock: " + Cursor.lockState);
        }
    }
}
