﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;
using UnityEngine.UI;

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
    [SerializeField] private GameObject _playerNameForAdminPrefab;

    [SerializeField] private Dropdown _sceneSelector;
    private int indexOfScene = 1;

    [SerializeField] public GameObject defaultAvatar;
    [SerializeField] public GameObject smallAvatar;
    [SerializeField] public GameObject customizedAvatar;

    //Funktionalitaten fuer private-Room Funktionalitaeten
    [SerializeField] public Toggle _privateRoomToggle;
    [SerializeField] private TMP_Text _roomCodeText;
    private string _roomCode;
    [SerializeField] private GameObject _privateRoomPanel;
    [SerializeField] private TMP_InputField _roomCodeInputField;
    [SerializeField] private Button _roomCodeSubmitButton;
    private RoomInfo _selectedRoomInfo;
    [SerializeField] private TMP_Text _localErrorMessage;
    [SerializeField] private GameObject _privateRoomContainer;

    public string playerModel;
    public void SelectAvatar(int avatarTypeIndex)
    {
        AvatarType avatarType = (AvatarType)avatarTypeIndex;
        switch (avatarType)
        {
            case AvatarType.Default:
                playerModel = defaultAvatar.name;
                break;
            case AvatarType.Small:
                playerModel = smallAvatar.name;
                break;
            case AvatarType.Customizable:
                playerModel = customizedAvatar.name;
                break;
            default:
                Debug.LogError("Unknown avatar type selected");
                break;
        }
    }

    //Es gibt ein Bug: wenn man das Raum selbst erstellt und danach verlaesst. Kann man bereits existierte Raume nicht meehr sehen. Das losen wir durch "refresh"
    public void RefreshRoomlist()
    {
        PhotonNetwork.JoinLobby();
    }


    [SerializeField] private GameObject _startGameButton;
    [SerializeField] private Slider _chairsSlider;
    [SerializeField] private TMP_Text _chairsSliderText;
    [SerializeField] private Toggle _chatToggle;
    [SerializeField] private Toggle _voiceChatToggle;
    [SerializeField] private TMP_Text _countdownText;
    private float _toWaitBeforeStart = 3.0f;
    [SerializeField] private Button _leaveRoomButton;


    private void Start()
    {
        instance = this;
        //damit man Spielern aus dem Server "kicken" kann
        PhotonNetwork.EnableCloseConnection = true;
        Debug.Log("Connected to the server");
        //wird zu eu-Region eine Konnektion erstellen (weil so Photon-Objekt konfiguriert ist. Kann man aendern)
        PhotonNetwork.ConnectUsingSettings();
        MenuManager.current.OpenMenu("loading");

        playerModel = defaultAvatar.name;

        _chairsSlider.onValueChanged.AddListener(OnSliderValueChanged);
        OnSliderValueChanged(_chairsSlider.value);

        _chatToggle.onValueChanged.AddListener(OnChatToggleValueChanged);
        _voiceChatToggle.onValueChanged.AddListener(OnVoiceChatToggleValueChanged);

        _sceneSelector.onValueChanged.AddListener(delegate { DropdownItemSelected(_sceneSelector); });
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

        Menu currMenu = MenuManager.current.GetActiveMenu();
        Debug.Log("CurrentMenuName: " + currMenu.menuName);
        if (currMenu != null && currMenu.menuName == "findRoom")
        {
            Debug.Log("Refresh of Roomlist");
        }
        else MenuManager.current.OpenMenu("title");
        //spaeter ersetzen
        //PhotonNetwork.NickName = "Player " + Random.Range(0, 2000).ToString("0000");
    }

    public void StartGame()
    {
        //den Code, um Anzahl an Stuhle im Room zu uebergeben
        ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable();
        props["ChairCount"] = RoomManager.instance.chairsNumber;
        props["IsChatOn"] = RoomManager.instance.chatIsOn;
        props["IsVoiceChatOn"] = RoomManager.instance.voicechatIsOn;
        props["RoomCode"] = _roomCode;

        //hier Starten wir Scene namens "GameScene", weil sie Index 1 in BuildSetting hat
        //PhotonNetwork.LoadLevel(1);

        PhotonNetwork.CurrentRoom.SetCustomProperties(props);

        // Warten, um sicherzustellen, dass die Eigenschaft gesetzt ist
        StartCoroutine(WaitAndStartGame());

        //da man alle Einstellungen fuer Zimmer OnClick() uebernimmt, sollen wir die Moeglichkeit ausschalten,
        //diese Einstellungen nach dem Klick zu aendern (um die Frustrationserfahrung zu vermeiden)
        DisableAllInteractableObjects();

        _countdownText.gameObject.SetActive(true);
    }
    //sonst koennen die Daten nicht hochgeladen werden, da die naechste schneller Scene geladen wird
    private IEnumerator WaitAndStartGame()
    {
        //yield return new WaitForSeconds(Mathf.RoundToInt(_toWaitBeforeStart)); // Warte 3 Sekunden
        float countdown = _toWaitBeforeStart;
        while (countdown >= 0)
        {
            _countdownText.text = $"we will start in {Mathf.RoundToInt(countdown)}";
            yield return new WaitForSeconds(1f);
            countdown--;
        }
        MenuManager.current.OpenMenu("loading");
        // Hier Starten wir die Szene namens "GameScene", weil sie Index 1 in BuildSettings hat
        PhotonNetwork.LoadLevel(indexOfScene);
    }
    private void DisableAllInteractableObjects()
    {
        _startGameButton.gameObject.SetActive(false);
        _chairsSlider.gameObject.SetActive(false);
        _chairsSliderText.gameObject.SetActive(false);
        _chatToggle.gameObject.SetActive(false);
        _voiceChatToggle.gameObject.SetActive(false);
        _sceneSelector.gameObject.SetActive(false);
        _leaveRoomButton.gameObject.SetActive(false);
        foreach (Transform item in _playerList)
        {
            Button temp = item.gameObject.GetComponentInChildren<Button>();
            if (temp != null)
            {
                temp.gameObject.SetActive(false);
            }
        }
    }

    public void CreateRoom()
    {
        if (!string.IsNullOrEmpty(_roomInputField.text))
        {
            RoomOptions options = new RoomOptions();

            //falls das eingeschaltet ist, dann erzeuge ich einen Code, damit Leute das Raum beitreten koennten
            if (_privateRoomToggle.isOn)
            {
                _roomCode = Random.Range(1000, 15000).ToString();
                options.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable() { { "RoomCode", _roomCode } };
                options.CustomRoomPropertiesForLobby = new string[] { "RoomCode" };

                // Zeige den Code für den Admin an
                _roomCodeText.text = $"Room Code:\n{_roomCode}";
                RoomManager.instance.roomCode = _roomCode;
            }
            else
            {
                RoomManager.instance.roomCode = "public";
            }
            PhotonNetwork.CreateRoom(_roomInputField.text, options);
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

        //hier muessen wir die Fallunterscheidung treffen. Wenn IsMasterClient - muss Prefab mit dem Knopf zum kicken erstellt werden.
        if (PhotonNetwork.IsMasterClient)
        {
            for (int i = 0; i < players.Length; i++)
            {
                if (players[i].IsLocal)
                {
                    Instantiate(_playerNamePrefab, _playerList).GetComponent<PlayerListItem>().SetUp(players[i]);
                    continue;
                }
                Instantiate(_playerNameForAdminPrefab, _playerList).GetComponent<PlayerListItemForAdmin>().SetUp(players[i]);
            }
        }
        else
        {
            for (int i = 0; i < players.Length; i++)
            {
                Instantiate(_playerNamePrefab, _playerList).GetComponent<PlayerListItem>().SetUp(players[i]);
            }
        }


        //falls Speieler "host" ist, wird Knopf zum Start des Spieles visible. Wenn nicht - invisible
        SetVisibilityOfRoomSettings(PhotonNetwork.IsMasterClient);
    }

    //wenn host das Raum verlaest, wird host aktuelisiert
    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        SetVisibilityOfRoomSettings(PhotonNetwork.IsMasterClient);
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
        //beitreten von private-Raumen
        if (info.CustomProperties.ContainsKey("RoomCode"))
        {
            _privateRoomPanel.SetActive(true);
            _selectedRoomInfo = info;
            //ab hier erwarten wir die Funktionalitaeten von InputField und Submit Button
        }
        //beitreten von public-Raumen
        else
        {
            PhotonNetwork.JoinRoom(info.Name);
            MenuManager.current.OpenMenu("loading");
        }
    }
    // Methode, die aufgerufen wird, wenn der Spieler den Code eingibt und bestätigen möchte
    public void OnSubmitRoomCode()
    {
        if (_roomCodeInputField.text == _selectedRoomInfo.CustomProperties["RoomCode"].ToString())
        {
            PhotonNetwork.JoinRoom(_selectedRoomInfo.Name);
            MenuManager.current.OpenMenu("loading");
        }
        else
        {
            //falls mit dem Code nicht geklappt ist, geben wir die Rueckmeldung
            StartCoroutine(ShowErrorAndHidePanel());
        }
    }
    //um die Reuckmeldung zu geben
    private IEnumerator ShowErrorAndHidePanel()
    {
        // Blende die Fehlermeldung ein und andren Komponenten aus
        _localErrorMessage.gameObject.SetActive(true);
        _privateRoomContainer.SetActive(false);

        // Warte 1 Sekunde
        yield return new WaitForSeconds(1f);

        // Blende die Fehlermeldung aus
        _localErrorMessage.gameObject.SetActive(false);
        _privateRoomContainer.SetActive(true);

        // Schließe das Panel
        _privateRoomPanel.SetActive(false);
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
        if (PhotonNetwork.IsMasterClient)
        {
            Instantiate(_playerNameForAdminPrefab, _playerList).GetComponent<PlayerListItemForAdmin>().SetUp(player);
        }
        else
        {
            Instantiate(_playerNamePrefab, _playerList).GetComponent<PlayerListItem>().SetUp(player);
        }
    }
        

    public void ExitGame()
    {
        Application.Quit();
    }


    //Funktionalitaten, um das Raum zu gestalten.
    private void OnSliderValueChanged(float value)
    {
        int chairNumber = Mathf.RoundToInt(value);
        RoomManager.instance.chairsNumber = chairNumber;
        _chairsSliderText.text = $"Chair Number: {chairNumber}";
    }
    private void OnChatToggleValueChanged(bool isOn)
    {
        RoomManager.instance.chatIsOn = isOn;
    }
    private void OnVoiceChatToggleValueChanged(bool isOn)
    {
        RoomManager.instance.voicechatIsOn = isOn;
    }

    private void DropdownItemSelected(Dropdown dropdown)
    {
        indexOfScene = dropdown.value + 1;
    }

    private void SetVisibilityOfRoomSettings(bool val)
    {
        _startGameButton.SetActive(val);
        _chairsSlider.gameObject.SetActive(val);
        _chairsSliderText.gameObject.SetActive(val);
        _chatToggle.gameObject.SetActive(val);
        _voiceChatToggle.gameObject.SetActive(val);
        _sceneSelector.gameObject.SetActive(val);
        _roomCodeText.gameObject.SetActive(val);
    }


}

public enum AvatarType
{
    Default,
    Small,
    Customizable
}
