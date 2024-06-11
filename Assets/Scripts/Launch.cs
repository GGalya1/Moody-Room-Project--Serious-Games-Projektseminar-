using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;

//Benutzen MonoBehaviourPunCallbacks - damit wir sogenannte "callbacks" benutzen koennten. Also, Benachrichtigungen bekommen, 
//wenn Player Lobby beitritt oder erstellt
public class Launch : MonoBehaviourPunCallbacks
{
    //wieder singelton, damit die Funktion "JoinRoom" von aussen erreichbar ist
    public static Launch instance;

    //hier speichern wir Name von Room, das in InputField eingegeben war
    [SerializeField] private TMP_InputField _roomInputField;

    //fuer korrekte Wiedergabe von Name des Zimmers
    [SerializeField] private TMP_Text _errorText;
    [SerializeField] private TMP_Text _roomNameText;

    //fuer korrekte Wiedergabe von bereits an der Server erstellten Zimmern
    [SerializeField] private Transform _roomList;
    [SerializeField] private GameObject _roomButtonPrefab;

    //fuer korrekte Wiedergabe von bereits in einem Room beigetretenen Leuten
    [SerializeField] private Transform _playerList;
    [SerializeField] private GameObject _playerNamePrefab;


    //BAUARBEITEN !!!!!!!!!!!!!!!!!
    //PROBE: Wahlen von Player Model
    public string playerModel = "PlayerController";
    public void selectRedSphereAvatar()
    {
        playerModel = "PlayerController - RedSphere";
    }
    public void selectDefaultAvatar()
    {
        playerModel = "PlayerController";
    }
    public void selectCustomizedAvatar()
    {
        playerModel = "CustomizedPlayer";
    }
    //BAUARBEITEN !!!!!!!!!!!!!!!!!


    [SerializeField] private GameObject _startGameButton;

    private void Start()
    {
        instance = this;

        Debug.Log("Connected to the server");
        //wird zu eu-Region eine Konnektion erstellen (weil so Photon-Objekt konfiguriert ist. Kann man aendern)
        PhotonNetwork.ConnectUsingSettings();
        MenuManager.current.OpenMenu("loading");
    }

    //"OnConnectedToMaster" fuhrt Aktionen, sobald eine Konnektion erstellt wurde
    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to master");
        PhotonNetwork.JoinLobby();
        //damit bei allen Spieler das gleiche Scene geladen wird
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Connected to Lobby");
        MenuManager.current.OpenMenu("title");
        //spaeter ersetzen
        //PhotonNetwork.NickName = "Player " + Random.Range(0, 2000).ToString("0000");
    }

    public void StartGame()
    {
        //hier Starten wir Scene namens "GameScene", weil sie Index 1 in BuildSetting hat
        PhotonNetwork.LoadLevel(1);
    }

    public void CreateRoom()
    {
        if (!string.IsNullOrEmpty(_roomInputField.text))
        {
            PhotonNetwork.CreateRoom(_roomInputField.text);
            MenuManager.current.OpenMenu("loading");
        }
    }

    public override void OnJoinedRoom()
    {
        _roomNameText.text = PhotonNetwork.CurrentRoom.Name;
        MenuManager.current.OpenMenu("room");

        Player[] players = PhotonNetwork.PlayerList;
        //fixen von Bug: wenn man Room verlaesst und ein anderes Startet, werden alle 
        //Spieler aus dem letzten Room in den aktuellen übertragen
        //darum loeschen wir alle Spieler am Anfang
        for (int i = 0; i < _playerList.childCount; i++)
        {
            Destroy(_playerList.GetChild(i).gameObject);
        }


        for (int i = 0; i < players.Length; i++)
        {
            Instantiate(_playerNamePrefab, _playerList).GetComponent<PlayerListItem>().SetUp(players[i]);
        }

        //falls Speieler "host" ist, wird Knopf zum Start des SPieles visible. Wenn nicht - invisible
        _startGameButton.SetActive(PhotonNetwork.IsMasterClient);
    }

    //wenn host das Raum verlaest, wird host aktuelisiert
    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        _startGameButton.SetActive(PhotonNetwork.IsMasterClient);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        _errorText.text = "Error: " + message;
        MenuManager.current.OpenMenu("error");
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        MenuManager.current.OpenMenu("loading");
    }

    public override void OnLeftRoom()
    {
        MenuManager.current.OpenMenu("title");
    }

    //fuehrt Information uber den gewuenschten Room fuer Photon
    public void JoinRoom(RoomInfo info)
    {
        PhotonNetwork.JoinRoom(info.Name);
        MenuManager.current.OpenMenu("loading");
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        //bevor die Information an Knopfen aktualisiert wird, sollen wir die alle loeschen
        for (int i = 0; i < _roomList.childCount; i++)
        {
            Destroy(_roomList.GetChild(i).gameObject);
        }

        //fehleranfaelig !!!
        //iterriere alle rooms uns erstelle fuer jede neuen Knopf
        foreach (RoomInfo r in roomList)
        {
            //RemovedFromList sichert, dass falls Room voll/ hidden ist, wird es nicht angezeigt
            if (r.RemovedFromList)
            {
                continue;
            }
            Instantiate(_roomButtonPrefab, _roomList).GetComponent<RoomListItem>().SetUp(r);
        }
    }


    public override void OnPlayerEnteredRoom(Player player)
    {
        Instantiate(_playerNamePrefab, _playerList).GetComponent<PlayerListItem>().SetUp(player);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
