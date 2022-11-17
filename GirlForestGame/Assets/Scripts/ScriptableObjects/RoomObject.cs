using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class RoomObject : ScriptableObject
{
    public GameObject model;

    public float northCamBoundary;
    public float southCamBoundary;
    public float eastCamBoundary;
    public float westCamBoundary;
}

