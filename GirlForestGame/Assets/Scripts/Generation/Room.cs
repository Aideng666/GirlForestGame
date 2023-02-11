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

    bool roomCompleted; //Has the player defeated all the enemies within this room

    int distanceFromStartRoom = 0;
    Vector2 spotInGrid = Vector2.zero;

    public int DistanceFromStart { get { return distanceFromStartRoom; } set { distanceFromStartRoom = value; } }

    private void OnEnable()
    {
        if (DungeonGenerator.Instance.GetCurrentRoom() == this && currentType == RoomTypes.Fight)
        {
            roomCompleted = false;

            for (int i = 0; i < Random.Range(1, 2); i++)
            {
                EnemyPool.Instance.GetEnemyFromPool(EnemyTypes.MushroomSpirit);
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
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
        }
        else if (EnemyPool.Instance.availableBoars.Count % 5 == 0
            && EnemyPool.Instance.availableMushrooms.Count % 5 == 0)
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
                    case NodeTypes.Default:

                       

                        break;

                    case NodeTypes.Marking:

                        DungeonGenerator.Instance.RespawnRoomModel(RoomTypes.Marking, true);

                        break;

                    case NodeTypes.Shop:

                        DungeonGenerator.Instance.RespawnRoomModel(RoomTypes.Shop, true);

                        break;

                    case NodeTypes.Boss:

                        

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
