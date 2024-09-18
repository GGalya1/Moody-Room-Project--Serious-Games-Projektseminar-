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
    [SerializeField] private DrawingUIManager whiteBoardManager;
    public List<AudioClip> customTracks;

    public void Start()
    {
        customTracks = musikManager.customTracks;
    }

    #region musicLoading
    public void OpenFileBrowserForMusikSearch()
    {
        // Filter: nur MP3 und WAV Dateien
        FileBrowser.SetFilters(true, new FileBrowser.Filter("Audio Files", ".mp3", ".wav"));
        FileBrowser.SetDefaultFilter(".mp3");

        // Show file browser
        StartCoroutine(ShowLoadMusicDialogCoroutine());
    }
    private IEnumerator ShowLoadMusicDialogCoroutine()
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
                    musikManager.SendAudioClip(clip);
                    Debug.Log("Music was loaded");
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
    #endregion

    #region imageLoading
    public void OpenFileBrowserForImagesSearch()
    {
        // Filter: nur PNG und JPEG
        FileBrowser.SetFilters(true, new FileBrowser.Filter("Images", ".png", ".jpg"));
        FileBrowser.SetDefaultFilter(".png");

        // Show file browser
        StartCoroutine(ShowLoadImageDialogCoroutine());
    }
    private IEnumerator ShowLoadImageDialogCoroutine()
    {
        yield return FileBrowser.WaitForLoadDialog(FileBrowser.PickMode.Files, false, null, null, "Load Image", "Load");

        if (FileBrowser.Success)
        {
            // Pfad der ausgewaehlten Datei
            string path = FileBrowser.Result[0];

            // Audio-Datei laden
            StartCoroutine(LoadImage(path));
        }
    }
    private IEnumerator LoadImage(string path)
    {
        using (UnityWebRequest www = UnityWebRequestTexture.GetTexture("file:///" + path))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(www.error);
            }
            else
            {
                // Texture laden
                Texture2D texture = DownloadHandlerTexture.GetContent(www);

                if (texture != null)
                {
                    texture = whiteBoardManager.ResizeTexture(texture, whiteBoardManager.GetRectTransform().rect.width, whiteBoardManager.GetRectTransform().rect.height);
                    whiteBoardManager.image = texture;
                    whiteBoardManager.ShareImage();
                    //whiteBoardManager.drawingBoard.SetImage(texture);
                    Debug.Log("Image was loaded");
                }
            }
        }
    }
    #endregion
}
