using UnityEngine;
using TMPro;
using Photon.Realtime;
using Photon.Pun;
using UnityEngine.UI;

public class PlayerListItemForAdmin: MonoBehaviourPunCallbacks
{
    [SerializeField] private TMP_Text playerName;
    [SerializeField] private Button kickPlayerButton;
    private Player _player;

    public void SetUp(Player player)
    {
        _player = player;
        playerName.text = player.NickName;
        //damit man sich selbst nicht kicken kann
        if (player.IsLocal)
        {
            kickPlayerButton.gameObject.SetActive(false);
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (_player == otherPlayer) Destroy(gameObject);
    }

    public override void OnLeftRoom()
    {
        Destroy(gameObject);
    }

    public void KickPlayer()
    {
        PhotonNetwork.CloseConnection(_player);
    }
}
