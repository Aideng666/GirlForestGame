using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AI_Mushroom_Idle : AI_BaseClass
{
    [SerializeField] float activationDistance = 4;
    [HideInInspector] public FMOD.Studio.EventInstance Spook;

    //private void OnEnable()
    //{
    //    Spook = FMODUnity.RuntimeManager.CreateInstance("event:/Enemy/Fungi/Spook");
    //}

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

        if (Vector3.Distance(animator.transform.position, player.transform.position) < activationDistance)
        {
            FMODUnity.RuntimeManager.PlayOneShotAttached("vent:/Enemy/Fungi/Spook", animator.gameObject);

            animator.SetTrigger("Awaken_From_Idle");
            
            if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("Tutorial"))
            {
                TutorialManager.Instance.TriggerTutorialSection(2, true);
            }
        }
    }
}
