using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Realtime;
using Photon.Pun;


/// <summary>
/// Represents a UI item in the player list, displaying the name of the player.
/// Handles events such as a player leaving the room in <see cref="OnPlayerLeftRoom(Photon.Realtime.Player)"/>, and updates the UI accordingly.
/// </summary>
public class PlayerListItem : MonoBehaviourPunCallbacks
{
    [SerializeField] private TMP_Text playerName;
    private Player _player;

    /// <summary>
    /// Initializes the PlayerListItem with the player information.
    /// Sets the player name in the UI.
    /// </summary>
    /// <param name="player">The Photon Player object representing this player.</param>
    public void SetUp(Player player)
    {
        _player = player;
        playerName.text = player.NickName;
    }

    /// <summary>
    /// Called automatically by Photon when a player leaves the room.
    /// If the player represented by this item leaves, the item is destroyed.
    /// </summary>
    /// <param name="otherPlayer">The player who left the room.</param>
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (_player == otherPlayer) Destroy(gameObject);
    }

    public override void OnLeftRoom()
    {
        Destroy(gameObject);
    }
}
