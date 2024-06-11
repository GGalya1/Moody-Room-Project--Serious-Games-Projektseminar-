using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Voice.PUN;

public class SpeakIndicatorScript : MonoBehaviour
{
    [SerializeField] private TMP_Text iSayText;
    [SerializeField] private TMP_Text iHearText;
    [SerializeField] private PhotonVoiceView photonVoiceView;

    private void Awake()
    {
        iSayText.gameObject.SetActive(false);
        iHearText.gameObject.SetActive(false);
    }
   
    // Update is called once per frame
    void Update()
    {
        if (photonVoiceView.IsSpeaking)
        {
            iHearText.gameObject.SetActive(photonVoiceView.IsSpeaking);
        }
        else
        {
            iSayText.gameObject.SetActive(photonVoiceView.IsRecording);
        }
    }
}
