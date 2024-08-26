using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Billboard : MonoBehaviourPunCallbacks, IUpdateObserver
{
    //dient dazu, username in die richtige Richtung zu rotieren
    Camera cam;
    PhotonView _photonView;

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
            cam = FindObjectOfType<Camera>();
        if (cam == null)
            return;
        transform.LookAt(cam.transform);
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
