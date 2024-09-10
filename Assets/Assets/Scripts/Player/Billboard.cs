using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour, IUpdateObserver
{
    //dient dazu, username in die richtige Richtung zu rotieren
    Camera cam;

    #region UpdateManager connection
    private void Awake()
    {
        UpdateManager.Instance.RegisterObserver(this);
    }
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
