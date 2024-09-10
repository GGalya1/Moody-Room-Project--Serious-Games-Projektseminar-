using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;
using Photon.Voice.Unity;
using TMPro;

public class RoomSettingManager : MonoBehaviourPunCallbacks
{
    public GameObject chairPrefab;
    public List<GameObject> chairsList;
    public Transform[] spawnPoints;
    public bool chatIsOn;

    public Recorder voiceRecorder;
    public bool voiceChatIsOn;

    MusikManager musikManager;

    public void Awake()
    {
        int _chairsCount = (int)PhotonNetwork.CurrentRoom.CustomProperties["ChairCount"];
        chairsList = new List<GameObject>();
        SpawnChairs(_chairsCount);
        chatIsOn = (bool)PhotonNetwork.CurrentRoom.CustomProperties["IsChatOn"];
        voiceChatIsOn = (bool)PhotonNetwork.CurrentRoom.CustomProperties["IsVoiceChatOn"];
        SetVoiceChat(voiceChatIsOn);
    }

    private void SpawnChairs(int chairsCount)
    {
        int curr = chairsList.Count;
        
        for (int i = curr; i < chairsCount; i++)
        {
            int spawnIndex = i % spawnPoints.Length;
            chairsList.Add(PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", chairPrefab.name), spawnPoints[spawnIndex].position, spawnPoints[spawnIndex].rotation));
        }
        for (int i = curr; i > chairsCount; i--)
        {
            PhotonNetwork.Destroy(chairsList[i-1]);
            chairsList.RemoveAt(chairsList.Count-1);
        }
    }
    private void SetVoiceChat(bool isEnabled)
    {
        if (voiceRecorder != null)
        {
            voiceRecorder.TransmitEnabled = isEnabled;
            Debug.Log("VoiceChatEnabled set to: " + isEnabled);
        }
        else
        {
            Debug.LogWarning("Voice Recorder not found!");
        }
    }

    //spaeter wird fuer Admin-Panel nuetzlich
    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        if (propertiesThatChanged.ContainsKey("ChairCount"))
        {
            int chairCount = (int)propertiesThatChanged["ChairCount"];
            SpawnChairs(chairCount);
        }

        if (propertiesThatChanged.ContainsKey("IsVoiceChatOn"))
        {
            bool voiceChatEnabled = (bool)propertiesThatChanged["IsVoiceChatOn"];
            SetVoiceChat(voiceChatEnabled);
        }
        if (propertiesThatChanged.ContainsKey("IsChatOn"))
        {
            bool chatEnabled = (bool)propertiesThatChanged["IsChatOn"];
            RoomManager.instance.chatIsOn = chatEnabled;
            chatIsOn = chatEnabled;
        }
    }
}
