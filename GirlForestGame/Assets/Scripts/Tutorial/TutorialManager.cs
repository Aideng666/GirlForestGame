using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] List<GameObject> rooms;

    PlayerController player;
    int currentRoomNum = 0;
    public int currentDialogueNum { get; private set; } = 0;
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
        currentDialogueNum = 0;
        InputManager.Instance.GetPlayerInput().actions.FindActionMap("Player").Enable();

        player = PlayerController.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        if (!DialogueManager.Instance.isTyping)
        {
            switch(currentDialogueNum)
            {
                case 0:

                    TutorialIcons.Instance.SwapIcons(ButtonIcons.LStick);
                    TriggerTutorialSection(1);

                    break;

                case 1:

                    TutorialIcons.Instance.DisableIcon();

                    break;

                case 2:

                    TriggerTutorialSection(3);
                    TutorialIcons.Instance.EnableIcon();
                    TutorialIcons.Instance.SwapIcons(ButtonIcons.Y);

                    break;

                case 3:

                    if (player.playerCombat.Form == Planes.Astral)
                    {
                        TriggerTutorialSection(4);
                        TutorialIcons.Instance.DisableIcon();
                    }

                    break;

                case 4:

                    TriggerTutorialSection(5);
                    TutorialIcons.Instance.EnableIcon();
                    TutorialIcons.Instance.SwapIcons(ButtonIcons.LB);

                    break;

                case 6:

                    TutorialIcons.Instance.SwapIcons(ButtonIcons.RB);

                    break;

                case 7:

                    TriggerTutorialSection(8);
                    TutorialIcons.Instance.DisableIcon();

                    break;

                case 8:

                    TriggerTutorialSection(9);

                    break;

                case 11:

                    TriggerTutorialSection(12);

                    break;

                case 14:

                    TriggerTutorialSection(15);

                    break;

                case 16:

                    TriggerTutorialSection(17);

                    break;

                case 17:

                    TutorialIcons.Instance.EnableIcon();
                    TutorialIcons.Instance.SwapIcons(ButtonIcons.A);

                    break;

                case 18:

                    TutorialIcons.Instance.DisableIcon();

                    break;

                case 19:

                    TriggerTutorialSection(20);

                    break;

                case 20:

                    TutorialIcons.Instance.EnableIcon();

                    break;

                case 21:

                    if (rooms[4].GetComponent<TutorialRoom>().GetRoomComplete())
                    {
                        TriggerTutorialSection(22);
                        TutorialIcons.Instance.DisableIcon();
                    }

                    break;

                case 22:

                    FinishTutorial();

                    break;
            }
        }
    }

    public void TriggerTutorialSection(int section, bool displayImmediately = false)
    {
        if (currentDialogueNum == section - 1)
        {
            currentDialogueNum++;

            if (displayImmediately && !DialogueManager.Instance.isTyping)
            {
                DialogueManager.Instance.DisplayNextSentence();

                return;
            }

            DialogueManager.Instance.QueueNextSentence();
        }
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

    void FinishTutorial()
    {
        print("Tutorial Complete");
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