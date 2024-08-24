using System.Collections;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class Pause : MonoBehaviour
{
    public void Resume()
    {
        IngameMenuManager.OnMenuRequest?.Invoke(MenuType.PauseMenu);
    }

    public void Quit()
    {
        IngameMenuManager.OnMenuRequest?.Invoke(MenuType.PauseMenu);
        StartCoroutine(DisconnectPlayer());
        Destroy(RoomManager.instance.gameObject);
        Destroy(GameObject.Find("VoiceManager"));
        //PhotonNetwork.LeaveRoom();
        
        /*Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;*/
        
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
}
