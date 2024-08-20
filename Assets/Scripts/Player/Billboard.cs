using UnityEngine;

public class Billboard : MonoBehaviour, IUpdateObserver
{
    //dient dazu, username in die richtige Richtung zu rotieren
    Camera cam;

    #region UpdateManager connection
    private void Awake()
    {
        UpdateManager.Instance.RegisterObserver(this);
        UpdateManager.Instance.RegisterObserverName("Billboard");
    }
    private void OnEnable()
    {
        UpdateManager.Instance.RegisterObserver(this);
        UpdateManager.Instance.RegisterObserverName("Billboard");
    }
    private void OnDisable()
    {
        UpdateManager.Instance.UnregisterObserver(this);
        UpdateManager.Instance.UnregisterOberverName("Billboard");
    }
    private void OnDestroy()
    {
        UpdateManager.Instance.UnregisterObserver(this);
        UpdateManager.Instance.UnregisterOberverName("Billboard");
    }
    #endregion

    // Update is called once per frame
    public void ObservedUpdate()
    {
        //falls Camera nicht gefunden wird, dann versuchen wir eine zu finden
        if (cam == null)
            cam = FindObjectOfType<Camera>();
        if (cam == null)
            return;
        transform.LookAt(cam.transform);
        transform.Rotate(Vector3.up * 180); //da sonst username gespiegelt war
    }
}
