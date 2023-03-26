using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    [SerializeField] Material entranceMaterial; //temp material to emphasize which spots in the room have exits
    [SerializeField] GameObject totemPrefab; //prefab to spawn totems in rooms
    [SerializeField] GameObject markingPrefab; //prefab to spawn markings in rooms
    [SerializeField] GameObject shopItemPrefab; //prefab to spawn markings in rooms

    Room[] connectedRooms = new Room[4]; //0,1,2,3 = North, South, East, West respectively
    Room originRoom; //The room that this room was originally attached to

    RoomObject[] possibleRooms; // List of all of the possible room models for the room to pick
    RoomModel spawnedModel; // the selected model for the room that was spawned, used to access the doors
    RoomObject selectedRoom;

    RoomTypes currentType = RoomTypes.Fight;

    bool roomCompleted = false; //Has the player defeated all the enemies within this room
    bool rewardReceived;

    int distanceFromStartRoom = 0;
    Vector2 spotInGrid = Vector2.zero;

    Vector2[] enemyCountRangesPerLevel = new Vector2[3] { new Vector2(3, 4), new Vector2(3, 5), new Vector2(4, 6) };
    float[] totemChancePerLevel;

    public int DistanceFromStart { get { return distanceFromStartRoom; } set { distanceFromStartRoom = value; } }

    private void OnEnable()
    {
        if (!roomCompleted)
        {
            if (DungeonGenerator.Instance.GetCurrentRoom() == this && currentType == RoomTypes.Fight)
            {
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
            else if (DungeonGenerator.Instance.GetCurrentRoom() == this && currentType == RoomTypes.End)
            {
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
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        enemyCountRangesPerLevel = new Vector2[3] { new Vector2(3, 4), new Vector2(3, 5) , new Vector2(4, 6) };
        totemChancePerLevel = new float[3] { 0.2f, 0.4f, 0.6f };
    }

    // Update is called once per frame
    void Update()
    {
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

            if (currentType == RoomTypes.End && DungeonGenerator.Instance.GetCurrentFloorType() == NodeTypes.Default && !rewardReceived)
            {
                Instantiate(totemPrefab, totemPrefab.transform.position, Quaternion.identity, transform);

                rewardReceived = true;
            }
        }
        else if (EnemyPool.Instance.availableBoars.Count % 5 == 0
            && EnemyPool.Instance.availableMushrooms.Count % 5 == 0
            && EnemyPool.Instance.availableDraugrs.Count % 5 == 0
            && EnemyPool.Instance.availableGolems.Count % 5 == 0)
        {
            roomCompleted = true;
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

    public void UpdateVisualExits()
    {
        for (int i = 0; i < connectedRooms.Length; i++)
        {
            if (connectedRooms[i] == null)
            {
                spawnedModel.exits[i].SetActive(false);
            }
        }
    }

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
