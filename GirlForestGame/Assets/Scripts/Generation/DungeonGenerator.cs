using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    [SerializeField] int totalRooms = 15;
    [SerializeField] GameObject roomPrefab;
    [SerializeField] GameObject doorPrefab;

    List<Room> rooms = new List<Room>();

    public static DungeonGenerator Instance { get; set; }

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        //InitDungeon();
    }

    // Update is called once per frame
    void Update()
    {
        //if(Input.GetKeyDown(KeyCode.Space))
        //{
        //    Regenerate();

        //    //ChooseNewRoomLocation();
        //}
    }

    public void InitDungeon()
    {
        PlayerController.Instance.transform.position = Vector3.zero;

        SpawnRoom(Vector3.zero);

        rooms[0].SetRoomType(RoomTypes.Start);

        for (int i = 0; i < totalRooms - 1; i++)
        {
            ChooseNewRoomLocation();
        }

        Room currentEndRoom = rooms[0];

        for (int i = 0; i < rooms.Count; i++)
        {
            if (rooms[i].GetDistanceFromStart().magnitude > currentEndRoom.GetDistanceFromStart().magnitude)
            {
                currentEndRoom = rooms[i];
            }
        }

        currentEndRoom.SetRoomType(RoomTypes.End);
    }

    void Regenerate()
    {
        rooms = new List<Room>();

        foreach (Room room in FindObjectsOfType<Room>())
        {
            Destroy(room.gameObject);
        }

        InitDungeon();
    }

    void SpawnRoom(Vector3 pos, Directions directionOfOrigin = Directions.None, Vector2 distanceFromStart = default(Vector2))
    {
        var room = Instantiate(roomPrefab, pos, Quaternion.identity);

        room.GetComponent<Room>().ChooseRoom();
        room.GetComponent<Room>().SetDistanceFromStart(distanceFromStart);

        if (directionOfOrigin == Directions.North)
        {
            room.transform.position += -room.GetComponent<Room>().GetSelectedRoom().northExitOffset;

            room.GetComponent<Room>().SetNorthExitUsed();
        }
        if (directionOfOrigin == Directions.East)
        {
            room.transform.position += -room.GetComponent<Room>().GetSelectedRoom().eastExitOffset;

            room.GetComponent<Room>().SetEastExitUsed();
        }
        if (directionOfOrigin == Directions.South)
        {
            room.transform.position += -room.GetComponent<Room>().GetSelectedRoom().southExitOffset;

            room.GetComponent<Room>().SetSouthExitUsed();
        }
        if (directionOfOrigin == Directions.West)
        {
            room.transform.position += -room.GetComponent<Room>().GetSelectedRoom().westExitOffset;

            room.GetComponent<Room>().SetWestExitUsed();
        }

        if (!CanSpawnRoom(room.GetComponent<Room>()))
        {
            Destroy(room);

            ChooseNewRoomLocation();

            return;
        }

        rooms.Add(room.GetComponent<Room>());

        UpdateUsedExits();
    }

    bool CanSpawnRoom(Room room)
    {
        for (int i = 0; i < rooms.Count; i++)
        {
            if (room.transform.position.x < rooms[i].transform.position.x + (rooms[i].GetSelectedRoom().eastExitOffset.x + -room.GetSelectedRoom().westExitOffset.x)
                && room.transform.position.x + (room.GetSelectedRoom().eastExitOffset.x + -rooms[i].GetSelectedRoom().westExitOffset.x) > rooms[i].transform.position.x
                && room.transform.position.z < rooms[i].transform.position.z + (rooms[i].GetSelectedRoom().northExitOffset.z + -room.GetSelectedRoom().southExitOffset.z)
                && room.transform.position.z + (room.GetSelectedRoom().northExitOffset.z + -rooms[i].GetSelectedRoom().southExitOffset.z) > rooms[i].transform.position.z)
            {
                return false;
            }
        }

        return true;
    }

    void ChooseNewRoomLocation()
    {
        int randomRoomChoice = Random.Range(0, rooms.Count - 1);

        Room currentRoom = rooms[randomRoomChoice];

        List<Directions> possibleDirections = new List<Directions>();

        if (!currentRoom.GetNorthExitUsed())
        {
            possibleDirections.Add(Directions.North);
        }

        if (!currentRoom.GetEastExitUsed())
        {
            possibleDirections.Add(Directions.East);
        }

        if (!currentRoom.GetSouthExitUsed())
        {
            possibleDirections.Add(Directions.South);
        }

        if (!currentRoom.GetWestExitUsed())
        {
            possibleDirections.Add(Directions.West);
        }

        if (possibleDirections.Count == 0)
        {
            ChooseNewRoomLocation();

            return;
        }

        int directionChoice = Random.Range(0, possibleDirections.Count);

        Vector2 newRoomDistance;

        switch (possibleDirections[directionChoice])
        {
            case Directions.North:

                newRoomDistance = currentRoom.GetDistanceFromStart() + new Vector2(0, 1);

                for (int i = 0; i < rooms.Count; i++)
                {
                    if ((rooms[i].GetDistanceFromStart().x - 1 == newRoomDistance.x && rooms[i].GetDistanceFromStart().y == newRoomDistance.y)
                        || (rooms[i].GetDistanceFromStart().x + 1 == newRoomDistance.x && rooms[i].GetDistanceFromStart().y == newRoomDistance.y)
                        || (rooms[i].GetDistanceFromStart().x == newRoomDistance.x && rooms[i].GetDistanceFromStart().y - 1 == newRoomDistance.y))
                    {
                        ChooseNewRoomLocation();

                        return;
                    }
                }

                SpawnRoom(currentRoom.transform.position + currentRoom.GetSelectedRoom().northExitOffset, Directions.South, newRoomDistance);

                currentRoom.SetNorthExitUsed();

                break;

            case Directions.East:

                newRoomDistance = currentRoom.GetDistanceFromStart() + new Vector2(1, 0);

                for (int i = 0; i < rooms.Count; i++)
                {
                    if ((rooms[i].GetDistanceFromStart().x - 1 == newRoomDistance.x && rooms[i].GetDistanceFromStart().y == newRoomDistance.y)
                        || (rooms[i].GetDistanceFromStart().x == newRoomDistance.x && rooms[i].GetDistanceFromStart().y + 1 == newRoomDistance.y)
                        || (rooms[i].GetDistanceFromStart().x == newRoomDistance.x && rooms[i].GetDistanceFromStart().y - 1 == newRoomDistance.y))
                    {
                        ChooseNewRoomLocation();

                        return;
                    }
                }

                SpawnRoom(currentRoom.transform.position + currentRoom.GetSelectedRoom().eastExitOffset, Directions.West, newRoomDistance);

                currentRoom.SetEastExitUsed();

                break;

            case Directions.South:

                newRoomDistance = currentRoom.GetDistanceFromStart() + new Vector2(0, -1);

                for (int i = 0; i < rooms.Count; i++)
                {
                    if ((rooms[i].GetDistanceFromStart().x - 1 == newRoomDistance.x && rooms[i].GetDistanceFromStart().y == newRoomDistance.y)
                        || (rooms[i].GetDistanceFromStart().x + 1 == newRoomDistance.x && rooms[i].GetDistanceFromStart().y == newRoomDistance.y)
                        || (rooms[i].GetDistanceFromStart().x == newRoomDistance.x && rooms[i].GetDistanceFromStart().y + 1 == newRoomDistance.y))
                    {
                        ChooseNewRoomLocation();

                        return;
                    }
                }

                SpawnRoom(currentRoom.transform.position + currentRoom.GetSelectedRoom().southExitOffset, Directions.North, newRoomDistance);

                currentRoom.SetSouthExitUsed();

                break;

            case Directions.West:

                newRoomDistance = currentRoom.GetDistanceFromStart() + new Vector2(-1, 0);

                for (int i = 0; i < rooms.Count; i++)
                {
                    if ((rooms[i].GetDistanceFromStart().x + 1 == newRoomDistance.x && rooms[i].GetDistanceFromStart().y == newRoomDistance.y)
                        || (rooms[i].GetDistanceFromStart().x == newRoomDistance.x && rooms[i].GetDistanceFromStart().y + 1 == newRoomDistance.y)
                        || (rooms[i].GetDistanceFromStart().x == newRoomDistance.x && rooms[i].GetDistanceFromStart().y - 1 == newRoomDistance.y))
                    {
                        ChooseNewRoomLocation();

                        return;
                    }
                }

                SpawnRoom(currentRoom.transform.position + currentRoom.GetSelectedRoom().westExitOffset, Directions.East, newRoomDistance);

                currentRoom.SetWestExitUsed();

                break;
        }
    }

    void UpdateUsedExits()
    {
        for (int i = 0; i < rooms.Count; i++)
        {
            var room = rooms[i].GetComponent<Room>();

            Vector3 currentExitCheckOffset;

            for (int j = 0; j < 4; j++)
            {
                currentExitCheckOffset = room.GetSelectedRoom().GetOffset((Directions)j);

                for (int k = 0; k < rooms.Count; k++)
                {
                    if (i != k)
                    {
                        for (int l = 0; l < 4; l++)
                        {
                            if (Vector3.Distance(room.transform.position + currentExitCheckOffset,
                                rooms[k].transform.position + rooms[k].GetComponent<Room>().GetSelectedRoom().GetOffset((Directions)l)) < 1)
                            {
                                if (!room.GetExitUsed((Directions)j))
                                {
                                    room.SetExitUsed((Directions)j);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
