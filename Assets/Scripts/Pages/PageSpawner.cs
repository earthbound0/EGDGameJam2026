using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class PageSpawner : MonoBehaviour
{
    [Header("Spawning References")]
    public GameObject objectToSpawn;
    public Transform[] spawnPoints;
    public int PageCount = 10;

    [Header("Material Settings")]
    public Material[] pageMaterials = new Material[10];
    private int currentMaterialIndex = 0;

    private List<Transform> availableSpawnPoints = new List<Transform>();

    void Start()
    {
        RefillSpawnPoints();
    }

    void Update()
    {
        if (PageCount > 0)
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

        int randomIndex = UnityEngine.Random.Range(0, availableSpawnPoints.Count);
        Transform selectedSpawnPoint = availableSpawnPoints[randomIndex];
        availableSpawnPoints.RemoveAt(randomIndex);

        GameObject spawnedObject = Instantiate(objectToSpawn, selectedSpawnPoint.position, selectedSpawnPoint.rotation);

        Renderer rend = spawnedObject.GetComponent<Renderer>();
        if (rend != null && pageMaterials.Length > 0)
        {
            rend.material = pageMaterials[currentMaterialIndex % pageMaterials.Length];

            currentMaterialIndex++;
        }
        else if (rend == null)
        {
            UnityEngine.Debug.LogWarning("The spawned object is missing a Renderer component! Material not applied.");
        }
    }

    private void RefillSpawnPoints()
    {
        availableSpawnPoints.Clear();
        availableSpawnPoints.AddRange(spawnPoints);
    }
}