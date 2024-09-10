using ExitGames.Client.Photon;
using Photon.Chat;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Realtime;

public class PhotonChatManager : MonoBehaviour, IChatClientListener, IUpdateObserver
{
    ChatClient chatClient;
    [SerializeField] string _username = PhotonNetwork.NickName;
    bool isConnected;
    //bool chatRegistered = false;
    public static bool chatTrigger = false;
    [SerializeField] TMP_InputField chatField;
    [SerializeField] TMP_Text chatDisplay;
    [SerializeField] GameObject joinChatButton;
    [SerializeField] GameObject chatPanel;
    string currentChat;
    public string privateReciever = "";
    [SerializeField] private RoomSettingManager roomSettingManager;

    //um die Liste des Spielers sehen zu koennen
    [SerializeField] private Transform _playerList;
    [SerializeField] private GameObject _playerNicknameButtonPrefab;
    private Player[] oldListOfPlayers; //um zu schauen, ob die Liste sich aktualisiert hat oder nicht
    [SerializeField] public TMP_Text sendMessageToButton_Text;
    private bool isPlayerlistOn;

    #region UpdateManager connection
    private void OnEnable()
    {
        UpdateManager.Instance.RegisterObserver(this);
    }
    private void OnDisable()
    {
        UpdateManager.Instance.UnregisterObserver(this);
    }
    private void OnDestroy()
    {
        UpdateManager.Instance.UnregisterObserver(this);
    }
    #endregion

    public void UserNameOnValueChange(string valueIn)
    {
        _username = valueIn;
    }
    public void Awake()
    {
        UpdateManager.Instance.RegisterObserver(this);
        _username = PhotonNetwork.NickName;
        ChatConnect();
    }
    public void ChatConnect()
    {
        isConnected = true;
        chatClient = new ChatClient(this);
        chatClient.Connect(PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat, PhotonNetwork.AppVersion, new Photon.Chat.AuthenticationValues(_username));
        Debug.Log("Connecting to the chat...");
    }

    public void DebugReturn(DebugLevel level, string message)
    {
        //throw new System.NotImplementedException();
    }

    public void OnChatStateChange(ChatState state)
    {
        if (state == ChatState.Uninitialized)
        {
            isConnected = false;
            ChatConnect();
            //joinChatButton.SetActive(true);
            //chatPanel.SetActive(false);
        }
    }

    public void OnConnected()
    {
        Debug.Log("Connected to the chat!");
        joinChatButton.SetActive(false);
        chatClient.Subscribe(new string[] { "RegionChannel"});
    }

    public void OnDisconnected()
    {
        isConnected = false;
        ChatConnect();
        //joinChatButton.SetActive(true);
        //chatPanel.SetActive(false);
    }

    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        string message = "";
        for (int i = 0; i < senders.Length; i++)
        {
            message = string.Format("{0}: {1}", senders[i], messages[i]);
            chatDisplay.text += "\n" + message;
            Debug.Log(message);
        }
    }

    public void OnPrivateMessage(string sender, object message, string channelName)
    {
        string msg = "";
        msg = string.Format("(private) {0}: {1}", sender, message);
        chatDisplay.text += "\n" + msg;
        Debug.Log(msg);
    }

    public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
    {
        throw new System.NotImplementedException();
    }

    public void OnSubscribed(string[] channels, bool[] results)
    {
        chatPanel.SetActive(true);
    }

    public void OnUnsubscribed(string[] channels)
    {
        throw new System.NotImplementedException();
    }

    public void OnUserSubscribed(string channel, string user)
    {
        UpdatePlayerList();
        //throw new System.NotImplementedException();
        /*Debug.Log("YES, IT MUSS BE DISPLAYED A BUTTON");
        Player[] players = PhotonNetwork.PlayerList;

        for (int i = 0; i < _playerList.childCount; i++)
        {
            Destroy(_playerList.GetChild(i).gameObject);
        }


        for (int i = 0; i < players.Length; i++)
        {
            Instantiate(_playerNicknameButtonPrefab, _playerList).GetComponent<PlayerListItem>().SetUp(players[i]);
        }*/
    }

    public void OnUserUnsubscribed(string channel, string user)
    {
        throw new System.NotImplementedException();
    }

    public void UpdatePlayerList()
    {
        Player[] players = PhotonNetwork.PlayerList;

        for (int i = 1; i < _playerList.childCount; i++)
        {
            Destroy(_playerList.GetChild(i).gameObject);
        }


        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].NickName.Equals(PhotonNetwork.NickName))
            {
                continue;
            }
            Instantiate(_playerNicknameButtonPrefab, _playerList).GetComponent<PlayerChatItem>().SetUp(players[i]);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        oldListOfPlayers = PhotonNetwork.PlayerList;
        UpdatePlayerList();
    }

    // Update is called once per frame
    public void ObservedUpdate()
    {
        if (ChatCanBeOpened())
        {
            chatTrigger = !chatTrigger;
        }
        if (chatTrigger)
        {
            Cursor.visible = true;
            transform.GetChild(0).gameObject.SetActive(true);

            if (CheckPlayerListChanged(oldListOfPlayers, PhotonNetwork.PlayerList))
            {
                UpdatePlayerList();
                oldListOfPlayers = (Player[]) PhotonNetwork.PlayerList.Clone(); //magic aus dem Internet
            }
            

            if (isConnected)
                chatClient.Service();

            if (chatField.text != "" && Input.GetKey(KeyCode.Return))
            {
                SubmitPublicChatOnClick();
                SubmitPrivateChatOnClick();
            }
        }
        else if(Pause.paused || AdminPanelScript.adminPanelIsOn || DrawingUIManager.whiteboardOn)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Confined;
            transform.GetChild(0).gameObject.SetActive(false);
        }
        
    }
    public bool ChatCanBeOpened()
    {
        return Input.GetKeyDown(KeyCode.LeftControl) && //wenn wir versuchen, Chat zu oeffnen
            !Pause.paused &&
            !AdminPanelScript.adminPanelIsOn && 
            roomSettingManager.chatIsOn;
    }

    public void TypeChatOnValueChange(string valueIn)
    {
        currentChat = valueIn;
    }

    public void SubmitPublicChatOnClick()
    {
        if(privateReciever == "")
        {
            chatClient.PublishMessage("RegionChannel", currentChat);
            chatField.text = "";
            currentChat = "";
        }
    }
    public void SubmitPrivateChatOnClick()
    {
        if (privateReciever != "")
        {
            chatClient.SendPrivateMessage(privateReciever, currentChat);
            chatField.text = "";
            currentChat = "";
        }
    }
    public void RecieverOnValueChange(string valueIn)
    {
        privateReciever = valueIn;
    }

    //Funktionalitat, um Nichname von Spieler zu kriegen, der im Button war
    private bool CheckPlayerListChanged(Player[] oldL, Player[] newL)
    {
        if (oldL == null || newL == null || oldL.Length != newL.Length)
        {
            return true;
        }

        //hier werden nickname gespeichert. Falls zwei Leuten haben gleichen - wird auch behandelt
        Dictionary<string, int> lookUp = new Dictionary<string, int>();

        //befuellen HashSet mit Nicknames (nicht mit "Player")
        for (int i = 0; i < oldL.Length; i++)
        {
            int count = 0;
            if (!lookUp.TryGetValue(oldL[i].NickName, out count))
            {
                lookUp.Add(oldL[i].NickName, 1);
                continue;
            }
            lookUp[oldL[i].NickName] = count + 1;
        }

        for (int i = 0; i < newL.Length; i++)
        {
            int count = 0;
            if (!lookUp.TryGetValue(newL[i].NickName, out count))
            {
                // early exit as the current value in newL doesn't exist in the lookUp (and not in ListA)
                return false;
            }
            count--;
            if (count <= 0)
                lookUp.Remove(newL[i].NickName);
            else
                lookUp[newL[i].NickName] = count;
        }

        // if there are remaining elements in the lookUp, that means oldL contains elements that do not exist in newL
        return lookUp.Count != 0;
    }

    public void ShowListOfRecievers()
    {
        isPlayerlistOn = !isPlayerlistOn;
        _playerList.gameObject.SetActive(isPlayerlistOn);
    }
    public void SetRecieverToPublic()
    {
        sendMessageToButton_Text.text = "All";
        privateReciever = "";
    }
    public void HideListOfRecievers()
    {
        _playerList.gameObject.SetActive(false);
    }
}
