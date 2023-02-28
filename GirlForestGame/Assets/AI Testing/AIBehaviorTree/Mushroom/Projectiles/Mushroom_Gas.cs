using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mushroom_Gas : MonoBehaviour
{
    //[SerializeField] int collisionLayer;
    [SerializeField] float tickDamageFrequency = 0.5f; //In Seconds
    [SerializeField] float gasLife = 5f; //In seconds
    private float clock = 0f;
    private float currLife = 0f;

    private void Update()
    {
        clock += Time.deltaTime;
        currLife += Time.deltaTime;
        if(currLife >= gasLife) 
        {
            //TODO: Object Pooling
            Destroy(this.gameObject);
        }
    }
    private void OnTriggerStay(Collider other)
    {
        //TODO: update this with the correct layer
        if (other.gameObject.layer == this.gameObject.layer - 2) //This is attorcious, but with the current layout it will work. 
        {
            if (clock >= tickDamageFrequency)
            {
                //Damage

                //Reset the clock
                clock = 0f;
                //TODO: Don't destroy, use object pool
                //Destroy(this.gameObject);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        clock = 0;
    }
}
