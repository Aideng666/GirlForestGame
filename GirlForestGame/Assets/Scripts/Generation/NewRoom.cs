using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewRoom : MonoBehaviour
{
    Room[] connectedRooms = new Room[4]; //0,1,2,3 = North, East, South, West respectively

    RoomObject[] possibleRooms; // List of all of the possible room models for the room to pick
    RoomObject selectedRoom; // The randomly selected model out of the possible choices

    RoomTypes currentType = RoomTypes.Fight;

    bool isCurrentRoom; //Is the player currently in this room
    bool roomCompleted; //Has the player defeated all the enemies within this room

    // Start is called before the first frame update
    void Start()
    {
        ChooseRoom();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Selects one of the possible room models at random
    public void ChooseRoom()
    {
        possibleRooms = TypeHandler.GetAllInstances<RoomObject>("Rooms");

        int randomIndex = Random.Range(0, possibleRooms.Length);

        selectedRoom = possibleRooms[randomIndex];

        Instantiate(selectedRoom.model, transform.position, Quaternion.identity, transform);
    }
}
