using System;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public static Action OnButtonHover;
    public static Action OnButtonClick;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private Canvas canvas;

    [Header("Sound clips")]
    [SerializeField] private AudioClip hoverSound;
    [SerializeField] private AudioClip clickSound;

    private void Start()
    {
        AddSoundToButtons(canvas.transform);
    }

    void OnEnable()
    {
        // Abonniere das Event
        OnButtonHover += PlayHoverSound;
        OnButtonClick += PlayClickSound;
    }

    void OnDisable()
    {
        // Trenne das Event
        OnButtonHover -= PlayHoverSound;
        OnButtonClick -= PlayClickSound;
    }

    private void PlayHoverSound()
    {
        if (hoverSound != null)
        {
            audioSource.PlayOneShot(hoverSound);
        }
    }
    private void PlayClickSound()
    {
        if (clickSound != null)
        {
            audioSource.PlayOneShot(clickSound);
        }
    }

    //methode, die rekursiv alle Kinder durchgeht und findet GameObjects mit dem Komponent "Button"
    //fuegt zu diesem Objekt neuen Komponent, der OnClick eine Anfrage an diesem Script senden wird
    void AddSoundToButtons(Transform parent)
    {
        foreach (Transform child in parent)
        {
            //ueberprüfe, ob das Kind eine Button-Komponente hat
            Button button = child.GetComponent<Button>();
            if (button != null)
            {
                //fuege das ButtonSound-Komponent hinzu, falls es nicht bereits vorhanden ist
                if (child.GetComponent<ButtonSound>() == null)
                {
                    child.gameObject.AddComponent<ButtonSound>();
                }

                //wenn ein Button gefunden wurde, nicht weiter tiefer in diese Hierarchie gehen (da kein Sinn macht, falls Button in sich weitere Buttons enthaelt)
                continue;
            }

            //rekursiven Aufruf der Methode, um auch die Kinder des aktuellen Kindes zu ueberpruefen
            if (child.childCount > 0)
            {
                AddSoundToButtons(child);
            }
        }
    }
}
