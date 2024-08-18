using System.Collections;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class Pause : MonoBehaviour, IUpdateObserver
{
    private bool disconnecting = false;

    #region UpdateManager connection
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

    public void Resume()
    {
        IngameMenuManager.OnMenuRequest?.Invoke(MenuType.PauseMenu);
    }

    public void Quit()
    {
        disconnecting = true;
        StartCoroutine(DisconnectPlayer());
        Destroy(RoomManager.instance.gameObject);
        Destroy(GameObject.Find("VoiceManager"));
        //PhotonNetwork.LeaveRoom();
        
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
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            IngameMenuManager.OnMenuRequest?.Invoke(MenuType.PauseMenu);
        }
    }
}
