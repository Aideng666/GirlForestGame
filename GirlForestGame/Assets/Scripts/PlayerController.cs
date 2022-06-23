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
    int enemyLayer;
    bool isAttacking;

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

        enemyLayer = LayerMask.NameToLayer("Enemy");
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

                    StartCoroutine(CompleteAttackMovement(0.5f));
                }
                CombatManager.Instance.Attack();
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
        RaycastHit info;

        if (Physics.SphereCast(transform.position, 3f, new Vector3(aimDirection.x, 0, aimDirection.y), out info, 10))
        {
            targetEnemy = info.collider.gameObject;
        }
        else
        {
            if (targetEnemy == null)
            {
                targetEnemy = FindObjectOfType<Enemy>().gameObject;
            }
            //Get the list of enemies to loop through here
            foreach (Enemy enemy in FindObjectsOfType<Enemy>())
            {
                if (Vector3.Distance(enemy.transform.position, transform.position + new Vector3(aimDirection.x, 0, aimDirection.y) * 10)
                  < Vector3.Distance(targetEnemy.transform.position, transform.position + new Vector3(aimDirection.x, 0, aimDirection.y) * 10))
                {
                    targetEnemy = enemy.gameObject;
                }
            }
        }
    }

    void MoveTowardsTargetEnemy(float duration)
    {
        isAttacking = true;

        transform.DOLookAt(targetEnemy.transform.position, 0.2f);
        transform.DOMove(targetEnemy.transform.position + ((transform.position - targetEnemy.transform.position).normalized) * 4, duration);
    }

    IEnumerator CompleteAttackMovement(float duration)
    {
        yield return new WaitForSeconds(duration);

        isAttacking = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawRay(transform.position, new Vector3(aimDirection.x, 0, aimDirection.y) * 5);

        Gizmos.DrawSphere(transform.position + new Vector3(aimDirection.x, 0, aimDirection.y) * 10, 0.25f);

        if (targetEnemy != null)
        {
            Gizmos.DrawWireSphere(targetEnemy.transform.position, 2);
        }
    }
}
