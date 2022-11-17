using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleCameraFollow : MonoBehaviour
{
    [SerializeField] float followOffset;
    [SerializeField] float zDistanceFromPlayer = 7;
    [SerializeField] float cameraHeight = 15;

    PlayerController player;

    bool mapActive;
    float cameraSpeed = 20;
    
    // Start is called before the first frame update
    void Start()
    {
        player = PlayerController.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        //Checks which mode the camera is in between node map or combat
        if (InputManager.Instance.GetPlayerInput().actions.FindActionMap("Player").enabled)
        {
            mapActive = false;
        }
        if (InputManager.Instance.GetPlayerInput().actions.FindActionMap("NodeMap").enabled)
        {
            mapActive = true;
        }
        /////////////////////////////////////////////////////////////////

        transform.position = new Vector3(player.transform.position.x, cameraHeight, player.transform.position.z - zDistanceFromPlayer);

        if (!mapActive)
        {
            RoomObject currentRoomInfo = DungeonGenerator.Instance.GetCurrentRoom().GetRoomObject();

            //Blocks the camera from moving too far out of the room
            if (transform.position.x <= currentRoomInfo.westCamBoundary)
            {
                transform.position = new Vector3(currentRoomInfo.westCamBoundary, transform.position.y, transform.position.z);
            }
            if (transform.position.x >= currentRoomInfo.eastCamBoundary)
            {
                transform.position = new Vector3(currentRoomInfo.eastCamBoundary, transform.position.y, transform.position.z);
            }
            if (transform.position.z <= currentRoomInfo.southCamBoundary)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y, currentRoomInfo.southCamBoundary);
            }
            if (transform.position.z >= currentRoomInfo.northCamBoundary)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y, currentRoomInfo.northCamBoundary);
            }
        }


        //Allows the player to move the camera up and down when viewing the node map
        if (mapActive)
        {
            if (InputManager.Instance.MapScroll() > 0)
            {
                transform.position += Vector3.forward * cameraSpeed * Time.deltaTime;
            }
            if (InputManager.Instance.MapScroll() < 0)
            {
                transform.position += Vector3.back * cameraSpeed * Time.deltaTime;
            }
        }
    }
}
