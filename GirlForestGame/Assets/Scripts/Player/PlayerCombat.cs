using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [SerializeField] GameObject arrowPrefab;
    [SerializeField] GameObject bowAimCanvas;
    [SerializeField] Material livingFormMaterial;
    [SerializeField] Material spiritFormMaterial;

    List<Enemy> visibleEnemies = new List<Enemy>();
    GameObject targetEnemy;

    public bool isAttacking { get; private set; }
    public bool isKnockbackApplied { get; private set; }

    bool canAttack = true;

    //Bow Stuff
    bool bowDrawn;
    bool bowCharging;
    bool isDrawingBow;
    bool quickfirePerformed;
    float currentBowChargeTime = 0;

    //Sword Stuff
    int currentAttackNum = 1;

    Forms currentForm = Forms.Living;
    LayerMask livingLayer;
    LayerMask spiritLayer;

    public Forms Form { get { return currentForm; } set { currentForm = value; } }

    PlayerController player;
    Rigidbody body;

    // Start is called before the first frame update
    void Start()
    {
        livingLayer = LayerMask.NameToLayer("Living");
        spiritLayer = LayerMask.NameToLayer("Spirit");

        bowAimCanvas.SetActive(false);

        player = GetComponent<PlayerController>();
        body = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        //Detects the release of the arrow once the bow is completely drawn back
        if (bowDrawn)
        {
            bowAimCanvas.SetActive(true);

            if (body.velocity == Vector3.zero)
            {
                bowCharging = true;
            }
            else
            {
                bowCharging = false;

                currentBowChargeTime = 0;
            }

            if (bowCharging)
            {
                if (currentBowChargeTime < player.playerAttributes.BowChargeTime)
                {
                    currentBowChargeTime += Time.deltaTime;

                    currentBowChargeTime = Mathf.Clamp(currentBowChargeTime, 0, player.playerAttributes.BowChargeTime);
                }
            }

            if (InputManager.Instance.ReleaseArrow())
            {
                GetComponentInChildren<Animator>().SetTrigger("ReleaseArrow");

                bowDrawn = false;

                bowAimCanvas.SetActive(false);

                if (quickfirePerformed)
                {
                    SpawnArrow(targetEnemy);

                    quickfirePerformed = false;
                }
                else
                {
                    SpawnArrow();

                    currentBowChargeTime = 0;
                }
            }
        }

        if (isDrawingBow && InputManager.Instance.ReleaseArrow())
        {
            quickfirePerformed = true;
        }
        
        //Detects Form Swapping
        if (InputManager.Instance.ChangeForm())
        {
            if (currentForm == Forms.Living)
            {
                currentForm = Forms.Spirit;
                GetComponentInChildren<SkinnedMeshRenderer>().material = spiritFormMaterial;
                gameObject.layer = spiritLayer;
            }
            else
            {
                currentForm = Forms.Living;
                GetComponentInChildren<SkinnedMeshRenderer>().material = livingFormMaterial;
                gameObject.layer = livingLayer;
            }
        }
    }

    public void ApplyKnockback(Vector3 direction, float power)
    {
        isKnockbackApplied = true;

        body.velocity = direction * power;

        StartCoroutine(DeactivateKnockback());
    }

    IEnumerator DeactivateKnockback()
    {
        Vector3 startVel = body.velocity;
        float elaspedTime = 0;

        while (elaspedTime <= 0.5f)
        {
            body.velocity = Vector3.Lerp(startVel, Vector3.zero, elaspedTime / 0.5f);

            Vector3 direction = -body.velocity;

            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;

            transform.rotation = Quaternion.Euler(0f, targetAngle, 0f);

            elaspedTime += Time.deltaTime;

            yield return null;
        }

        isKnockbackApplied = false;

        yield return null;
    }


    //allows for the player to perform the little dash forwards before each sword swing
    //chooses the direction of the dash and attack based on the target enemy and distance
    public void InitSwordAttack()
    {
        if (canAttack)
        {
            if (targetEnemy != null && Vector3.Distance(targetEnemy.transform.position, transform.position) <= player.playerAttributes.SwordRange)
            {
                SwordAttack();

                return;
            }

            StartCoroutine(MoveTowardsAttack(player.playerAttributes.SwordCooldown, targetEnemy));
        }
    }

    IEnumerator MoveTowardsAttack(float duration, GameObject target = null)
    {
        isAttacking = true;

        StartCoroutine(CompleteAttackMovement(duration));

        //Uses lerp to give the player a small dash forward towards their attack
        float elaspedTime = 0;
        Vector3 endVelo = Vector3.zero;
        Vector3 startVelo = Vector3.zero;

        if (targetEnemy == null)
        {
            startVelo = player.playerAttributes.Speed * player.aimDirection;
        }
        else
        {
            startVelo = player.playerAttributes.Speed * (targetEnemy.transform.position - transform.position).normalized;
        }

        float targetAngle = Mathf.Atan2(startVelo.x, startVelo.z) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0f, targetAngle, 0f);

        while (elaspedTime < duration)
        {
            body.velocity = Vector3.Lerp(startVelo, endVelo, elaspedTime / duration);

            elaspedTime += Time.deltaTime;

            yield return null;
        }

        yield return null;
    }

    IEnumerator CompleteAttackMovement(float duration)
    {
        yield return new WaitForSeconds(duration * 0.25f);

        SwordAttack();

        yield return new WaitForSeconds(duration * 0.75f);

        isAttacking = false;
    }

    public void SwordAttack()
    {
        //Chooses which animation to play based on which attack number they are in the current combo
        switch (currentAttackNum)
        {
            case 1:

                GetComponentInChildren<Animator>().SetTrigger("Attack1");

                break;

            case 2:

                GetComponentInChildren<Animator>().SetTrigger("Attack2");

                break;

            case 3:

                GetComponentInChildren<Animator>().SetTrigger("Attack3");

                break;
        }
        ////////////////////////////////////////////////////////////////////

        ActivateSwordHitbox(currentAttackNum);

        canAttack = false;
    }

    //Initializes the drawing of the bow
    public void BowAttack()
    {
        GetComponentInChildren<Animator>().SetTrigger("DrawBow");

        canAttack = false;
        isDrawingBow = true;
    }

    void SpawnArrow(GameObject target = null)
    {
        GameObject arrow;

        if (target == null)
        {
            arrow = Instantiate(arrowPrefab, transform.position + Vector3.up + transform.forward, Quaternion.Euler(90, transform.rotation.eulerAngles.y, 0));

            arrow.GetComponent<PlayerArrow>().SetArrowChargeMultiplier(currentBowChargeTime / player.playerAttributes.BowChargeTime);
            arrow.GetComponent<Rigidbody>().velocity = transform.forward * player.playerAttributes.ProjectileSpeed;

            return;
        }

        arrow = Instantiate(arrowPrefab, transform.position + Vector3.up + transform.forward, Quaternion.identity);
        //arrow.transform.forward = target.transform.position - arrow.transform.position;
        arrow.transform.LookAt(target.transform);

        arrow.GetComponent<PlayerArrow>().SetArrowChargeMultiplier(currentBowChargeTime / player.playerAttributes.BowChargeTime);
        arrow.GetComponent<Rigidbody>().velocity = arrow.transform.forward * player.playerAttributes.ProjectileSpeed;

        arrow.transform.Rotate(new Vector3(1, 0, 0), 90);
    }

    void ActivateSwordHitbox(int attackNum)
    {
        Collider[] enemyColliders = null;
        List<Enemy> enemiesHit = new List<Enemy>();

        switch (attackNum)
        {
            case 1:

                enemyColliders = Physics.OverlapSphere(transform.position + (transform.forward * 2), player.playerAttributes.SwordRange);

                //Loops through each hit collider and adds all of the enemies into a list
                if (enemyColliders.Length > 0)
                {
                    for (int i = 0; i < enemyColliders.Length; i++)
                    {
                        if (enemyColliders[i].gameObject.TryGetComponent<Enemy>(out Enemy enemy) && enemy.Form == Form)
                        {
                            enemiesHit.Add(enemy);
                        }
                    }
                }
                //////////////////////////////////////////////////////////////////////////

                for (int i = 0; i < enemiesHit.Count; i++)
                {
                    enemiesHit[i].ApplyKnockback(transform.forward, 5);
                    enemiesHit[i].TakeDamage(player.playerAttributes.SwordDamage);
                }

                if (enemiesHit.Count > 0)
                {
                    EventManager.Instance.InvokeOnSwordHit(enemiesHit);
                }

                break;

            case 2:

                enemyColliders = Physics.OverlapSphere(transform.position + (transform.forward * 2), player.playerAttributes.SwordRange);

                if (enemyColliders.Length > 0)
                {
                    for (int i = 0; i < enemyColliders.Length; i++)
                    {
                        if (enemyColliders[i].gameObject.TryGetComponent<Enemy>(out Enemy enemy) && enemy.Form == Form)
                        {
                            enemiesHit.Add(enemy);
                        }
                    }
                }

                for (int i = 0; i < enemiesHit.Count; i++)
                {
                    enemiesHit[i].ApplyKnockback(transform.forward, 10);
                    enemiesHit[i].TakeDamage(player.playerAttributes.SwordDamage);
                }

                if (enemiesHit.Count > 0)
                {
                    EventManager.Instance.InvokeOnSwordHit(enemiesHit);
                }

                break;

            case 3: // CHANGE THIS WHEN WE GET THE THIRD ATTACK - this is for aiden dw abt it

                enemyColliders = Physics.OverlapSphere(transform.position + (transform.forward * 2), player.playerAttributes.SwordRange);

                if (enemyColliders.Length > 0)
                {
                    for (int i = 0; i < enemyColliders.Length; i++)
                    {
                        if (enemyColliders[i].gameObject.TryGetComponent<Enemy>(out Enemy enemy) && enemy.Form == Form)
                        {
                            enemiesHit.Add(enemy);
                        }
                    }
                }

                for (int i = 0; i < enemiesHit.Count; i++)
                {
                    enemiesHit[i].ApplyKnockback(transform.forward, 10);
                    enemiesHit[i].TakeDamage(player.playerAttributes.SwordDamage);
                }

                if (enemiesHit.Count > 0)
                {
                    EventManager.Instance.InvokeOnSwordHit(enemiesHit);
                }

                break;
        }
    }

    public void SelectTargetEnemy(Vector3 direction)
    {
        Collider[] collidersDetected = new Collider[0];
        Collider[] collidersDetected2 = new Collider[0];
        Collider[] collidersDetected3 = new Collider[0];

        if (direction.magnitude < 0.1f)
        {
            targetEnemy = null;

            return;
        }

        //Creates a "cone" that acts as the players "view" and adds every visible collider to the player into arrays
        collidersDetected = Physics.OverlapSphere(transform.position + (direction.normalized * 1), 1f);
        collidersDetected2 = Physics.OverlapSphere(transform.position + (direction.normalized * 3), 2f);
        collidersDetected3 = Physics.OverlapSphere(transform.position + (direction.normalized * 5), 3f);

        visibleEnemies = new List<Enemy>();

        //Filters out only enemies from the visible colliders
        if (collidersDetected.Length > 0)
        {
            for (int i = 0; i < collidersDetected.Length; i++)
            {
                if (collidersDetected[i].gameObject.TryGetComponent<Enemy>(out Enemy enemy))
                {
                    visibleEnemies.Add(enemy);
                }
            }
        }
        if (collidersDetected2.Length > 0)
        {
            for (int i = 0; i < collidersDetected2.Length; i++)
            {
                if (collidersDetected2[i].gameObject.TryGetComponent<Enemy>(out Enemy enemy))
                {
                    visibleEnemies.Add(enemy);
                }
            }
        }
        if (collidersDetected3.Length > 0)
        {
            for (int i = 0; i < collidersDetected3.Length; i++)
            {
                if (collidersDetected3[i].gameObject.TryGetComponent<Enemy>(out Enemy enemy))
                {
                    visibleEnemies.Add(enemy);
                }
            }
        }
        /////////////////////////////////////////////////////

        //Sets the target enemy to the closest enemy that is within the player's view
        if (visibleEnemies.Count > 0)
        {
            targetEnemy = visibleEnemies[0].gameObject;

            for (int i = 0; i < visibleEnemies.Count; i++)
            {
                if (Vector3.Distance(visibleEnemies[i].transform.position, transform.position) < Vector3.Distance(targetEnemy.transform.position, transform.position))
                {
                    targetEnemy = visibleEnemies[i].gameObject;
                }
            }
        }
        else
        {
            targetEnemy = null;
        }
        ///////////////////////////////////////////////////////////////////////////////
    }

    public int GetCurrentAttackNum()
    {
        return currentAttackNum;
    }

    public void SetCurrentAttackNum(int num)
    {
        currentAttackNum = num;
    }

    public bool GetCanAttack()
    {
        return canAttack;
    }

    public void SetBowDrawn(bool isDrawn)
    {
        bowDrawn = isDrawn;

        if (isDrawn)
        {
            isDrawingBow = false;
        }
    }

    public void SetCanAttack(bool value, bool applyCooldown, Weapons weaponChoice = Weapons.None)
    {
        if (value)
        {
            if (applyCooldown)
            {
                if (weaponChoice == Weapons.Sword)
                {
                    StartCoroutine(BeginAttackCooldown(player.playerAttributes.SwordCooldown));
                }

                if (weaponChoice == Weapons.Bow)
                {
                    StartCoroutine(BeginAttackCooldown(player.playerAttributes.BowCooldown));
                }

                return;
            }

            canAttack = true;

            return;
        }

        canAttack = false;
    }

    IEnumerator BeginAttackCooldown(float cooldown)
    {
        yield return new WaitForSeconds(cooldown);

        canAttack = true;
    }
}

public enum Forms
{
    Living,
    Spirit
}
