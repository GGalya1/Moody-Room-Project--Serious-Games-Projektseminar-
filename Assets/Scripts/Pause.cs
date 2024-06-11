using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class Pause : MonoBehaviour
{
    //[SerializeField] CanvasGroup canvasGroup;

    public static bool paused = false;
    private bool disconnecting = false;


    /*public void TogglePause()
    {
        if (disconnecting) return;
        paused = !paused;

        //transform.GetChild(0).gameObject.SetActive(paused);
        Cursor.lockState = (paused) ? CursorLockMode.None : CursorLockMode.Confined;
    }*/

    public void Resume()
    {
        paused = !paused;
        //canvasGroup.alpha = 0;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
        
        transform.GetChild(0).gameObject.SetActive(false);
    }

    public void Quit()
    {
        disconnecting = true;
        Destroy(RoomManager.instance.gameObject);
        PhotonNetwork.LeaveRoom();
        paused = false;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        PhotonNetwork.LoadLevel(0);
    }

    private void Update()
    {
        if (disconnecting) return;
        if (Input.GetKeyDown(KeyCode.Escape) && !PhotonChatManager.chatTrigger && !PlayerBoard.playerBoardIsOn)
            paused = !paused;

        if (paused)
        {
            Cursor.visible = true;
            //Debug.Log("Cursor must be visible, but: " + Cursor.visible);
            Cursor.lockState = CursorLockMode.None;
            transform.GetChild(0).gameObject.SetActive(true);
        }
        else
        {
            Cursor.visible = false;
            //Debug.Log("Cursor must not be visible, but: " + Cursor.visible);
            Cursor.lockState = CursorLockMode.Confined;
            transform.GetChild(0).gameObject.SetActive(false);
        }
        

        /*
        if (paused)
        {
            canvasGroup.alpha = 1;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            canvasGroup.alpha = 0;
            Cursor.lockState = CursorLockMode.Confined;
        }
        */
    }
}
