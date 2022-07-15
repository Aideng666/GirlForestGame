using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    PlayerInput playerInput;

    InputAction dashAction;
    InputAction moveAction;
    InputAction aimAction;
    InputAction mouseAimAction;
    InputAction swordAttackAction;
    InputAction bowAttackAction;
    InputAction completeRoomAction;
    InputAction changeFormAction;
    InputAction mapScrollAction;

    public static InputManager Instance { get; set; }
    private void Awake()
    {
        Instance = this;

        //controls = new PlayerControls();
        playerInput = GetComponent<PlayerInput>();

        dashAction = playerInput.actions["Dash"];
        moveAction = playerInput.actions["Move"];
        aimAction = playerInput.actions["Aim"];
        mouseAimAction = playerInput.actions["MouseAim"];
        swordAttackAction = playerInput.actions["SwordAttack"];
        bowAttackAction = playerInput.actions["BowAttack"];
        completeRoomAction = playerInput.actions["CompleteRoom"];
        changeFormAction = playerInput.actions["ChangeForm"];
        mapScrollAction = playerInput.actions["MapScroll"];
    }

    public bool Dash()
    {
        return dashAction.triggered;
    }

    public bool SwordAttack()
    {
        return swordAttackAction.triggered;
    }

    public bool BowAttack()
    {
        return bowAttackAction.triggered;
    }

    public Vector2 Move()
    {
        return moveAction.ReadValue<Vector2>();
    }

    public Vector2 Aim()
    {
        return aimAction.ReadValue<Vector2>();
    }

    public Vector2 MouseAim()
    {
        return mouseAimAction.ReadValue<Vector2>();
    }

    public bool CompleteRoom()
    {
        return completeRoomAction.triggered;
    }

    public bool ChangeForm()
    {
        return changeFormAction.triggered;
    }

    public float MapScroll()
    {
        return mapScrollAction.ReadValue<float>();
    }
}
