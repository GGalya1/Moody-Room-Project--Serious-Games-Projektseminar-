using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleFileBrowser;
using UnityEngine.Networking;
using System.IO;
using Photon.Pun;

public class FileManager : MonoBehaviour
{
    [SerializeField] private MusikManager musikManager;
    public List<AudioClip> customTracks;

    public void Start()
    {
        customTracks = musikManager.customTracks;
    }

    public void OpenFileBrowserForMusikSearch()
    {
        // Filter: nur MP3 und WAV Dateien
        FileBrowser.SetFilters(true, new FileBrowser.Filter("Audio Files", ".mp3", ".wav"));
        FileBrowser.SetDefaultFilter(".mp3");

        // Show file browser
        StartCoroutine(ShowLoadDialogCoroutine());
    }
    private IEnumerator ShowLoadDialogCoroutine()
    {
        yield return FileBrowser.WaitForLoadDialog(FileBrowser.PickMode.Files, false, null, null, "Load Audio File", "Load");

        if (FileBrowser.Success)
        {
            // Pfad der ausgewaehlten Datei
            string path = FileBrowser.Result[0];

            // Audio-Datei laden
            StartCoroutine(LoadAudio(path));
        }
    }
    private IEnumerator LoadAudio(string path)
    {
        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip("file:///" + path, AudioType.UNKNOWN))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(www.error);
            }
            else
            {
                AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
                if (clip != null)
                {
                    clip.name = Path.GetFileNameWithoutExtension(path);
                    customTracks.Add(clip);
                    Debug.Log("Loaded audio clip: " + clip.name);

                    /*object[] temp = customTracks.ToArray();
                    PhotonView photonView = musikManager.GetComponent<PhotonView>();
                    photonView.RPC("RPC_SetCustomTracks", RpcTarget.Others, temp);*/

                    // Konvertiere AudioClip in Byte-Array
                    /*byte[] audioData = WavUtility.FromAudioClip(clip);

                    PhotonView photonView = musikManager.GetComponent<PhotonView>();
                    // Senden des neuen Liedes an andere Spieler
                    photonView.RPC("RPC_SendNewSong", RpcTarget.Others, audioData, clip.name);*/
                }
            }
        }
    }

    [PunRPC]
    public void RPC_SetCustomTracks(object[] listOfSongs)
    {
        for (int i = 0; i < listOfSongs.Length; i++)
        {
            AudioClip temp = (AudioClip) listOfSongs[i];
            Debug.Log($"{temp.name} was added to playlist!");
            musikManager.customTracks.Add(temp);
        }
    }

    [PunRPC]
    public void RPC_SendNewSong(byte[] audioData, string name)
    {
        // Konvertiere Byte-Array zurück in AudioClip
        AudioClip newClip = WavUtility.ToAudioClip(audioData, name);
        if (newClip != null)
        {
            musikManager.allTracks.Add(newClip);
            Debug.Log("Received new audio clip: " + newClip.name);
            
        }
    }
}
