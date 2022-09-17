using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomExit : MonoBehaviour
{
    [SerializeField] Directions exitDir;

    public Directions GetExitDirection()
    {
        return exitDir;
    }
}
