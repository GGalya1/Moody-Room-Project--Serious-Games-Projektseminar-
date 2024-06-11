using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManagerScript : MonoBehaviour
{
    //diesen Script ist fuer Verteilung von SpawnPoints zwischen Users verantwortlich

    public static SpawnManagerScript instance;

    SpawnpointScript[] spawnpoints;

    private void Awake()
    {
        instance = this;
        spawnpoints = GetComponentsInChildren<SpawnpointScript>();
    }

    public Transform GetSpawnpoint()
    {
        return spawnpoints[Random.Range(0, spawnpoints.Length)].transform;
    }
}
