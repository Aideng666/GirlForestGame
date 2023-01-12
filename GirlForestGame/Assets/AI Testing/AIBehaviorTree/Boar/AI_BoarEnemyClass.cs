using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

/// <summary>
/// Is childed to enemy data which has a bunch of basic things like HP and attack strength. 
/// This contains all the specific things and handles the throwing. It's using the bezier curve addon since it's a lot easier to do something with that than DOTween
/// </summary>
public class AI_BoarEnemyClass : EnemyData
{
    /*
     DESCRIPTION: Is childed to enemy data which has a bunch of basic things like HP and attack strength. 
     This contains all the specific things and handles the throwing. It's using the bezier curve addon since it's a lot easier to do something with that than DOTween
     */
    //[SerializeField] float strength = 2f;
    [SerializeField] GameObject projectile;
    [SerializeField] BezierCurve curve;
    [SerializeField] float throwDuration = 3f;

    public void ThrowProjectile(AI_Boar_ThrowAttack throwAttack) 
    {
        StartCoroutine(projectileAnimation(throwAttack));
    }
    //Using the bezier curve created it will move the projectile to the next point along the curve
    IEnumerator projectileAnimation(AI_Boar_ThrowAttack throwAttack) 
    {
        for (float time = 0; time < throwDuration; time += Time.deltaTime)
        {
            projectile.transform.position = curve.GetPointAt(time / throwDuration);
            //transform.localRotation = Quaternion.Euler(0f, 360f * time / duration, 0f); //Spinning
            yield return new WaitForEndOfFrame();
        }
        //throwAttack.setProjectileStatus(true); //The projectile has returned and can now change states back to tracking if the player has moved too far away
    }
}
