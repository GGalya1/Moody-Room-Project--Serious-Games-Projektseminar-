using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

/// <summary>
/// This class is responsible for managing and storing persistent the player's username in a multiplayer and between sessions.
/// </summary>
public class PlayerNameManager : MonoBehaviour
{
    [SerializeField] TMP_InputField usernameInputFromField;

    private void Start()
    {
        if (PlayerPrefs.HasKey("username"))
        {
            usernameInputFromField.text = PlayerPrefs.GetString("username");
            PhotonNetwork.NickName = PlayerPrefs.GetString("username"); 
        }
        else
        {
            usernameInputFromField.text = "Gast " + Random.Range(0, 2000).ToString("0000");
            OnUsernameInputValueChanged();
        }
    }

    public void OnUsernameInputValueChanged()
    {
        PhotonNetwork.NickName = usernameInputFromField.text;
        PlayerPrefs.SetString("username", usernameInputFromField.text);
    }
}
