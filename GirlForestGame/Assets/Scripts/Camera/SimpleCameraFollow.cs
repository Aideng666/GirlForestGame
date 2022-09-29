using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleCameraFollow : MonoBehaviour
{
    [SerializeField] float followOffset;
    [SerializeField] float zDistanceFromPlayer = 7;
    [SerializeField] float cameraHeight = 15;
    [SerializeField] float boundry;

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

        //follows the player if the room is bigger than the screen size
        //if (!DungeonGenerator.Instance.GetCurrentRoom().GetSpawnedModel().isBigRoom)
        //{
        //    transform.position = new Vector3(0, cameraHeight, zDistanceFromPlayer);
        //}
        //else
        //{
        transform.position = new Vector3(player.transform.position.x, cameraHeight, player.transform.position.z - zDistanceFromPlayer);
        //}

        //Blocks the camera from moving too far out of the room if in a big room
        if (transform.position.x <= -boundry)
        {
            transform.position = new Vector3(-boundry, transform.position.y, transform.position.z);
        }
        if (transform.position.x >= boundry)
        {
            transform.position = new Vector3(boundry, transform.position.y, transform.position.z);
        }
        if (transform.position.z <= -boundry)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, -boundry);
        }
        if (transform.position.z >= boundry)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, boundry);
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
