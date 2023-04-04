using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnemyPool : MonoBehaviour
{
    [SerializeField] GameObject boarPrefab;
    [SerializeField] GameObject mushroomPrefab;
    [SerializeField] GameObject draugrPrefab;
    [SerializeField] GameObject stoneGolemPrefab;

    //For Damage Numbers UI
    [SerializeField] GameObject dmgNumbersPrefab;

    public Queue<GameObject> availableBoars { get; private set; } = new Queue<GameObject>();
    public Queue<GameObject> availableMushrooms { get; private set; } = new Queue<GameObject>();
    public Queue<GameObject> availableDraugrs { get; private set; } = new Queue<GameObject>();
    public Queue<GameObject> availableGolems { get; private set; } = new Queue<GameObject>();
    public Queue<GameObject> availableNumbers { get; private set; } = new Queue<GameObject>();

    float enemySpawnWallOffset = 5;
    int numEachEnemy = 5;
    int amountOfDmgNumbers = 10;

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

    public GameObject GetEnemyFromPool(EnemyTypes enemy)
    {
        GameObject instance = null;

        switch (enemy)
        {
            case EnemyTypes.Boar:

                if (availableBoars.Count == 0)
                {
                    CreatePools();
                }

                instance = availableBoars.Dequeue();
                instance.transform.position = SelectSpawnPosition(instance);
                instance.SetActive(true);

                break;

            case EnemyTypes.MushroomSpirit:

                if (availableMushrooms.Count == 0)
                {
                    CreatePools();
                }

                instance = availableMushrooms.Dequeue();
                instance.transform.position = SelectSpawnPosition(instance);
                instance.SetActive(true);

                break;

            case EnemyTypes.Draugr:

                if (availableDraugrs.Count == 0)
                {
                    CreatePools();
                }

                instance = availableDraugrs.Dequeue();
                instance.transform.position = SelectSpawnPosition(instance);
                instance.SetActive(true);

                break;

            case EnemyTypes.StoneGolem:

                if (availableGolems.Count == 0)
                {
                    CreatePools();
                }

                instance = availableGolems.Dequeue();
                instance.transform.position = SelectSpawnPosition(instance);
                instance.SetActive(true);

                break;
        }

        return instance;
    }

    public GameObject GetDmgNumberFromPool(Vector3 position, string text)
    {
        if (availableNumbers.Count == 0)
        {
            CreateNumberPool();
        }

        GameObject instance = availableNumbers.Dequeue();
        instance.transform.position = position;
        instance.SetActive(true);

        instance.GetComponentInChildren<TextMeshProUGUI>().text = text;

        return instance;
    }

    public void CreateNumberPool()
    {
        for (int i = 0; i < amountOfDmgNumbers; i++)
        {
            GameObject instanceToAdd = Instantiate(dmgNumbersPrefab);
            instanceToAdd.transform.SetParent(transform);
            AddNumberToPool(instanceToAdd);
        }
    }

    private void CreatePools()
    {
        for (int i = 0; i < numEachEnemy; ++i)
        {
            GameObject instanceToAdd = Instantiate(boarPrefab);
            instanceToAdd.transform.SetParent(transform);
            AddEnemyToPool(EnemyTypes.Boar, instanceToAdd);
        }

        for (int i = 0; i < numEachEnemy; ++i)
        {
            GameObject instanceToAdd = Instantiate(mushroomPrefab);
            instanceToAdd.transform.SetParent(transform);
            AddEnemyToPool(EnemyTypes.MushroomSpirit, instanceToAdd);
        }

        for (int i = 0; i < numEachEnemy; ++i)
        {
            GameObject instanceToAdd = Instantiate(draugrPrefab);
            instanceToAdd.transform.SetParent(transform);
            AddEnemyToPool(EnemyTypes.Draugr, instanceToAdd);
        }

        for (int i = 0; i < numEachEnemy; ++i)
        {
            GameObject instanceToAdd = Instantiate(stoneGolemPrefab);
            instanceToAdd.transform.SetParent(transform);
            AddEnemyToPool(EnemyTypes.StoneGolem, instanceToAdd);
        }
    }

    public void AddEnemyToPool(EnemyTypes enemy, GameObject instance)
    {
        switch (enemy)
        {
            case EnemyTypes.Boar:

                instance.SetActive(false);
                availableBoars.Enqueue(instance);

                break;

            case EnemyTypes.MushroomSpirit:

                instance.SetActive(false);
                availableMushrooms.Enqueue(instance);

                break;

            case EnemyTypes.Draugr:

                instance.SetActive(false);
                availableDraugrs.Enqueue(instance);

                break;

            case EnemyTypes.StoneGolem:

                instance.SetActive(false);
                availableGolems.Enqueue(instance);

                break;
        }
    }

    public void AddNumberToPool(GameObject instance)
    {
        instance.SetActive(false);
        availableNumbers.Enqueue(instance);
    }
}

public enum EnemyTypes
{
    Boar,
    MushroomSpirit,
    StoneGolem,
    Draugr
}
