using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MagicProjectileEnemy : Enemy
{
    [SerializeField] GameObject spiritProjectilePrefab;
    [SerializeField] GameObject livingProjectilePrefab;
    [SerializeField] float projectileMoveSpeed;
    [SerializeField] float projectileMaxSize;

    GameObject spawnedProjectile;
    int selectedProjectile; // 0 = Living | 1 = Spirit
    bool projectileSpawned;

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();

        if (!isKnockbackApplied)
        {
            switch (currentState)
            {
                case EnemyStates.Moving:

                    if (Vector3.Distance(transform.position, player.transform.position) < attackRange && CanAttack())
                    {
                        donePreparing = false;

                        StartCoroutine(AttackPreparationDelay());

                        selectedProjectile = Random.Range(0, 2);

                        currentState = EnemyStates.Preparing;
                    }

                    if (CanMove())
                    {
                        float playerDistance = Vector3.Distance(transform.position, player.transform.position);
                        float anglePercentage;

                        if (playerDistance > attackRange)
                        {
                            if (playerDistance <= 20)
                            {
                                anglePercentage = Mathf.InverseLerp(attackRange, 20, playerDistance);
                            }
                            else
                            {
                                anglePercentage = 1;
                            }

                            moveDirection = (player.transform.position - transform.position).normalized;
                            moveDirection.y = 0;
                            moveDirection = moveDirection.normalized;
                            moveDirection = Quaternion.AngleAxis(Random.Range(-Mathf.Lerp(90, 0.01f, anglePercentage), Mathf.Lerp(90, 0.01f, anglePercentage)), Vector3.up) * moveDirection;
                        }
                        else if (playerDistance <= attackRange)
                        {
                            anglePercentage = Mathf.InverseLerp(0, attackRange, playerDistance);

                            moveDirection = (transform.position - player.transform.position).normalized;
                            moveDirection.y = 0;
                            moveDirection = moveDirection.normalized;
                            moveDirection = Quaternion.AngleAxis(Random.Range(-Mathf.Lerp(90, 0.01f, anglePercentage), Mathf.Lerp(90, 0.01f, anglePercentage)), Vector3.up) * moveDirection;
                        }

                        float targetAngle = Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg;

                        transform.rotation = Quaternion.Euler(0f, targetAngle, 0f);

                        body.velocity = moveDirection * moveSpeed;

                        StartCoroutine(StopMovement(0.5f));
                    }

                    break;

                case EnemyStates.Preparing:

                    if (donePreparing)
                    {
                        currentState = EnemyStates.Attacking;
                    }
                    else if (!projectileSpawned)
                    {
                        if (selectedProjectile == 1)
                        {
                            spawnedProjectile = Instantiate(spiritProjectilePrefab, transform.position + transform.forward, Quaternion.identity);
                            spawnedProjectile.transform.localScale = Vector3.zero;

                            //spawnedProjectile.transform.DOScale(projectileMaxSize, attackPreparationTime);
                        }
                        else
                        {
                            spawnedProjectile = Instantiate(livingProjectilePrefab, transform.position + transform.forward + Vector3.down, Quaternion.identity);

                            //spawnedProjectile.transform.DOScale(projectileMaxSize, attackPreparationTime);
                            //spawnedProjectile.transform.DOMoveY(transform.position.y, attackPreparationTime);
                        }
                        projectileSpawned = true;
                    }
                    else
                    {
                        playerDirection = (player.transform.position - transform.position).normalized;

                        playerDirection.y = 0;

                        playerDirection = playerDirection.normalized;

                        float targetAngle = Mathf.Atan2(playerDirection.x, playerDirection.z) * Mathf.Rad2Deg;

                        transform.rotation = Quaternion.Euler(0f, targetAngle, 0f);

                        if (selectedProjectile == 1)
                        {
                            spawnedProjectile.transform.position = transform.position + transform.forward;
                        }
                        else
                        {
                            spawnedProjectile.transform.position = new Vector3(transform.position.x + transform.forward.x, spawnedProjectile.transform.position.y, transform.position.z + transform.forward.z);
                        }
                    }

                    break;

                case EnemyStates.Attacking:

                    spawnedProjectile.GetComponent<Rigidbody>().velocity = playerDirection * projectileMoveSpeed;

                    if (selectedProjectile == 1)
                    {
                        spawnedProjectile.GetComponent<SpiritProjectile>().SetActive(true);

                        //spawnedProjectile.transform.DOPunchScale(new Vector3(-0.5f, -0.5f, -0.5f), 0.25f);
                    }
                    else
                    {
                        spawnedProjectile.GetComponent<LivingProjectile>().SetActive(true);

                        //spawnedProjectile.transform.DOPunchScale(new Vector3(0.5f, 0.5f, 0.5f), 0.25f);
                    }

                    //transform.DOPunchScale(new Vector3(0, 0, -0.5f), 0.05f);

                    projectileSpawned = false;

                    currentState = EnemyStates.Moving;

                    break;
            }
        }
    }

    protected override void CancelPrep()
    {
        Destroy(spawnedProjectile);

        projectileSpawned = false;

        currentState = EnemyStates.Moving;
    }
}
