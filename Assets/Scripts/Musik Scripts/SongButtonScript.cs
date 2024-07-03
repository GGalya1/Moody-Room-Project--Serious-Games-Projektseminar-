using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Realtime;
using Photon.Pun;

public class SongButtonScript : MonoBehaviour
{
    public TMP_Text songNameOnButton;
    public TMP_Text songNameAsText;
    public AudioSource audioSource;
    public AudioClip audioClip;

    //damit wenn wir Playlists durchblaetern, unseren Playlist aktuell bleibt
    public MusikManager musikManager;

    public void Inizialise(AudioClip tempAudioClip, AudioSource tempAudioSource, TMP_Text tempSongName, MusikManager musikManager)
    {
        audioClip = tempAudioClip;
        songNameOnButton.text = tempAudioClip.name;
        songNameAsText = tempSongName;
        audioSource = tempAudioSource;
        this.musikManager = musikManager;
    }
    public void PlaySong()
    {
        audioSource.clip = audioClip;
        PhotonView photonView = musikManager.GetComponent<PhotonView>();
        photonView.RPC("RPC_SetClip", RpcTarget.Others, audioClip.name);
        songNameAsText.text = songNameOnButton.text;

        //leeren unseren Playlist und fuegen alle in der Liste (transform) gefundene Lieder in CurrentPlaylist
        Transform temp = musikManager.playlistContent.transform;
        musikManager.currentPlaylist = new List<AudioClip>();
        for (int i = 0; i < temp.childCount; i++)
        {
            musikManager.currentPlaylist.Add(temp.GetChild(i).gameObject.GetComponent<SongButtonScript>().audioClip);
        }
        musikManager.iWillThatMusicPlay = true;

        //hier taucht potenziell ein NullPointerException, wenn wir Track waehlen, das als letztes ist und currentTrackIndex wird 0
        musikManager.currentTrackIndex = musikManager.currentPlaylist.IndexOf(audioClip);
        musikManager.trackTimeSlider.value = 0;
        audioSource.Play();
        //GetComponentInParent<Image>().color = Color.yellow;

        photonView.RPC("RPC_PlayMusik", RpcTarget.Others);
        //musikManager.RPC_PlayMusik();
    }
    

    public void PushSongUp()
    {
        int trackIndex = musikManager.currentPlaylist.IndexOf(audioClip);
        musikManager.PushTrackUpInPlaylist(trackIndex);
    }
    public void PushSongDown()
    {
        int trackIndex = musikManager.currentPlaylist.IndexOf(audioClip);
        musikManager.PushTrackDownInPlaylist(trackIndex);
    }
}   

