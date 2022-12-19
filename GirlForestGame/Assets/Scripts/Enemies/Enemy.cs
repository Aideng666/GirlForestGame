using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] protected Forms form;
    [SerializeField] protected float health;
    [SerializeField] protected float damage;
    [SerializeField] protected float moveSpeed;
    [SerializeField] protected float moveDelay = 1.5f;
    [SerializeField] protected float attackDelay;
    [SerializeField] protected float attackRange;
    [SerializeField] protected float attackPreparationTime = 1.5f;

    public Forms Form { get { return form; }}
    public float AttackPrepTime { get { return attackPreparationTime; }/* set { attackPreparationTime = value; } */}

    protected PlayerController player;
    protected Rigidbody body;

    protected EnemyStates currentState = EnemyStates.Moving;

    protected float timeToNextMove = 0;
    protected float timeToNextAttack = 0;
    protected float yHeight;
    protected bool donePreparing;
    protected bool isKnockbackApplied;

    protected Vector3 moveDirection;
    protected Vector3 playerDirection;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        player = PlayerController.Instance;
        body = GetComponent<Rigidbody>();

        yHeight = transform.position.y;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        //switch (currentState)
        //{
        //    case EnemyStates.Moving:

        //        if (Vector3.Distance(transform.position, player.transform.position) < attackRange && CanAttack())
        //        {
        //            donePreparing = false;

        //            playerDirection = (player.transform.position - transform.position).normalized;

        //            playerDirection.y = 0;

        //            playerDirection = playerDirection.normalized;

        //            float targetAngle = Mathf.Atan2(playerDirection.x, playerDirection.z) * Mathf.Rad2Deg;

        //            transform.rotation = Quaternion.Euler(0f, targetAngle, 0f);

        //            StartCoroutine(AttackPreparationDelay());

        //            GetComponent<Animator>().SetTrigger("Prepare");

        //            currentState = EnemyStates.Preparing;
        //        }

        //        if (CanMove())
        //        {
        //            if (Vector3.Distance(transform.position, player.transform.position) > attackRange)
        //            {
        //                moveDirection = (player.transform.position - transform.position).normalized;

        //                moveDirection.y = 0;

        //                moveDirection = moveDirection.normalized;

        //                moveDirection = Quaternion.AngleAxis(Random.Range(-60, 60), Vector3.up) * moveDirection;


        //                float targetAngle = Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg;

        //                transform.rotation = Quaternion.Euler(0f, targetAngle, 0f);

        //                transform.DOBlendableMoveBy(moveDirection * 2, 1f);
        //            }
        //            else if (Vector3.Distance(transform.position, player.transform.position) <= attackRange)
        //            {
        //                moveDirection = (transform.position - player.transform.position).normalized;

        //                moveDirection.y = 0;

        //                moveDirection = moveDirection.normalized;

        //                moveDirection = Quaternion.AngleAxis(Random.Range(-60, 60), Vector3.up) * moveDirection;

        //                float targetAngle = Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg;

        //                transform.rotation = Quaternion.Euler(0f, targetAngle, 0f);

        //                transform.DOBlendableMoveBy(moveDirection * 2, 1f);
        //            }
        //        }

        //        break;

        //    case EnemyStates.Preparing:

        //        if (donePreparing)
        //        {
        //            currentState = EnemyStates.Attacking;
        //        }

        //        break;

        //    case EnemyStates.Attacking:

        //        transform.DOBlendableMoveBy(playerDirection * attackRange, 0.5f);

        //        currentState = EnemyStates.Moving;

        //        break;
        //}

        if (health <= 0)
        {
            Die();
        }
    }

    public void ApplyKnockback(Vector3 direction, float power)
    {
        if (currentState == EnemyStates.Preparing)
        {
            CancelPrep();
        }

        direction.y = 0;

        direction = direction.normalized;

        isKnockbackApplied = true;

        body.velocity = direction * power;

        StartCoroutine(DeactivateKnockback());
    }

    public void TakeDamage(float value)
    {
        health -= value;

        transform.DOPunchScale(new Vector3(-0.5f, -0.5f, -0.5f), 0.1f);
    }

    void Die()
    {
        Destroy(gameObject);
    }

    protected virtual void CancelPrep()
    {

    }

    IEnumerator DeactivateKnockback()
    {
        Vector3 startVel = body.velocity;
        float elaspedTime = 0;

        while (elaspedTime <= 1)
        {
            body.velocity = Vector3.Lerp(startVel, Vector3.zero, elaspedTime / 1);

            Vector3 direction = -body.velocity;

            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;

            transform.rotation = Quaternion.Euler(0f, targetAngle, 0f);

            elaspedTime += Time.deltaTime;

            yield return null;
        }

        isKnockbackApplied = false;

        yield return null;
    }

    protected bool CanMove()
    {
        if (timeToNextMove <= Time.time)
        {
            timeToNextMove = Time.time + moveDelay;

            return true;
        }

        return false;
    }

    protected bool CanAttack()
    {
        if (timeToNextAttack <= Time.time)
        {
            timeToNextAttack = Time.time + attackDelay;

            return true;
        }

        return false;
    }

    protected IEnumerator AttackPreparationDelay()
    {
        yield return new WaitForSeconds(attackPreparationTime);

        donePreparing = true;
    }

    protected IEnumerator StopMovement(float duration)
    {
        yield return new WaitForSeconds(duration);

        body.velocity = Vector3.zero;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawRay(transform.position, moveDirection);
    }
}

public enum EnemyStates
{
    Idle,
    Moving,
    Preparing,
    Attacking,
}

