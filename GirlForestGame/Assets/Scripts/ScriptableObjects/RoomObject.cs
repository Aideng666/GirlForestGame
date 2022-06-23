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
