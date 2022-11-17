using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomModel : MonoBehaviour
{
    // 0, 1, 2, 3 represent if the model for this room contains a North, South, East, or West door respectively
    public GameObject[] doors = new GameObject[4];

    // 0, 1, 2, 3 represent if the model for this room contains a North, South, East, or West exit respectively
    public GameObject[] exits = new GameObject[4];

    //Is the room bigger than the standard 1 screen size
    public bool isBigRoom;
}
