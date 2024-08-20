using UnityEngine;
using Photon.Pun;
using System.IO;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using UnityEngine.SceneManagement;
using Photon.Realtime;

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
            }
            else
            {
                Debug.LogWarning("Kein Kontroller gefunden! MeshRender wird nicht deaktiviert");
            }
            
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
}
