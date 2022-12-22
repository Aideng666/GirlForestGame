using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    //Movement
    [SerializeField] bool controlWithMouse;
    Rigidbody body;
    Vector3 moveDir;

    //Combat
    [SerializeField] GameObject arrowPrefab;
    [SerializeField] GameObject bowAimCanvas;
    [SerializeField] Material livingFormMaterial;
    [SerializeField] Material spiritFormMaterial;
    GameObject targetEnemy;
    Vector3 aimDirection;
    bool isAttacking;
    bool isKnockbackApplied;
    bool canAttack = true;
    bool bowDrawn;
    List<Enemy> visibleEnemies = new List<Enemy>();
    int currentAttackNum = 1;
    Forms currentForm = Forms.Living;
    LayerMask livingLayer;
    LayerMask spiritLayer;

    public delegate void OnAttack(List<EnemyData> enemiesHit);
    public static event OnAttack OnSwordHit;

    public Forms Form { get { return currentForm; } set { currentForm = value; } }

    //Player Components
    public PlayerAttributes playerAttributes;
    public PlayerMarkings playerMarkings;
    public PlayerInventory playerInventory;

    //List<Totem> totems = new List<Totem>();

    public static PlayerController Instance { get; set; }

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

        livingLayer = LayerMask.NameToLayer("Living");
        spiritLayer = LayerMask.NameToLayer("Spirit");

        bowAimCanvas.SetActive(false);

        //enemyLayer = LayerMask.NameToLayer("Enemy");
    }

    // Update is called once per frame
    void Update()
    {
        if (!isKnockbackApplied)
        {
            if (!isAttacking)
            {
                Move();

                SelectTargetEnemy();
            }

            //Detects the release of the arrow once the bow is completely drawn back
            if (bowDrawn)
            {
                bowAimCanvas.SetActive(true);

                if (InputManager.Instance.ReleaseArrow())
                {
                    GetComponentInChildren<Animator>().SetTrigger("ReleaseArrow");

                    bowDrawn = false;

                    bowAimCanvas.SetActive(false);

                    SpawnArrow();
                }
            }
            ////////////////////////////////////////////////////////////////////////


            if (InputManager.Instance.ChangeForm())
            {
                if (currentForm == Forms.Living)
                {
                    currentForm = Forms.Spirit;
                    GetComponentInChildren<SkinnedMeshRenderer>().material = spiritFormMaterial;
                    gameObject.layer = spiritLayer;
                }
                else
                {
                    currentForm = Forms.Living;
                    GetComponentInChildren<SkinnedMeshRenderer>().material = livingFormMaterial;
                    gameObject.layer = livingLayer;
                }
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

        float aimAngle = Mathf.Atan2(aimDir.x, aimDir.y) * Mathf.Rad2Deg;/* + Camera.main.transform.eulerAngles.y;*/
        float mouseAimAngle = Mathf.Atan2(mouseAimDirection.x, mouseAimDirection.z) * Mathf.Rad2Deg;
        aimDirection = Quaternion.AngleAxis(45, Vector3.up) * Quaternion.Euler(0f, aimAngle, 0f) * Vector3.forward;


        //Sets the aiming direction of the player along with their rotation to face in the direction that they are aiming
        if (controlWithMouse)
        {
            aimDirection = Quaternion.AngleAxis(45, Vector3.up) * Quaternion.Euler(0f, mouseAimAngle, 0f) * Vector3.forward;
            transform.rotation = Quaternion.Euler(0, mouseAimAngle, 0);
        }
        else if (aimDir.magnitude <= 0.1f && direction.magnitude >= 0.1f)
        {
            aimDirection = moveDir;
            transform.forward = moveDir.normalized;
        }
        else if (aimDir.magnitude > 0.1f)
        {
            aimDirection = Quaternion.AngleAxis(45, Vector3.up) * Quaternion.Euler(0f, aimAngle, 0f) * Vector3.forward;
            transform.rotation = Quaternion.Euler(0f, aimAngle + 45, 0f);
        }
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        if (direction.magnitude <= 0.1f)
        {
            moveDir = new Vector3(0, 0, 0);

            body.velocity = new Vector3(0, 0, 0);
        }

        body.velocity = playerAttributes.Speed * moveDir;
    }

    public void ApplyKnockback(Vector3 direction, float power)
    {
        isKnockbackApplied = true;

        body.velocity = direction * power;

        StartCoroutine(DeactivateKnockback());
    }

    IEnumerator DeactivateKnockback()
    {
        Vector3 startVel = body.velocity;
        float elaspedTime = 0;

        while(elaspedTime <= 0.5f)
        {
            body.velocity = Vector3.Lerp(startVel, Vector3.zero, elaspedTime / 0.5f);

            Vector3 direction = -body.velocity;

            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;

            transform.rotation = Quaternion.Euler(0f, targetAngle, 0f);

            elaspedTime += Time.deltaTime;

            yield return null;
        }

        isKnockbackApplied = false;

        yield return null;
    }


    //allows for the player to perform the little dash forwards before each sword swing
    //chooses the direction of the dash and attack based on the target enemy and distance
    public void InitSwordAttack()
    {
        if (canAttack)
        {
            if (targetEnemy == null)
            {
                StartCoroutine(MoveTowardsAttack(playerAttributes.SwordCooldown));
            }
            else if (Vector3.Distance(targetEnemy.transform.position, transform.position) > playerAttributes.SwordRange)
            {
                StartCoroutine(MoveTowardsTargetEnemy(playerAttributes.SwordCooldown));
            }
            else
            {
                SwordAttack();
            }
        }
    }

    public void SwordAttack()
    {
        //Chooses which animation to play based on which attack number they are in the current combo
        switch (currentAttackNum)
        {
            case 1:

                GetComponentInChildren<Animator>().SetTrigger("Attack1");

                break;

            case 2:

                GetComponentInChildren<Animator>().SetTrigger("Attack2");

                break;

            case 3:

                GetComponentInChildren<Animator>().SetTrigger("Attack3");

                break;
        }
        ////////////////////////////////////////////////////////////////////

        ActivateSwordHitbox(currentAttackNum);

        canAttack = false;
    }

    //Initializes the drawing of the bow
    public void BowAttack()
    {
        GetComponentInChildren<Animator>().SetTrigger("DrawBow");

        canAttack = false;
    }

    void SpawnArrow()
    {
        var arrow = Instantiate(arrowPrefab, transform.position + Vector3.up + transform.forward, Quaternion.Euler(90, transform.rotation.eulerAngles.y, 0));

        arrow.GetComponent<Rigidbody>().velocity = transform.forward * playerAttributes.ProjectileSpeed;
    }

    void ActivateSwordHitbox(int attackNum)
    {
        Collider[] enemyColliders = null;
        List<EnemyData> enemiesHit = new List<EnemyData>();

        switch (attackNum)
        {
            case 1:

                enemyColliders = Physics.OverlapSphere(transform.position + (transform.forward * 2), playerAttributes.SwordRange);

                //Loops through each hit collider and adds all of the enemies into a list
                if (enemyColliders.Length > 0)
                {
                    for (int i = 0; i < enemyColliders.Length; i++)
                    {
                        if (enemyColliders[i].gameObject.TryGetComponent(out EnemyData enemy) && enemy.form == Form)
                        {
                            enemiesHit.Add(enemy);
                        }
                    }
                }
                //////////////////////////////////////////////////////////////////////////
                
                for (int i = 0; i < enemiesHit.Count; i++)
                {
                    //enemiesHit[i].ApplyKnockback(transform.forward, 5);
                    enemiesHit[i].TakeDamage(playerAttributes.SwordDamage, true);
                }

                if (enemiesHit.Count > 0)
                {
                    OnSwordHit?.Invoke(enemiesHit);
                }

                break;

            case 2:
                enemyColliders = Physics.OverlapSphere(transform.position + (transform.forward * 2), playerAttributes.SwordRange);

                //Loops through each hit collider and adds all of the enemies into a list
                if (enemyColliders.Length > 0)
                {
                    for (int i = 0; i < enemyColliders.Length; i++)
                    {
                        if (enemyColliders[i].gameObject.TryGetComponent(out EnemyData enemy) && enemy.form == Form)
                        {
                            enemiesHit.Add(enemy);
                        }
                    }
                }
                //////////////////////////////////////////////////////////////////////////

                for (int i = 0; i < enemiesHit.Count; i++)
                {
                    //enemiesHit[i].ApplyKnockback(transform.forward, 5);
                    enemiesHit[i].TakeDamage(playerAttributes.SwordDamage, true);
                }

                if (enemiesHit.Count > 0)
                {
                    OnSwordHit?.Invoke(enemiesHit);
                }

                break;

            case 3: // CHANGE THIS WHEN WE GET THE THIRD ATTACK - this is for aiden dw abt it

                enemyColliders = Physics.OverlapSphere(transform.position + (transform.forward * 2), playerAttributes.SwordRange);

                //Loops through each hit collider and adds all of the enemies into a list
                if (enemyColliders.Length > 0)
                {
                    for (int i = 0; i < enemyColliders.Length; i++)
                    {
                        if (enemyColliders[i].gameObject.TryGetComponent(out EnemyData enemy) && enemy.form == Form)
                        {
                            enemiesHit.Add(enemy);
                        }
                    }
                }

                for (int i = 0; i < enemiesHit.Count; i++)
                {
                    //enemiesHit[i].ApplyKnockback(transform.forward, 5);
                    enemiesHit[i].TakeDamage(playerAttributes.SwordDamage, true);
                }

                if (enemiesHit.Count > 0)
                {
                    OnSwordHit?.Invoke(enemiesHit);
                }

                break;

                break;
        }
    }

    void SelectTargetEnemy()
    {
        Collider[] collidersDetected = new Collider[0];
        Collider[] collidersDetected2 = new Collider[0];
        Collider[] collidersDetected3 = new Collider[0];

        //Creates a "cone" that acts as the players "view" and adds every visible collider to the player into arrays
        if (aimDirection.magnitude > 0)
        {
            collidersDetected = Physics.OverlapSphere(transform.position + (aimDirection.normalized * 1), 1f);
            collidersDetected2 = Physics.OverlapSphere(transform.position + (aimDirection.normalized * 3), 2f);
            collidersDetected3 = Physics.OverlapSphere(transform.position + (aimDirection.normalized * 5), 3f);
        }
        else if (moveDir.magnitude > 0)
        {
            collidersDetected = Physics.OverlapSphere(transform.position + (moveDir.normalized * 1), 1f);
            collidersDetected2 = Physics.OverlapSphere(transform.position + (moveDir.normalized * 3), 2f);
            collidersDetected3 = Physics.OverlapSphere(transform.position + (moveDir.normalized * 5), 3f);
        }
        else
        {
            targetEnemy = null;
        }
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////


        visibleEnemies = new List<Enemy>();

        //Filters out only enemies from the visible colliders
        if (collidersDetected.Length > 0)
        {
            for (int i = 0; i < collidersDetected.Length; i++)
            {
                if (collidersDetected[i].gameObject.TryGetComponent<Enemy>(out Enemy enemy))
                {
                    visibleEnemies.Add(enemy);
                }
            }
        }
        if (collidersDetected2.Length > 0)
        {
            for (int i = 0; i < collidersDetected2.Length; i++)
            {
                if (collidersDetected2[i].gameObject.TryGetComponent<Enemy>(out Enemy enemy))
                {
                    visibleEnemies.Add(enemy);
                }
            }
        }
        if (collidersDetected3.Length > 0)
        {
            for (int i = 0; i < collidersDetected3.Length; i++)
            {
                if (collidersDetected3[i].gameObject.TryGetComponent<Enemy>(out Enemy enemy))
                {
                    visibleEnemies.Add(enemy);
                }
            }
        }
        /////////////////////////////////////////////////////

        //Sets the target enemy to the closest enemy that is within the player's view
        if (visibleEnemies.Count > 0)
        {
            targetEnemy = visibleEnemies[0].gameObject;

            for (int i = 0; i < visibleEnemies.Count; i++)
            {
                if (Vector3.Distance(visibleEnemies[i].transform.position, transform.position) < Vector3.Distance(targetEnemy.transform.position, transform.position))
                {
                    targetEnemy = visibleEnemies[i].gameObject;
                }
            }
        }
        else
        {
            targetEnemy = null;
        }
        ///////////////////////////////////////////////////////////////////////////////
    }

    //Called when player picks up a totem
    //public void AddTotemToList(Totem totem)
    //{
    //    totems.Add(totem);
    //}

    IEnumerator MoveTowardsTargetEnemy(float duration)
    {
        isAttacking = true;

        StartCoroutine(CompleteAttackMovement(duration));
        
        //Uses lerp to give the player a small dash forward towards their attack
        float elaspedTime = 0;
        Vector3 endVelo = Vector3.zero;
        Vector3 startVelo = playerAttributes.Speed * (targetEnemy.transform.position - transform.position).normalized;

        float targetAngle = Mathf.Atan2(startVelo.x, startVelo.z) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0f, targetAngle, 0f);

        while (elaspedTime < duration)
        {
            body.velocity = Vector3.Lerp(startVelo, endVelo, elaspedTime / duration);

            elaspedTime += Time.deltaTime;

            yield return null;
        }
        //////////////////////////////////////////////////////////////////////////

        yield return null;
    }

    IEnumerator MoveTowardsAttack(float duration)
    {
        isAttacking = true;

        StartCoroutine(CompleteAttackMovement(duration));

        //Uses lerp to give the player a small dash forward towards their attack
        float elaspedTime = 0;
        Vector3 endVelo = Vector3.zero;
        Vector3 startVelo = playerAttributes.Speed * aimDirection;

        while (elaspedTime < duration)
        {
            body.velocity = Vector3.Lerp(startVelo, endVelo, elaspedTime / duration);

            elaspedTime += Time.deltaTime;

            yield return null;
        }
        /////////////////////////////////////////////////////////////////////////

        yield return null;
    }

    //Starts the sword attack only a little bit into the dash in order to be able to attack at the end of the dash instead of beginning
    IEnumerator CompleteAttackMovement(float duration)
    {
        yield return new WaitForSeconds(duration * 0.25f);

        SwordAttack();

        yield return new WaitForSeconds(duration * 0.75f);

        isAttacking = false;
    }

    public int GetCurrentAttackNum()
    {
        return currentAttackNum;
    }

    public void SetCurrentAttackNum(int num)
    {
        currentAttackNum = num;
    }

    public bool GetCanAttack()
    {
        return canAttack;
    }

    public void SetBowDrawn(bool isDrawn)
    {
        bowDrawn = isDrawn;
    }

    public void SetCanAttack(bool value, bool applyCooldown, Weapons weaponChoice = Weapons.None)
    {
        if (value)
        {
            if (applyCooldown)
            {
                if (weaponChoice == Weapons.Sword)
                {
                    StartCoroutine(BeginAttackCooldown(playerAttributes.SwordCooldown));
                }

                if (weaponChoice == Weapons.Bow)
                {
                    StartCoroutine(BeginAttackCooldown(playerAttributes.BowCooldown));
                }

                return;
            }

            canAttack = true;

            return;
        }

        canAttack = false;
    }

    IEnumerator BeginAttackCooldown(float cooldown)
    {
        yield return new WaitForSeconds(cooldown);

        canAttack = true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        //Gizmos.DrawSphere(transform.position + (transform.forward * 2), 2);

        Gizmos.color = Color.green;

        if (targetEnemy != null)
        {
            Gizmos.DrawWireSphere(targetEnemy.transform.position, 2);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        //To transition from room to room
        if (collision.gameObject.CompareTag("Exit"))
        {
            UIManager.Instance.GetFadePanel().BeginRoomTransition();

            StartCoroutine(EnterNewRoom(
                DungeonGenerator.Instance.GetCurrentRoom().GetConnectedRooms()[(int)collision.gameObject.GetComponent<RoomExit>().GetExitDirection()],
                DungeonGenerator.Instance.GetCurrentRoom().GetConnectedRooms()[(int)collision.gameObject.GetComponent<RoomExit>().GetExitDirection()]
                .GetDoors()[(int)DungeonGenerator.Instance.ReverseDirection(collision.gameObject.GetComponent<RoomExit>().GetExitDirection())]
                .transform.parent.transform.position, DungeonGenerator.Instance.ReverseDirection(collision.gameObject.GetComponent<RoomExit>().GetExitDirection())));
        }
        //To transition to the node map after completing a full floor
        if (collision.gameObject.CompareTag("FloorExit"))
        {
            NodeMapManager.Instance.SetNextLevel();
        }
    }

    IEnumerator EnterNewRoom(Room room, Vector3 updatedPlayerPos, Directions dirOfPrevRoom)
    {
        yield return new WaitForSeconds(UIManager.Instance.GetFadePanel().GetTransitionTime() / 2);

        DungeonGenerator.Instance.SetCurrentRoom(room);

        transform.position = updatedPlayerPos;

        Minimap.Instance.VisitRoom(room, dirOfPrevRoom);

        yield return null;
    }
}

public enum Forms
{
    Living,
    Spirit
}

