using System.Collections;
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
    [SerializeField] private Slider _chairsSlider;
    [SerializeField] private TMP_Text _chairsSliderText;
    [SerializeField] private Toggle _chatToggle;
    [SerializeField] private Toggle _voiceChatToggle;
    [SerializeField] private TMP_Text _countdownText;
    private bool _startCountdown;
    private float _toWaitBeforeStart = 3.0f;


    /*#region UpdateManager connection
    private void Awake()
    {
        UpdateManager.Instance.RegisterObserver(this);
    }
    public override void OnEnable()
    {
        UpdateManager.Instance.RegisterObserver(this);
    }
    public override void OnDisable()
    {
        UpdateManager.Instance.UnregisterObserver(this);
    }
    private void OnDestroy()
    {
        UpdateManager.Instance.UnregisterObserver(this);
    }
    #endregion*/


    private void Start()
    {
        instance = this;

        Debug.Log("Connected to the server");
        //wird zu eu-Region eine Konnektion erstellen (weil so Photon-Objekt konfiguriert ist. Kann man aendern)
        PhotonNetwork.ConnectUsingSettings();
        MenuManager.current.OpenMenu("loading");

        _chairsSlider.onValueChanged.AddListener(OnSliderValueChanged);
        OnSliderValueChanged(_chairsSlider.value);

        _chatToggle.onValueChanged.AddListener(OnChatToggleValueChanged);
        _voiceChatToggle.onValueChanged.AddListener(OnVoiceChatToggleValueChanged);
    }
    public void Update()
    {
        //Debug sagt, dass Cursor ist visible, ist aber nicht (
        //Debug.Log("Cursor is visible: " + Cursor.visible);

        if (_startCountdown && _toWaitBeforeStart > 0)
        {
            _toWaitBeforeStart -= Time.deltaTime;
            _countdownText.text = $"we will start in {Mathf.RoundToInt(_toWaitBeforeStart)}";
        }
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
        //den Code, um Anzahl an Stuhle im Room zu uebergeben
        ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable();
        props["ChairCount"] = RoomManager.instance.chairsNumber;
        props["IsChatOn"] = RoomManager.instance.chatIsOn;
        props["IsVoiceChatOn"] = RoomManager.instance.voicechatIsOn;

        //hier Starten wir Scene namens "GameScene", weil sie Index 1 in BuildSetting hat
        //PhotonNetwork.LoadLevel(1);

        PhotonNetwork.CurrentRoom.SetCustomProperties(props);

        // Warten, um sicherzustellen, dass die Eigenschaft gesetzt ist
        StartCoroutine(WaitAndStartGame());

        //da man alle Einstellungen fuer Zimmer OnClick() uebernimmt, sollen wir die Moeglichkeit ausschalten,
        //diese Einstellungen nach dem Klick zu aendern (um die Frustrationserfahrung zu vermeiden)
        _startGameButton.gameObject.SetActive(false);
        _chairsSlider.gameObject.SetActive(false);
        _chairsSliderText.gameObject.SetActive(false);
        _chatToggle.gameObject.SetActive(false);
        _voiceChatToggle.gameObject.SetActive(false);

        _countdownText.gameObject.SetActive(true);
        _startCountdown = true;
    }
    //sonst koennen die Daten nicht hochgeladen werden, da die naechste schneller Scene geladen wird
    private IEnumerator WaitAndStartGame()
    {
        yield return new WaitForSeconds(Mathf.RoundToInt(_toWaitBeforeStart)); // Warte 3 Sekunden

        // Hier Starten wir die Szene namens "GameScene", weil sie Index 1 in BuildSettings hat
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

        //falls Speieler "host" ist, wird Knopf zum Start des Spieles visible. Wenn nicht - invisible
        _startGameButton.SetActive(PhotonNetwork.IsMasterClient);
        _chairsSlider.gameObject.SetActive(PhotonNetwork.IsMasterClient);
        _chairsSliderText.gameObject.SetActive(PhotonNetwork.IsMasterClient);
        _chatToggle.gameObject.SetActive(PhotonNetwork.IsMasterClient);
        _voiceChatToggle.gameObject.SetActive(PhotonNetwork.IsMasterClient);
    }

    //wenn host das Raum verlaest, wird host aktuelisiert
    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        _startGameButton.SetActive(PhotonNetwork.IsMasterClient);
        _chairsSlider.gameObject.SetActive(PhotonNetwork.IsMasterClient);
        _chairsSliderText.gameObject.SetActive(PhotonNetwork.IsMasterClient);
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
}
