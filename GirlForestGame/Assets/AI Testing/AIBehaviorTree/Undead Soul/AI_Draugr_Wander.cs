using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Draugr_Wander : AI_BaseClass
{
    [SerializeField] Vector2 moveTimerRange;
    [SerializeField] Vector2 teleportDistanceRange;

    float elaspedTime;
    float moveTimer;
    float teleportDistance;

    Ray moveRay;

    [HideInInspector] public FMOD.Studio.EventInstance blinkSFX;

    private void OnEnable()
    {
        blinkSFX = FMODUnity.RuntimeManager.CreateInstance("event:/Enemy/Draugr/Blink");

    }
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        agent.speed = 0;
        elaspedTime = 0;
        moveTimer = Random.Range(moveTimerRange.x, moveTimerRange.y);
        teleportDistance = Random.Range(teleportDistanceRange.x, teleportDistanceRange.y);
        agent.updateRotation = false;

        moveRay = new Ray(agent.transform.position, (player.transform.position - agent.transform.position).normalized);
        moveRay.direction = Quaternion.Euler(0, Random.Range(-75f, 75f), 0) * moveRay.direction;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);

        animator.GetComponentInParent<EnemyData>().RunCooldownTimer();

        if (elaspedTime >= moveTimer)
        {
            if (Physics.Raycast(moveRay, teleportDistance))
            {
                moveRay.direction = Quaternion.Euler(0, 5, 0) * moveRay.direction;
            }
            else
            {
                Teleport();

                elaspedTime = 0;
                moveTimer = Random.Range(moveTimerRange.x, moveTimerRange.y);
                teleportDistance = Random.Range(teleportDistanceRange.x, teleportDistanceRange.y);

                moveRay = new Ray(agent.transform.position, (player.transform.position - agent.transform.position).normalized);
                moveRay.direction = Quaternion.Euler(0, Random.Range(-75f, 75f), 0) * moveRay.direction;
            }
        }

        agent.transform.LookAt(player.transform.position);
        elaspedTime += Time.deltaTime;
    }

    void Teleport()
    {
        ParticleManager.Instance.SpawnParticle(ParticleTypes.Teleport, agent.transform.position);
        blinkSFX.start();
        agent.transform.position += moveRay.direction * teleportDistance;
    }
}
