using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Winded : AI_BaseClass
{
    float duration;
    float elaspedTime;
    [HideInInspector] public FMOD.Studio.EventInstance breathSFX;

    private void OnEnable()
    {
        breathSFX = FMODUnity.RuntimeManager.CreateInstance("event:/Enemy/Boar/Recover");
    }
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        breathSFX.start();
        duration = PlayerController.Instance.playerMarkings.GetWindedDuration();
        elaspedTime = 0;

        agent.speed = 0;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);

        if (elaspedTime >= duration)
        {
            breathSFX.keyOff();
            animator.SetTrigger("Done_Winded");
        }

        elaspedTime += Time.deltaTime;
    }
}
