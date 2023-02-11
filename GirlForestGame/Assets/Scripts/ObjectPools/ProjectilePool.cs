using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectilePool : MonoBehaviour
{
    [SerializeField] GameObject astralOrbPrefab;
    [SerializeField] GameObject terrestrialOrbPrefab;

    public Queue<GameObject> availableAstralOrbs { get; private set; } = new Queue<GameObject>();
    public Queue<GameObject> availableTerrestrialOrbs { get; private set; } = new Queue<GameObject>();

    int numEachOrb = 10;

    public static ProjectilePool Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        CreatePools();
    }

    public GameObject GetProjectileFromPool(Planes plane, Vector3 position)
    {
        GameObject instance = null;

        switch (plane)
        {
            case Planes.Astral:

                if (availableAstralOrbs.Count == 0)
                {
                    CreatePools();
                }

                instance = availableAstralOrbs.Dequeue();
                instance.transform.position = position;
                instance.SetActive(true);

                break;

            case Planes.Terrestrial:

                if (availableTerrestrialOrbs.Count == 0)
                {
                    CreatePools();
                }

                instance = availableTerrestrialOrbs.Dequeue();
                instance.transform.position = position;
                instance.SetActive(true);

                break;
        }

        return instance;
    }

    private void CreatePools()
    {
        for (int i = 0; i < numEachOrb; ++i)
        {
            GameObject instanceToAdd = Instantiate(astralOrbPrefab);
            instanceToAdd.transform.SetParent(transform);
            AddProjectileToPool(Planes.Astral, instanceToAdd);
        }

        for (int i = 0; i < numEachOrb; ++i)
        {
            GameObject instanceToAdd = Instantiate(terrestrialOrbPrefab);
            instanceToAdd.transform.SetParent(transform);
            AddProjectileToPool(Planes.Terrestrial, instanceToAdd);
        }
    }

    public void AddProjectileToPool(Planes plane, GameObject instance)
    {
        switch (plane)
        {
            case Planes.Astral:

                availableAstralOrbs.Enqueue(instance);
                instance.SetActive(false);

                break;

            case Planes.Terrestrial:

                availableTerrestrialOrbs.Enqueue(instance);
                instance.SetActive(false);

                break;
        }
    }
}
