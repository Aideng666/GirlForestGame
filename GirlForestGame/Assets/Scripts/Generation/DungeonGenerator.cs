using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    [SerializeField] int totalRooms = 15;
    [SerializeField] GameObject roomPrefab;
    //[SerializeField] GameObject doorPrefab;

    List<NewRoom> rooms = new List<NewRoom>();

    NewRoom currentRoom;

    public static DungeonGenerator Instance { get; set; }

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        InitDungeon();

        InputManager.Instance.SwapActionMap("Player");
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

    public void InitDungeon()
    {
        rooms = new List<NewRoom>();

        foreach (NewRoom room in FindObjectsOfType<NewRoom>())
        {
            Destroy(room.gameObject);
        }

        PlayerController.Instance.transform.position = Vector3.zero;

        SpawnRoom();

        //rooms.Add(Instantiate(roomPrefab, Vector3.zero, Quaternion.identity).GetComponent<NewRoom>());

        rooms[0].SetRoomType(RoomTypes.Start);
        currentRoom = rooms[0];

        for (int i = 0; i < totalRooms - 1; i++)
        {
            ChooseNewRoomLocation();
        }

        //Sets the proper End room for the floor
        //Will be altered later to allow for the different types of end rooms(totem, shop, marking)
        NewRoom currentEndRoom = rooms[0];

        for (int i = 0; i < rooms.Count; i++)
        {
            if (rooms[i].DistanceFromStart > currentEndRoom.DistanceFromStart)
            {
                currentEndRoom = rooms[i];
            }
        }

        currentEndRoom.SetRoomType(RoomTypes.End);
    }

    void Regenerate()
    {
        rooms = new List<NewRoom>();

        foreach (NewRoom room in FindObjectsOfType<NewRoom>())
        {
            Destroy(room.gameObject);
        }

        InitDungeon();
    }

    void SpawnRoom(NewRoom originRoom = null/*, Vector3 pos = default(Vector3)*/, Directions directionFromOrigin = Directions.None)
    {
        var room = Instantiate(roomPrefab, Vector3.zero, Quaternion.identity);

        room.GetComponent<NewRoom>().ChooseRoom();

        //If it is not the very first room, it will set the distance of the new room and create the neccessary exits for the connected rooms
        if (originRoom != null)
        {
            room.GetComponent<NewRoom>().DistanceFromStart = originRoom.DistanceFromStart + 1;
            room.GetComponent<NewRoom>().AttachConnectedRoom((int)ReverseDirection(directionFromOrigin), originRoom);

            originRoom.AttachConnectedRoom((int)directionFromOrigin, room.GetComponent<NewRoom>());
        }

        //Adds the new room to the list of all rooms
        rooms.Add(room.GetComponent<NewRoom>());
    }

    //void SpawnRoom(Vector3 pos, Directions directionOfOrigin = Directions.None, Vector2 distanceFromStart = default(Vector2))
    //{
        //var room = Instantiate(roomPrefab, pos, Quaternion.identity);

        //room.GetComponent<Room>().ChooseRoom();
        //room.GetComponent<Room>().SetDistanceFromStart(distanceFromStart);

        //if (directionOfOrigin == Directions.North)
        //{
        //    room.transform.position += -room.GetComponent<Room>().GetSelectedRoom().northExitOffset;

        //    room.GetComponent<Room>().SetNorthExitUsed();
        //}
        //if (directionOfOrigin == Directions.East)
        //{
        //    room.transform.position += -room.GetComponent<Room>().GetSelectedRoom().eastExitOffset;

        //    room.GetComponent<Room>().SetEastExitUsed();
        //}
        //if (directionOfOrigin == Directions.South)
        //{
        //    room.transform.position += -room.GetComponent<Room>().GetSelectedRoom().southExitOffset;

        //    room.GetComponent<Room>().SetSouthExitUsed();
        //}
        //if (directionOfOrigin == Directions.West)
        //{
        //    room.transform.position += -room.GetComponent<Room>().GetSelectedRoom().westExitOffset;

        //    room.GetComponent<Room>().SetWestExitUsed();
        //}

        //if (!CanSpawnRoom(room.GetComponent<Room>()))
        //{
        //    Destroy(room);

        //    ChooseNewRoomLocation();

        //    return;
        //}

        //rooms.Add(room.GetComponent<Room>());

        //UpdateUsedExits();
    //}

    //bool CanSpawnRoom(Room room)
    //{
    //    for (int i = 0; i < rooms.Count; i++)
    //    {
    //        if (room.transform.position.x < rooms[i].transform.position.x + (rooms[i].GetSelectedRoom().eastExitOffset.x + -room.GetSelectedRoom().westExitOffset.x)
    //            && room.transform.position.x + (room.GetSelectedRoom().eastExitOffset.x + -rooms[i].GetSelectedRoom().westExitOffset.x) > rooms[i].transform.position.x
    //            && room.transform.position.z < rooms[i].transform.position.z + (rooms[i].GetSelectedRoom().northExitOffset.z + -room.GetSelectedRoom().southExitOffset.z)
    //            && room.transform.position.z + (room.GetSelectedRoom().northExitOffset.z + -rooms[i].GetSelectedRoom().southExitOffset.z) > rooms[i].transform.position.z)
    //        {
    //            return false;
    //        }
    //    }

    //    return true;
    //}

    void ChooseNewRoomLocation()
    {
        //First selects a random room that already exists
        int randomRoomChoice = Random.Range(0, rooms.Count - 1);

        NewRoom currentRoom = rooms[randomRoomChoice];

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

        //switch (possibleDirections[directionChoice])
        //{
        //    case Directions.North:

        //        SpawnRoom(currentRoom, currentRoom.transform.position + new Vector3(0, 0, 30), possibleDirections[directionChoice]);

        //        break;

        //    case Directions.South:

        //        SpawnRoom(currentRoom, currentRoom.transform.position + new Vector3(0, 0, -30), possibleDirections[directionChoice]);

        //        break;

        //    case Directions.East:

        //        SpawnRoom(currentRoom, currentRoom.transform.position + new Vector3(30, 0, 0), possibleDirections[directionChoice]);

        //        break;

        //    case Directions.West:

        //        SpawnRoom(currentRoom, currentRoom.transform.position + new Vector3(-30, 0, 0), possibleDirections[directionChoice]);

        //        break;
        //}

        //Vector2 newRoomDistance;

        //switch (possibleDirections[directionChoice])
        //{
        //    case Directions.North:

        //        newRoomDistance = currentRoom.GetDistanceFromStart() + new Vector2(0, 1);

        //        for (int i = 0; i < rooms.Count; i++)
        //        {
        //            if ((rooms[i].GetDistanceFromStart().x - 1 == newRoomDistance.x && rooms[i].GetDistanceFromStart().y == newRoomDistance.y)
        //                || (rooms[i].GetDistanceFromStart().x + 1 == newRoomDistance.x && rooms[i].GetDistanceFromStart().y == newRoomDistance.y)
        //                || (rooms[i].GetDistanceFromStart().x == newRoomDistance.x && rooms[i].GetDistanceFromStart().y - 1 == newRoomDistance.y))
        //            {
        //                ChooseNewRoomLocation();

        //                return;
        //            }
        //        }

        //        SpawnRoom(currentRoom.transform.position + currentRoom.GetSelectedRoom().northExitOffset, Directions.South, newRoomDistance);

        //        currentRoom.SetNorthExitUsed();

        //        break;

        //    case Directions.East:

        //        newRoomDistance = currentRoom.GetDistanceFromStart() + new Vector2(1, 0);

        //        for (int i = 0; i < rooms.Count; i++)
        //        {
        //            if ((rooms[i].GetDistanceFromStart().x - 1 == newRoomDistance.x && rooms[i].GetDistanceFromStart().y == newRoomDistance.y)
        //                || (rooms[i].GetDistanceFromStart().x == newRoomDistance.x && rooms[i].GetDistanceFromStart().y + 1 == newRoomDistance.y)
        //                || (rooms[i].GetDistanceFromStart().x == newRoomDistance.x && rooms[i].GetDistanceFromStart().y - 1 == newRoomDistance.y))
        //            {
        //                ChooseNewRoomLocation();

        //                return;
        //            }
        //        }

        //        SpawnRoom(currentRoom.transform.position + currentRoom.GetSelectedRoom().eastExitOffset, Directions.West, newRoomDistance);

        //        currentRoom.SetEastExitUsed();

        //        break;

        //    case Directions.South:

        //        newRoomDistance = currentRoom.GetDistanceFromStart() + new Vector2(0, -1);

        //        for (int i = 0; i < rooms.Count; i++)
        //        {
        //            if ((rooms[i].GetDistanceFromStart().x - 1 == newRoomDistance.x && rooms[i].GetDistanceFromStart().y == newRoomDistance.y)
        //                || (rooms[i].GetDistanceFromStart().x + 1 == newRoomDistance.x && rooms[i].GetDistanceFromStart().y == newRoomDistance.y)
        //                || (rooms[i].GetDistanceFromStart().x == newRoomDistance.x && rooms[i].GetDistanceFromStart().y + 1 == newRoomDistance.y))
        //            {
        //                ChooseNewRoomLocation();

        //                return;
        //            }
        //        }

        //        SpawnRoom(currentRoom.transform.position + currentRoom.GetSelectedRoom().southExitOffset, Directions.North, newRoomDistance);

        //        currentRoom.SetSouthExitUsed();

        //        break;

        //    case Directions.West:

        //        newRoomDistance = currentRoom.GetDistanceFromStart() + new Vector2(-1, 0);

        //        for (int i = 0; i < rooms.Count; i++)
        //        {
        //            if ((rooms[i].GetDistanceFromStart().x + 1 == newRoomDistance.x && rooms[i].GetDistanceFromStart().y == newRoomDistance.y)
        //                || (rooms[i].GetDistanceFromStart().x == newRoomDistance.x && rooms[i].GetDistanceFromStart().y + 1 == newRoomDistance.y)
        //                || (rooms[i].GetDistanceFromStart().x == newRoomDistance.x && rooms[i].GetDistanceFromStart().y - 1 == newRoomDistance.y))
        //            {
        //                ChooseNewRoomLocation();

        //                return;
        //            }
        //        }

        //        SpawnRoom(currentRoom.transform.position + currentRoom.GetSelectedRoom().westExitOffset, Directions.East, newRoomDistance);

        //        currentRoom.SetWestExitUsed();

        //        break;
        //}
    }

    //void UpdateUsedExits()
    //{
    //    for (int i = 0; i < rooms.Count; i++)
    //    {
    //        var room = rooms[i].GetComponent<Room>();

    //        Vector3 currentExitCheckOffset;

    //        for (int j = 0; j < 4; j++)
    //        {
    //            currentExitCheckOffset = room.GetSelectedRoom().GetOffset((Directions)j);

    //            for (int k = 0; k < rooms.Count; k++)
    //            {
    //                if (i != k)
    //                {
    //                    for (int l = 0; l < 4; l++)
    //                    {
    //                        if (Vector3.Distance(room.transform.position + currentExitCheckOffset,
    //                            rooms[k].transform.position + rooms[k].GetComponent<Room>().GetSelectedRoom().GetOffset((Directions)l)) < 1)
    //                        {
    //                            if (!room.GetExitUsed((Directions)j))
    //                            {
    //                                room.SetExitUsed((Directions)j);
    //                            }
    //                        }
    //                    }
    //                }
    //            }
    //        }
    //    }
    //}

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

    public void SetCurrentRoom(NewRoom room)
    {
        currentRoom = room;
    }

    public NewRoom GetCurrentRoom()
    {
        return currentRoom;
    }
}
