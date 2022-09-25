using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    //Movement
    [SerializeField] bool controlWithMouse;
    [SerializeField] float defaultSpeed;
    //[SerializeField] float dashTimespan;
    //[SerializeField] float dashCooldown = 2;
    CharacterController controller;
    Rigidbody body;
    //TrailRenderer dashTrail;
    //float timeToNextDash = 0;
    //bool isDashing;
    float speed;
    Vector3 moveDir;
    //Vector3 dashDirection;

    //Combat
    [SerializeField] Material livingFormMaterial;
    [SerializeField] Material spiritFormMaterial;
    [SerializeField] float swordAttackRange;
    GameObject targetEnemy;
    Vector3 aimDirection;
    //[SerializeField] LayerMask enemyLayer;
    bool isAttacking;
    bool isKnockbackApplied;
    bool canAttack = true;
    List<Enemy> visibleEnemies = new List<Enemy>();
    int currentAttackNum = 1;
    Forms currentForm = Forms.Living;
    LayerMask livingLayer;
    LayerMask spiritLayer;

    public Forms Form { get { return currentForm; } set { currentForm = value; } }

    EffectBlessing currentSwordEffect = null;
    EffectBlessing currentBowEffect = null;
    StyleBlessing currentSwordStyle = new StyleBlessing();
    StyleBlessing currentBowStyle = null;
    public EffectBlessing SwordEffect { get { return currentSwordEffect; } set { currentSwordEffect = value; } }
    public EffectBlessing BowEffect { get { return currentBowEffect; } set { currentBowEffect = value; } }
    public StyleBlessing SwordStyle { get { return currentSwordStyle; } set { currentSwordStyle = value; } }
    public StyleBlessing BowStyle { get { return currentBowStyle; } set { currentBowStyle = value; } }

    public static PlayerController Instance { get; set; }

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
        body = GetComponent<Rigidbody>();
        //dashTrail = GetComponentInChildren<TrailRenderer>();

        //dashTrail.emitting = false;

        speed = defaultSpeed;

        livingLayer = LayerMask.NameToLayer("Living");
        spiritLayer = LayerMask.NameToLayer("Spirit");

        //enemyLayer = LayerMask.NameToLayer("Enemy");
    }

    // Update is called once per frame
    void Update()
    {
        //if (!isDashing)
        //{
        if (!isKnockbackApplied)
        {
            if (!isAttacking)
            {
                Move();

                SelectTargetEnemy();
            }

            if (InputManager.Instance.SwordAttack())
            {
                if (canAttack)
                {
                    if (targetEnemy == null)
                    {
                        StartCoroutine(MoveTowardsAttack(0.5f));
                    }
                    else if (Vector3.Distance(targetEnemy.transform.position, transform.position) > swordAttackRange )
                    {
                        StartCoroutine(MoveTowardsTargetEnemy(0.5f));
                    }
                    else
                    {
                        SwordAttack();
                        //StartCoroutine(MoveTowardsAttack(0.5f));
                    }
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

            //if (InputManager.Instance.Dash() && CanDash())
            //{
            //    StartCoroutine(Dash());
            //}
            //}
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
        moveDir = /*Quaternion.AngleAxis(45, Vector3.up) * */Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

        float aimAngle = Mathf.Atan2(aimDir.x, aimDir.y) * Mathf.Rad2Deg;/* + Camera.main.transform.eulerAngles.y;*/
        float mouseAimAngle = Mathf.Atan2(mouseAimDirection.x, mouseAimDirection.z) * Mathf.Rad2Deg;
        aimDirection = /*Quaternion.AngleAxis(45, Vector3.up) **/ Quaternion.Euler(0f, aimAngle, 0f) * Vector3.forward;

        if (controlWithMouse)
        {
            aimDirection = /*Quaternion.AngleAxis(45, Vector3.up) **/ Quaternion.Euler(0f, mouseAimAngle, 0f) * Vector3.forward;
            transform.rotation = Quaternion.Euler(0, mouseAimAngle/* + 45*/, 0);
        }
        else if (aimDir.magnitude <= 0.1f)
        {
            aimDirection = moveDir;
            transform.rotation = Quaternion.Euler(0f, targetAngle/* + 45*/, 0f);
        }
        else
        {
            aimDirection = /*Quaternion.AngleAxis(45, Vector3.up) * */Quaternion.Euler(0f, aimAngle, 0f) * Vector3.forward;
            transform.rotation = Quaternion.Euler(0f, aimAngle/* + 45*/, 0f);
        }

        //if (moveDir != Vector3.zero)
        //{
        //    dashDirection = moveDir;
        //}

        if (direction.magnitude <= 0.1f)
        {
            moveDir = new Vector3(0, 0, 0);

            body.velocity = new Vector3(0, 0, 0);
        }

        body.velocity = speed * moveDir;
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

    void SwordAttack()
    {
        if (canAttack)
        {
            SwordStyle.Attack(currentAttackNum);

            canAttack = false;
        }
    }

    //IEnumerator Dash()
    //{
    //    isDashing = true;

    //    foreach (MeshRenderer mesh in GetComponentsInChildren<MeshRenderer>())
    //    {
    //        mesh.enabled = false;
    //    }

    //    //gameObject.layer = invincibleLayer;

    //    ParticleManager.Instance.SpawnParticle(ParticleTypes.DashStart, transform.position);

    //    dashTrail.emitting = true;

    //    float timeElasped = 0;

    //    while (timeElasped < dashTimespan)
    //    {
    //        controller.Move(dashDirection * defaultSpeed * 5 * Time.deltaTime);

    //        timeElasped += Time.deltaTime;

    //        yield return null;
    //    }

    //    dashTrail.emitting = false;

    //    foreach (MeshRenderer mesh in GetComponentsInChildren<MeshRenderer>())
    //    {
    //        mesh.enabled = true;
    //    }

    //    //gameObject.layer = playerLayer;

    //    isDashing = false;

    //    ParticleManager.Instance.SpawnParticle(ParticleTypes.DashEnd, transform.position);

    //    yield return null;
    //}

    //bool CanDash()
    //{
    //    if (timeToNextDash <= Time.time)
    //    {
    //        timeToNextDash = Time.time + dashCooldown;

    //        return true;
    //    }

    //    return false;
    //}

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

    IEnumerator MoveTowardsTargetEnemy(float duration)
    {
        float elaspedTime = 0;
        isAttacking = true;

        StartCoroutine(CompleteAttackMovement(duration));

        Vector3 endVelo = Vector3.zero;
        Vector3 startVelo = speed * (targetEnemy.transform.position - transform.position).normalized;

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
        Vector3 startVelo = speed * aimDirection;

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

    public void SetCurrentAttackNum(int num)
    {
        currentAttackNum = num;
    }

    public void SetCanAttack(bool value)
    {
        canAttack = value;
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
        //if (collision.gameObject.CompareTag("Room"))
        //{
        //    collision.gameObject.GetComponentInParent<Room>().SetCurrentRoom(true);
        //}

        if (collision.gameObject.CompareTag("Exit"))
        {
            print("Changing Rooms");

            UIManager.Instance.GetFadePanel().BeginRoomTransition();

            StartCoroutine(EnterNewRoom(
                DungeonGenerator.Instance.GetCurrentRoom().GetConnectedRooms()[(int)collision.gameObject.GetComponent<RoomExit>().GetExitDirection()],
                DungeonGenerator.Instance.GetCurrentRoom().GetConnectedRooms()[(int)collision.gameObject.GetComponent<RoomExit>().GetExitDirection()].GetDoors()[(int)DungeonGenerator.Instance.ReverseDirection(collision.gameObject.GetComponent<RoomExit>().GetExitDirection())]
                .transform.parent.transform.position));
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

