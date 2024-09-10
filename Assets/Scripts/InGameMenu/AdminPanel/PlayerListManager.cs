using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using Photon.Voice.Unity;

public class PlayerListManager : MonoBehaviourPunCallbacks
{
    [SerializeField] Transform container;
    [SerializeField] GameObject playerItemPrefab;
    private List<Recorder> allRecorders;

    Dictionary<Player, PlayerItemIngame> boardItems = new Dictionary<Player, PlayerItemIngame>();

    private void Start()
    {
        CollectAllRecorders();
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            AddBoardItem(player);
        }
        
    }
    void AddBoardItem(Player player)
    {
        PlayerItemIngame item = Instantiate(playerItemPrefab, container).GetComponent<PlayerItemIngame>();
        item.SetUp(player, GetRecorderForPlayer(player));
        boardItems[player] = item;
    }
    private void CollectAllRecorders()
    {
        allRecorders = new List<Recorder>();

        Recorder[] recordersInScene = FindObjectsOfType<Recorder>();

        foreach (var recorder in recordersInScene)
        {
            allRecorders.Add(recorder);
        }
    }
    public Recorder GetRecorderForPlayer(Player player)
    {
        foreach (var recorder in allRecorders)
        {
            PhotonView photonView = recorder.GetComponent<PhotonView>();

            if (photonView != null && photonView.Owner == player)
            {
                return recorder;
            }
        }

        return null; // Falls kein Recorder gefunden wurde
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        AddBoardItem(newPlayer);
    }
    public override void OnPlayerLeftRoom(Player newPlayer)
    {
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (player.IsMasterClient)
            {
                RemoveBoardItem(player);
                AddBoardItem(player);
            }
        }
        RemoveBoardItem(newPlayer);
    }
    void RemoveBoardItem(Player player)
    {
        Destroy(boardItems[player].gameObject);
        boardItems.Remove(player);
    }
}
