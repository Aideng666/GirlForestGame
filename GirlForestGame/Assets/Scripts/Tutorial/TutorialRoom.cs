using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialRoom : MonoBehaviour
{
    [SerializeField] TutorialRoom nextRoom;

    RoomModel model;
    bool roomComplete = false;

    // Start is called before the first frame update
    void Start()
    {
        model = GetComponent<RoomModel>();

        roomComplete = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (roomComplete)
        {
            model.doors[2].SetActive(false);
        }
    }

    public TutorialRoom GetNextRoom()
    {
        return nextRoom;
    }

    public RoomModel GetModel()
    {
        return model;
    }
}
