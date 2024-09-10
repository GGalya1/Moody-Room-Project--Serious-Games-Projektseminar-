using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Voice.PUN;

public class SpeakIndicatorScript : MonoBehaviour, IUpdateObserver
{
    [Header("Speak indicator holders")]
    [SerializeField] private TMP_Text iSayText;
    [SerializeField] private TMP_Text iHearText;
    [SerializeField] private PhotonVoiceView photonVoiceView;

    #region UpdateManager connection
    private void OnEnable()
    {
        UpdateManager.Instance.RegisterObserver(this);
    }
    private void OnDisable()
    {
        UpdateManager.Instance.UnregisterObserver(this);
    }
    private void OnDestroy()
    {
        UpdateManager.Instance.UnregisterObserver(this);
    }
    #endregion

    private void Awake()
    {
        UpdateManager.Instance.RegisterObserver(this);
        iSayText.gameObject.SetActive(false);
        iHearText.gameObject.SetActive(false);
    }
   
    // Update is called once per frame
    public void ObservedUpdate()
    {
        if (photonVoiceView.IsSpeaking)
        {
            iSayText.gameObject.SetActive(photonVoiceView.IsSpeaking);
        }
        else if (photonVoiceView.IsRecording)
        {
            iHearText.gameObject.SetActive(photonVoiceView.IsRecording);
        }
        else
        {
            iSayText.gameObject.SetActive(false);
            iHearText.gameObject.SetActive(false);
        }
    }
}
