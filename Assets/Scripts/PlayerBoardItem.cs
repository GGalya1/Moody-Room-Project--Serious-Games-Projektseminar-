using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Realtime;

public class PlayerBoardItem : MonoBehaviour
{
    public TMP_Text usernameText;

    public void Initialize(Player player)
    {
        if (player.IsMasterClient)
        {
            usernameText.text = player.NickName + " (admin)";
        }
        else
        {
            usernameText.text = player.NickName;
        }
        
    }
}
