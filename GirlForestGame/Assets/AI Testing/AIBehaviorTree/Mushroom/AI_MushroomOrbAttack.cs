using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;

public class AI_MushroomOrbAttack : StateMachineBehaviour
{
    [SerializeField] float projectileForce = 1f;
    //Used in the "3 shots". The gaps between each shot
    [SerializeField] float timeBetweenShots = 0.3f;
    [SerializeField] GameObject orb;
    AI_MushroomData mushroomData;
    Animator animator;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        mushroomData = animator.GetComponentInParent<AI_MushroomData>();
        mushroomData.startTimer(1);
        this.animator = animator;

        if (Random.Range(0, 1) == 0)
        {
            FanAttack();
        }
        else 
        {
            Timing.RunCoroutine(_ThreeInARow());
        }

    }

    void FanAttack() 
    {
        List<GameObject> orbRef = new List<GameObject>();
        for (int i = 0; i < 3; i++)
        {
            //TODO: REMOVE INSTANTIATE AND USE OBJECT POOLING
            orbRef.Add(Instantiate(orb, animator.transform.position, Quaternion.identity));
            //Add force to projectile
        }
        orbRef[0].GetComponent<Rigidbody>().AddRelativeForce(Vector3.forward * projectileForce);
        orbRef[1].GetComponent<Rigidbody>().AddRelativeForce(Vector3.Normalize(Vector3.forward + Vector3.right) * projectileForce);
        orbRef[2].GetComponent<Rigidbody>().AddRelativeForce(Vector3.Normalize(Vector3.forward + Vector3.left) * projectileForce);
    }

    IEnumerator<float> _ThreeInARow() 
    {
        for (int i = 0; i < 3; i++)
        {
            FireOrb();
            yield return Timing.WaitForSeconds(0.3f);
        }
    }

    void FireOrb() 
    {
        Instantiate(orb, animator.transform.position, Quaternion.identity).GetComponent<Rigidbody>().AddRelativeForce(Vector3.forward * projectileForce);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateMove is called right after Animator.OnAnimatorMove()
    override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("OrbAttackReady", mushroomData.canOrbAttack);
    }

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
