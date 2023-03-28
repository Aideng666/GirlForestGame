using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;
using DG.Tweening;

public class AI_MushroomOrbAttack : AI_BaseClass
{
    [SerializeField] float projectileForce = 1f;
    //Used in the "3 shots". The gaps between each shot
    [SerializeField] float timeBetweenShots = 0.3f;
    [SerializeField] GameObject orb;
    [SerializeField] float rotationSpeed = 0.4f;

    [HideInInspector] public FMOD.Studio.EventInstance Shot = FMODUnity.RuntimeManager.CreateInstance("event:/Enemy/Fungi/Shoot");

    //EnemyData mushroomData;
    Animator animator;
    List<CoroutineHandle> handles;
    
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        agent = animator.GetComponentInParent<UnityEngine.AI.NavMeshAgent>();
        //This is required in order to get the cube to look at the player as it fires the orbs off
        //If this is true, unity's AI system will take over and not allow a rotation
        //It is re-enabled on exit
        agent.updateRotation = false;
        animator.transform.parent.DOLookAt(PlayerController.Instance.transform.position, rotationSpeed);
        this.animator = animator;
        handles = new List<CoroutineHandle>();
        handles.Add(Timing.RunCoroutine(_waitUntilLooking()));

    }
    IEnumerator<float> _waitUntilLooking()
    {
        yield return Timing.WaitForSeconds(rotationSpeed * 1.02f);
        if (Random.Range(0, 2) == 0)
        {
            FanAttack();
            Shot.start();
        }
        else
        {
            handles.Add(Timing.RunCoroutine(_ThreeInARow()));
            Shot.start();
        }
    }

    void FanAttack() 
    {
        //This is gross but it's a quick and easy way to do it
        Instantiate(orb, animator.transform.position, animator.transform.rotation).GetComponent<Rigidbody>().AddRelativeForce(Vector3.forward * projectileForce);
        Instantiate(orb, animator.transform.position, animator.transform.rotation).GetComponent<Rigidbody>().AddRelativeForce(Vector3.Normalize(Vector3.forward + Vector3.right) * projectileForce);
        Instantiate(orb, animator.transform.position, animator.transform.rotation).GetComponent<Rigidbody>().AddRelativeForce(Vector3.Normalize(Vector3.forward + Vector3.left) * projectileForce);
        //This is used so that it can delay firing the shots enough to be looking at the player, as opposed to firing any which way
        animator.SetTrigger("Has_Fired");
    }

    IEnumerator<float> _ThreeInARow() 
    {
        for (int i = 0; i < 3; i++)
        {
            FireOrb();
            yield return Timing.WaitForSeconds(0.3f);
        }
        //This is used so that it can delay firing the shots enough to be looking at the player, as opposed to firing any which way
        animator.SetTrigger("Has_Fired");
    }

    void FireOrb() 
    {
        Instantiate(orb, animator.transform.position, animator.transform.rotation).GetComponent<Rigidbody>().AddRelativeForce(Vector3.forward * projectileForce);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        agent.updateRotation = true;
        foreach (CoroutineHandle h in handles)
        {
            Timing.KillCoroutines(h);
        }
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    //animator.SetBool("OrbAttackReady", mushroomData.canOrbAttack);
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
