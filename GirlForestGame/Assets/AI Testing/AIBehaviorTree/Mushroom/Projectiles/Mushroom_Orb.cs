using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mushroom_Orb : MonoBehaviour
{
    [SerializeField] int collisionLayer;
    private void OnTriggerEnter(Collider collision)
    {
        //TODO: update this with the correct layer
        if (collision.gameObject.layer == this.gameObject.layer - 2) //This is attorcious, but with the current layout it will work. 
        {
            //Damage

            //TODO: Don't destroy, use object pool
            Destroy(this.gameObject);
        }
        else if (collision.gameObject.CompareTag("Environment")) 
        {
            //TODO: Don't destroy, use object pool
            Destroy(this.gameObject);
        }

    }

}
