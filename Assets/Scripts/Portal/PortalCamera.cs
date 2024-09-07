using UnityEngine;

public class PortalCamera : MonoBehaviour, IUpdateObserver
{

    public Transform playerCamera;
    public Transform portal;
    public Transform otherPortal;
    
    public void SetCamera(Transform myCam)
    {
        playerCamera = myCam;
        UpdateManager.Instance.RegisterObserver(this);
    }

    public void OnDestroy()
    {
        UpdateManager.Instance.UnregisterObserver(this);
    }

    // Update is called once per frame
    public void ObservedUpdate()
    {
        Vector3 playerOffsetFromPortal = playerCamera.position - otherPortal.position;
        transform.position = portal.position + playerOffsetFromPortal;

        float angularDifferenceBetweenPortalRotations = Quaternion.Angle(portal.rotation, otherPortal.rotation);

        Quaternion portalRotationDifference = Quaternion.AngleAxis(angularDifferenceBetweenPortalRotations, Vector3.up);
        Vector3 newCameraDirection = portalRotationDifference * playerCamera.forward;
        transform.rotation = Quaternion.LookRotation(newCameraDirection, Vector3.up);
    }
}
