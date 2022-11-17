using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    [SerializeField] int totalRooms = 15;
    [SerializeField] GameObject roomPrefab;
    //[SerializeField] GameObject doorPrefab;

    List<Room> rooms = new List<Room>();

    Room currentRoom;
    NodeTypes currentFloorType = NodeTypes.Default;

    public static DungeonGenerator Instance { get; set; }

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        //InitDungeon();

        //InputManager.Instance.SwapActionMap("Player");
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < rooms.Count; i++)
        {
            if (rooms[i] != currentRoom)
            {
                rooms[i].gameObject.SetActive(false);
            }
            if (rooms[i] == currentRoom)
            {
                rooms[i].gameObject.SetActive(true);
            }
        }
    }

    public void InitDungeon(NodeTypes floorType = NodeTypes.Default)
    {
        rooms = new List<Room>();

        foreach (Room room in FindObjectsOfType<Room>())
        {
            Destroy(room.gameObject);
        }

        currentFloorType = floorType;

        PlayerController.Instance.transform.position = Vector3.zero;

        SpawnRoom();

        //rooms.Add(Instantiate(roomPrefab, Vector3.zero, Quaternion.identity).GetComponent<Room>());

        rooms[0].SetRoomType(RoomTypes.Start);
        currentRoom = rooms[0];

        for (int i = 0; i < totalRooms - 1; i++)
        {
            ChooseNewRoomLocation();
        }

        //Sets the proper End room for the floor
        //Will be altered later to allow for the different types of end rooms(totem, shop, marking)
        Room currentEndRoom = rooms[0];

        for (int i = 0; i < rooms.Count; i++)
        {
            if (rooms[i].DistanceFromStart > currentEndRoom.DistanceFromStart)
            {
                currentEndRoom = rooms[i];
            }
        }

        currentEndRoom.SetRoomType(RoomTypes.End);

        for (int i = 0; i < rooms.Count; i++)
        {
            rooms[i].UpdateVisualExits();
        }
    }

    void Regenerate(NodeTypes type = NodeTypes.Default)
    {
        rooms = new List<Room>();

        foreach (Room room in FindObjectsOfType<Room>())
        {
            Destroy(room.gameObject);
        }

        currentFloorType = type;

        InitDungeon();
    }

    void SpawnRoom(Room originRoom = null/*, Vector3 pos = default(Vector3)*/, Directions directionFromOrigin = Directions.None)
    {
        var room = Instantiate(roomPrefab, Vector3.zero, Quaternion.identity);

        room.GetComponent<Room>().ChooseRoom();

        //If it is not the very first room, it will set the distance of the new room and create the neccessary exits for the connected rooms
        if (originRoom != null)
        {
            room.GetComponent<Room>().DistanceFromStart = originRoom.DistanceFromStart + 1;
            room.GetComponent<Room>().AttachConnectedRoom((int)ReverseDirection(directionFromOrigin), originRoom);

            originRoom.AttachConnectedRoom((int)directionFromOrigin, room.GetComponent<Room>());
        }

        //Adds the new room to the list of all rooms
        rooms.Add(room.GetComponent<Room>());
    }

    void ChooseNewRoomLocation()
    {
        //First selects a random room that already exists
        int randomRoomChoice = Random.Range(0, rooms.Count - 1);

        Room currentRoom = rooms[randomRoomChoice];

        //selects all possible directions for a new room location to be based on if the chosen room already has connected rooms on any side
        List<Directions> possibleDirections = new List<Directions>();

        if (!currentRoom.GetConnectedRooms()[0])
        {
            possibleDirections.Add(Directions.North);
        }

        if (!currentRoom.GetConnectedRooms()[1])
        {
            possibleDirections.Add(Directions.South);
        }

        if (!currentRoom.GetConnectedRooms()[2])
        {
            possibleDirections.Add(Directions.East);
        }

        if (!currentRoom.GetConnectedRooms()[3])
        {
            possibleDirections.Add(Directions.West);
        }

        //If there are no possible directions to connect a new room to the selected room
        //the function resets and picks a new random room to check the directions
        if (possibleDirections.Count == 0)
        {
            ChooseNewRoomLocation();

            return;
        }

        //Picks a random direction out of the possible directions that were selected above
        int directionChoice = Random.Range(0, possibleDirections.Count);

        SpawnRoom(currentRoom, possibleDirections[directionChoice]);

    }

    //outputs the opposite direction of the input
    public Directions ReverseDirection(Directions dir)
    {
        Directions reversedDir = Directions.None;

        switch (dir)
        {
            case Directions.North:

                reversedDir = Directions.South;

                break;

            case Directions.South:

                reversedDir = Directions.North;

                break;

            case Directions.East:

                reversedDir = Directions.West;

                break;

            case Directions.West:

                reversedDir = Directions.East;

                break;
        }

        return reversedDir;
    }

    public void SetCurrentRoom(Room room)
    {
        currentRoom = room;
    }

    public Room GetCurrentRoom()
    {
        return currentRoom;
    }

    public NodeTypes GetCurrentFloorType()
    {
        return currentFloorType;
    }
}
