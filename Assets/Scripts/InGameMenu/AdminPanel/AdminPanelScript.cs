using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class AdminPanelScript : MonoBehaviour
{
    [SerializeField] private GameObject panel; //Panel hat keine eigene Klasse, darum GameObjekt
    [SerializeField] private GameObject roomSettingPanel; //Sammlung von allen Objekten, die zu diesem Thema gehoeren
    [SerializeField] private GameObject musikAndSoundPanel;
    [SerializeField] private GameObject playerListPanel;
    [SerializeField] private Slider chairsSlider;
    [SerializeField] private TMP_Text chairsText;
    [SerializeField] private Toggle chatToggle;
    [SerializeField] private Toggle voiceChatToggle;
    [SerializeField] private Dropdown skyboxDropdown;

    [SerializeField] private Material[] skyboxMaterials;
    PhotonView photonView;
    private int selectedSkyboxIndex = 0;

    public void Start()
    {
        //jetzt muessen wir Werte entsprechend der gewaehlten Infos in Menu waehlen
        chatToggle.isOn = RoomManager.instance.chatIsOn;
        voiceChatToggle.isOn = RoomManager.instance.voicechatIsOn;
        chairsSlider.value = RoomManager.instance.chairsNumber;

        photonView = GetComponent<PhotonView>();

        chairsSlider.onValueChanged.AddListener(OnSliderValueChanged);
        OnSliderValueChanged(chairsSlider.value);

        chatToggle.onValueChanged.AddListener(OnChatToggleValueChanged);
        voiceChatToggle.onValueChanged.AddListener(OnVoiceChatToggleValueChanged);

        //Fuellen Drowdown mit den Werten aus dem Array
        skyboxDropdown.options.Clear();
        for (int i = 0; i < skyboxMaterials.Length; i++)
        {
            skyboxDropdown.options.Add(new Dropdown.OptionData(skyboxMaterials[i].name));
        }

        skyboxDropdown.onValueChanged.AddListener(delegate { OnDropdownValueChanged(); });
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
            playerListPanel.SetActive(false);
            musikAndSoundPanel.SetActive(true);
        }
    }
    public void OpenRoomSettingMenu()
    {
        if (panel.activeSelf)
        {
            musikAndSoundPanel.SetActive(false);
            playerListPanel.SetActive(false);
            roomSettingPanel.SetActive(true);
        }
    }
    public void OpenPlayerList()
    {
        if (panel.activeSelf)
        {
            musikAndSoundPanel.SetActive(false);
            roomSettingPanel.SetActive(false);
            playerListPanel.SetActive(true);
        }
    }

    public void OnDropdownValueChanged()
    {
        selectedSkyboxIndex = skyboxDropdown.value;
        photonView.RPC("ChangeSkyboxForAll", RpcTarget.AllBuffered, selectedSkyboxIndex);
    }
    [PunRPC]
    void ChangeSkyboxForAll(int index)
    {
        RenderSettings.skybox = skyboxMaterials[index];
        DynamicGI.UpdateEnvironment(); //Update von globalen Beleuchtung
    }
}
