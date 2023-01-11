using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [SerializeField] GameObject arrowPrefab;
    [SerializeField] GameObject bowAimCanvas;
    [SerializeField] Material livingFormMaterial;
    [SerializeField] Material spiritFormMaterial;
    [SerializeField] Collider bowTargetCollider;

    public bool isAttacking { get; private set; }
    public bool isKnockbackApplied { get; private set; }

    bool canAttack = true;

    //Bow Stuff
    GameObject bowTargetEnemy;
    List<Enemy> bowTargetsInView = new List<Enemy>();
    bool bowDrawn;
    bool bowCharging;
    bool isDrawingBow;
    bool quickfirePerformed;
    float currentBowChargeTime = 0;

    //Sword Stuff
    GameObject swordTargetEnemy;
    List<Enemy> swordTargetsInView = new List<Enemy>();
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
                    SpawnArrow(bowTargetEnemy);

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
            if (player.playerInventory.totemDictionary[typeof(PlaneSwapEmpowermentTotem)] > 0 && player.playerInventory.GetTotemFromList(typeof(PlaneSwapEmpowermentTotem)).Totem.effectApplied)
            {
                player.playerInventory.GetTotemFromList(typeof(PlaneSwapEmpowermentTotem)).Totem.RemoveEffect();
            }

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

            //EventManager.Instance.InvokeTotemTrigger(TotemEvents.OnPlaneSwitch);

            if (player.playerInventory.totemDictionary[typeof(PlaneSwapEmpowermentTotem)] > 0)
            {
                player.playerInventory.GetTotemFromList(typeof(PlaneSwapEmpowermentTotem)).Totem.ApplyEffect();
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
            if (swordTargetEnemy != null && Vector3.Distance(swordTargetEnemy.transform.position, transform.position) <= player.playerAttributes.SwordRange)
            {
                SwordAttack();

                return;
            }

            StartCoroutine(MoveTowardsAttack(player.playerAttributes.SwordCooldown, swordTargetEnemy));
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

        if (swordTargetEnemy == null)
        {
            startVelo = player.playerAttributes.Speed * player.aimDirection;
        }
        else
        {
            startVelo = player.playerAttributes.Speed * (swordTargetEnemy.transform.position - transform.position).normalized;
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

        ActivateSwordHitbox(/*currentAttackNum*/);

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
            arrow = Instantiate(arrowPrefab, transform.position + Vector3.up + transform.forward, Quaternion.identity);
            arrow.transform.forward = player.aimDirection;

            arrow.GetComponent<PlayerArrow>().SetArrowChargeMultiplier(currentBowChargeTime / player.playerAttributes.BowChargeTime);
            arrow.GetComponent<Rigidbody>().velocity = player.aimDirection * player.playerAttributes.ProjectileSpeed;

            arrow.transform.Rotate(new Vector3(1, 0, 0), 90);

            return;
        }

        arrow = Instantiate(arrowPrefab, transform.position + Vector3.up + transform.forward, Quaternion.identity);
        //arrow.transform.forward = target.transform.position - arrow.transform.position;
        arrow.transform.LookAt(target.transform);

        arrow.GetComponent<PlayerArrow>().SetArrowChargeMultiplier(currentBowChargeTime / player.playerAttributes.BowChargeTime);
        arrow.GetComponent<Rigidbody>().velocity = arrow.transform.forward * player.playerAttributes.ProjectileSpeed;

        arrow.transform.Rotate(new Vector3(1, 0, 0), 90);
    }

    void ActivateSwordHitbox(/*int attackNum*/)
    {
        print("Checking Who was hit");

        Collider[] enemyColliders = null;
        List<EnemyData> enemiesHit = new List<EnemyData>();

        //switch (attackNum)
        //{
        //    case 1:

        //        enemyColliders = Physics.OverlapSphere(transform.position + (transform.forward * (player.playerAttributes.SwordRange / 2)), player.playerAttributes.SwordRange);

        //        //Loops through each hit collider and adds all of the enemies into a list
        //        if (enemyColliders.Length > 0)
        //        {
        //            for (int i = 0; i < enemyColliders.Length; i++)
        //            {
        //                if (enemyColliders[i].gameObject.TryGetComponent(out Enemy enemy) && enemy.Form == Form)
        //                {
        //                    enemiesHit.Add(enemy);
        //                }
        //            }
        //        }
        //        //////////////////////////////////////////////////////////////////////////

        //        for (int i = 0; i < enemiesHit.Count; i++)
        //        {
        //            //Checks for hitting from behind
        //            if (Vector3.Angle((enemiesHit[i].transform.position - transform.position).normalized, enemiesHit[i].transform.forward) < 90)
        //            {
        //                AssassinTotem t;
        //                bool assassinTotemExists = player.playerInventory.totemDictionary[typeof(AssassinTotem)] > 0;

        //                if (assassinTotemExists)
        //                {
        //                    t = (AssassinTotem)player.playerInventory.GetTotemFromList(typeof(AssassinTotem)).Totem;

        //                    t.SetWeaponUsed(Weapons.Sword);
        //                }

        //                enemiesHit[i].ApplyKnockback(transform.forward, 10);
        //                enemiesHit[i].TakeDamage(player.playerAttributes.SwordDamage);

        //                if (assassinTotemExists)
        //                {
        //                    t = (AssassinTotem)player.playerInventory.GetTotemFromList(typeof(AssassinTotem)).Totem;

        //                    t.RemoveEffect();
        //                }
        //            }
        //            else
        //            {
        //                enemiesHit[i].ApplyKnockback(transform.forward, 10);
        //                enemiesHit[i].TakeDamage(player.playerAttributes.SwordDamage);
        //            }
        //        }

        //        if (enemiesHit.Count > 0)
        //        {
        //            EventManager.Instance.InvokeOnSwordHit(enemiesHit);
        //        }

        //        break;

        //    case 2:

        //        enemyColliders = Physics.OverlapSphere(transform.position + (transform.forward * 2), player.playerAttributes.SwordRange);

        //        if (enemyColliders.Length > 0)
        //        {
        //            for (int i = 0; i < enemyColliders.Length; i++)
        //            {
        //                if (enemyColliders[i].gameObject.TryGetComponent(out Enemy enemy) && enemy.Form == Form)
        //                {
        //                    enemiesHit.Add(enemy);
        //                }
        //            }
        //        }

        //        for (int i = 0; i < enemiesHit.Count; i++)
        //        {
        //            if (Vector3.Angle((enemiesHit[i].transform.position - transform.position).normalized, enemiesHit[i].transform.forward) < 90)
        //            {
        //                AssassinTotem t;
        //                bool assassinTotemExists = player.playerInventory.totemDictionary[typeof(AssassinTotem)] > 0;

        //                if (assassinTotemExists)
        //                {
        //                    t = (AssassinTotem)player.playerInventory.GetTotemFromList(typeof(AssassinTotem)).Totem;

        //                    t.SetWeaponUsed(Weapons.Sword);
        //                }

        //                enemiesHit[i].ApplyKnockback(transform.forward, 10);
        //                enemiesHit[i].TakeDamage(player.playerAttributes.SwordDamage);

        //                if (assassinTotemExists)
        //                {
        //                    t = (AssassinTotem)player.playerInventory.GetTotemFromList(typeof(AssassinTotem)).Totem;

        //                    t.RemoveEffect();
        //                }
        //            }
        //            else
        //            {
        //                enemiesHit[i].ApplyKnockback(transform.forward, 10);
        //                enemiesHit[i].TakeDamage(player.playerAttributes.SwordDamage);
        //            }
        //        }

        //        if (enemiesHit.Count > 0)
        //        {
        //            EventManager.Instance.InvokeOnSwordHit(enemiesHit);
        //        }

        //        break;

        //    case 3: // CHANGE THIS WHEN WE GET THE THIRD ATTACK - this is for aiden dw abt it

        //        enemyColliders = Physics.OverlapSphere(transform.position + (transform.forward * 2), player.playerAttributes.SwordRange);

        //        if (enemyColliders.Length > 0)
        //        {
        //            for (int i = 0; i < enemyColliders.Length; i++)
        //            {
        //                if (enemyColliders[i].gameObject.TryGetComponent<Enemy>(out Enemy enemy) && enemy.Form == Form)
        //                {
        //                    enemiesHit.Add(enemy);
        //                }
        //            }
        //        }

        //        for (int i = 0; i < enemiesHit.Count; i++)
        //        {
        //            if (Vector3.Angle((enemiesHit[i].transform.position - transform.position).normalized, enemiesHit[i].transform.forward) < 90)
        //            {
        //                AssassinTotem t;
        //                bool assassinTotemExists = player.playerInventory.totemDictionary[typeof(AssassinTotem)] > 0;

        //                if (assassinTotemExists)
        //                {
        //                    t = (AssassinTotem)player.playerInventory.GetTotemFromList(typeof(AssassinTotem)).Totem;

        //                    t.SetWeaponUsed(Weapons.Sword);
        //                }

        //                enemiesHit[i].ApplyKnockback(transform.forward, 10);
        //                enemiesHit[i].TakeDamage(player.playerAttributes.SwordDamage);

        //                if (assassinTotemExists)
        //                {
        //                    t = (AssassinTotem)player.playerInventory.GetTotemFromList(typeof(AssassinTotem)).Totem;

        //                    t.RemoveEffect();
        //                }
        //            }
        //            else
        //            {
        //                enemiesHit[i].ApplyKnockback(transform.forward, 10);
        //                enemiesHit[i].TakeDamage(player.playerAttributes.SwordDamage);
        //            }
        //        }

        //        if (enemiesHit.Count > 0)
        //        {
        //            EventManager.Instance.InvokeOnSwordHit(enemiesHit);
        //        }

        //        break;
        //}

        enemyColliders = Physics.OverlapSphere(transform.position + (transform.forward * (player.playerAttributes.SwordRange / 2)), player.playerAttributes.SwordRange);

        //Loops through each hit collider and adds all of the enemies into a list
        if (enemyColliders.Length > 0)
        {
            for (int i = 0; i < enemyColliders.Length; i++)
            {
                if (enemyColliders[i].gameObject.TryGetComponent(out EnemyData enemy) && enemy.Form == Form)
                {
                    enemiesHit.Add(enemy);

                    print("Adding an enemy");
                }
            }
        }
        //////////////////////////////////////////////////////////////////////////

        for (int i = 0; i < enemiesHit.Count; i++)
        {
            //Checks for hitting from behind
            if (Vector3.Angle((enemiesHit[i].transform.position - transform.position).normalized, enemiesHit[i].transform.forward) < 90)
            {
                AssassinTotem t;
                bool assassinTotemExists = player.playerInventory.totemDictionary[typeof(AssassinTotem)] > 0;

                if (assassinTotemExists)
                {
                    t = (AssassinTotem)player.playerInventory.GetTotemFromList(typeof(AssassinTotem)).Totem;

                    t.SetWeaponUsed(Weapons.Sword);
                }

                enemiesHit[i].ApplyKnockback(transform.forward, 10);
                enemiesHit[i].TakeDamage(player.playerAttributes.SwordDamage);

                if (assassinTotemExists)
                {
                    t = (AssassinTotem)player.playerInventory.GetTotemFromList(typeof(AssassinTotem)).Totem;

                    t.RemoveEffect();
                }
            }
            else
            {
                enemiesHit[i].ApplyKnockback(transform.forward, 10);
                enemiesHit[i].TakeDamage(player.playerAttributes.SwordDamage);
            }
        }

        if (enemiesHit.Count > 0)
        {
            EventManager.Instance.InvokeOnSwordHit(enemiesHit);
        }
    }

    public void SelectSwordTargetEnemy()
    {
        //Sets the target enemy to the closest enemy that is within the player's view
        if (swordTargetsInView.Count > 0)
        {
            swordTargetEnemy = swordTargetsInView[0].gameObject;

            for (int i = 0; i < swordTargetsInView.Count; i++)
            {
                if (Vector3.Distance(swordTargetsInView[i].transform.position, transform.position) < Vector3.Distance(swordTargetEnemy.transform.position, transform.position))
                {
                    swordTargetEnemy = swordTargetsInView[i].gameObject;
                }
            }
        }
        else
        {
            swordTargetEnemy = null;
        }
    }

    public void SelectBowTargetEnemy()
    {
        if (bowTargetsInView.Count > 0)
        {
            bowTargetEnemy = bowTargetsInView[0].gameObject;

            for (int i = 0; i < bowTargetsInView.Count; i++)
            {
                if (Vector3.Distance(bowTargetsInView[i].transform.position, transform.position) < Vector3.Distance(bowTargetEnemy.transform.position, transform.position))
                {
                    bowTargetEnemy = bowTargetsInView[i].gameObject;
                }
            }
        }
        else
        {
            bowTargetEnemy = null;
        }
    }

    public void TakeDamage()
    {
        player.playerAttributes.Health -= 1;
    }

    public void AddBowTarget(Enemy enemy)
    {
        bowTargetsInView.Add(enemy);
    }

    public void RemoveBowTarget(Enemy enemy)
    {
        bowTargetsInView.RemoveAt(bowTargetsInView.IndexOf(enemy));
    }

    public void AddSwordTarget(Enemy enemy)
    {
        swordTargetsInView.Add(enemy);
    }

    public void RemoveSwordTarget(Enemy enemy)
    {
        swordTargetsInView.RemoveAt(swordTargetsInView.IndexOf(enemy));
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
