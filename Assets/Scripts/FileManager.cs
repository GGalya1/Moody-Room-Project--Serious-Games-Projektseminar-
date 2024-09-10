using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleFileBrowser;
using UnityEngine.Networking;
using System.IO;

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
                }
            }
        }
    }
}
