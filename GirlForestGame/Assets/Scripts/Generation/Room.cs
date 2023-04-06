using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    [SerializeField] Material entranceMaterial; //temp material to emphasize which spots in the room have exits
    [SerializeField] GameObject totemPrefab; //prefab to spawn totems in rooms
    [SerializeField] GameObject markingPrefab; //prefab to spawn markings in rooms
    [SerializeField] GameObject shopItemPrefab; //prefab to spawn markings in rooms
    //[SerializeField] GameObject heartPrefab;
    //[SerializeField] GameObject coinPrefab; 

    public Room[] connectedRooms = new Room[4]; //0,1,2,3 = North, South, East, West respectively
    Room originRoom; //The room that this room was originally attached to

    RoomObject[] possibleRooms; // List of all of the possible room models for the room to pick
    RoomModel spawnedModel; // the selected model for the room that was spawned, used to access the doors
    RoomObject selectedRoom;

    RoomTypes currentType = RoomTypes.Fight;

    bool roomCompleted = false; //Has the player defeated all the enemies within this room
    bool rewardReceived;
    int waveNum = 0;
    int totalWavesInBossFight = 3;

    int distanceFromStartRoom = 0;
    Vector2 spotInGrid = Vector2.zero;

    Vector2[] enemyCountRangesPerLevel = new Vector2[3] { new Vector2(3, 4), new Vector2(3, 5), new Vector2(4, 6) };
    float[] totemChancePerLevel;

    public int DistanceFromStart { get { return distanceFromStartRoom; } set { distanceFromStartRoom = value; } }

    private FMOD.Studio.PLAYBACK_STATE roamState;
   // [HideInInspector] public FMOD.Studio.EventInstance roamingBGM;
    [HideInInspector] public FMOD.Studio.EventInstance battleBGM;
    float combatValue;

    private void OnEnable()
    {
        waveNum = 0;
        //roamingBGM = FMODUnity.RuntimeManager.CreateInstance("event:/Music/Roaming");
        battleBGM = FMODUnity.RuntimeManager.CreateInstance("event:/Music/Battle");
        if (!roomCompleted)
        {
            //If this is a regular fighting room
            if (DungeonGenerator.Instance.GetCurrentRoom() == this && currentType == RoomTypes.Fight)
            {

                battleBGM.setParameterByName("Combat", 1);

                battleBGM.getParameterByName("Combat", out combatValue);
                Debug.Log("batGM start");
                Debug.Log("enable: " + combatValue);

                //Num of enemies in a room
                for (int i = 0; i < Random.Range((int)enemyCountRangesPerLevel[NodeMapManager.Instance.GetCurrentMapCycle() - 1].x, (int)enemyCountRangesPerLevel[NodeMapManager.Instance.GetCurrentMapCycle() - 1].y + 1); i++)
                {
                    int randomEnemySelection = Random.Range(0, 4);

                    switch (randomEnemySelection)
                    {
                        case 0:

                            EnemyPool.Instance.GetEnemyFromPool(EnemyTypes.Boar);

                            break;

                        case 1:

                            EnemyPool.Instance.GetEnemyFromPool(EnemyTypes.MushroomSpirit);

                            break;

                        case 2:

                            EnemyPool.Instance.GetEnemyFromPool(EnemyTypes.Draugr);

                            break;

                        case 3:

                            EnemyPool.Instance.GetEnemyFromPool(EnemyTypes.StoneGolem);

                            break;
                    }
                }
                

            }
            //If this is the final room in a floor
            else if (DungeonGenerator.Instance.GetCurrentRoom() == this && currentType == RoomTypes.End)
            {
                battleBGM.setParameterByName("Combat", 1);

                if (DungeonGenerator.Instance.GetCurrentFloorType() == NodeTypes.Default)
                {

                    //Num of enemies for final room
                    for (int i = 0; i < Random.Range((int)enemyCountRangesPerLevel[NodeMapManager.Instance.GetCurrentMapCycle() - 1].x + 2, (int)enemyCountRangesPerLevel[NodeMapManager.Instance.GetCurrentMapCycle() - 1].y + 3); i++)
                    {
                        int randomEnemySelection = Random.Range(0, 4);

                        switch (randomEnemySelection)
                        {
                            case 0:

                                EnemyPool.Instance.GetEnemyFromPool(EnemyTypes.Boar);

                                break;

                            case 1:

                                EnemyPool.Instance.GetEnemyFromPool(EnemyTypes.MushroomSpirit);

                                break;

                            case 2:

                                EnemyPool.Instance.GetEnemyFromPool(EnemyTypes.Draugr);

                                break;

                            case 3:

                                EnemyPool.Instance.GetEnemyFromPool(EnemyTypes.StoneGolem);

                                break;
                        }
                    }
                }
                if (DungeonGenerator.Instance.GetCurrentFloorType() == NodeTypes.Boss)
                {

                    waveNum = 0;

                    SpawnNextWave();
                }
            }
        }       
        
    }

    // Start is called before the first frame update
    void Start()
    {
        enemyCountRangesPerLevel = new Vector2[3] { new Vector2(3, 4), new Vector2(3, 5) , new Vector2(4, 6) };
        totemChancePerLevel = new float[3] { 0.2f, 0.4f, 0.6f };
        battleBGM.start();
        //battleBGM.setParameterByName("Combat",0);

    }

    // Update is called once per frame
    void Update()
    {
        battleBGM.getParameterByName("Combat", out combatValue);
        Debug.Log("State: "+combatValue);
        if (roomCompleted)
        {
                       
            //Opens all doors that have connected rooms
            for (int i = 0; i < connectedRooms.Length; i++)
            {
                if (connectedRooms[i] != null)
                {
                    spawnedModel.doors[i].SetActive(false);

                    if (currentType == RoomTypes.End)
                    {
                        int directionOfExit = (int)DungeonGenerator.Instance.ReverseDirection((Directions)i);

                        CreateExit(directionOfExit);

                        //spawnedModel.doors[directionOfExit].transform.parent.GetComponentInChildren<RoomExit>().tag = "FloorExit";
                        spawnedModel.exits[directionOfExit].SetActive(true);
                        spawnedModel.exits[directionOfExit].tag = "FloorExit";

                        spawnedModel.doors[directionOfExit].SetActive(false);
                    }
                }
            }
            
            //Give totem reward on completion of a regular floor
            if (currentType == RoomTypes.End && DungeonGenerator.Instance.GetCurrentFloorType() == NodeTypes.Default && !rewardReceived)
            {
                Instantiate(totemPrefab, totemPrefab.transform.position, Quaternion.identity, transform);

                rewardReceived = true;
            }
            else if (currentType == RoomTypes.End && DungeonGenerator.Instance.GetCurrentFloorType() == NodeTypes.Boss && !rewardReceived)
            {
                Instantiate(totemPrefab, totemPrefab.transform.position + ((Vector3.forward + Vector3.left) * 5), Quaternion.identity, transform);
                Instantiate(totemPrefab, totemPrefab.transform.position + ((Vector3.back + Vector3.right) * 5), Quaternion.identity, transform);
                Instantiate(totemPrefab, totemPrefab.transform.position, Quaternion.identity, transform);

                PickupPool.Instance.GetHeartFromPool(totemPrefab.transform.position + (Vector3.left + Vector3.back) * 5 + ((Vector3.forward + Vector3.left) * 5));
                PickupPool.Instance.GetHeartFromPool(totemPrefab.transform.position + (Vector3.left + Vector3.back) * 5 + ((Vector3.back + Vector3.right) * 5));
                PickupPool.Instance.GetHeartFromPool(totemPrefab.transform.position + (Vector3.left + Vector3.back) * 5);

                for (int i = 0; i < 10; i++)
                {
                    PickupPool.Instance.GetCoinFromPool(totemPrefab.transform.position + new Vector3(Random.Range(-10f, 10f), 0, Random.Range(-10f, 10f)));
                }

                rewardReceived = true;
            }
        }
        else if (EnemyPool.Instance.availableBoars.Count % 5 == 0
            && EnemyPool.Instance.availableMushrooms.Count % 5 == 0
            && EnemyPool.Instance.availableDraugrs.Count % 5 == 0
            && EnemyPool.Instance.availableGolems.Count % 5 == 0)
        {
            if (waveNum > 0 && waveNum < totalWavesInBossFight)
            {
                SpawnNextWave();

                return;
            }
            else if (waveNum == totalWavesInBossFight)
            {
                
            }
            battleBGM.setParameterByName("Combat", 0);
            Debug.Log("Roam Start");
            roomCompleted = true;
        }
    }

    //Called for the boss fights for spawning multiple waves of enemies. This is to replace the actual boss fight.
    void SpawnNextWave()
    {
        waveNum++;

        for (int i = 0; i < Random.Range((int)enemyCountRangesPerLevel[NodeMapManager.Instance.GetCurrentMapCycle() - 1].x + 1, (int)enemyCountRangesPerLevel[NodeMapManager.Instance.GetCurrentMapCycle() - 1].y + 2); i++)
        {
            int randomEnemySelection = Random.Range(0, 4);

            switch (randomEnemySelection)
            {
                case 0:

                    EnemyPool.Instance.GetEnemyFromPool(EnemyTypes.Boar);

                    break;

                case 1:

                    EnemyPool.Instance.GetEnemyFromPool(EnemyTypes.MushroomSpirit);

                    break;

                case 2:

                    EnemyPool.Instance.GetEnemyFromPool(EnemyTypes.Draugr);

                    break;

                case 3:

                    EnemyPool.Instance.GetEnemyFromPool(EnemyTypes.StoneGolem);

                    break;
            }
        }
    }

    //Selects one of the possible room models at random
    public void ChooseRoom(string roomFolder)
    {
        possibleRooms = TypeHandler.GetAllInstances<RoomObject>(roomFolder);

        int randomIndex = Random.Range(0, possibleRooms.Length);

        selectedRoom = possibleRooms[randomIndex];

        selectedRoom.FindBoundaries();

        var roomModel = Instantiate(selectedRoom.model, transform.position, selectedRoom.model.transform.rotation, transform);

        spawnedModel = roomModel.GetComponent<RoomModel>();
    }

    public void CreateExit(int direction)
    {
        spawnedModel.doors[direction].transform.parent.GetComponent<MeshRenderer>().material = entranceMaterial;
    }

    //public void UpdateVisualExits()
    //{
    //    for (int i = 0; i < connectedRooms.Length; i++)
    //    {
    //        if (connectedRooms[i] == null)
    //        {
    //            spawnedModel.exits[i].SetActive(false);
    //        }
    //    }
    //}

    //Creates a new room that attaches to the current room on one of its sides
    //Used during dungeon generation
    public void AttachConnectedRoom(int directionIndex, Room roomToAttach)
    {
        bool firstRoom = true;

        for (int i = 0; i < 4; i++)
        {
            if (connectedRooms[i] != null)
            {
                firstRoom = false;

                break;
            }
        }

        if (firstRoom)
        {
            originRoom = roomToAttach;
        }

        connectedRooms[directionIndex] = roomToAttach;

        CreateExit(directionIndex);
    }

    public Room[] GetConnectedRooms()
    {
        return connectedRooms;
    }

    public GameObject[] GetDoors()
    {
        return spawnedModel.doors;
    }

    public RoomModel GetSpawnedModel()
    {
        return spawnedModel;
    }

    public RoomObject GetRoomObject()
    {
        return selectedRoom;
    }

    public void SetRoomType(RoomTypes type)
    {
        currentType = type;

        switch (type)
        {
            case RoomTypes.Start:

                roomCompleted = true;

                break;

            case RoomTypes.End:

                switch (DungeonGenerator.Instance.GetCurrentFloorType())
                {
                    case NodeTypes.Marking:

                        DungeonGenerator.Instance.RespawnRoomModel(RoomTypes.Marking, true);

                        break;

                    case NodeTypes.Shop:

                        DungeonGenerator.Instance.RespawnRoomModel(RoomTypes.Shop, true);

                        break;
                }

                break;

            case RoomTypes.Totem:

                roomCompleted = true;

                break;
        }
    }

    public RoomTypes GetRoomType()
    {
        return currentType;
    }
    private void OnDestroy()
    {
       // roamingBGM.release();
        battleBGM.release();
    }
}

public enum RoomTypes
{
    Start,
    Fight,
    End,
    Totem,
    Shop,
    Marking,
    Rare
}
