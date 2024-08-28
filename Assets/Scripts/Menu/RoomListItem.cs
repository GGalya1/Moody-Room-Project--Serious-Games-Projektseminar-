using UnityEngine;
using TMPro;
using Photon.Realtime;
using UnityEngine.UI;

//diesen Skript erstellt neue Zimmer in der Lobby-Liste (nimmt von Prefabs)
public class RoomListItem : MonoBehaviour
{
    
    [SerializeField] private TMP_Text roomName;
    [SerializeField] private Image lockImage;

    public RoomInfo info;
    public void SetUp(RoomInfo roomInfo)
    {
        info = roomInfo;
        roomName.text = info.Name;

        //geben den Hinweis, ob das ein privates oder public Raum
        if (roomInfo.CustomProperties.ContainsKey("RoomCode"))
        {
            lockImage.gameObject.SetActive(true);
        }
        else
        {
            lockImage.gameObject.SetActive(false);
        }
    }

    //erstellt eine Verbindung zum gewuenschten Room 
    public void OnClick()
    {
        Launch.instance.JoinRoom(info);
    }

}
