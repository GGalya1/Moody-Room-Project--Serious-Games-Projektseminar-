using UnityEngine;

public class ChairScript : MonoBehaviour, IInteractable
{
    Transform chairTransform;
    [SerializeField] private float sitOffsetY = 2.5f;

    void Start()
    {
        chairTransform = GetComponent<Transform>();
    }

    public void Interact(Transform playerPosition)
    {
        //es muss noch die Geschwindigkeit des Spielers vernichtet werden
        Vector3 tempTrans = chairTransform.position;
        tempTrans.y += sitOffsetY;
        playerPosition.position = tempTrans;
        playerPosition.rotation = chairTransform.rotation;
    }
    public void Interact()
    {

    }
}
