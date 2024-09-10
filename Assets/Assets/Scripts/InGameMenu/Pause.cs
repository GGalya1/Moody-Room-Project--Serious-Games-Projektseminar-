using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class Pause : MonoBehaviour, IUpdateObserver
{
    //[SerializeField] CanvasGroup canvasGroup;

    public static bool paused = false;
    private bool disconnecting = false;

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
        StartCoroutine(DisconnectPlayer());
        Destroy(RoomManager.instance.gameObject);
        Destroy(GameObject.Find("VoiceManager"));
        //PhotonNetwork.LeaveRoom();
        
        paused = false;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        
    }
    IEnumerator DisconnectPlayer()
    {
        PhotonNetwork.Disconnect();
        while (PhotonNetwork.IsConnected)
        {
            yield return null;
        }
        SceneManager.LoadScene(0);
    }

    public void ObservedUpdate()
    {
        if (disconnecting) return;
        if (Input.GetKeyDown(KeyCode.Escape) && !PhotonChatManager.chatTrigger && !PlayerBoard.playerBoardIsOn)
            paused = !paused;

        if (paused)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            transform.GetChild(0).gameObject.SetActive(true);
        }
        else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Confined;
            transform.GetChild(0).gameObject.SetActive(false);
        }
    }
}
