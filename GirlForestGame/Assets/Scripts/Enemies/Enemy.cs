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
    bool donePreparing;

    Vector3 moveDirection;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        player = PlayerController.Instance;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        switch (currentState)
        {
            case EnemyStates.Moving:

                if (Vector3.Distance(transform.position, player.transform.position) < attackRange && CanAttack())
                {
                    print("Preparing Attack");

                    donePreparing = false;

                    Vector3 playerdirection = (player.transform.position - transform.position).normalized;

                    float targetAngle = Mathf.Atan2(playerdirection.x, playerdirection.z) * Mathf.Rad2Deg;

                    transform.rotation = Quaternion.Euler(0f, targetAngle, 0f);

                    StartCoroutine(AttackPreparationDelay());

                    GetComponent<Animator>().SetTrigger("Prepare");

                    currentState = EnemyStates.Preparing;
                }

                print("Moving");

                if (CanMove())
                {
                    if (Vector3.Distance(transform.position, player.transform.position) > attackRange)
                    {
                        print("Moving Towards Player");

                        moveDirection = (player.transform.position - transform.position).normalized;

                        moveDirection = Quaternion.AngleAxis(Random.Range(-60, 60), Vector3.up) * moveDirection;

                        float targetAngle = Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg;

                        transform.rotation = Quaternion.Euler(0f, targetAngle, 0f);

                        transform.DOBlendableMoveBy(moveDirection * 2, 1f);
                    }
                    else if (Vector3.Distance(transform.position, player.transform.position) <= attackRange)
                    {
                        print("Moving Away From Player");

                        moveDirection = (transform.position - player.transform.position).normalized;

                        moveDirection = Quaternion.AngleAxis(Random.Range(-60, 60), Vector3.up) * moveDirection;

                        float targetAngle = Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg;

                        transform.rotation = Quaternion.Euler(0f, targetAngle, 0f);

                        transform.DOBlendableMoveBy(moveDirection * 2, 1f);
                    }
                }

                break;

            case EnemyStates.Preparing:

                print("In Preparing State");

                if (donePreparing)
                {
                    currentState = EnemyStates.Attacking;
                }

                break;

            case EnemyStates.Attacking:

                print("Attacking Now");

                transform.DOBlendableMoveBy((player.transform.position - transform.position).normalized * attackRange, 0.5f);

                currentState = EnemyStates.Moving;

                break;
        }
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

