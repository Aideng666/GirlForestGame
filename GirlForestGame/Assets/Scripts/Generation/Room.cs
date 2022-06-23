using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    RoomObject[] possibleRooms;

    RoomObject selectedRoom;

    bool northExitUsed;
    bool eastExitUsed;
    bool southExitUsed;
    bool westExitUsed;

    Vector2 distanceFromStart;

    public void ChooseRoom()
    {
        possibleRooms = TypeHandler.GetAllInstances<RoomObject>("Rooms");

        int randomIndex = Random.Range(0, possibleRooms.Length);

        selectedRoom = possibleRooms[randomIndex];

        Instantiate(selectedRoom.model, transform.position, Quaternion.identity, transform);
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

    public Vector2 GetDistanceFromStart()
    {
        return distanceFromStart;
    }

    public void SetDistanceFromStart(Vector2 dist)
    {
        distanceFromStart = dist;
    }
}
