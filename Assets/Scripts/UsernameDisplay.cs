using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class UsernameDisplay : MonoBehaviour
{
    [SerializeField] PhotonView playerPV;
    [SerializeField] TMP_Text nickName;

    void Start()
    {
        //um eigenes Nickname nicht zu sehen
        if (playerPV.IsMine)
        {
            gameObject.SetActive(false);
        }
        nickName.text = playerPV.Owner.NickName;
    }
}
