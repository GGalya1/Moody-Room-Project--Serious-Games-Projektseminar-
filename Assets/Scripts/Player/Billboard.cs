using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Billboard : MonoBehaviourPunCallbacks, IUpdateObserver
{
    //dient dazu, username in die richtige Richtung zu rotieren
    Camera cam;
    PhotonView _photonView;
    private Camera _cameraA;
    private Camera _cameraB;

    void Start()
    {
        Camera[] allCameras = FindObjectsOfType<Camera>();

        foreach (var camera in allCameras)
        {
            if (camera.gameObject.name == "Camera_a")
            {
                _cameraA = camera;
            }
            if (camera.gameObject.name == "Camera_b")
            {
                _cameraB = camera;
            }
            if (_cameraB != null && _cameraA != null) break;
        }
    }

    #region UpdateManager connection
    private void Awake()
    {
        _photonView = GetComponent<PhotonView>();
        UpdateManager.Instance.RegisterObserver(this);
        UpdateManager.Instance.RegisterObserverName("Billboard");
    }
    //damit alle Spieler, die nach dem Start des Spieles das Raum beitreten, auch andere Billboards sehen koenten
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.LogError("I try to send the RPC to " + newPlayer.NickName);
        _photonView.RPC("FindAndSubscribeBillboard", newPlayer, PhotonNetwork.LocalPlayer.NickName);
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        _photonView.RPC("UnsubscribeBillboard", otherPlayer, PhotonNetwork.LocalPlayer.NickName);
    }
    private void OnDestroy()
    {
        UpdateManager.Instance.UnregisterObserver(this);
        UpdateManager.Instance.UnregisterOberverName("Billboard");
    }
    #endregion

    // Update is called once per frame
    public void ObservedUpdate()
    {
        //falls Camera nicht gefunden wird, dann versuchen wir eine zu finden
        if (cam == null)
        {
            //suchen nach der Kamera von Spieler (und nicht nach iwelcher)
            Camera[] allCameras = FindObjectsOfType<Camera>();

            foreach (var camera in allCameras)
            {
                PhotonView photonView = camera.GetComponentInParent<PhotonView>();
                if (photonView != null && photonView.IsMine)
                {
                    cam = camera;
                    break;
                }
            }
        }
 
        if (cam == null)
            return;

        //berechnen die Distanz zwischen den Spieler (lokal) und Billboard
        float distance = Vector3.Distance(transform.position, cam.transform.position);

        if (distance > 90 && _cameraA != null && _cameraB != null)
        {
            //falls Portal exisiert, dann nach dem teleportieren orientieren sich auf CameraA/B
            //Portal a befindet sich oben, Portal b unten (in der Welt relativ einander)
            //Billboard orientiert sich auf Camera_b, falls Spieler hoeher ist
            if (transform.position.y < cam.transform.position.y)
            {
                transform.LookAt(_cameraB.transform);
            }
            //Billboard orientiert sich auf Camera_a, falls Spieler sich niedriger in der Welt befindet
            else
            {
                transform.LookAt(_cameraA.transform);
            }
            
            
        }
        else
        {
            //rotiert sich normal
            transform.LookAt(cam.transform);
        }
        
        transform.Rotate(Vector3.up * 180); //da sonst username gespiegelt war
    }
    [PunRPC]
    public void FindAndSubscribeBillboard(string nicknameToFind)
    {
        Debug.LogError("I have become RPC from: " + nicknameToFind + "!!!");
        // Suche nach dem Spieler-Objekt anhand des Nicknames
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player"); // Setze den Tag auf "Player" für alle Spieler-Objekte
        foreach (GameObject player in players)
        {
            if (player.GetComponent<PhotonView>().Owner.NickName == nicknameToFind)
            {
                Debug.LogError("I have set it in UpdateManager");
                // Wenn der Nickname übereinstimmt, finde PlayerCustomizationManager und rufe die Methoden auf (fehleranfaelig, falls zwei Spieler mit dem gleichen Name)
                UpdateManager.Instance.RegisterObserver(player.GetComponent<Billboard>());
                UpdateManager.Instance.RegisterObserverName("Billboard von: " + nicknameToFind);
            }
        }
    }
    [PunRPC]
    public void UnsubscribeBillboard(string nicknameToFind)
    {
        // Suche nach dem Spieler-Objekt anhand des Nicknames
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player"); // Setze den Tag auf "Player" für alle Spieler-Objekte
        foreach (GameObject player in players)
        {
            if (player.GetComponent<PhotonView>().Owner.NickName == nicknameToFind)
            {
                // Wenn der Nickname übereinstimmt, finde PlayerCustomizationManager und rufe die Methoden auf (fehleranfaelig, falls zwei Spieler mit dem gleichen Name)
                UpdateManager.Instance.UnregisterObserver(player.GetComponent<Billboard>());
                UpdateManager.Instance.UnregisterOberverName("Billboard von: " + nicknameToFind);
            }
        }
    }
}
