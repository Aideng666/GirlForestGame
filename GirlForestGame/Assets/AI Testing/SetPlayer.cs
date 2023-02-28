using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetPlayer : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] GameObject enemyToSpawn;
    void Start()
    {
        InputManager.Instance.SwapActionMap("Player");
        //Instantiate(enemyToSpawn, Vector3.up, Quaternion.identity);
    }

}
