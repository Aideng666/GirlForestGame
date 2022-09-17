using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    [SerializeField] List<GameObject> enemyPrefabs = new List<GameObject>();
    [SerializeField] GameObject exitPrefab;

    RoomObject[] possibleRooms;
    RoomObject selectedRoom;

    RoomTypes currentType = RoomTypes.Fight;

    Vector2 distanceFromStart;

    bool northExitUsed;
    bool eastExitUsed;
    bool southExitUsed;
    bool westExitUsed;

    Door[] doors = new Door[4];

    bool isCurrentRoom;
    bool roomCompleted;
    bool roomLocked;
    bool combatStarted;

    private void Update()
    {
        if (roomCompleted && isCurrentRoom)
        {
            if (!doors[0].GetOpen() && northExitUsed)
            {
                doors[0].OpenDoor();
            }

            if (!doors[1].GetOpen() && eastExitUsed)
            {
                doors[1].OpenDoor();
            }

            if (!doors[2].GetOpen() && southExitUsed)
            {
                doors[2].OpenDoor();
            }

            if (!doors[3].GetOpen() && westExitUsed)
            {
                doors[3].OpenDoor();
            }
        }
        else if (!roomCompleted && isCurrentRoom)
        {
            if (!roomLocked)
            {
                StartCoroutine(RoomLockDelay());
            }

            if (combatStarted)
            {
                if (GetComponentsInChildren<Enemy>().Length <= 0)
                {
                    print("Room Complete");

                    roomCompleted = true;
                }
            }

        }
        else if (!isCurrentRoom)
        {
            foreach (Door door in doors)
            {
                door.OpenDoor();
            }
        }
    }

    public void ChooseRoom()
    {
        possibleRooms = TypeHandler.GetAllInstances<RoomObject>("Rooms");

        int randomIndex = Random.Range(0, possibleRooms.Length);

        selectedRoom = possibleRooms[randomIndex];

        Instantiate(selectedRoom.model, transform.position, Quaternion.identity, transform);

        foreach(Door door in GetComponentsInChildren<Door>())
        {
            switch(door.GetDirection())
            {
                case Directions.North:

                    doors[0] = door;

                    break;

                case Directions.East:

                    doors[1] = door;

                    break;

                case Directions.South:

                    doors[2] = door;

                    break;

                case Directions.West:

                    doors[3] = door;

                    break;
            }
        }
    }

    void BeginCombat()
    {
        int numEnemies = Random.Range(3, 6);

        for (int i = 0; i < numEnemies; i++)
        {
            float xPos = Random.Range(transform.position.x + (selectedRoom.GetOffset(Directions.West).x / 2), transform.position.x + (selectedRoom.GetOffset(Directions.East).x / 2));
            float zPos = Random.Range(transform.position.z + (selectedRoom.GetOffset(Directions.South).z / 2), transform.position.z + (selectedRoom.GetOffset(Directions.North).z / 2));

            //int enemyChoice = Random.Range(0, enemyPrefabs.Count);
            int enemyChoice = 1;

            Instantiate(enemyPrefabs[enemyChoice], new Vector3(xPos, enemyPrefabs[enemyChoice].transform.position.y, zPos), Quaternion.identity, transform);
        }

        combatStarted = true;
    }

    IEnumerator RoomLockDelay()
    {
        roomLocked = true;

        yield return new WaitForSeconds(1);

        foreach (Door door in doors)
        {
            door.CloseDoor();
        }

        BeginCombat();
    }

    public RoomObject GetSelectedRoom()
    {
        return selectedRoom;
    }

    public bool GetNorthExitUsed()
    {
        return northExitUsed;
    }

    public bool GetEastExitUsed()
    {
        return eastExitUsed;
    }

    public bool GetSouthExitUsed()
    {
        return southExitUsed;
    }

    public bool GetWestExitUsed()
    {
        return westExitUsed;
    }

    public void SetNorthExitUsed()
    {
        northExitUsed = true;
    }

    public void SetEastExitUsed()
    {
        eastExitUsed = true;
    }

    public void SetSouthExitUsed()
    {
        southExitUsed = true;
    }

    public void SetWestExitUsed()
    {
        westExitUsed = true;
    }

    public void SetCurrentRoom(bool isCurrent)
    {
        isCurrentRoom = isCurrent;
    }

    public void SetRoomCompleted(bool iscomplete)
    {
        roomCompleted = iscomplete;
    }

    public bool GetExitUsed(Directions exit)
    {
        bool isUsed = false;

        switch (exit)
        {
            case Directions.North:

                isUsed = GetNorthExitUsed();

                break;

            case Directions.East:

                isUsed = GetEastExitUsed();

                break;

            case Directions.South:

                isUsed = GetSouthExitUsed();

                break;

            case Directions.West:

                isUsed = GetWestExitUsed();

                break;
        }

        return isUsed;
    }

    public void SetExitUsed(Directions exit)
    {
        switch (exit)
        {
            case Directions.North:

                SetNorthExitUsed();

                break;

            case Directions.East:

                SetEastExitUsed();

                break;

            case Directions.South:

                SetSouthExitUsed();

                break;

            case Directions.West:

                SetWestExitUsed();

                break;
        }
    }

    public Door[] GetDoors()
    {
        return doors;
    }

    public Vector2 GetDistanceFromStart()
    {
        return distanceFromStart;
    }

    public void SetDistanceFromStart(Vector2 dist)
    {
        distanceFromStart = dist;
    }

    public void SetRoomType(RoomTypes type)
    {
        currentType = type;

        if (type == RoomTypes.Start)
        {
            GetComponentInChildren<MeshRenderer>().material.color = Color.blue;

            roomCompleted = true;
            isCurrentRoom = true;
        }
        else if (type == RoomTypes.End)
        {
            GetComponentInChildren<MeshRenderer>().material.color = Color.red;

            Instantiate(exitPrefab, transform.position, Quaternion.identity, transform);
        }
    }
}


public enum RoomTypes
{
    Start,
    Fight,
    End,
    Totem
}
