using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    PlayerInput playerInput;

    //Gameplay Controls
    InputAction interactAction;
    InputAction moveAction;
    InputAction aimAction;
    InputAction mouseAimAction;
    InputAction swordAttackAction;
    InputAction bowAttackAction;
    InputAction completeRoomAction;
    InputAction changeFormAction;
    InputAction selectSwordAction;
    InputAction selectBowAction;

    //Map Controls
    InputAction mapScrollAction;
    InputAction regenMapAction;
    InputAction swapActionMapAction;
    InputAction selectNodeAction;
    InputAction moveSelectionAction;

    //Global Controls
     public InputAction openInventoryAction;

    public static InputManager Instance { get; set; }
    private void Awake()
    {
        Instance = this;

        //controls = new PlayerControls();
        playerInput = GetComponent<PlayerInput>();


        playerInput.actions.FindActionMap("Player").Disable();
        playerInput.actions.FindActionMap("NodeMap").Enable();
        playerInput.actions.FindActionMap("Global").Enable();

        interactAction = playerInput.actions["Interact"];
        moveAction = playerInput.actions["Move"];
        aimAction = playerInput.actions["Aim"];
        mouseAimAction = playerInput.actions["MouseAim"];
        swordAttackAction = playerInput.actions["SwordAttack"];
        bowAttackAction = playerInput.actions["BowAttack"];
        completeRoomAction = playerInput.actions["CompleteRoom"];
        changeFormAction = playerInput.actions["ChangeForm"];
        selectSwordAction = playerInput.actions["SelectSword"];
        selectBowAction = playerInput.actions["SelectBow"];
        mapScrollAction = playerInput.actions["MapScroll"];
        regenMapAction = playerInput.actions["RegenerateMap"];
        swapActionMapAction = playerInput.actions["SwapActionMap"];
        selectNodeAction = playerInput.actions["SelectNode"];
        moveSelectionAction = playerInput.actions["MoveSelection"];
        openInventoryAction = playerInput.actions["OpenInventory"];

        openInventoryAction.started += ToggleInventory;
    }

    private void OnDisable()
    {
        openInventoryAction.started -= ToggleInventory;
    }

    public void SwapActionMap(string mapName)
    {
        if (mapName == "Player")
        {
            playerInput.actions.FindActionMap("Player").Enable();
            playerInput.actions.FindActionMap("NodeMap").Disable();
        }
        else if (mapName == "NodeMap")
        {
            playerInput.actions.FindActionMap("Player").Disable();
            playerInput.actions.FindActionMap("NodeMap").Enable();
        }
    }

    public PlayerInput GetPlayerInput()
    {
        return playerInput;
    }

    public bool Interact()
    {
        if (playerInput.actions.FindActionMap("Player").enabled)
        {
            return interactAction.triggered;
        }

        return false;
    }

    public bool SwordAttack()
    {
        if (playerInput.actions.FindActionMap("Player").enabled)
        {
            return swordAttackAction.triggered;
        }

        return false;
    }

    public bool ShootBow()
    {
        if (playerInput.actions.FindActionMap("Player").enabled)
        {
            //return bowAttackAction.triggered;

            return bowAttackAction.IsInProgress();
        }

        return false;
    }

    public bool ReleaseArrow()
    {
        if (playerInput.actions.FindActionMap("Player").enabled)
        {
            return bowAttackAction.phase == InputActionPhase.Started || bowAttackAction.phase == InputActionPhase.Waiting;
        }

        return false;
    }

    public Vector2 Move()
    {
        if (playerInput.actions.FindActionMap("Player").enabled)
        {
            return moveAction.ReadValue<Vector2>();
        }

        return Vector2.zero;
    }

    public Vector2 MoveSelection()
    {
        if (playerInput.actions.FindActionMap("NodeMap").enabled && moveSelectionAction.triggered)
        {
            return moveSelectionAction.ReadValue<Vector2>();
        }

        return Vector2.zero;
    }

    public Vector2 Aim()
    {
        if (playerInput.actions.FindActionMap("Player").enabled)
        {
            return aimAction.ReadValue<Vector2>();
        }

        return Vector2.zero;
    }

    public Vector2 MouseAim()
    {
        if (playerInput.actions.FindActionMap("Player").enabled)
        {
            return mouseAimAction.ReadValue<Vector2>();
        }

        return Vector2.zero;
    }

    public bool CompleteRoom()
    {
        if (playerInput.actions.FindActionMap("Player").enabled)
        {
            return completeRoomAction.triggered;
        }

        return false;
    }

    public bool ChangeForm()
    {
        if (playerInput.actions.FindActionMap("Player").enabled)
        {
            return changeFormAction.triggered;
        }

        return false;
    }

    public bool SelectSword()
    {
        if (playerInput.actions.FindActionMap("Player").enabled)
        {
            return selectSwordAction.triggered;
        }

        return false;
    }

    public bool SelectBow()
    {
        if (playerInput.actions.FindActionMap("Player").enabled)
        {
            return selectBowAction.triggered;
        }

        return false;
    }

    public float MapScroll()
    {
        if (playerInput.actions.FindActionMap("NodeMap").enabled)
        {
            return mapScrollAction.ReadValue<float>();
        }

        return 0;
    }

    public bool RegenMap()
    {
        if (playerInput.actions.FindActionMap("NodeMap").enabled)
        {
            return regenMapAction.triggered;
        }

        return false;
    }

    public bool SwapMapButton()
    {
        if (playerInput.actions.FindActionMap("NodeMap").enabled)
        {
            return swapActionMapAction.triggered;
        }

        return false;
    }

    public bool SelectNode()
    {
        if (playerInput.actions.FindActionMap("NodeMap").enabled)
        {
            return selectNodeAction.triggered;
        }

        return false;
    }

    void ToggleInventory(InputAction.CallbackContext ctx)
    {
        PlayerController.Instance.playerInventory.ToggleInventory();
    }
}
