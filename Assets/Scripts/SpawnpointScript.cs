using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnpointScript : MonoBehaviour
{
    [SerializeField] GameObject graphics;

    //da alle Spawnpoints nur fuer Entwickler sichtbar sein sollten
    private void Awake()
    {
        graphics.SetActive(false);
    }
}
