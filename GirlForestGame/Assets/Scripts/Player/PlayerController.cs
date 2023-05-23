using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    //Movement
    [SerializeField] GameObject aimColliders;
    [SerializeField] Minimap minimap;
    [SerializeField] bool controlWithMouse;
    Rigidbody body;
    Vector3 moveDir;
    public Vector3 aimDirection { get; private set; }

    //Player Components
    [HideInInspector] public PlayerAttributes playerAttributes;
    [HideInInspector] public PlayerMarkings playerMarkings;
    [HideInInspector] public PlayerInventory playerInventory;
    [HideInInspector] public PlayerCombat playerCombat;

    [HideInInspector] public FMOD.Studio.EventInstance deadSFX;
    FMOD.Studio.Bus EnemyBus;

    bool deathStarted;
    bool roomTransitionStarted;

    public static PlayerController Instance { get; set; }
    private void OnEnable()
    {
        deadSFX = FMODUnity.RuntimeManager.CreateInstance("event:/UI/Die");

    }
    private void Awake()
    {
        Instance = this;

    }

    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody>();

        playerAttributes = GetComponent<PlayerAttributes>();
        playerMarkings = GetComponent<PlayerMarkings>();
        playerInventory = GetComponent<PlayerInventory>();
        playerCombat = GetComponent<PlayerCombat>();

        EnemyBus = FMODUnity.RuntimeManager.GetBus("bus:/SFX/Enemy");
    }

    // Update is called once per frame
    void Update()
    {
        if (UIManager.Instance.isPaused)
        {
            return;
        }

        //Checks for player death
        if (playerAttributes.Health <= 0)
        {
            if (playerInventory.totemDictionary[typeof(ExtraLifeTotem)] < 1)
            {
                if (!deathStarted)
                {
                    StartCoroutine(Die());
                }
            }
            else
            {
                /*EventManager.Instance.InvokeTotemTrigger(TotemEvents.OnPlayerDeath);*/
                playerInventory.GetTotemFromList(typeof(ExtraLifeTotem)).ApplyEffect();
                playerInventory.RemoveTotem(typeof(ExtraLifeTotem));
            }
        }

        if (!playerCombat.isKnockbackApplied)
        {
            if (!playerCombat.isAttacking && !UIManager.Instance.inventoryOpen)
            {
                Move();

                Interact();
            }
        }

        transform.position = new Vector3(transform.position.x, 0, transform.position.z);
    }

    void Move()
    {
        Vector2 direction = InputManager.Instance.Move();
        Vector2 aimDir = InputManager.Instance.Aim();

        Vector3 mousePos;
        Vector3 mouseAimDirection = Vector3.zero;

        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;

        //Sets mouse variables based on the mouse position in the world
        if (Physics.Raycast(ray, out hit, 100))
        {
            mousePos = hit.point;

            mouseAimDirection = (mousePos - transform.position).normalized;

            mouseAimDirection.y = 0;
        }
        ///////////////////////////////////////////////////////////////

        float targetAngle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;/* + Camera.main.transform.eulerAngles.y;*/
        moveDir = Quaternion.AngleAxis(45, Vector3.up) * Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

        if (direction.magnitude <= 0.1f)
        {
            moveDir = new Vector3(0, 0, 0);

            body.velocity = new Vector3(0, 0, 0);
        }

        float aimAngle = Mathf.Atan2(aimDir.x, aimDir.y) * Mathf.Rad2Deg;/* + Camera.main.transform.eulerAngles.y;*/
        float mouseAimAngle = Mathf.Atan2(mouseAimDirection.x, mouseAimDirection.z) * Mathf.Rad2Deg;
        aimDirection = Quaternion.AngleAxis(45, Vector3.up) * Quaternion.Euler(0f, aimAngle, 0f) * Vector3.forward;

        //Sets the aiming direction of the player along with their rotation to face in the direction that they are aiming
        if (controlWithMouse)
        {
            aimDirection = /*Quaternion.AngleAxis(45, Vector3.up) * */Quaternion.Euler(0f, mouseAimAngle, 0f) * Vector3.forward;
            aimColliders.transform.rotation = Quaternion.Euler(0, mouseAimAngle, 0);
            //transform.rotation = Quaternion.Euler(0, mouseAimAngle, 0);
        }
        else if (aimDir.magnitude <= 0.1f && direction.magnitude >= 0.1f)
        {
            aimDirection = moveDir;
            aimColliders.transform.forward = moveDir.normalized;
            //transform.forward = moveDir.normalized;
        }
        else if (aimDir.magnitude > 0.1f)
        {
            aimDirection = Quaternion.AngleAxis(45, Vector3.up) * Quaternion.Euler(0f, aimAngle, 0f) * Vector3.forward;
            aimColliders.transform.rotation = Quaternion.Euler(0, aimAngle + 45, 0);
            //transform.rotation = Quaternion.Euler(0f, aimAngle + 45, 0f);
        }
        else if (aimDir.magnitude < 0.1f && direction.magnitude <= 0.1f)
        {
            aimDirection = transform.forward;
        }

        if (moveDir != Vector3.zero)
        {
            transform.forward = moveDir.normalized;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        body.velocity = playerAttributes.Speed * moveDir;

        if (moveDir.magnitude >= 0.1f)
        {
            GetComponentInChildren<Animator>().SetBool("Walking", true);
        }
        else
        {
            GetComponentInChildren<Animator>().SetBool("Walking", false);
        }

        //Starts the selection of the target enemy based on the direction the player is aiming
        playerCombat.SelectSwordTargetEnemy();
        playerCombat.SelectBowTargetEnemy();
    }

    void Interact()
    {
        GameObject itemToInteract = GetComponentInChildren<InteractRing>().selectedObject;

        if (itemToInteract != null && InputManager.Instance.Interact())
        {
            if (itemToInteract.TryGetComponent(out InteractableObject item))
            {
                item.Pickup();
            }
        }
    }

    IEnumerator Die()
    {
        deathStarted = true;

        yield return new WaitForSeconds(1);

        UIManager.Instance.ToggleDeathScreen();

        deadSFX.start();
    }

    public bool MouseControlActive()
    {
        return controlWithMouse;
    }

    private void OnCollisionEnter(Collision collision)
    {
        switch(collision.gameObject.tag)
        {
            //To transition from room to room
            case "Exit":

                UIManager.Instance.GetFadePanel().BeginRoomTransition();

                if (!roomTransitionStarted)
                {
                    StartCoroutine(EnterNewRoom(
                        DungeonGenerator.Instance.GetCurrentRoom().GetConnectedRooms()[(int)collision.gameObject.GetComponent<RoomExit>().GetExitDirection()],
                        DungeonGenerator.Instance.GetCurrentRoom().GetConnectedRooms()[(int)collision.gameObject.GetComponent<RoomExit>().GetExitDirection()]
                        .GetDoors()[(int)DungeonGenerator.Instance.ReverseDirection(collision.gameObject.GetComponent<RoomExit>().GetExitDirection())]
                        .transform.parent.transform.position, DungeonGenerator.Instance.ReverseDirection(collision.gameObject.GetComponent<RoomExit>().GetExitDirection())));
                }

                break;

            //To transition to the node map after completing a full floor
            case "FloorExit":

                NodeMapManager.Instance.SetNextLevel();

                break;

            case "Enemy":

                playerCombat.TakeDamage();
                //playerCombat.ApplyKnockback((transform.position - collision.gameObject.transform.position), 2);
                break;

            case "EnemyProjectile":

                if (collision.gameObject.GetComponentInParent<EnemyData>() == null || collision.gameObject.GetComponentInParent<EnemyData>().IsAttacking)
                {
                    playerCombat.TakeDamage();
                    //playerCombat.ApplyKnockback((transform.position - collision.gameObject.gameObject.transform.position), 2);
                }

                break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        switch (other.gameObject.tag)
        {
            case "HalfHeart":

                if (playerAttributes.Health < playerAttributes.MaxHealth)
                {
                    playerAttributes.Health += 1;
                    PickupPool.Instance.AddHalfHeart(other.gameObject);
                }

                break;

            case "Heart":

                if (playerAttributes.Health < playerAttributes.MaxHealth)
                {
                    playerAttributes.Health += 2;
                    PickupPool.Instance.AddHeart(other.gameObject);
                }

                break;

            case "Coin":

                playerInventory.ModifyMoney(1);
                PickupPool.Instance.AddCoin(other.gameObject);

                if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("Tutorial"))
                {
                    TutorialManager.Instance.TriggerTutorialSection(11, true);
                }

                break;

            //case "EnemyProjectile":

            //    if (other.GetComponentInParent<EnemyData>() == null || other.GetComponentInParent<EnemyData>().IsAttacking)
            //    {
            //        playerCombat.TakeDamage();
            //        playerCombat.ApplyKnockback((transform.position - other.gameObject.transform.position), 2);
            //    }

            //    break;
        }
    }

    IEnumerator EnterNewRoom(Room room, Vector3 updatedPlayerPos, Directions dirOfPrevRoom)
    {
        roomTransitionStarted = true;

        yield return new WaitForSeconds(UIManager.Instance.GetFadePanel().GetTransitionTime() / 2);

        foreach (ParticleSystem particle in DungeonGenerator.Instance.GetCurrentRoom().GetComponentsInChildren<ParticleSystem>()) 
        {
            Destroy(particle.gameObject);
        }

        DungeonGenerator.Instance.SetCurrentRoom(room);

        transform.position = updatedPlayerPos;

        minimap.VisitRoom(room, dirOfPrevRoom);

        yield return null;

        roomTransitionStarted = false;
    }
    private void OnDestroy()
    {
        deadSFX.release();
    }
}

