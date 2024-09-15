using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;
using UnityEngine.UI;
using static UnityEngine.UIElements.UxmlAttributeDescription;
using Unity.VisualScripting;

/// <summary>
/// The Launch class handles the creation, joining, and management of rooms in a multiplayer game using Photon.
/// It provides functions to set up rooms, manage players, and sync room properties.
/// 
/// MonoBehaviourPunCallbacks - to use Photon callback methods. Notifications appears when a player joins or creates a lobby.
/// </summary>
public class Launch : MonoBehaviourPunCallbacks
{
    // Singleton instance of Launch to allow access from other scripts (e.g., RoomListItem)
    public static Launch instance;

    // Room input field (room name) and UI elements for error and room name display
    [SerializeField] private TMP_InputField _roomInputField;
    [SerializeField] private TMP_Text _errorText;
    [SerializeField] private TMP_Text _roomNameText;

    // Used to display the list of rooms available on the server. 
    [SerializeField] private Transform _roomList; //A Transform for the list of rooms available on the server. (Buttons)
    [SerializeField] private GameObject _roomButtonPrefab; //Prefab for buttons representing available rooms.

    // Used to display the list of players in the room.
    [SerializeField] private Transform _playerList;
    [SerializeField] private GameObject _playerNamePrefab;
    [SerializeField] private GameObject _playerNameForAdminPrefab;

    [SerializeField] private Dropdown _sceneSelector;
    private int indexOfScene = 1;

    [SerializeField] public GameObject defaultAvatar;
    [SerializeField] public GameObject smallAvatar;
    [SerializeField] public GameObject customizedAvatar;
    public string playerModel; // the name of the selected player model

    // Private room settings
    [SerializeField] public Toggle _privateRoomToggle;
    [SerializeField] private TMP_Text _roomCodeText;
    private string _roomCode;
    [SerializeField] private GameObject _privateRoomPanel;
    [SerializeField] private TMP_InputField _roomCodeInputField;
    [SerializeField] private Button _roomCodeSubmitButton;
    private RoomInfo _selectedRoomInfo;
    [SerializeField] private TMP_Text _localErrorMessage;
    [SerializeField] private GameObject _privateRoomContainer;


    // Room settings 
    [SerializeField] private GameObject _startGameButton;
    [SerializeField] private Slider _chairsSlider;
    [SerializeField] private TMP_Text _chairsSliderText;
    [SerializeField] private Toggle _chatToggle;
    [SerializeField] private Toggle _voiceChatToggle;
    [SerializeField] private TMP_Text _countdownText;
    private float _toWaitBeforeStart = 3.0f;
    [SerializeField] private Button _leaveRoomButton;


    /// <summary>
    /// Initializes the instance, connects to the Photon server, and sets up UI listeners.
    /// </summary>
    private void Start()
    {
        instance = this;
        PhotonNetwork.EnableCloseConnection = true; // Allows master client to kick players from the server
        Debug.Log("Connected to the server");
        PhotonNetwork.ConnectUsingSettings(); // Connect to Photon server in the eu region (can be changed in the Photon object configuration)
        MenuManager.current.OpenMenu("loading");

        playerModel = defaultAvatar.name; // Set the default avatar

        _chairsSlider.onValueChanged.AddListener(OnSliderValueChanged);
        OnSliderValueChanged(_chairsSlider.value);
        _chatToggle.onValueChanged.AddListener(OnChatToggleValueChanged);
        _voiceChatToggle.onValueChanged.AddListener(OnVoiceChatToggleValueChanged);

        // When the player selects an option from the scene dropdown, the DropdownItemSelected method is called.
        _sceneSelector.onValueChanged.AddListener(delegate { DropdownItemSelected(_sceneSelector); });
    }

    /// <summary>
    /// Called when connected to the Photon master server.
    /// Joins the default lobby.
    /// </summary>
    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to master");
        PhotonNetwork.JoinLobby();
        PhotonNetwork.AutomaticallySyncScene = true; // Automatically synchronize the scene between players
    }

    /// <summary>
    /// Called when joining the Photon lobby.
    /// Sets up the lobby UI.
    /// </summary>
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


    // there is a bug: if you create the room yourself and then leave it.
    // You can no longer see the rooms that already exist.
    // Solved by "refresh"
    public void RefreshRoomlist()
    {
        PhotonNetwork.JoinLobby();
    }

    /// <summary>
    ///  Creates a new room based on user input and toggles.
    ///  If a private room is created, generates a room code.
    /// </summary>
    public void CreateRoom()
    {
        if (!string.IsNullOrEmpty(_roomInputField.text))
        {
            RoomOptions options = new RoomOptions();

            // If the private room toggle is on, generate a private code
            if (_privateRoomToggle.isOn)
            {
                _roomCode = Random.Range(1000, 15000).ToString();
                options.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable() { { "RoomCode", _roomCode } };
                options.CustomRoomPropertiesForLobby = new string[] { "RoomCode" };

                // Show the code to the admin
                _roomCodeText.text = $"Room Code:\n{_roomCode}";
                RoomManager.instance.roomCode = _roomCode;
            }
            else
            {
                RoomManager.instance.roomCode = "public";
            }
            PhotonNetwork.CreateRoom(_roomInputField.text, options); // Create a room with the name entered
            MenuManager.current.OpenMenu("loading");
        }
    }

    /// <summary>
    /// Called when creating a room fails. Displays an error message.
    /// </summary>
    /// <param name="returnCode"></param>
    /// <param name="message"></param>
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        _errorText.text = "Error: " + message;
        MenuManager.current.OpenMenu("error");
    }

    /// <summary>
    /// Called when a room is successfully joined.
    /// Updates the room and player UI for hosts and regular players.
    /// </summary>
    public override void OnJoinedRoom()
    {
        _roomNameText.text = PhotonNetwork.CurrentRoom.Name;
        MenuManager.current.OpenMenu("room");

        Player[] players = PhotonNetwork.PlayerList;

        // Bug: when you leave the room and start another one, all players from the last room are transferred to the current one
        // Solution: delete all players
        for (int i = 0; i < _playerList.childCount; i++)
        {
            Destroy(_playerList.GetChild(i).gameObject);
        }

        // It differentiates the UI elements used for the master client (host) and regular players.
        // If host, the button to kick players will be created. If not - no button.
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

        // If the player is the host, the button to start the game (and other properties) will be visible. If not - invisible
        SetVisibilityOfRoomSettings(PhotonNetwork.IsMasterClient);
    }

    /// <summary>
    /// Called when the master client leaves. Updates the new master client UI.
    /// </summary>
    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        SetVisibilityOfRoomSettings(PhotonNetwork.IsMasterClient);
    }


    /// <summary>
    /// Attempts to join a selected room (public or private), passing the info about the room to Photon.
    /// </summary>
    /// <param name="info"> Room details </param>
    public void JoinRoom(RoomInfo info)
    {
        // joining private rooms
        if (info.CustomProperties.ContainsKey("RoomCode"))
        {
            _privateRoomPanel.SetActive(true);
            _selectedRoomInfo = info;
            // from here we expect the functionalities of InputField and Submit Button
        }
        // joining public rooms
        else
        {
            PhotonNetwork.JoinRoom(info.Name);
            MenuManager.current.OpenMenu("loading");
        }
    }


    /// <summary>
    ///  Submits the room code for private rooms, by entering the code in the input field and clicking submit button.
    ///  If correct, the player joins the room.
    /// </summary>
    public void OnSubmitRoomCode()
    {
        if (_roomCodeInputField.text == _selectedRoomInfo.CustomProperties["RoomCode"].ToString())
        { // if the code is correct, the player joins the room
            PhotonNetwork.JoinRoom(_selectedRoomInfo.Name);
            MenuManager.current.OpenMenu("loading");
        }
        else
        { // if the code is incorrect, the error message is displayed
            StartCoroutine(ShowErrorAndHidePanel());
        }
    }

    /// <summary>
    /// Shows an error message and hides the private room panel if the room code is incorrect.
    /// </summary>
    private IEnumerator ShowErrorAndHidePanel()
    {
        // Show the error message and hide other components
        _localErrorMessage.gameObject.SetActive(true);
        _privateRoomContainer.SetActive(false);

        yield return new WaitForSeconds(1f);

        // Hide the error message and show the components
        _localErrorMessage.gameObject.SetActive(false);
        _privateRoomContainer.SetActive(true);

        // hide the whole private room panel
        _privateRoomPanel.SetActive(false);
    }

    /// <summary>
    /// Updates the room list UI with the available rooms.
    /// </summary>
    /// <param name="roomList"></param>
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        // Before updating the information on the buttons, we need to delete all the buttons (room list items)
        for (int i = 0; i < _roomList.childCount; i++)
        {
            Destroy(_roomList.GetChild(i).gameObject);
        }

        // error-prone !!!
        // iterates through all rooms and creates a new button for each
        foreach (RoomInfo r in roomList)
        {
            if (r.RemovedFromList) // If the room is full or hidden, it will not be displayed
            {
                continue;
            }
            Instantiate(_roomButtonPrefab, _roomList).GetComponent<RoomListItem>().SetUp(r);
        }
    }

    /// <summary>
    /// Handles the avatar selection process based on user input.
    /// </summary>
    /// <param name="avatarTypeIndex"></param>
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

    /// <summary>
    /// Initializes the game when the host starts it.
    /// </summary>
    public void StartGame()
    {
        // Pass the number of chairs in the room
        ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable();
        props["ChairCount"] = RoomManager.instance.chairsNumber;
        props["IsChatOn"] = RoomManager.instance.chatIsOn;
        props["IsVoiceChatOn"] = RoomManager.instance.voicechatIsOn;
        props["RoomCode"] = _roomCode;

        PhotonNetwork.CurrentRoom.SetCustomProperties(props);

        // Wait to make sure the property is set and then start the game scene (index = 1)
        StartCoroutine(WaitAndStartGame());

        // Disable all interactable objects in the room to prevent changing settings after clicking OnCLick()
        DisableAllInteractableObjects();

        // Show the countdown text
        _countdownText.gameObject.SetActive(true);
    }

    private IEnumerator WaitAndStartGame()//otherwise the data cannot be uploaded because the next scene is loaded faster
    {
        float countdown = _toWaitBeforeStart;
        while (countdown >= 0)
        {
            _countdownText.text = $"we will start in {Mathf.RoundToInt(countdown)}";
            yield return new WaitForSeconds(1f);
            countdown--;
        }
        // Start the scene "GameScene", under the index 1 in BuildSettings
        PhotonNetwork.LoadLevel(indexOfScene);
    }
    /// <summary>
    /// Disables all interactable objects in the room to prevent changing settings after clicking the Start Game button.
    /// </summary>
    private void DisableAllInteractableObjects()
    {
        _startGameButton.gameObject.SetActive(false);
        _chairsSlider.gameObject.SetActive(false);
        _chairsSliderText.gameObject.SetActive(false);
        _chatToggle.gameObject.SetActive(false);
        _voiceChatToggle.gameObject.SetActive(false);
        _sceneSelector.gameObject.SetActive(false);
        _leaveRoomButton.gameObject.SetActive(false);
        foreach (Transform item in _playerList) // Disable all buttons in the player list
        {
            Button temp = item.gameObject.GetComponentInChildren<Button>();
            if (temp != null)
            {
                temp.gameObject.SetActive(false);
            }
        }
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


    /// <summary>
    /// Updates the room settings based on the slider value.
    /// </summary>
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

    /// <summary>
    /// Shows or hides UI elements depending on whether the player is
    /// </summary>
    /// <param name="val"></param>
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
