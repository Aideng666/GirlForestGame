using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPool : MonoBehaviour
{
    [SerializeField] GameObject boarPrefab;
    //[SerializeField] GameObject sharpshooterPrefab;

    public Queue<GameObject> availableBoars { get; private set; } = new Queue<GameObject>();
    //Queue<GameObject> availableShooters = new Queue<GameObject>();

    float enemySpawnWallOffset = 5;
    int numEachEnemy = 5;

    public static EnemyPool Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        CreatePools();
    }

    Vector3 SelectSpawnPosition(GameObject instance)
    {
        //Selects a random spawn location for the enemy
        float randomXPos = Random.Range(DungeonGenerator.Instance.GetCurrentRoom().GetRoomObject().westCamBoundary,
            DungeonGenerator.Instance.GetCurrentRoom().GetRoomObject().eastCamBoundary);
        float randomZPos = Random.Range(DungeonGenerator.Instance.GetCurrentRoom().GetRoomObject().southCamBoundary,
            DungeonGenerator.Instance.GetCurrentRoom().GetRoomObject().northCamBoundary);

        Vector3 selectedSpawnPosition = new Vector3(randomXPos, instance.transform.position.y, randomZPos);

        //Checks if the selected spawn location is too close to an obstacle in the room
        foreach (GameObject obstacle in DungeonGenerator.Instance.GetCurrentRoom().GetSpawnedModel().obstacles)
        {
            if (Vector3.Distance(selectedSpawnPosition, obstacle.transform.position) < enemySpawnWallOffset)
            {
                return SelectSpawnPosition(instance);
            }
        }

        return selectedSpawnPosition;
    }

    public GameObject GetBoarFromPool()
    {
        if (availableBoars.Count == 0)
        {
            CreatePools();
        }

        GameObject instance = availableBoars.Dequeue();

        instance.transform.position = SelectSpawnPosition(instance);

        instance.SetActive(true);
        return instance;
    }

    //public GameObject GetShooterFromPool(Vector3 spawnerPos)
    //{
    //    if (availableShooters.Count == 0)
    //    {
    //        CreatePools();
    //    }

    //    var instance = availableShooters.Dequeue();

    //    instance.transform.position = spawnerPos;

    //    instance.SetActive(true);
    //    return instance;
    //}

    private void CreatePools()
    {
        for (int i = 0; i < numEachEnemy; ++i)
        {
            GameObject instanceToAdd = Instantiate(boarPrefab);
            instanceToAdd.transform.SetParent(transform);
            AddBoarToPool(instanceToAdd);
        }

        //for (int i = 0; i < 50; ++i)
        //{
        //    var instanceToAdd = Instantiate(sharpshooterPrefab);
        //    instanceToAdd.transform.SetParent(transform);
        //    AddToShooterPool(instanceToAdd);
        //}
    }

    public void AddBoarToPool(GameObject instance)
    {
        instance.SetActive(false);
        availableBoars.Enqueue(instance);
    }

    //public void AddToShooterPool(GameObject instance)
    //{
    //    instance.SetActive(false);
    //    availableShooters.Enqueue(instance);
    //}
}

public enum EnemyTypes
{
    Boar,
    MushroomSpirit,
    StoneGolem
}
