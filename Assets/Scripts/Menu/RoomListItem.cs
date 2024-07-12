using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Realtime;

//diesen Skript erstellt neue Zimmer in der Lobby-Liste (nimmt von Prefabs)
public class RoomListItem : MonoBehaviour
{
    
    [SerializeField] private TMP_Text roomName;

    public RoomInfo info;
    public void SetUp(RoomInfo roomInfo)
    {
        info = roomInfo;
        roomName.text = info.Name;
    }

    //erstellt eine Verbindung zum gewuenschten Room 
    public void OnClick()
    {
        Launch.instance.JoinRoom(info);
    }

}
