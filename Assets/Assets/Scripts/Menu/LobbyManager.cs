using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    public InputField roomInputField;
    public GameObject lobbyPanel;
    public GameObject roomPanel;
    public Text roomName;

    private void Start()
    {
        PhotonNetwork.JoinLobby();
    }

    //erstellen ein Lobby, falls Name nicht leer ist
    public void OnClickCreate()
    {
        if(roomInputField.text.Length >= 1)
        {
            PhotonNetwork.CreateRoom(roomInputField.text, new RoomOptions() { MaxPlayers = 12});
        }
    }

    //wird aufgerufen, wenn wir ein neues Room erstellen/ beitreten. Wird Scene in der Variable "lobbyPanel" ausblenden
    //und "roomPanel" einblenden
    public override void OnJoinedRoom()
    {
        lobbyPanel.SetActive(false);
        roomPanel.SetActive(true);
        roomName.text = "Name von deinem Lobby ist... " + PhotonNetwork.CurrentRoom.Name + "!";
    }
}
