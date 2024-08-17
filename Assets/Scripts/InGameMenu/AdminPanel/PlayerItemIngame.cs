using UnityEngine;
using TMPro;
using Photon.Realtime;
using Photon.Pun;
using UnityEngine.UI;
using Photon.Voice.Unity;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerItemIngame: MonoBehaviourPunCallbacks
{
    [SerializeField] private TMP_Text playerName;
    [SerializeField] private Button mutePlayerButton;
    [SerializeField] private Sprite unmuteImage;
    [SerializeField] private Sprite muteImage;
    private bool muted = false;

    [SerializeField] private Button kickPlayerButton;
    private Player _player;
    private Recorder recorder;
    private PhotonView _photonView;

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
            if (photonView.Owner == player)
            {
                return photonView;
            }
        }
        return null; // Kein PhotonView gefunden
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
