using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] List<GameObject> rooms;

    int currentRoomNum = 0;
    bool roomTransitionStarted;

    public static TutorialManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        currentRoomNum = 0;
        InputManager.Instance.GetPlayerInput().actions.FindActionMap("Player").Enable();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void NextRoom()
    {
        UIManager.Instance.GetFadePanel().BeginRoomTransition();

        if (!roomTransitionStarted)
        {
            currentRoomNum++;

            StartCoroutine(EnterNewRoom(rooms[currentRoomNum].GetComponent<RoomModel>().doors[3].transform.parent.position));
        }
    }

    IEnumerator EnterNewRoom(Vector3 updatedPlayerPos)
    {
        roomTransitionStarted = true;

        yield return new WaitForSeconds(UIManager.Instance.GetFadePanel().GetTransitionTime() / 2);

        PlayerController.Instance.transform.position = updatedPlayerPos;

        foreach (GameObject room in rooms)
        {
            if (room == rooms[currentRoomNum])
            {
                room.SetActive(true);
            }
            else
            {
                room.SetActive(false);
            }
        }

        yield return null;

        roomTransitionStarted = false;
    }
}
