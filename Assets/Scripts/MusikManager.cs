using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MusikManager : MonoBehaviour
{
    public AudioSource musikAudioSource;
    [SerializeField] private TMP_Text musikName;
    [SerializeField] private Toggle loopTrackToggle;
    [SerializeField] private TMP_Text trackTimetext;

    [SerializeField] private TMP_Text volumeText;
    [SerializeField] private Slider volumeSlider;

    [SerializeField] private Button allTracksButton;
    [SerializeField] private Button battlePlaylistButton;
    [SerializeField] private Button chillPlaylistButton;
    [SerializeField] public GameObject playlistContent;
    public List<AudioClip> allTracks;
    public List<AudioClip> battleTracks;
    public List<AudioClip> chillTracks;
    public List<AudioClip> customTracks;
    [SerializeField] private GameObject songPrefab;

    public List<AudioClip> currentPlaylist;
    public int currentTrackIndex;
    public bool iWillThatMusicPlay = false;

    [SerializeField] public Slider trackTimeSlider;

    private void Start()
    {
        musikName.text = "music was not selected";
        musikAudioSource.loop = loopTrackToggle.isOn;
        loopTrackToggle.onValueChanged.AddListener(OnLoopToggleValueChanged);
        volumeSlider.onValueChanged.AddListener(OnVolumeValueChanged);
        trackTimeSlider.onValueChanged.AddListener(OnTrackTimeValueChanged);

        allTracks.AddRange(chillTracks);
        allTracks.AddRange(battleTracks);

        currentPlaylist = new List<AudioClip>();
    }
    private void Update()
    {
        if (musikAudioSource.isPlaying)
        {
            UpdateTrackTime();
            UpdateTrackSlider();
        }
        else if (iWillThatMusicPlay && currentPlaylist != null && currentPlaylist.Count > 0 && !musikAudioSource.isPlaying && musikAudioSource.time == 0)
        {
            PlayNextTrackInPlaylist();
        }
    }
    public void PlayMusik()
    {
        iWillThatMusicPlay = true;
        musikName.text = musikAudioSource.clip.name;
        musikAudioSource.Play();
    }
    public void StopMusik()
    {
        iWillThatMusicPlay = false;
        musikAudioSource.Pause();
    }
    public void PlayNextTrackInPlaylist()
    {
        if (currentTrackIndex + 1 < currentPlaylist.Count)
        {
            musikAudioSource.clip = currentPlaylist[currentTrackIndex + 1];
            currentTrackIndex++;
        }
        else
        {
            currentTrackIndex = 0;
            musikAudioSource.clip = currentPlaylist[currentTrackIndex];
        }
        trackTimeSlider.value = 0;
        PlayMusik();
    }
    public void UpdateTrackTime()
    {
        if (musikAudioSource.clip != null)
        {
            trackTimetext.text = $"{Mathf.RoundToInt(musikAudioSource.time / 60)}:{Mathf.RoundToInt(musikAudioSource.time % 60)} / {Mathf.RoundToInt(musikAudioSource.clip.length / 60)}:{Mathf.RoundToInt(musikAudioSource.clip.length % 60)}";
        }
    }
    public void UpdateTrackSlider()
    {
        if (musikAudioSource.clip != null)
        {
            trackTimeSlider.maxValue = musikAudioSource.clip.length;
            trackTimeSlider.value = musikAudioSource.time;
        }
    }

    public void OpenAllSongs()
    {
        DisableAllPlaylistUI();
        allTracksButton.image.color = Color.yellow;
        for (int i = 0; i < customTracks.Count; i++)
        {
            SongButtonScript temp = Instantiate(songPrefab, playlistContent.transform).GetComponent<SongButtonScript>();
            temp.Inizialise(customTracks[i], musikAudioSource, musikName, this);
        }
        for (int i = 0; i < chillTracks.Count; i++)
        {
            SongButtonScript temp = Instantiate(songPrefab, playlistContent.transform).GetComponent<SongButtonScript>();
            temp.Inizialise(chillTracks[i], musikAudioSource, musikName, this);

        }
        for (int i = 0; i < battleTracks.Count; i++)
        {
            SongButtonScript temp = Instantiate(songPrefab, playlistContent.transform).GetComponent<SongButtonScript>();
            temp.Inizialise(battleTracks[i], musikAudioSource, musikName, this);
        }
    }
    public void OpenChillPlaylist()
    {
        DisableAllPlaylistUI();
        chillPlaylistButton.image.color = Color.yellow;
        for (int i = 0; i < chillTracks.Count; i++)
        {
            SongButtonScript temp = Instantiate(songPrefab, playlistContent.transform).GetComponent<SongButtonScript>();
            temp.Inizialise(chillTracks[i], musikAudioSource, musikName, this);
        }
    }
    public void OpenBattlePlaylist()
    {
        DisableAllPlaylistUI();
        battlePlaylistButton.image.color = Color.yellow;
        for (int i = 0; i < battleTracks.Count; i++)
        {
            SongButtonScript temp = Instantiate(songPrefab, playlistContent.transform).GetComponent<SongButtonScript>();
            temp.Inizialise(battleTracks[i], musikAudioSource, musikName, this);
        }
    }
    public void DisableAllPlaylistUI()
    {
        allTracksButton.image.color = Color.white;
        battlePlaylistButton.image.color = Color.white;
        chillPlaylistButton.image.color = Color.white;
        Transform parentTransform = playlistContent.transform;
        foreach (Transform child in parentTransform)
        {
            Destroy(child.gameObject);
        }
    }

    private void OnLoopToggleValueChanged(bool isOn)
    {
        musikAudioSource.loop = isOn;
    }
    private void OnVolumeValueChanged(float volume)
    {
        musikAudioSource.volume = volume;
        volumeText.text = $"Volume: {Mathf.RoundToInt(volume * 100)}%";
    }
    private void OnTrackTimeValueChanged(float value)
    {
        musikAudioSource.time = value;
        UpdateTrackTime();
    }

    //andaern die Reihenfolge von Track
    public void PushTrackUpInPlaylist(int trackIndex)
    {
        if (trackIndex <= 0 || trackIndex >= currentPlaylist.Count)
            return;

        //aktualisieren von currentTrackIndex, damit kein OutOfBounds passiert
        if (trackIndex == currentTrackIndex)
        {
            currentTrackIndex--;
        }
        else if(trackIndex == currentTrackIndex - 1)
        {
            currentTrackIndex++;
        }
        
        //tauschen in der Liste
        AudioClip temp = currentPlaylist[trackIndex];
        currentPlaylist[trackIndex] = currentPlaylist[trackIndex - 1];
        currentPlaylist[trackIndex - 1] = temp;

        RefreshPlaylistUI();
    }
    public void PushTrackDownInPlaylist(int trackIndex)
    {
        if (trackIndex < 0 || trackIndex >= currentPlaylist.Count - 1)
            return;

        if (trackIndex == currentTrackIndex)
        {
            currentTrackIndex++;
        }
        else if (trackIndex == currentTrackIndex + 1)
        {
            currentTrackIndex--;
        }

        AudioClip temp = currentPlaylist[trackIndex];
        currentPlaylist[trackIndex] = currentPlaylist[trackIndex + 1];
        currentPlaylist[trackIndex + 1] = temp;

        RefreshPlaylistUI();
    }

    public void RefreshPlaylistUI()
    {
        //kopiert von DisableAllPlaylistUI (((
        Transform parentTransform = playlistContent.transform;
        foreach (Transform child in parentTransform)
        {
            Destroy(child.gameObject);
        }

        //jetzt erstellen wir die aktualisierte Liste
        for (int i = 0; i < currentPlaylist.Count; i++)
        {
            SongButtonScript temp = Instantiate(songPrefab, playlistContent.transform).GetComponent<SongButtonScript>();
            temp.Inizialise(currentPlaylist[i], musikAudioSource, musikName, this);
        }
    }
}
