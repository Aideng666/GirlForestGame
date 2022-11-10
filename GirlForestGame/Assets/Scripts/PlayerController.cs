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
    //[SerializeField] LayerMask enemyLayer;
    bool isAttacking;
    bool isKnockbackApplied;
    bool canAttack = true;
    bool bowDrawn;
    List<Enemy> visibleEnemies = new List<Enemy>();
    int currentAttackNum = 1;
    Forms currentForm = Forms.Living;
    LayerMask livingLayer;
    LayerMask spiritLayer;

    public Forms Form { get { return currentForm; } set { currentForm = value; } }

    //Player Attributes
    [Header("Default Player Attributes")]
    [SerializeField] float defaultHealth = 6;
    [SerializeField] float maximumHealth = 12;
    [SerializeField] float defaultSpeed;
    [SerializeField] float defaultSwordDamage;
    [SerializeField] float defaultBowDamage;
    [SerializeField] float defaultSwordCooldown;
    [SerializeField] float defaultBowCooldown;
    [SerializeField] float defaultSwordRange = 5;
    [SerializeField] float defaultProjectileSpeed;
    [SerializeField] float defaultCritChance = 0.05f; // between 0 and 1 for 0%-100%
    float currentMaxHealth;
    float currentHealth;
    float currentSpeed;
    float currentSwordDamage;
    float currentBowDamage;
    float currentSwordCooldown;
    float currentBowCooldown;
    float currentSwordRange;
    float currentProjectileSpeed;
    float currentCritChance;

    public float MaxHealth { get { return currentMaxHealth; } set { currentMaxHealth = value; if (currentMaxHealth > maximumHealth) currentMaxHealth = maximumHealth; } }
    public float Health { get { return currentHealth; } set { currentHealth = value;} }
    public float Speed { get { return currentSpeed; } set { currentSpeed = value; } }
    public float SwordDamage { get { return currentSwordDamage; } set { currentSwordDamage = value; } }
    public float BowDamage { get { return currentBowDamage; } set { currentBowDamage = value; } }
    public float SwordCooldown { get { return currentSwordCooldown; } set { currentSwordCooldown = value; } }
    public float BowCooldown { get { return currentBowCooldown; } set { currentBowCooldown = value; } }
    public float SwordRange { get { return currentSwordRange; } set { currentSwordRange = value; } }
    public float ProjectileSpeed { get { return currentProjectileSpeed; } set { currentProjectileSpeed = value; } }
    public float CritChance { get { return currentCritChance; } set { currentCritChance = value; } }


    //Markings and Totems
    Spirit bowAttribute = null;
    Spirit swordAttribute = null;
    Spirit bowElement = null;
    Spirit swordElement = null;

    public Spirit BowAttribute { get { return bowAttribute; } set { bowAttribute = value; } }
    public Spirit SwordAttribute { get { return swordAttribute; } set { swordAttribute = value; } }
    public Spirit BowElement { get { return bowElement; } set { bowElement = value; } }
    public Spirit SwordElement { get { return swordElement; } set { swordElement = value; } }

    List<Totem> totems = new List<Totem>();

    public static PlayerController Instance { get; set; }
    
    //Sound
    public FMODUnity.EventReference inputSound;
    public float stepDelay;
    bool isWalking;
    void CallFootsteps()
    {
        if (isWalking == true)
        {
            Debug.Log(stepDelay);
            FMODUnity.RuntimeManager.PlayOneShot(inputSound);
        }
    }


    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody>();

        currentMaxHealth = defaultHealth;
        currentHealth = defaultHealth;
        currentSpeed = defaultSpeed;
        currentSwordDamage = defaultSwordDamage;
        currentBowDamage = defaultBowDamage;
        currentSwordCooldown = defaultSwordCooldown;
        currentBowCooldown = defaultBowCooldown;
        currentSwordRange = defaultSwordRange;
        currentProjectileSpeed = defaultProjectileSpeed;
        currentCritChance = defaultCritChance;

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

        if (Physics.Raycast(ray, out hit, 100))
        {
            mousePos = hit.point;

            mouseAimDirection = (mousePos - transform.position).normalized;

            mouseAimDirection.y = 0;
        }

        float targetAngle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;/* + Camera.main.transform.eulerAngles.y;*/
        moveDir = Quaternion.AngleAxis(45, Vector3.up) * Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

        float aimAngle = Mathf.Atan2(aimDir.x, aimDir.y) * Mathf.Rad2Deg;/* + Camera.main.transform.eulerAngles.y;*/
        float mouseAimAngle = Mathf.Atan2(mouseAimDirection.x, mouseAimDirection.z) * Mathf.Rad2Deg;
        aimDirection = Quaternion.AngleAxis(45, Vector3.up) * Quaternion.Euler(0f, aimAngle, 0f) * Vector3.forward;

        if (controlWithMouse)
        {
            aimDirection = Quaternion.AngleAxis(45, Vector3.up) * Quaternion.Euler(0f, mouseAimAngle, 0f) * Vector3.forward;
            transform.rotation = Quaternion.Euler(0, mouseAimAngle + 45, 0);
        }
        else if (aimDir.magnitude <= 0.1f)
        {
            aimDirection = moveDir;
            transform.rotation = Quaternion.Euler(0f, targetAngle + 45, 0f);
        }
        else
        {
            aimDirection = Quaternion.AngleAxis(45, Vector3.up) * Quaternion.Euler(0f, aimAngle, 0f) * Vector3.forward;
            transform.rotation = Quaternion.Euler(0f, aimAngle + 45, 0f);
        }

        if (direction.magnitude <= 0.1f)
        {
            moveDir = new Vector3(0, 0, 0);

            body.velocity = new Vector3(0, 0, 0);
            isWalking = false;
        }
        else
        {
            isWalking = true;
        }

        body.velocity = currentSpeed * moveDir;
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

    public void InitSwordAttack()
    {
        if (canAttack)
        {
            if (targetEnemy == null)
            {
                StartCoroutine(MoveTowardsAttack(SwordCooldown));
            }
            else if (Vector3.Distance(targetEnemy.transform.position, transform.position) > currentSwordRange)
            {
                StartCoroutine(MoveTowardsTargetEnemy(SwordCooldown));
            }
            else
            {
                SwordAttack();
            }
        }
    }

    public void SwordAttack()
    {
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

        ActivateSwordHitbox(currentAttackNum);

        canAttack = false;
    }

    public void BowAttack()
    {
        GetComponentInChildren<Animator>().SetTrigger("DrawBow");

        canAttack = false;
    }

    void SpawnArrow()
    {
        var arrow = Instantiate(arrowPrefab, transform.position + Vector3.up + transform.forward, Quaternion.Euler(90, transform.rotation.eulerAngles.y, 0));

        arrow.GetComponent<Rigidbody>().velocity = transform.forward * ProjectileSpeed;
    }

    void UpdateMarking(Spirit spirit, MarkingTypes type, Weapons weapon)
    {
        if (type == MarkingTypes.Attribute)
        {
            for (int i = 0; i < spirit.buffedAttributes.Count; i++)
            {
                switch (spirit.buffedAttributes[i])
                {
                    //Buff all of these based on the level of the marking when adding level functionality
                    case PlayerAttributes.Health:



                        break;

                    case PlayerAttributes.Attack:

                        if (weapon == Weapons.Sword)
                        {
                            SwordDamage *= 1.75f;
                        }
                        else if (weapon == Weapons.Bow)
                        {
                            BowDamage *= 1.75f;
                        }

                        break;

                    case PlayerAttributes.AtkSpd:

                        if (weapon == Weapons.Sword)
                        {
                            SwordCooldown /= 1.75f;
                        }
                        else if (weapon == Weapons.Bow)
                        {
                            BowCooldown /= 1.75f;
                        }

                        break;

                    case PlayerAttributes.Speed:

                        Speed *= 1.75f;

                        break;

                    case PlayerAttributes.Accuracy:



                        break;

                    case PlayerAttributes.CritChance:

                        CritChance *= 1.75f;

                        break;


                }
            }
        }
    }

    void RemoveMarking(Spirit spirit, MarkingTypes type, Weapons weapon)
    {
        if (type == MarkingTypes.Attribute)
        {
            for (int i = 0; i < spirit.buffedAttributes.Count; i++)
            {
                switch (spirit.buffedAttributes[i])
                {
                    case PlayerAttributes.Health:



                        break;

                    case PlayerAttributes.Attack:

                        if (weapon == Weapons.Sword)
                        {
                            SwordDamage /= 1.75f;
                        }
                        else if (weapon == Weapons.Bow)
                        {
                            BowDamage /= 1.75f;
                        }

                        break;

                    case PlayerAttributes.AtkSpd:

                        if (weapon == Weapons.Sword)
                        {
                            SwordCooldown *= 1.75f;
                        }
                        else if (weapon == Weapons.Bow)
                        {
                            BowCooldown *= 1.75f;
                        }

                        break;

                    case PlayerAttributes.Speed:

                        Speed /= 1.75f;

                        break;

                    case PlayerAttributes.Accuracy:



                        break;

                    case PlayerAttributes.CritChance:

                        CritChance /= 1.75f;

                        break;
                }
            }
        }
    }

    public void ChooseWeapon(Spirit spirit, MarkingTypes type)
    {
        StartCoroutine(SelectWeapon(spirit, type));
    }

    IEnumerator SelectWeapon(Spirit spirit, MarkingTypes type)
    {
        bool weaponSelected = false;

        while(!weaponSelected)
        {
            if (InputManager.Instance.SelectSword())
            {
                if (type == MarkingTypes.Attribute)
                {
                    //CHECK IF THE PLAYER ALREADY HAS A SPIRIT IN EACH SLOT TO BE ABLE TO SWAP
                    SwordAttribute = spirit;
                }
                else if (type == MarkingTypes.Element)
                {
                    SwordElement = spirit;
                }

                UpdateMarking(spirit, type, Weapons.Sword);

                weaponSelected = true;
            }
            if (InputManager.Instance.SelectBow())
            {
                if (type == MarkingTypes.Attribute)
                {
                    BowAttribute = spirit;
                }
                else if (type == MarkingTypes.Element)
                {
                    BowElement = spirit;
                }

                UpdateMarking(spirit, type, Weapons.Bow);

                weaponSelected = true;
            }

            yield return null;
        }
    }

    void ActivateSwordHitbox(int attackNum)
    {
        Collider[] enemyColliders = null;
        List<Enemy> enemiesHit = new List<Enemy>();

        switch (attackNum)
        {
            case 1:

                enemyColliders = Physics.OverlapSphere(transform.position + (transform.forward * 2), SwordRange);

                if (enemyColliders.Length > 0)
                {
                    for (int i = 0; i < enemyColliders.Length; i++)
                    {
                        if (enemyColliders[i].gameObject.TryGetComponent<Enemy>(out Enemy enemy) && enemy.Form == Form)
                        {
                            enemiesHit.Add(enemy);
                        }
                    }
                }

                for (int i = 0; i < enemiesHit.Count; i++)
                {
                    enemiesHit[i].ApplyKnockback(transform.forward, 5);
                    enemiesHit[i].TakeDamage(SwordDamage);
                }

                break;

            case 2:

                enemyColliders = Physics.OverlapSphere(transform.position + (transform.forward * 2), SwordRange);

                if (enemyColliders.Length > 0)
                {
                    for (int i = 0; i < enemyColliders.Length; i++)
                    {
                        if (enemyColliders[i].gameObject.TryGetComponent<Enemy>(out Enemy enemy) && enemy.Form == Form)
                        {
                            enemiesHit.Add(enemy);
                        }
                    }
                }

                for (int i = 0; i < enemiesHit.Count; i++)
                {
                    enemiesHit[i].ApplyKnockback(transform.forward, 10);
                    enemiesHit[i].TakeDamage(SwordDamage);
                }

                break;

            case 3: // CHANGE THIS WHEN WE GET THE THIRD ATTACK

                enemyColliders = Physics.OverlapSphere(transform.position + (transform.forward * 2), SwordRange);

                if (enemyColliders.Length > 0)
                {
                    for (int i = 0; i < enemyColliders.Length; i++)
                    {
                        if (enemyColliders[i].gameObject.TryGetComponent<Enemy>(out Enemy enemy) && enemy.Form == Form)
                        {
                            enemiesHit.Add(enemy);
                        }
                    }
                }

                for (int i = 0; i < enemiesHit.Count; i++)
                {
                    enemiesHit[i].ApplyKnockback(transform.forward, 10);
                    enemiesHit[i].TakeDamage(SwordDamage);
                }

                break;
        }
    }

    void SelectTargetEnemy()
    {
        Collider[] collidersDetected = new Collider[0];
        Collider[] collidersDetected2 = new Collider[0];
        Collider[] collidersDetected3 = new Collider[0];

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

        visibleEnemies = new List<Enemy>();

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
    }

    public void AddTotemToList(Totem totem)
    {
        totems.Add(totem);
    }

    IEnumerator MoveTowardsTargetEnemy(float duration)
    {
        float elaspedTime = 0;
        isAttacking = true;

        StartCoroutine(CompleteAttackMovement(duration));

        Vector3 endVelo = Vector3.zero;
        Vector3 startVelo = currentSpeed * (targetEnemy.transform.position - transform.position).normalized;

        float targetAngle = Mathf.Atan2(startVelo.x, startVelo.z) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0f, targetAngle, 0f);

        while (elaspedTime < duration)
        {
            body.velocity = Vector3.Lerp(startVelo, endVelo, elaspedTime / duration);

            elaspedTime += Time.deltaTime;

            yield return null;
        }

        yield return null;
    }

    IEnumerator MoveTowardsAttack(float duration)
    {
        isAttacking = true;

        StartCoroutine(CompleteAttackMovement(duration));

        float elaspedTime = 0;
        Vector3 endVelo = Vector3.zero;
        Vector3 startVelo = currentSpeed * aimDirection;

        while (elaspedTime < duration)
        {
            body.velocity = Vector3.Lerp(startVelo, endVelo, elaspedTime / duration);

            elaspedTime += Time.deltaTime;

            yield return null;
        }

        yield return null;
    }

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
                    StartCoroutine(BeginAttackCooldown(SwordCooldown));
                }

                if (weaponChoice == Weapons.Bow)
                {
                    StartCoroutine(BeginAttackCooldown(BowCooldown));
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
        if (collision.gameObject.CompareTag("Exit"))
        {
            UIManager.Instance.GetFadePanel().BeginRoomTransition();

            StartCoroutine(EnterNewRoom(
                DungeonGenerator.Instance.GetCurrentRoom().GetConnectedRooms()[(int)collision.gameObject.GetComponent<RoomExit>().GetExitDirection()],
                DungeonGenerator.Instance.GetCurrentRoom().GetConnectedRooms()[(int)collision.gameObject.GetComponent<RoomExit>().GetExitDirection()].GetDoors()[(int)DungeonGenerator.Instance.ReverseDirection(collision.gameObject.GetComponent<RoomExit>().GetExitDirection())]
                .transform.parent.transform.position));
        }
        if (collision.gameObject.CompareTag("FloorExit"))
        {
            NodeMapManager.Instance.SetNextLevel();
        }
    }

    IEnumerator EnterNewRoom(Room room, Vector3 updatedPlayerPos)
    {
        yield return new WaitForSeconds(UIManager.Instance.GetFadePanel().GetTransitionTime() / 2);

        DungeonGenerator.Instance.SetCurrentRoom(room);

        transform.position = updatedPlayerPos;

        yield return null;
    }
}

public enum Forms
{
    Living,
    Spirit
}

