using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class AI_MushroomGasAttack : AI_BaseClass
{
    //[SerializeField] GameObject gas;
    [SerializeField] float delayBeforeAttack = 1;
    float elaspedDelayTime;

    bool delayComplete;
    bool attackStarted;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        elaspedDelayTime = 0;

        delayComplete = false;
        attackStarted = false;
    }

   // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (elaspedDelayTime >= delayBeforeAttack)
        {
            delayComplete = true;
        }

        if (delayComplete && !attackStarted)
        {
            //GameObject gasCloud = Instantiate(gas, animator.transform.position, Quaternion.identity);
            ParticleManager.Instance.SpawnParticle(ParticleTypes.GasCloud, agent.transform.position);

            animator.SetTrigger("Has_Fired");

            attackStarted = true;
        }

        elaspedDelayTime += Time.deltaTime;
    }
}
