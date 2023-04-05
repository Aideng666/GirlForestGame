using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageNumber : MonoBehaviour
{
    [SerializeField] float duration;

    float elaspedTime;

    private void OnEnable()
    {
        elaspedTime = 0;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (elaspedTime >= duration) 
        {
            EnemyPool.Instance.AddNumberToPool(gameObject);
        }

        elaspedTime += Time.deltaTime;
    }
}
