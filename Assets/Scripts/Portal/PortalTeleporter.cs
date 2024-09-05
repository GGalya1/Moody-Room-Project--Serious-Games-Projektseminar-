using UnityEngine;
using Photon.Pun;

//die Klasse ist zur Erkennung der Kollision verantwortlich und zum schieben des Spielers
public class PortalTeleporter : MonoBehaviour, IUpdateObserver
{
    private Transform playerTransform;
    public Transform reciever;

    private bool playerIsOverlapping = false;

    #region UpdateManager connection
    private void OnEnable()
    {
        UpdateManager.Instance.RegisterObserver(this);
        UpdateManager.Instance.RegisterObserverName("PortalTeleporter");
    }
    private void OnDisable()
    {
        UpdateManager.Instance.UnregisterObserver(this);
        UpdateManager.Instance.UnregisterOberverName("PortalTeleporter");
    }
    private void OnDestroy()
    {
        UpdateManager.Instance.UnregisterObserver(this);
        UpdateManager.Instance.UnregisterOberverName("PortalTeleporter");
    }
    #endregion

    private void OnTriggerEnter(Collider other)
    {
        playerIsOverlapping = true;
    }
    private void OnTriggerExit(Collider other)
    {
        playerIsOverlapping = false;
    }

    public void ObservedUpdate()
    {
        if (playerIsOverlapping)
        {
            if (playerTransform == null)
            {
                Debug.LogError("Rufe Suche von PlayerTransform auf, da aktuelle Variable gleich null ist");
                FindPlayerTransform();
            }
            Vector3 portalToPlayer = playerTransform.position - transform.position;
            float dotProduct = Vector3.Dot(transform.up, portalToPlayer);

            //falls wahr, dann Spieler bewegt sich durch Portal
            if (dotProduct < 0f)
            {
                //wir teleportieren ihn
                float rotationDiff = -Quaternion.Angle(transform.rotation, reciever.rotation);
                //weil wir wollen auf die andere Seite von anderen Portal kommen
                rotationDiff += 180;
                playerTransform.Rotate(Vector3.up, rotationDiff);

                Vector3 positionOffset = Quaternion.Euler(0f, rotationDiff, 0f) * portalToPlayer;
                playerTransform.position = reciever.position + positionOffset;

                playerIsOverlapping = false;
            }
        }
    }

    //suchen nach dem Spieler (da die Variable aus unklaren Gruenden staendig auf null gesetzt wird)
    private void FindPlayerTransform()
    {
        PlayerController[] playerControllers = FindObjectsOfType<PlayerController>();

        foreach (PlayerController controller in playerControllers)
        {
            PhotonView photonView = controller.gameObject.GetComponent<PhotonView>();
            if (photonView != null && photonView.IsMine)
            {
                playerTransform = controller.gameObject.transform;
                Debug.Log("PlayerTransform gefunden.");
                break;
            }
        }

        if (playerTransform == null)
        {
            Debug.LogWarning("Kein PlayerTransform gefunden.");
        }
    }
}
