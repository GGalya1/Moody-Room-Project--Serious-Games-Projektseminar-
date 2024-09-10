using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateManager : MonoBehaviour
{
    //Da diesen Objekt fuer den ganzen Projekt funktioniert, ist er Singleton.
    //Dazu ist hier Observer-Pattern realisiert, um die Geschwindigkeit des Codes zu leisten,
    //indem alle Update() in diesem einzelnem Objekt realisiert sind.
    #region Singleton declaration
    public static UpdateManager Instance { get; private set; }

    //konnte problematisch sein, da Awake() nicht deterministisch ist!
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion


    private static List<IUpdateObserver> _observers = new List<IUpdateObserver>();

    private void Update()
    {
        foreach (var observer in _observers)
        {
            observer.ObservedUpdate();
        }
        Debug.Log("Anzahl an Subscribers: " + _observers.Count);
    }

    public void RegisterObserver(IUpdateObserver observer)
    {
        if (!_observers.Contains(observer))
            _observers.Add(observer);
        
    }
    public void UnregisterObserver(IUpdateObserver observer)
    {
        if (_observers.Contains(observer))
            _observers.Remove(observer);
    }

    public void OnDestroy()
    {
        _observers.Clear();
    }
}
