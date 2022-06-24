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
    [SerializeField] float dashTimespan;
    [SerializeField] float dashCooldown = 2;
    CharacterController controller;
    TrailRenderer dashTrail;
    float timeToNextDash = 0;
    bool isDashing;
    float speed;
    Vector3 moveDir;
    Vector3 dashDirection;

    //Combat
    [SerializeField] float swordAttackRange;
    GameObject targetEnemy;
    Vector2 aimDirection;
    //[SerializeField] LayerMask enemyLayer;
    bool isAttacking;
    List<Enemy> visibleEnemies = new List<Enemy>();

    EffectBlessing currentSwordEffect = null;
    EffectBlessing currentBowEffect = null;
    StyleBlessing currentSwordStyle = null;
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
        dashTrail = GetComponentInChildren<TrailRenderer>();

        dashTrail.emitting = false;

        speed = defaultSpeed;

        //enemyLayer = LayerMask.NameToLayer("Enemy");
    }

    // Update is called once per frame
    void Update()
    {
        if (!isDashing)
        {
            if (!isAttacking)
            {
                Move();

                SelectTargetEnemy();
            }

            if (InputManager.Instance.SwordAttack())
            {
                if (Vector3.Distance(targetEnemy.transform.position, transform.position) > swordAttackRange)
                {
                    MoveTowardsTargetEnemy(0.5f);
                }
                else
                {
                    CombatManager.Instance.Attack();
                }
            }

            if (InputManager.Instance.Dash() && CanDash())
            {
                StartCoroutine(Dash());
            }
        }
    }

    void Move()
    {
        Vector2 direction = InputManager.Instance.Move();
        aimDirection = InputManager.Instance.Aim();

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
        moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

        float aimAngle = Mathf.Atan2(aimDirection.x, aimDirection.y) * Mathf.Rad2Deg;/* + Camera.main.transform.eulerAngles.y;*/
        float mouseAimAngle = Mathf.Atan2(mouseAimDirection.x, mouseAimDirection.z) * Mathf.Rad2Deg;

        if (controlWithMouse)
        {
            transform.rotation = Quaternion.Euler(0, mouseAimAngle, 0);
        }
        else if (aimDirection.magnitude <= 0.1f)
        {
            transform.rotation = Quaternion.Euler(0f, targetAngle, 0f);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, aimAngle, 0);
        }

        if (moveDir != Vector3.zero)
        {
            dashDirection = moveDir;
        }

        if (direction.magnitude <= 0.1f)
        {
            moveDir = new Vector3(0, 0, 0);
        }

        controller.Move(moveDir * speed * Time.deltaTime);

        transform.position = new Vector3(transform.position.x, 1, transform.position.z);
    }

    IEnumerator Dash()
    {
        isDashing = true;

        foreach (MeshRenderer mesh in GetComponentsInChildren<MeshRenderer>())
        {
            mesh.enabled = false;
        }

        //gameObject.layer = invincibleLayer;

        ParticleManager.Instance.SpawnParticle(ParticleTypes.DashStart, transform.position);

        dashTrail.emitting = true;

        float timeElasped = 0;

        while (timeElasped < dashTimespan)
        {
            controller.Move(dashDirection * defaultSpeed * 5 * Time.deltaTime);

            timeElasped += Time.deltaTime;

            yield return null;
        }

        dashTrail.emitting = false;

        foreach (MeshRenderer mesh in GetComponentsInChildren<MeshRenderer>())
        {
            mesh.enabled = true;
        }

        //gameObject.layer = playerLayer;

        isDashing = false;

        ParticleManager.Instance.SpawnParticle(ParticleTypes.DashEnd, transform.position);

        yield return null;
    }

    bool CanDash()
    {
        if (timeToNextDash <= Time.time)
        {
            timeToNextDash = Time.time + dashCooldown;

            return true;
        }

        return false;
    }

    void SelectTargetEnemy()
    {
        Collider[] collidersDetected = new Collider[0];
        Collider[] collidersDetected2 = new Collider[0];
        Collider[] collidersDetected3 = new Collider[0];

        if (aimDirection.magnitude > 0)
        {
            collidersDetected = Physics.OverlapSphere(transform.position + (new Vector3(aimDirection.x, 0, aimDirection.y).normalized * 1), 1f);
            collidersDetected2 = Physics.OverlapSphere(transform.position + (new Vector3(aimDirection.x, 0, aimDirection.y).normalized * 3), 2f);
            collidersDetected3 = Physics.OverlapSphere(transform.position + (new Vector3(aimDirection.x, 0, aimDirection.y).normalized * 5), 3f);
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

    void MoveTowardsTargetEnemy(float duration)
    {
        isAttacking = true;

        StartCoroutine(CompleteAttackMovement(duration));

        transform.DOLookAt(targetEnemy.transform.position, 0.2f);
        transform.DOMove(targetEnemy.transform.position + ((transform.position - targetEnemy.transform.position).normalized) * 2/*Change to enemy size offset*/, duration);
    }

    IEnumerator CompleteAttackMovement(float duration)
    {
        yield return new WaitForSeconds(duration * 0.25f);

        CombatManager.Instance.Attack();

        yield return new WaitForSeconds(duration * 0.75f);

        isAttacking = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(transform.position + (new Vector3(aimDirection.x, 0, aimDirection.y).normalized * 5), 3f);
        Gizmos.DrawWireSphere(transform.position + (new Vector3(aimDirection.x, 0, aimDirection.y).normalized * 3), 2f);
        Gizmos.DrawWireSphere(transform.position + (new Vector3(aimDirection.x, 0, aimDirection.y).normalized * 1), 1f);

        Gizmos.color = Color.green;

        if (targetEnemy != null)
        {
            Gizmos.DrawWireSphere(targetEnemy.transform.position, 2);
        }
    }
}
