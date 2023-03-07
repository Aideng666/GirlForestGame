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
    }

    // Update is called once per frame
    void Update()
    {
        if (roomComplete)
        {
            model.doors[2].SetActive(false);
        }
        else
        {
            model.doors[2].SetActive(true);
        }

        if (FindObjectsOfType<EnemyData>().Length == 0 && !roomComplete)
        {
            roomComplete = true;
        }

        if (TutorialManager.Instance.currentDialogueNum == 13 || TutorialManager.Instance.currentDialogueNum == 14 
            || TutorialManager.Instance.currentDialogueNum == 16 || TutorialManager.Instance.currentDialogueNum == 17
            || TutorialManager.Instance.currentDialogueNum == 19 || TutorialManager.Instance.currentDialogueNum == 20)
        {
            roomComplete = false;
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

    public bool GetRoomComplete()
    {
        return roomComplete;
    }

    public void SetRoomComplete(bool isComplete)
    {
        roomComplete = isComplete;
    }
}
