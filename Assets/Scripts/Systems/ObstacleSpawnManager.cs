﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleSpawnManager : MonoBehaviour 
{
    [Header("Obstacles")]
    public GameObject playerCharacter;

    [Space(10)]
    public Transform obstacleHolder;

    [Space(10)]
    public GameObject[] obstaclePrefabs;
    public List<GameObject> activeObstacleList = new List<GameObject>();
    private List<GameObject> toBeRemovedObjects = new List<GameObject>();

    GameObject spawnedObstacle;

    [Header("Obstacle Spawn Settings")]
    public float baseSpawnTimeMin = 1f;
    public float baseSpawnTimeMax = 1f;

    [Space(10)]
    public float spawnDistanceMax;
    public float spawnDistanceMin;

    [Space(10)]
    public float obstacleMaxDistance = 200f;

    private void Start()
    {
        InitializeObstacleManager();
    }

    private void Update()
    {
        ManageActiveObstacles();
        DestroyOutOfBoundsObstacles();
    }

    private void InitializeObstacleManager()
    {
        if (playerCharacter == null)
        {
            playerCharacter = GameObject.FindGameObjectWithTag("Player");
        }

        PrepareNextObstacle();
    }

    private void PrepareNextObstacle()
    {
        float nextObstacleSpawnTime = ReturnRandomObstacleSpawnTime();

        StartCoroutine(SpawnObstacleAfterTime(nextObstacleSpawnTime));
    }

    private IEnumerator SpawnObstacleAfterTime(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        SpawnObject();

        PrepareNextObstacle();

        yield return null;
    }

    void SpawnObject() 
    {
        spawnedObstacle = Instantiate(obstaclePrefabs[Random.Range(0, (obstaclePrefabs.Length))], obstacleHolder);
        SetSpawnLocation(spawnedObstacle);

        activeObstacleList.Add(spawnedObstacle);
    }

    void SetSpawnLocation(GameObject objectToPosition) 
    {
        Vector3 spawnLocation = new Vector3();

        spawnLocation.x = ResolveRandomMath() * (Mathf.Clamp((Mathf.Abs(Random.insideUnitSphere.x) * Random.Range(spawnDistanceMin, spawnDistanceMax)), spawnDistanceMin, spawnDistanceMax));
        spawnLocation.y = 1;
        spawnLocation.z = ResolveRandomMath()* (Mathf.Clamp((Mathf.Abs(Random.insideUnitSphere.z) * Random.Range(spawnDistanceMin, spawnDistanceMax)), spawnDistanceMin, spawnDistanceMax));
        objectToPosition.transform.position = spawnLocation;
        //Set Approach Vector
        objectToPosition.GetComponent<Asteroid>().obstacleTrajectoryDirection = FindApproachVector(spawnLocation, playerCharacter.transform.position);
        objectToPosition.GetComponent<Asteroid>().canObstacleMove = true;
    }

    float ResolveRandomMath() 
    {
        if (Random.insideUnitSphere.x > 0)
        {
            return -1f;
        }
        else {
            return 1f;
        }

    }

    Vector3 FindApproachVector(Vector3 objSpawnPosition, Vector3 playerCurrentPosition) 
    {
        Vector3 approachVector = new Vector3();
        approachVector.x =  (playerCurrentPosition.x * Random.Range(-5f,5f)) - objSpawnPosition.x;
        approachVector.y = 0 ;
        approachVector.z = (playerCurrentPosition.z * Random.Range(-5f,5f)) - objSpawnPosition.z;

        approachVector.Normalize();
        return approachVector;

    }

    private float ReturnRandomObstacleSpawnTime()
    {
        return Random.Range(baseSpawnTimeMin, baseSpawnTimeMax);
    }

    private void ManageActiveObstacles()
    {
        if (activeObstacleList.Count > 0)
        {
            foreach (GameObject obstacle in activeObstacleList)
            {
                if (Vector3.Distance(playerCharacter.transform.position, obstacle.transform.position) > obstacleMaxDistance)
                {
                    toBeRemovedObjects.Add(obstacle);

                    activeObstacleList.Remove(obstacle);
                }
            }
        }
    }

    private void DestroyOutOfBoundsObstacles()
    {
        if (toBeRemovedObjects.Count > 0)
        {
            for (int i = toBeRemovedObjects.Count - 1; i >= 0; i--)
            {
                Debug.Log("Obstacle Exceeded Max Range :: Destroying :: " + toBeRemovedObjects[i]);

                Destroy(toBeRemovedObjects[i]);
            }
        }
    }
    
}

