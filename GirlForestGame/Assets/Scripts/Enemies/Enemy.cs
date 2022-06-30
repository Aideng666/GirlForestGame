using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] float health;
    [SerializeField] float damage;
    [SerializeField] float moveSpeed;
    [SerializeField] float attackDelay;
    [SerializeField] float attackRange;

    PlayerController player;

    EnemyStates currentState = EnemyStates.Moving;

    float timeToNextMove = 0;
    float timeToNextAttack = 0;
    float moveDelay = 1.5f;
    float attackPreparationTime = 1.5f;
    float yHeight;
    bool donePreparing;

    Vector3 moveDirection;
    Vector3 playerDirection;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        player = PlayerController.Instance;

        yHeight = transform.position.y;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        switch (currentState)
        {
            case EnemyStates.Moving:

                if (Vector3.Distance(transform.position, player.transform.position) < attackRange && CanAttack())
                {
                    donePreparing = false;

                    playerDirection = (player.transform.position - transform.position).normalized;

                    playerDirection.y = 0;

                    playerDirection = playerDirection.normalized;

                    float targetAngle = Mathf.Atan2(playerDirection.x, playerDirection.z) * Mathf.Rad2Deg;

                    transform.rotation = Quaternion.Euler(0f, targetAngle, 0f);

                    StartCoroutine(AttackPreparationDelay());

                    GetComponent<Animator>().SetTrigger("Prepare");

                    currentState = EnemyStates.Preparing;
                }

                if (CanMove())
                {
                    if (Vector3.Distance(transform.position, player.transform.position) > attackRange)
                    {
                        moveDirection = (player.transform.position - transform.position).normalized;

                        moveDirection.y = 0;

                        moveDirection = moveDirection.normalized;

                        moveDirection = Quaternion.AngleAxis(Random.Range(-60, 60), Vector3.up) * moveDirection;


                        float targetAngle = Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg;

                        transform.rotation = Quaternion.Euler(0f, targetAngle, 0f);

                        transform.DOBlendableMoveBy(moveDirection * 2, 1f);
                    }
                    else if (Vector3.Distance(transform.position, player.transform.position) <= attackRange)
                    {
                        moveDirection = (transform.position - player.transform.position).normalized;

                        moveDirection.y = 0;

                        moveDirection = moveDirection.normalized;

                        moveDirection = Quaternion.AngleAxis(Random.Range(-60, 60), Vector3.up) * moveDirection;

                        float targetAngle = Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg;

                        transform.rotation = Quaternion.Euler(0f, targetAngle, 0f);

                        transform.DOBlendableMoveBy(moveDirection * 2, 1f);
                    }
                }

                break;

            case EnemyStates.Preparing:

                if (donePreparing)
                {
                    currentState = EnemyStates.Attacking;
                }

                break;

            case EnemyStates.Attacking:

                transform.DOBlendableMoveBy(playerDirection * attackRange, 0.5f);

                currentState = EnemyStates.Moving;

                break;
        }
    }

    public void ApplyKnockback(Vector3 direction, float power)
    {
        direction.y = 0;

        direction = direction.normalized;

        transform.DOBlendableMoveBy(direction * power, 0.5f);
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

    IEnumerator AttackPreparationDelay()
    {
        yield return new WaitForSeconds(attackPreparationTime);

        donePreparing = true;
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

