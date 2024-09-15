using UnityEngine;
using TMPro;
using Photon.Realtime;
using UnityEngine.UI;

/// <summary>
/// The RoomListItem class is responsible for creating and setting up new room items in the lobby list.
/// It displays room information like the room name and whether it's private or not.
/// </summary>
public class RoomListItem : MonoBehaviour
{
    
    [SerializeField] private TMP_Text roomName;
    [SerializeField] private Image lockImage;

    public RoomInfo info; //stores the room's details
    public void SetUp(RoomInfo roomInfo)
    {
        info = roomInfo;
        roomName.text = info.Name;

        //shows if the room is private or public
        if (roomInfo.CustomProperties.ContainsKey("RoomCode"))
        {
            lockImage.gameObject.SetActive(true);
        }
        else
        {
            lockImage.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Handles the event when the player clicks on the room list item.
    /// It sends a request to join the selected room using the stored room information.
    /// </summary>
    public void OnClick()
    {
        Launch.instance.JoinRoom(info);
    }

}
