using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using TMPro;

public class PlayerBoard : MonoBehaviourPunCallbacks, IUpdateObserver
{
    [SerializeField] Transform container;
    [SerializeField] GameObject playerboardItemPrefab;
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] TMP_Text roomName;
    public static bool playerBoardIsOn = false;

    Dictionary<Player, PlayerBoardItem> boardItems = new Dictionary<Player, PlayerBoardItem>();

    private void Start()
    {
        roomName.text = "there are all the Kittens from the room " + PhotonNetwork.CurrentRoom.Name;
        foreach(Player player in PhotonNetwork.PlayerList)
        {
            AddBoardItem(player);
        }
    }

    #region UpdateManager connection
    private void Awake()
    {
        UpdateManager.Instance.RegisterObserver(this);
    }
    public override void OnEnable()
    {
        UpdateManager.Instance.RegisterObserver(this);
    }
    public override void OnDisable()
    {
        UpdateManager.Instance.UnregisterObserver(this);
    }
    private void OnDestroy()
    {
        UpdateManager.Instance.UnregisterObserver(this);
    }
    #endregion

    void AddBoardItem(Player player)
    {
        PlayerBoardItem item = Instantiate(playerboardItemPrefab, container).GetComponent<PlayerBoardItem>();
        item.Initialize(player);
        boardItems[player] = item;
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        AddBoardItem(newPlayer);
    }
    public override void OnPlayerLeftRoom(Player newPlayer)
    {
        foreach(Player player in PhotonNetwork.PlayerList)
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

    public void ObservedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Tab) && !Pause.paused)
        {
            playerBoardIsOn = true;
            canvasGroup.alpha = 1;
        }
        else if (Input.GetKeyUp(KeyCode.Tab) && !Pause.paused)
        {
            playerBoardIsOn = false;
            canvasGroup.alpha = 0;
        }
    }
}
