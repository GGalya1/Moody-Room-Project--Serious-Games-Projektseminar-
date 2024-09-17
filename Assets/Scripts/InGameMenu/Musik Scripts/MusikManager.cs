using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using System.Linq;
using System;


public class MusikManager : MonoBehaviour, IUpdateObserver
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

    private Color pressedButtonColor;

    public List<AudioClip> currentPlaylist;
    public int currentTrackIndex;
    public bool iWillThatMusicPlay = false;

    [SerializeField] public Slider trackTimeSlider;

    //um das ganze zu synchronisieren
    private PhotonView photonView;

    //BAUARBEITEN: wir senden Musik per Photon
    private const int PacketSize = 300 * 1024; //jedes Packet wird 300KB gross
    private List<byte[]> audioPackets = new List<byte[]>(); //um alle Paketen zwischenzuspeichern

    public void SendAudioClip(AudioClip clip)
    {
        byte[] audioData = ConvertAudioClipToBytes(clip); // Конвертируем AudioClip в байты

        int totalPackets = Mathf.CeilToInt((float)audioData.Length / PacketSize); // Количество пакетов
        for (int i = 0; i < totalPackets; i++)
        {
            int start = i * PacketSize;
            int length = Mathf.Min(PacketSize, audioData.Length - start); // Определяем размер текущего пакета

            byte[] packet = new byte[length];
            System.Array.Copy(audioData, start, packet, 0, length); // Копируем часть аудиоданных в пакет

            photonView.RPC("RPC_ReceiveAudioPacket", RpcTarget.All, packet, i, totalPackets, clip.frequency); // Отправляем пакет
        }
    }
    public byte[] ConvertAudioClipToBytes(AudioClip clip)
    {
        float[] samples = new float[clip.samples];
        clip.GetData(samples, 0);

        byte[] bytes = new byte[samples.Length * sizeof(float)];
        Buffer.BlockCopy(samples, 0, bytes, 0, bytes.Length);

        return bytes;
    }

    [PunRPC]
    public void RPC_ReceiveAudioPacket(byte[] packet, int packetIndex, int totalPackets, int freq)
    {
        if (audioPackets.Count == 0)
        {
            audioPackets = new List<byte[]>(new byte[totalPackets][]); // Инициализируем список для пакетов
        }

        audioPackets[packetIndex] = packet; // Сохраняем пакет на его место

        // Проверяем, все ли пакеты получены
        if (audioPackets.All(p => p != null))
        {
            // Все пакеты получены, собираем аудиоданные
            byte[] fullAudioData = audioPackets.SelectMany(p => p).ToArray();
            AudioClip clip = ConvertBytesToAudioClip(fullAudioData, freq); // Преобразуем байты обратно в AudioClip

            // Воспроизводим аудио
            musikAudioSource.clip = clip;
            PlayMusik();
            allTracks.Add(clip);

            // Очищаем список пакетов
            audioPackets.Clear();
        }
    }
    public AudioClip ConvertBytesToAudioClip(byte[] audioData, int sampleRate)
    {
        int sampleCount = audioData.Length / sizeof(float);
        float[] samples = new float[sampleCount];
        Buffer.BlockCopy(audioData, 0, samples, 0, audioData.Length);

        AudioClip clip = AudioClip.Create("ReceivedAudio", sampleCount, 1, sampleRate, false);
        clip.SetData(samples, 0);
        return clip;
    }



    //BAUARBEITEN: wir senden Musik per Photon

    #region UpdateManager connection
    private void Awake()
    {
        UpdateManager.Instance.RegisterObserver(this);
        UpdateManager.Instance.RegisterObserverName("MusikManager");
    }
    private void OnDestroy()
    {
        UpdateManager.Instance.UnregisterObserver(this);
        UpdateManager.Instance.UnregisterOberverName("MusikManager");
    }
    #endregion

    private void Start()
    {
        photonView = GetComponent<PhotonView>();

        musikName.text = "music was not selected";
        musikAudioSource.loop = loopTrackToggle.isOn;
        loopTrackToggle.onValueChanged.AddListener(OnLoopToggleValueChanged);
        volumeSlider.onValueChanged.AddListener(OnVolumeValueChanged);
        trackTimeSlider.onValueChanged.AddListener(OnTrackTimeValueChanged);

        allTracks.AddRange(chillTracks);
        allTracks.AddRange(battleTracks);

        currentPlaylist = new List<AudioClip>();

        pressedButtonColor = HexToColor("#9A92B980");
    }
    public void ObservedUpdate()
    {
        if (PhotonNetwork.IsMasterClient)
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
    }
    public void PlayMusik()
    {
        iWillThatMusicPlay = true;
        musikName.text = musikAudioSource.clip.name;
        musikAudioSource.Play();
        photonView.RPC("RPC_PlayMusik", RpcTarget.Others);
    }
    [PunRPC]
    public void RPC_PlayMusik()
    {
        iWillThatMusicPlay = true;
        musikAudioSource.Play();
    }
    [PunRPC]
    public void RPC_SetClip(string nameOfClip)
    {
        AudioClip clip = allTracks.Find(c => c.name.Equals(nameOfClip));
        musikAudioSource.clip = clip;
    }

    public void StopMusik()
    {
        if (musikAudioSource.isPlaying)
        {
            iWillThatMusicPlay = false;
            musikAudioSource.Pause();
            photonView.RPC("RPC_StopMusik", RpcTarget.Others);
        }
    }
    [PunRPC]
    public void RPC_StopMusik()
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
            RPC_SetClip(musikAudioSource.clip.name);
        }
        else
        {
            currentTrackIndex = 0;
            musikAudioSource.clip = currentPlaylist[currentTrackIndex];
            photonView.RPC("RPC_SetClip", RpcTarget.Others, musikAudioSource.clip.name);
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
        allTracksButton.image.color = pressedButtonColor;
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
        chillPlaylistButton.image.color = pressedButtonColor;
        for (int i = 0; i < chillTracks.Count; i++)
        {
            SongButtonScript temp = Instantiate(songPrefab, playlistContent.transform).GetComponent<SongButtonScript>();
            temp.Inizialise(chillTracks[i], musikAudioSource, musikName, this);
        }
    }
    public void OpenBattlePlaylist()
    {
        DisableAllPlaylistUI();
        battlePlaylistButton.image.color = pressedButtonColor;
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
        photonView.RPC("RPC_OnLoopToggleValueChanged", RpcTarget.Others, isOn);
    }
    [PunRPC]
    public void RPC_OnLoopToggleValueChanged(bool isOn)
    {
        musikAudioSource.loop = isOn;
    }

    private void OnVolumeValueChanged(float volume)
    {
        musikAudioSource.volume = volume;
        volumeText.text = $"Volume: {Mathf.RoundToInt(volume * 100)}%";

        photonView.RPC("RPC_OnVolumeValueChanged", RpcTarget.Others, volume);
    }
    [PunRPC]
    public void RPC_OnVolumeValueChanged(float volume)
    {
        musikAudioSource.volume = volume;
    }

    private void OnTrackTimeValueChanged(float value)
    {
        musikAudioSource.time = value;
        UpdateTrackTime();

        photonView.RPC("RPC_OnTrackTimeValueChanged", RpcTarget.Others, value);
    }
    [PunRPC]
    public void RPC_OnTrackTimeValueChanged(float value)
    {
        musikAudioSource.time = value;
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

    Color HexToColor(string hex)
    {
        // Entferne das `#` am Anfang des Strings, falls vorhanden
        hex = hex.TrimStart('#');

        // Extrahiere die RGBA-Werte aus dem Hex-String
        float r = Convert.ToInt32(hex.Substring(0, 2), 16) / 255f;
        float g = Convert.ToInt32(hex.Substring(2, 2), 16) / 255f;
        float b = Convert.ToInt32(hex.Substring(4, 2), 16) / 255f;
        float a = Convert.ToInt32(hex.Substring(6, 2), 16) / 255f;

        return new Color(r, g, b, a);
    }
}
