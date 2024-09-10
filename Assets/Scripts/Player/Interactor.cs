using System;
using UnityEngine;

//Interface, die alle Objekte implementieren, mit denen Spieler eine Interaktion durchfuehren kann
public interface IInteractable
{
    public void Interact();
    public void Interact(Transform transform);
}

public class Interactor : MonoBehaviour
{
    public static Action OnInteractionRequest; // Event fuer MenuAnfragen
    public Transform InteractionSource;
    [SerializeField] private Transform avatarTransform;
    public float InteractRange = 1f;

    //abonieren zu einem Event, der InputManager schicken wird
    private void OnEnable()
    {
        OnInteractionRequest += HandleInteractionRequest;
    }

    private void OnDisable()
    {
        OnInteractionRequest -= HandleInteractionRequest;
    }

    //aus unserem Camera senden wir ein Ray in die Rueckwaertsrichtung
    private void HandleInteractionRequest()
    {
        Ray r = new Ray(InteractionSource.position, InteractionSource.forward);
        if (Physics.Raycast(r, out RaycastHit hitInfo, InteractRange))
        {
            if (hitInfo.collider.gameObject.TryGetComponent(out IInteractable interactObj))
            {
                //koenen zusaetzlich checken, ob Objekt ein Stuhl ist.
                //dazu bewegen wir hier nicht das Spieler, sondern Kamera
                interactObj.Interact(avatarTransform);
            }
        }
    }
}
