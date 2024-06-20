using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;
using Photon.Voice.Unity;

public class RoomSettingManager : MonoBehaviour
{
    public GameObject chairPrefab;
    public Transform[] spawnPoints;
    public bool chatIsOn;

    public Recorder voiceRecorder;
    public bool voiceChatIsOn;

    public void Awake()
    {
        int _chairsCount = (int)PhotonNetwork.CurrentRoom.CustomProperties["ChairCount"];
        SpawnChairs(_chairsCount);
        chatIsOn = (bool)PhotonNetwork.CurrentRoom.CustomProperties["IsChatOn"];
        voiceChatIsOn = (bool)PhotonNetwork.CurrentRoom.CustomProperties["IsVoiceChatOn"];
        SetVoiceChat(voiceChatIsOn);
    }

    private void SpawnChairs(int chairsCount)
    {
        for (int i = 0; i < chairsCount; i++)
        {
            int spawnIndex = i % spawnPoints.Length;
            PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", chairPrefab.name), spawnPoints[spawnIndex].position, spawnPoints[spawnIndex].rotation);
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
    /*public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
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
    }*/
}
