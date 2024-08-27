using UnityEngine;
using TMPro;
using Photon.Realtime;
using Photon.Pun;
using UnityEngine.UI;
using Photon.Voice.Unity;


public class PlayerItemIngame: MonoBehaviourPunCallbacks
{
    [SerializeField] private TMP_Text playerName;

    //Funktionalitaeten, um das Spieler zu muten
    [SerializeField] private Button mutePlayerButton;
    [SerializeField] private Sprite unmuteImage;
    [SerializeField] private Sprite muteImage;
    private bool muted = false;

    //Funktionalitaeten, um Whiteboard fuer das Spieler an/ausschalten
    [SerializeField] private Button whiteboardButton;
    [SerializeField] private Sprite boardOnImage;
    [SerializeField] private Sprite boardOffImage;
    private bool whiteBoardOn = true;

    [SerializeField] private Button kickPlayerButton;
    private Player _player;
    private Recorder recorder;
    private PhotonView _photonView; //wofuer brauchen wir das?

    public void SetUp(Player player, Recorder recorder)
    {
        _player = player;
        playerName.text = player.NickName;
        this.recorder = recorder;

        //damit man sich selbst nicht kicken kann
        if (player.IsLocal)
        {
            kickPlayerButton.gameObject.SetActive(false);
        }
        mutePlayerButton.image.sprite = unmuteImage;
        _photonView = GetPhotonViewFromPlayer(_player);
    }

    public void MutePlayer()
    {
        muted = !muted;
        mutePlayerButton.image.sprite = muted ? muteImage : unmuteImage;

        if (recorder != null)
        {
            recorder.TransmitEnabled = !muted;
        }
    }
    public void KickPlayer()
    {
        /*if (_photonView != null)
        {
            _photonView.RPC("KickPlayerRPC", _player);
        }
        else
        {
            Debug.LogWarning("photonView von " + _player.NickName + "war nicht gefunden");
        }*/
        PhotonNetwork.CloseConnection(_player);
    }

    PhotonView GetPhotonViewFromPlayer(Player player)
    {
        // Durchlaufe alle PhotonViews in der Szene
        PhotonView[] photonViews = FindObjectsOfType<PhotonView>();
        foreach (PhotonView photonView in photonViews)
        {
            if (photonView.gameObject.name == "PlayerManager(Clone)" && photonView.Owner == player)
            {
                return photonView;
            }
        }
        return null; // Kein PhotonView gefunden
    }

    private void SetWhiteboardAcces(Player player, bool isOn)
    {
        Debug.Log("Ich sende die Anfrage an Spieler");
        _photonView.RPC("RPC_SetWhiteboardAcces", player, isOn);
    }
    private void SetWhiteboardOn()
    {
        whiteboardButton.image.sprite = boardOnImage;
        SetWhiteboardAcces(_player, true);
    }
    private void SetWhiteboardOff()
    {
        whiteboardButton.image.sprite = boardOffImage;
        SetWhiteboardAcces(_player, false);
    }
    public void SwitchWhiteboard()
    {
        whiteBoardOn = !whiteBoardOn;
        if (whiteBoardOn)
        {
            SetWhiteboardOn();
        }
        else
        {
            SetWhiteboardOff();
        }
    }


    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (_player == otherPlayer)
        {
            Destroy(gameObject);
        }
    }

    public override void OnLeftRoom()
    {
        Destroy(gameObject);
    }
}
