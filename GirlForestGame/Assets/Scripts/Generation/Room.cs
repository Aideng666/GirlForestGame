using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    [SerializeField] Material entranceMaterial; //temp material to emphasize which spots in the room have exits
    [SerializeField] GameObject totemPrefab; //prefab to spawn totems in rooms
    [SerializeField] GameObject markingPrefab; //prefab to spawn markings in rooms

    Room[] connectedRooms = new Room[4]; //0,1,2,3 = North, South, East, West respectively
    Room originRoom; //The room that this room was originally attached to

    RoomObject[] possibleRooms; // List of all of the possible room models for the room to pick
    RoomModel spawnedModel; // the selected model for the room that was spawned, used to access the doors
    RoomObject selectedRoom;

    RoomTypes currentType = RoomTypes.Fight;

    bool roomCompleted; //Has the player defeated all the enemies within this room

    int distanceFromStartRoom = 0;

    public int DistanceFromStart { get { return distanceFromStartRoom; } set { distanceFromStartRoom = value; } }

    // Start is called before the first frame update
    void Start()
    {
        //TEMP
        roomCompleted = true;
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
    }

    //Selects one of the possible room models at random
    public void ChooseRoom(string roomFolder)
    {
        possibleRooms = TypeHandler.GetAllInstances<RoomObject>(roomFolder);

        int randomIndex = Random.Range(0, possibleRooms.Length);

        selectedRoom = possibleRooms[randomIndex];

        var roomModel = Instantiate(selectedRoom.model, transform.position, Quaternion.identity, transform);

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

                        Instantiate(markingPrefab, transform.position + new Vector3(-5, 0, 5), Quaternion.identity, transform);
                        Instantiate(markingPrefab, transform.position + new Vector3(5, 0, -5), Quaternion.identity, transform);

                        break;

                    case NodeTypes.Shop:

                        

                        break;

                    case NodeTypes.Boss:

                        

                        break;
                }

                break;

            case RoomTypes.Totem:

                Instantiate(totemPrefab, transform);

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
    Marking
}
