using System.Collections.Generic;
using UnityEngine;

public class PageSpawner : MonoBehaviour
{
    [Header("Spawning References")]
    public GameObject objectToSpawn;       
    public Transform[] spawnPoints; 
    public int PageCount = 10;

    private List<Transform> availableSpawnPoints = new List<Transform>();

    void Start()
    {
        RefillSpawnPoints();
    }

    void Update()
    {
        if(PageCount > 0)
        {
            SpawnObjectAtRandomPoint();
            PageCount--;
        }
    }

    public void SpawnObjectAtRandomPoint()
    {
        if (availableSpawnPoints.Count == 0)
        {
            RefillSpawnPoints();
        }

        int randomIndex = Random.Range(0, availableSpawnPoints.Count);

        Transform selectedSpawnPoint = availableSpawnPoints[randomIndex];

        availableSpawnPoints.RemoveAt(randomIndex);

        Instantiate(objectToSpawn, selectedSpawnPoint.position, selectedSpawnPoint.rotation);
    }

    private void RefillSpawnPoints()
    {
        availableSpawnPoints.Clear();
        availableSpawnPoints.AddRange(spawnPoints);
    }
}