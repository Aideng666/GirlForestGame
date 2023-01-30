using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_MushroomData : EnemyData
{
    [SerializeField] float gasTime = 1f;
    [SerializeField] float orbTime = 1f;

    [HideInInspector] public bool canGasAttack = false;
    [HideInInspector] public bool canOrbAttack = false;


    //The timer I set up for the boar broke so I am making another instead of trying to break my head fixing
    public void startTimer(int x) 
    {
        if (x == 0) 
        {
            canGasAttack = false;
            Invoke("GasTrue", gasTime);
        }
        else if (x == 1)
        {
            canOrbAttack = false;
            Invoke("OrbTrue", orbTime);
        }
    }

    void GasTrue() 
    {
        canGasAttack = true;
    }

    void OrbTrue() 
    {
        canOrbAttack = true;
        Debug.Log("doing ur mom");
    }
}
