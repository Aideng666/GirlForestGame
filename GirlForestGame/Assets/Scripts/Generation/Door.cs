using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] Directions doorDirection;

    Animator anim;
    bool isOpen;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        if (InputManager.Instance.BowAttack())
        {
            print("Opening Doors");

            anim.SetBool("Open", true);
        }
    }

    public void OpenDoor()
    {
        //if (TryGetComponent<Animator>(out Animator anim) && !isOpen)
        //{
            anim.SetBool("Open", true);

            isOpen = true;
        //}
    }

    public void CloseDoor()
    {
        //if (TryGetComponent<Animator>(out Animator anim) && isOpen)
        //{
            anim.SetBool("Open", false);

            isOpen = false;
        //}
    }

    public Directions GetDirection()
    {
        return doorDirection;
    }

    public bool GetOpen()
    {
        return isOpen;
    }
}
