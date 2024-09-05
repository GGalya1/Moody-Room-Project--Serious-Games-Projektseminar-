using UnityEngine;
using Photon.Pun;
using System.IO;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using UnityEngine.SceneManagement;
using Photon.Realtime;
using System.Collections;

public class PlayerManager : MonoBehaviourPunCallbacks
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
            StartCoroutine(WaitForConnectionAndInstantiatePlayer());            
        }
    }
    private IEnumerator WaitForConnectionAndInstantiatePlayer()
    {
        // Warte, bis der Spieler vollständig mit dem Raum verbunden ist. Sonst wird PlayerAvatar gespawnet, der genuaso wie deinen aussieht (obwohl bei anderen es anders sein konnte)
        while (PhotonNetwork.NetworkClientState != ClientState.Joined)
        {
            yield return null; // wartet einen Frame und überprüft dann erneut
        }

        CreateController();

        if (controller != null)
        {
            //damit man eigenes Model nicht sieht (hier bekommt man eine Liste von allen Objekten + PlayerAvatar selbst)
            MeshRenderer[] renderers = controller.GetComponentsInChildren<MeshRenderer>(true);

            //Deaktivieren MeshRenderer von Aktiven Objekten (weil nur aktive Objekten unserer Avatar reprasentieren)
            foreach (MeshRenderer renderer in renderers)
            {
                if (renderer.gameObject.activeInHierarchy)
                {
                    renderer.enabled = false;
                }
            }

            //setzen von Kameras, damit Portals korrekt funktioniert
            SetCamerasForPortals();
        }
        else
        {
            Debug.LogWarning("Kein Kontroller gefunden! MeshRender wird nicht deaktiviert");
        }
    }

    private void CreateController()
    {
        Transform spawnpoint = SpawnManagerScript.instance.GetSpawnpoint();

        string whichPlayer = Launch.instance.playerModel;
        //Debug.LogError(whichPlayer);
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

            int redColorOfHar = (int)playerProperties["HatColorR"];
            int greenColorOfHat = (int)playerProperties["HatColorG"];
            int blueColorOfHat = (int)playerProperties["HatColorB"];


            //zu diesem Zeitpunkt ist unserem Prefab bereits so kustomisiert, wie wir das wollen. Also, wie muessen das jetzt fuer anderen Spielern steuern
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

            //!!!BAUARBEITEN
            //_photonView.RPC("ShareCustomization", RpcTarget.Others, hatIndex, eyeIndex, bodyIndex, clothesIndex, redColorOfHar, greenColorOfHat, blueColorOfHat, PhotonNetwork.LocalPlayer.NickName);
            _photonView.RPC("ShareCustomization", RpcTarget.Others, hatIndex, eyeIndex, bodyIndex, clothesIndex, redColorOfHar, greenColorOfHat, blueColorOfHat, PhotonNetwork.LocalPlayer.NickName);
            //!!!BAUARBEITEN
        }
        else
        {
            Debug.LogError("Player type not selected !");
            return;
        }

        

        //BAUARBEITEN !!!
        //controller = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerController"), spawnpoint.position, spawnpoint.rotation, 0, new object[] { _photonView.ViewID});
        //controller = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", Launch.instance.playerModel), spawnpoint.position, spawnpoint.rotation, 0, new object[] { _photonView.ViewID});
        //controller = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", Launch.instance.playerModel), spawnpoint.position, spawnpoint.rotation, 0, new object[] { _photonView.ViewID, hatIndex, eyeIndex, bodyIndex, clothesIndex });
        
        //BAUARBEITEN !!!
    }

    //jedes Mal wenn ein Spieler das Spiel beitritt, sendet er an anderen Spielern seine Kustomization Daten
    //alle andere Spielern finden dann lokal sein Model und wenden das Kustomization an
    [PunRPC]
    public void ShareCustomization(int hatIndex, int eyeIndex, int bodyIndex, int clothesIndex, int redColorOfHar, int greenColorOfHat, int blueColorOfHat, string playerName)
    {
        // Suche nach dem Spieler-Objekt anhand des Nicknames
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player"); // Setze den Tag auf "Player" für alle Spieler-Objekte
        foreach (GameObject player in players)
        {
            if (player.GetComponent<PhotonView>().Owner.NickName == playerName)
            {
                // Wenn der Nickname übereinstimmt, finde PlayerCustomizationManager und rufe die Methoden auf (fehleranfaelig, falls zwei Spieler mit dem gleichen Name)
                PlayerCustomizationManager customizationManager = player.GetComponent<PlayerCustomizationManager>();
                if (customizationManager != null)
                {
                    customizationManager.EquipItem(customizationManager.hats, hatIndex);

                    customizationManager.EquipItem(customizationManager.eyes, eyeIndex);
                    
                    customizationManager.EquipItem(customizationManager.bodies, bodyIndex);
                    customizationManager.EquipItem(customizationManager.clothes, clothesIndex);

                    
                    customizationManager.UpdateHatColor(redColorOfHar, greenColorOfHat, blueColorOfHat);
                }
                break;
            }
        }
    }
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Hashtable playerProperties = PhotonNetwork.LocalPlayer.CustomProperties;
        if (playerProperties == null || playerProperties["HatIndex"] == null)
        {
            return;
        }
        int hatIndex = (int)playerProperties["HatIndex"];
        int eyeIndex = (int)playerProperties["EyeIndex"];
        int bodyIndex = (int)playerProperties["BodyIndex"];
        int clothesIndex = (int)playerProperties["ClothesIndex"];

        int redColorOfHar = (int)playerProperties["HatColorR"];
        int greenColorOfHat = (int)playerProperties["HatColorG"];
        int blueColorOfHat = (int)playerProperties["HatColorB"];
        Debug.Log("Neuer Spieler ist dem Raum beigetreten: " + newPlayer.NickName);
        //neuer Spieler bekommt Daten ueber Customization von bereits beigetretenen Speilern
        _photonView.RPC("ShareCustomization", newPlayer, hatIndex, eyeIndex, bodyIndex, clothesIndex, redColorOfHar, greenColorOfHat, blueColorOfHat, PhotonNetwork.LocalPlayer.NickName);
    }

    public void Respawn()
    {
        PhotonNetwork.Destroy(controller);
        CreateController();
        SetCamerasForPortals();
    }

    // Diese Methode wird aufgerufen, wenn ein anderer Spieler den Raum verlässt
    //bsp wenn er "kicked" wird und es "CloseConnection" aufgerufen wird
    /*public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
        SceneManager.LoadScene(0);
    }*/
    public override void OnDisconnected(DisconnectCause cause)
    {
        base.OnDisconnected(cause);
        SceneManager.LoadScene(0);
    }

    [PunRPC]
    private void RPC_SetWhiteboardAcces(bool isOn)
    {
        InputManager.whiteboardIsOn = isOn;
    }

    private void SetCamerasForPortals()
    {
        PortalCamera[] allCams = FindObjectsOfType<PortalCamera>();
        foreach (PortalCamera cam in allCams)
        {
            cam.SetCamera(controller.GetComponentInChildren<Camera>().transform);
        }
    }
}
