using ExitGames.Client.Photon;
using Photon.Chat;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PhotonChatManager : MonoBehaviour, IChatClientListener
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
    string privateReciever = "";

    public void UserNameOnValueChange(string valueIn)
    {
        _username = valueIn;
    }
    public void Awake()
    {
        _username = PhotonNetwork.NickName;
        ChatConnect();
    }
    public void ChatConnect()
    {
        isConnected = true;
        chatClient = new ChatClient(this);
        chatClient.Connect(PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat, PhotonNetwork.AppVersion, new AuthenticationValues(_username));
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
        throw new System.NotImplementedException();
    }

    public void OnUserUnsubscribed(string channel, string user)
    {
        throw new System.NotImplementedException();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl) && !Pause.paused)
        {
            chatTrigger = !chatTrigger;
        }
        if (chatTrigger)
        {
            Cursor.visible = true;
            transform.GetChild(0).gameObject.SetActive(true);

            if (isConnected)
                chatClient.Service();

            if (chatField.text != "" && Input.GetKey(KeyCode.Return))
            {
                SubmitPublicChatOnClick();
                SubmitPrivateChatOnClick();
            }
        }
        else
        {
            Cursor.visible = false;
            transform.GetChild(0).gameObject.SetActive(false);
        }
        
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
}
