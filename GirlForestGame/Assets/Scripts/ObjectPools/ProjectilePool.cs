using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectilePool : MonoBehaviour
{
    [SerializeField] GameObject astralMushroomOrbPrefab;
    [SerializeField] GameObject terrestrialMushroomOrbPrefab;
    [SerializeField] GameObject astralGolemRock;
    [SerializeField] GameObject terrestrialGolemRock;

    public Queue<GameObject> availableAstralMushroomOrbs { get; private set; } = new Queue<GameObject>();
    public Queue<GameObject> availableTerrestrialMushroomOrbs { get; private set; } = new Queue<GameObject>();
    public Queue<GameObject> availableAstralGolemRocks { get; private set; } = new Queue<GameObject>();
    public Queue<GameObject> availableTerrestrialGolemRocks { get; private set; } = new Queue<GameObject>();

    int numEachOrb = 10;

    public static ProjectilePool Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        CreatePools();
    }

    public GameObject GetProjectileFromPool(Planes plane, Vector3 position, EnemyTypes enemyType)
    {
        GameObject instance = null;

        switch (plane)
        {
            case Planes.Astral:

                if (enemyType == EnemyTypes.MushroomSpirit)
                {
                    if (availableAstralMushroomOrbs.Count == 0)
                    {
                        CreatePools();
                    }

                    instance = availableAstralMushroomOrbs.Dequeue();
                    instance.transform.position = position;
                    instance.SetActive(true);
                }
                else if (enemyType == EnemyTypes.StoneGolem)
                {
                    if (availableAstralGolemRocks.Count == 0)
                    {
                        CreatePools();
                    }

                    instance = availableAstralGolemRocks.Dequeue();
                    instance.transform.position = position;
                    instance.SetActive(true);
                }

                break;

            case Planes.Terrestrial:

                if (enemyType == EnemyTypes.MushroomSpirit)
                {
                    if (availableTerrestrialMushroomOrbs.Count == 0)
                    {
                        CreatePools();
                    }

                    instance = availableTerrestrialMushroomOrbs.Dequeue();
                    instance.transform.position = position;
                    instance.SetActive(true);
                }
                else if (enemyType == EnemyTypes.StoneGolem)
                {
                    if (availableTerrestrialGolemRocks.Count == 0)
                    {
                        CreatePools();
                    }

                    instance = availableTerrestrialGolemRocks.Dequeue();
                    instance.transform.position = position;
                    instance.SetActive(true);
                }

                break;
        }

        return instance;
    }

    private void CreatePools()
    {
        for (int i = 0; i < numEachOrb; ++i)
        {
            GameObject instanceToAdd = Instantiate(astralMushroomOrbPrefab);
            instanceToAdd.transform.SetParent(transform);
            AddProjectileToPool(Planes.Astral, instanceToAdd, EnemyTypes.MushroomSpirit);
        }

        for (int i = 0; i < numEachOrb; ++i)
        {
            GameObject instanceToAdd = Instantiate(terrestrialMushroomOrbPrefab);
            instanceToAdd.transform.SetParent(transform);
            AddProjectileToPool(Planes.Terrestrial, instanceToAdd, EnemyTypes.MushroomSpirit);
        }

        for (int i = 0; i < numEachOrb / 2; ++i)
        {
            GameObject instanceToAdd = Instantiate(astralGolemRock);
            instanceToAdd.transform.SetParent(transform);
            AddProjectileToPool(Planes.Astral, instanceToAdd, EnemyTypes.StoneGolem);
        }

        for (int i = 0; i < numEachOrb / 2; ++i)
        {
            GameObject instanceToAdd = Instantiate(terrestrialGolemRock);
            instanceToAdd.transform.SetParent(transform);
            AddProjectileToPool(Planes.Terrestrial, instanceToAdd, EnemyTypes.StoneGolem);
        }
    }

    public void AddProjectileToPool(Planes plane, GameObject instance, EnemyTypes enemyType)
    {
        instance.GetComponent<Rigidbody>().velocity = Vector3.zero;

        switch (plane)
        {
            case Planes.Astral:

                if (enemyType == EnemyTypes.MushroomSpirit)
                {
                    availableAstralMushroomOrbs.Enqueue(instance);
                    instance.SetActive(false);
                }
                else if (enemyType == EnemyTypes.StoneGolem)
                {
                    availableAstralGolemRocks.Enqueue(instance);
                    instance.SetActive(false);
                }

                break;

            case Planes.Terrestrial:

                if (enemyType == EnemyTypes.MushroomSpirit)
                {
                    availableTerrestrialMushroomOrbs.Enqueue(instance);
                    instance.SetActive(false);
                }
                else if (enemyType == EnemyTypes.StoneGolem)
                {
                    availableTerrestrialGolemRocks.Enqueue(instance);
                    instance.SetActive(false);
                }

                break;
        }
    }
}
