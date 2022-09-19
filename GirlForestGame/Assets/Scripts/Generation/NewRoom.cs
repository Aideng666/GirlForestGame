using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewRoom : MonoBehaviour
{
    [SerializeField] Material entranceMaterial; //temp material to emphasize which spots in the room have exits
    //[SerializeField] GameObject[] doors = new GameObject[4]; //0,1,2,3 = North, South, East, West respectively

    NewRoom[] connectedRooms = new NewRoom[4]; //0,1,2,3 = North, South, East, West respectively

    RoomObject[] possibleRooms; // List of all of the possible room models for the room to pick
    RoomModel spawnedModel; // the selected model for the room that was spawned, used to access the doors

    RoomTypes currentType = RoomTypes.Fight;

    //bool isCurrentRoom; //Is the player currently in this room
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
                if (connectedRooms[i])
                {
                    spawnedModel.GetComponent<RoomModel>().doors[i].SetActive(false);
                }
            }
        }
    }

    //Selects one of the possible room models at random
    public void ChooseRoom()
    {
        possibleRooms = TypeHandler.GetAllInstances<RoomObject>("Rooms");

        int randomIndex = Random.Range(0, possibleRooms.Length);

        Instantiate(possibleRooms[randomIndex].model, transform.position, Quaternion.identity, transform);

        spawnedModel = GetComponentInChildren<RoomModel>();
    }

    public void CreateExit(int direction)
    {
        print($"Model: {spawnedModel.name}");
        print($"Direction: {direction}");
        print($"Parent: {spawnedModel.doors[direction].transform.parent}");

        spawnedModel.doors[direction].transform.parent.GetComponent<MeshRenderer>().material = entranceMaterial;
    }

    //Creates a new room that attaches to the current room on one of its sides
    //Used during dungeon generation
    public void AttachConnectedRoom(int directionIndex, NewRoom roomToAttach)
    {
        connectedRooms[directionIndex] = roomToAttach;

        CreateExit(directionIndex);
    }

    public NewRoom[] GetConnectedRooms()
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

    public void SetRoomType(RoomTypes type)
    {
        currentType = type;

        if (type == RoomTypes.Start)
        {
            roomCompleted = true;
        }
        if (type == RoomTypes.End)
        {
            GetComponentInChildren<MeshRenderer>().material = entranceMaterial;
        }
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
