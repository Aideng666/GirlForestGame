using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StyleBlessing : Blessing
{
    [SerializeField] GameObject bowModel;
    [SerializeField] GameObject swordModel;

    // Start is called before the first frame update
    protected override void Start()
    {

    }

    protected override void Update()
    {

    }

    public virtual void Attack(int attackNum)
    {
        switch (attackNum)
        {
            case 1:

                PlayerController.Instance.GetComponentInChildren<Animator>().SetTrigger("Attack1");

                break;

            case 2:

                PlayerController.Instance.GetComponentInChildren<Animator>().SetTrigger("Attack2");

                break;
        }
    }

    public virtual void ActivateHitbox(int attackNum)
    {
        Collider[] enemyColliders = null;
        List<Enemy> enemiesHit = new List<Enemy>();

        switch (attackNum)
        {
            case 1:

                enemyColliders = Physics.OverlapSphere(PlayerController.Instance.transform.position + (PlayerController.Instance.transform.forward * 2), 2);

                if (enemyColliders.Length > 0)
                {
                    for (int i = 0; i < enemyColliders.Length; i++)
                    {
                        if (enemyColliders[i].gameObject.TryGetComponent<Enemy>(out Enemy enemy))
                        {
                            enemiesHit.Add(enemy);
                        }
                    }
                }

                for (int i = 0; i < enemiesHit.Count; i++)
                {
                    enemiesHit[i].ApplyKnockback(PlayerController.Instance.transform.forward, 2);
                }

                break;

            case 2:

                enemyColliders = Physics.OverlapSphere(PlayerController.Instance.transform.position + (PlayerController.Instance.transform.forward * 2), 2);

                if (enemyColliders.Length > 0)
                {
                    for (int i = 0; i < enemyColliders.Length; i++)
                    {
                        if (enemyColliders[i].gameObject.TryGetComponent<Enemy>(out Enemy enemy))
                        {
                            enemiesHit.Add(enemy);
                        }
                    }
                }

                for (int i = 0; i < enemiesHit.Count; i++)
                {
                    enemiesHit[i].ApplyKnockback(PlayerController.Instance.transform.forward, 2);
                }

                break;
        }
    }
}
