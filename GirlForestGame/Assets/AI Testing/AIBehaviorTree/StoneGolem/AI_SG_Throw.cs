using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class AI_SG_Throw : AI_BaseClass
{
    Transform projectile;
    Vector3 defaultLocation;
    [SerializeField] float  heightOfShot = 1f;
    [SerializeField] float duration = 1f;
    [SerializeField] string stateChangeTrigger = "Projectile_Complete";
    [SerializeField] GameObject target;
    //[SerializeField] GameObject aoeTrigger;
    //Vector3 direction;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        GameObject temp = Instantiate(target, player.transform.position, Quaternion.identity);
        //Getting the projectile child 
        projectile = animator.transform.GetChild(0);
        defaultLocation = projectile.position;
        //direction =  PlayerController.Instance.transform.position;
        projectile.DOJump(PlayerController.Instance.transform.position, heightOfShot, 1, duration).OnComplete(
            () => { animator.SetTrigger(stateChangeTrigger); projectile.position = defaultLocation; Destroy(temp); /*aoeTrigger.GetComponent<SphereCollider>().enabled = true;*/ });
    }


    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);
    }
}
