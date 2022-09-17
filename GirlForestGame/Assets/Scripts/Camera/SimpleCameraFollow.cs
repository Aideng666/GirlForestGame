using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleCameraFollow : MonoBehaviour
{
    [SerializeField] float followOffset;
    [SerializeField] float zDistanceFromPlayer = 7;
    [SerializeField] float cameraHeight = 15;

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
    }
}
