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
    //float cameraSpeed = 20;
    
    // Start is called before the first frame update
    void Start()
    {
        player = PlayerController.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        //if (player.transform.position.x > transform.position.x + followOffset)
        //{
        //    transform.position += Vector3.right * cameraSpeed * Time.deltaTime;
        //}
        //if (player.transform.position.x < transform.position.x - followOffset)
        //{
        //    transform.position += Vector3.left * cameraSpeed * Time.deltaTime;
        //}
        //if (player.transform.position.z > transform.position.z + zDistanceFromPlayer + followOffset)
        //{
        //    transform.position += Vector3.forward * cameraSpeed * Time.deltaTime;
        //}
        //if (player.transform.position.z < transform.position.z + zDistanceFromPlayer - followOffset)
        //{
        //    transform.position += Vector3.back * cameraSpeed * Time.deltaTime;
        //}

        //transform.position = new Vector3(player.transform.position.x, cameraHeight, player.transform.position.z - zDistanceFromPlayer);


        //follows the player if the room is bigger than the screen size
        if (!DungeonGenerator.Instance.GetCurrentRoom().GetSpawnedModel().isBigRoom)
        {
            transform.position = new Vector3(0, cameraHeight, zDistanceFromPlayer);
        }
        else
        {
            transform.position = new Vector3(player.transform.position.x, cameraHeight, player.transform.position.z - zDistanceFromPlayer);
        }

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
    }
}
