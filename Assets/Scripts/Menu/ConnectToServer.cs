using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ConnectToServer : MonoBehaviourPunCallbacks
{
    public InputField username;
    public Text buttonText;

    //erstellen eine Verbindung zum Server
    public void OnClickConnect()
    {
        if(username.text.Length >= 1)
        {
            PhotonNetwork.NickName = username.text;
            buttonText.text = "Connecting...";
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    //wechseln Scene zum Lobby, nachdem eine Anmeldung passiert hat
    public override void OnConnectedToMaster()
    {
        SceneManager.LoadScene("Lobby");
    }
}
