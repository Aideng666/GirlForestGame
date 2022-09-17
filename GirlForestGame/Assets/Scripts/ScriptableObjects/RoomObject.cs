using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class RoomObject : ScriptableObject
{
    public GameObject model;

    public Vector3 northExitOffset;
    public Vector3 eastExitOffset;
    public Vector3 southExitOffset;
    public Vector3 westExitOffset;

    // 0, 1, 2, 3 represent if the model for this room contains a North, South, East, or West exit respectively
    public bool[] existingExits = new bool[4];

    public Vector3 GetOffset(Directions direction)
    {
        Vector3 selectedOffset = Vector3.zero;

        switch (direction)
        {
            case Directions.North:

                selectedOffset = northExitOffset;

                break;

            case Directions.East:

                selectedOffset = eastExitOffset;

                break;

            case Directions.South:

                selectedOffset = southExitOffset;

                break;

            case Directions.West:

                selectedOffset = westExitOffset;

                break;
        }

        return selectedOffset;
    }
}

//This enum dictates the layout of the exits for each room model
//public enum RoomExitTypes
//{
//    OneExit,
//    TwoExitAdjacent,
//    TwoExitOpposite,
//    ThreeExit,
//    FourExit
//}

