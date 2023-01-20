using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Distance_Check", menuName = "Conditions/Distance")]
/// <summary>
/// Is a condition that will compare if the AI is within the range (min and max) of the player and will change the parameter accordingly
/// </summary>
public class Cdt_Distance : Cdt_BaseClass
{

    [Range(0, 50)]
    public float minRange = 1f;
    [Range(-1, 51)]
    public float maxRange = -1f;
    public string Condition_Parameter = "Close_To_Player";
    public override void CheckCondition(Animator animator, AI_BaseClass enemy = null)
    {
        float dist = Vector3.Distance(animator.transform.position, PlayerController.Instance.transform.position);
        //Checks the distance and will set the bool to true if in range, otherwise false
        animator.SetBool(Condition_Parameter, (dist > minRange) && (dist < maxRange));
    }
}