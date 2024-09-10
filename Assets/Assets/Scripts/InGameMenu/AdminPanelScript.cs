using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class AdminPanelScript : MonoBehaviour, IUpdateObserver
{
    [SerializeField] private GameObject panel; //Panel hat keine eigene Klasse, darum GameObjekt
    [SerializeField] private GameObject roomSettingPanel; //Sammlung von allen Objekten, die zu diesem Thema gehoeren
    [SerializeField] private GameObject musikAndSoundPanel;
    [SerializeField] private GameObject diceRollerPanel;
    [SerializeField] private Slider chairsSlider;
    [SerializeField] private TMP_Text chairsText;
    [SerializeField] private Toggle chatToggle;
    [SerializeField] private Toggle voiceChatToggle;
    public static bool adminPanelIsOn;

    #region UpdateManager connection
    private void Awake()
    {
        UpdateManager.Instance.RegisterObserver(this);
    }
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

    public void Start()
    {
        //jetzt muessen wir Werte entsprechend der gewaehlten Infos in Menu waehlen
        chatToggle.isOn = RoomManager.instance.chatIsOn;
        voiceChatToggle.isOn = RoomManager.instance.voicechatIsOn;
        chairsSlider.value = RoomManager.instance.chairsNumber;

        chairsSlider.onValueChanged.AddListener(OnSliderValueChanged);
        OnSliderValueChanged(chairsSlider.value);

        chatToggle.onValueChanged.AddListener(OnChatToggleValueChanged);
        voiceChatToggle.onValueChanged.AddListener(OnVoiceChatToggleValueChanged);
    }
    public void ObservedUpdate()
    {
        if(PhotonNetwork.IsMasterClient && Input.GetKeyDown(KeyCode.RightControl) && !Pause.paused)
        {
            adminPanelIsOn = !adminPanelIsOn;
            panel.gameObject.SetActive(adminPanelIsOn);
            Cursor.visible = adminPanelIsOn;
            if (adminPanelIsOn)
            {
                Cursor.lockState = CursorLockMode.None;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Confined;
            }
            
        }

    }

    public void OnSliderValueChanged(float value)
    {
        int chairNumber = Mathf.RoundToInt(value);
        RoomManager.instance.chairsNumber = chairNumber;

        UpdateRoomHash();
        chairsText.text = $"Chair Number: {chairNumber}";
    }
    public void OnChatToggleValueChanged(bool isOn)
    {
        RoomManager.instance.chatIsOn = isOn;
        UpdateRoomHash();
    }
    public void OnVoiceChatToggleValueChanged(bool isOn)
    {
        RoomManager.instance.voicechatIsOn = isOn;
        UpdateRoomHash();
    }

    public void UpdateRoomHash() {
        ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable();
        props["ChairCount"] = RoomManager.instance.chairsNumber;
        props["IsChatOn"] = RoomManager.instance.chatIsOn;
        props["IsVoiceChatOn"] = RoomManager.instance.voicechatIsOn;
        PhotonNetwork.CurrentRoom.SetCustomProperties(props);
    }

    public void OpenMusikMenu()
    {
        if (panel.activeSelf)
        {
            roomSettingPanel.SetActive(false);
            diceRollerPanel.SetActive(false);
            musikAndSoundPanel.SetActive(true);
        }
    }
    public void OpenRoomSettingMenu()
    {
        if (panel.activeSelf)
        {
            musikAndSoundPanel.SetActive(false);
            diceRollerPanel.SetActive(false);
            roomSettingPanel.SetActive(true);
        }
    }
    public void OpenDiceRoller()
    {
        if (panel.activeSelf)
        {
            roomSettingPanel.SetActive(false);
            musikAndSoundPanel.SetActive(false);
            diceRollerPanel.SetActive(true);
        }
    }
}
