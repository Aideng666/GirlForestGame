using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu]
public class RoomObject : ScriptableObject
{
    public GameObject model;

    [HideInInspector] public float northCamBoundary;
    [HideInInspector] public float southCamBoundary;
    [HideInInspector] public float eastCamBoundary;
    [HideInInspector] public float westCamBoundary;

    private float boundaryOffset = 8.75f;

    public void FindBoundaries()
    {
        RoomModel roomModel = model.GetComponent<RoomModel>();

        for (int i = 0; i < roomModel.doors.Length; i++)
        {
            switch (i)
            {
                case 0:

                    northCamBoundary = (roomModel.doors[0].transform.position.z) - boundaryOffset;

                    break;

                case 1:

                    southCamBoundary = (roomModel.doors[1].transform.position.z) + boundaryOffset;

                    break;

                case 2:

                    eastCamBoundary = (roomModel.doors[2].transform.position.x) - boundaryOffset;

                    break;

                case 3:

                    westCamBoundary = (roomModel.doors[3].transform.position.x) + boundaryOffset;

                    break;
            }
        }
    }
}

