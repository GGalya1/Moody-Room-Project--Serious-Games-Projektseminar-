using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PlayerManager : MonoBehaviour
{
    private PhotonView _photonView;

    GameObject controller;

    void Awake()
    {
        _photonView = GetComponent<PhotonView>();
    }

    // Start is called before the first frame update
    void Start()
    {
        //spawn von Spieler (Player Controller), falls view zu diesem Spieler gehört
        if (_photonView.IsMine)
        {
            CreateController();
        }
    }

    private void CreateController()
    {
        Transform spawnpoint = SpawnManagerScript.instance.GetSpawnpoint();

        string whichPlayer = Launch.instance.playerModel;
        if (whichPlayer == "PlayerController - RedSphere" || whichPlayer == "PlayerController")
        {
            controller = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", Launch.instance.playerModel), spawnpoint.position, spawnpoint.rotation, 0, new object[] { _photonView.ViewID });
        }
        else if (whichPlayer == "CustomizedPlayer")
        {
            Hashtable playerProperties = PhotonNetwork.LocalPlayer.CustomProperties;
            int hatIndex = (int)playerProperties["HatIndex"];
            int eyeIndex = (int)playerProperties["EyeIndex"];
            int bodyIndex = (int)playerProperties["BodyIndex"];
            int clothesIndex = (int)playerProperties["ClothesIndex"];

            int redColorOfHar = Mathf.RoundToInt((int)playerProperties["HatColorR"] / 255f);
            int greenColorOfHat = Mathf.RoundToInt((int)playerProperties["HatColorG"] / 255f);
            int blueColorOfHat = Mathf.RoundToInt((int)playerProperties["HatColorB"] / 255f);

            controller = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", Launch.instance.playerModel), spawnpoint.position, spawnpoint.rotation, 0, 
                new object[] { 
                    _photonView.ViewID, 
                    hatIndex, 
                    eyeIndex, 
                    bodyIndex, 
                    clothesIndex,
                    redColorOfHar,
                    greenColorOfHat,
                    blueColorOfHat});
        }
        else
        {
            Debug.Log("Player type not selected !");
            return;
        }

        

        //BAUARBEITEN !!!
        //controller = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerController"), spawnpoint.position, spawnpoint.rotation, 0, new object[] { _photonView.ViewID});
        //controller = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", Launch.instance.playerModel), spawnpoint.position, spawnpoint.rotation, 0, new object[] { _photonView.ViewID});
        //controller = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", Launch.instance.playerModel), spawnpoint.position, spawnpoint.rotation, 0, new object[] { _photonView.ViewID, hatIndex, eyeIndex, bodyIndex, clothesIndex });
        
        //BAUARBEITEN !!!
    }

    public void Respawn()
    {
        PhotonNetwork.Destroy(controller);
        CreateController();
    }
}
