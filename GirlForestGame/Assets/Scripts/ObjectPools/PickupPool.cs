using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupPool : MonoBehaviour
{
    [SerializeField] GameObject halfHeartPrefab;
    [SerializeField] GameObject heartPrefab;
    [SerializeField] GameObject coinPrefab;

    Queue<GameObject> availableHalfHearts = new Queue<GameObject>();
    Queue<GameObject> availableHearts = new Queue<GameObject>();
    Queue<GameObject> availableCoins = new Queue<GameObject>();

    int numHearts = 5;
    int numCoins = 10;

    public static PickupPool Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        CreatePools();
    }

    public GameObject GetHalfHeartFromPool(Vector3 spawnPos)
    {
        if (availableHalfHearts.Count == 0)
        {
            CreatePools();
        }

        GameObject instance = availableHalfHearts.Dequeue();

        instance.transform.position = spawnPos;

        instance.SetActive(true);
        return instance;
    }

    public GameObject GetHeartFromPool(Vector3 spawnPos)
    {
        if (availableHearts.Count == 0)
        {
            CreatePools();
        }

        var instance = availableHearts.Dequeue();

        instance.transform.position = spawnPos;

        instance.SetActive(true);
        return instance;
    }

    public GameObject GetCoinFromPool(Vector3 spawnPos)
    {
        if (availableCoins.Count == 0)
        {
            CreatePools();
        }

        var instance = availableCoins.Dequeue();

        instance.transform.position = spawnPos;

        instance.SetActive(true);
        return instance;
    }

    private void CreatePools()
    {
        for (int i = 0; i < numHearts; ++i)
        {
            GameObject instanceToAdd = Instantiate(halfHeartPrefab);
            instanceToAdd.transform.SetParent(transform);
            AddHalfHeart(instanceToAdd);

            instanceToAdd = Instantiate(heartPrefab);
            instanceToAdd.transform.SetParent(transform);
            AddHeart(instanceToAdd);
        }

        for (int i = 0; i < numCoins; ++i)
        {
            var instanceToAdd = Instantiate(coinPrefab);
            instanceToAdd.transform.SetParent(transform);
            AddCoin(instanceToAdd);
        }
    }

    public void AddHalfHeart(GameObject instance)
    {
        instance.SetActive(false);
        availableHalfHearts.Enqueue(instance);
    }

    public void AddHeart(GameObject instance)
    {
        instance.SetActive(false);
        availableHearts.Enqueue(instance);
    }

    public void AddCoin(GameObject instance)
    {
        instance.SetActive(false);
        availableCoins.Enqueue(instance);
    }
}
