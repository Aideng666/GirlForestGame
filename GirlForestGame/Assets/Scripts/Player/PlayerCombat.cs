using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerCombat : MonoBehaviour
{
    [SerializeField] GameObject arrowPrefab;
    [SerializeField] GameObject bowAimCanvas;
    [SerializeField] ParticleSystem swordSlashLR;
    [SerializeField] ParticleSystem swordSlashRL;
    [SerializeField] ParticleSystem bowChargeParticle;
    [SerializeField] ParticleSystem bowChargeCompleteParticle;
    [SerializeField] Material livingFormMaterial;
    [SerializeField] Material spiritFormMaterial;
    [SerializeField] Shader iFrameShader;
    [SerializeField] float iFrameTime = 1;

    public bool isAttacking { get; private set; }
    public bool isKnockbackApplied { get; private set; }

    bool canAttack = true;
    bool iFramesActive = false;
    [HideInInspector] public FMOD.Studio.EventInstance hitSFX;
    [HideInInspector] public FMOD.Studio.EventInstance formSFX;


    //Bow Stuff
    GameObject bowTargetEnemy;
    List<EnemyData> bowTargetsInView = new List<EnemyData>();
    bool bowDrawn;
    bool bowCharging;
    bool chargeParticlePlayed;
    bool chargeCompleteParticlePlayed;
    bool isDrawingBow;
    bool quickfirePerformed;
    float currentBowChargeTime = 0;

    [HideInInspector] public FMOD.Studio.EventInstance BowSFX;
    [HideInInspector] public FMOD.Studio.EventInstance ArrowSFX;
    private FMOD.Studio.EventInstance DrawSFX;

    //Sword Stuff
    GameObject swordTargetEnemy;
    List<EnemyData> swordTargetsInView = new List<EnemyData>();
    int currentAttackNum = 1;
    [HideInInspector] public FMOD.Studio.EventInstance SwordSFX;

    Planes currentForm = Planes.Terrestrial;
    LayerMask livingLayer;
    LayerMask spiritLayer;
    LayerMask defaultLayer;
    LayerMask enemyTerrestrialLayer;
    LayerMask enemyAstralLayer;
    LayerMask iFramesLayer;

    public Planes Form { get { return currentForm; } set { currentForm = value; } }

    PlayerController player;
    Rigidbody body;

    // Start is called before the first frame update
    void Start()
    {
        livingLayer = LayerMask.NameToLayer("PlayerLiving");
        spiritLayer = LayerMask.NameToLayer("PlayerSpirit");
        enemyTerrestrialLayer = LayerMask.NameToLayer("EnemyLiving");
        enemyAstralLayer = LayerMask.NameToLayer("EnemySpirit");
        iFramesLayer = LayerMask.NameToLayer("IFrames");
        defaultLayer = LayerMask.NameToLayer("Default");

        bowAimCanvas.SetActive(false);

        player = PlayerController.Instance;
        body = GetComponent<Rigidbody>();
    }

    private void Awake()
    {
        BowSFX = FMODUnity.RuntimeManager.CreateInstance("event:/Player/Bow/Bow");
        ArrowSFX = FMODUnity.RuntimeManager.CreateInstance("event:/Player/Bow/Arrow");
        DrawSFX = FMODUnity.RuntimeManager.CreateInstance("event:/Player/Bow/Draw");
        SwordSFX = FMODUnity.RuntimeManager.CreateInstance("event:/Player/Sword/Sword");
        hitSFX = FMODUnity.RuntimeManager.CreateInstance("event:/Player/Hit");
        formSFX = FMODUnity.RuntimeManager.CreateInstance("event:/Player/Form");

        ArrowSFX.getParameterByName("SPCharge", out currentBowChargeTime);

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

                print(bowChargeParticle.gameObject.activeSelf);
                if (!bowChargeParticle.gameObject.activeSelf && !chargeParticlePlayed)
                {
                    bowChargeParticle.gameObject.SetActive(true);

                    bowChargeParticle.Stop();

                    var main = bowChargeParticle.main;
                    main.duration = player.playerAttributes.BowChargeTime - bowChargeParticle.main.startLifetime.constant;

                    bowChargeParticle.Play();

                    chargeParticlePlayed = true;

                    print(main.duration);
                }
            }
            else
            {
                bowCharging = false;
                currentBowChargeTime = 0;

                bowChargeParticle.gameObject.SetActive(false);
                chargeParticlePlayed = false;
                chargeCompleteParticlePlayed = false;
            }

            //Charges the bow when standing still
            if (bowCharging)
            {
                if (currentBowChargeTime < player.playerAttributes.BowChargeTime)
                {
                    currentBowChargeTime += Time.deltaTime;

                    currentBowChargeTime = Mathf.Clamp(currentBowChargeTime, 0, player.playerAttributes.BowChargeTime);
                    ArrowSFX.setParameterByName("SPCharge", currentBowChargeTime);
                }
                else if (!chargeCompleteParticlePlayed)
                {
                    if (!bowChargeCompleteParticle.gameObject.activeSelf)
                    {
                        bowChargeCompleteParticle.gameObject.SetActive(true);

                        chargeCompleteParticlePlayed = true;
                    }
                }
            }

            //Detects when the player released the arrow to fire
            if (InputManager.Instance.ReleaseArrow())
            {
                GetComponentInChildren<Animator>().SetTrigger("ReleaseArrow");

                bowDrawn = false;
                chargeParticlePlayed = false;
                chargeCompleteParticlePlayed = false;

                bowChargeParticle.gameObject.SetActive(false);
                bowChargeCompleteParticle.gameObject.SetActive(false);

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
                BowSFX.keyOff();
                ArrowSFX.start();
            }
        }

        //Detects if a quickfire was performed
        if (isDrawingBow && InputManager.Instance.ReleaseArrow())
        {
            quickfirePerformed = true;
        }
        
        //Detects Form Swapping
        if (InputManager.Instance.ChangeForm() && !iFramesActive)
        {
            if (player.playerInventory.totemDictionary[typeof(PlaneSwapEmpowermentTotem)] > 0 && player.playerInventory.GetTotemFromList(typeof(PlaneSwapEmpowermentTotem)).Totem.effectApplied)
            {
                player.playerInventory.GetTotemFromList(typeof(PlaneSwapEmpowermentTotem)).Totem.RemoveEffect();
            }

            if (currentForm == Planes.Terrestrial)
            {
                currentForm = Planes.Astral;
                GetComponentInChildren<SkinnedMeshRenderer>().material = spiritFormMaterial;
                gameObject.layer = spiritLayer;
                formSFX.setParameterByName("Astral", 1);

            }
            else
            {
                currentForm = Planes.Terrestrial;
                GetComponentInChildren<SkinnedMeshRenderer>().material = livingFormMaterial;
                gameObject.layer = livingLayer;
                formSFX.setParameterByName("Astral", 0);

            }

            if (player.playerInventory.totemDictionary[typeof(PlaneSwapEmpowermentTotem)] > 0)
            {
                player.playerInventory.GetTotemFromList(typeof(PlaneSwapEmpowermentTotem)).Totem.ApplyEffect();
            }

            if (player.playerInventory.totemDictionary[typeof(AstralBarrierTotem)] > 0)
            {
                player.playerInventory.GetTotemFromList(typeof(AstralBarrierTotem)).Totem.ApplyEffect();
            }
            formSFX.start();
        }

        //Sets the radius of the sword slashes
        if (swordSlashLR.gameObject.activeSelf) 
        {
            var particleShape = swordSlashLR.shape;

            particleShape.radius = player.playerAttributes.SwordRange / 2;

            //swordSlashLR.GetComponent<SphereCollider>().radius = particleShape.radius;
        }
        if (swordSlashRL.gameObject.activeSelf)
        {
            var particleShape = swordSlashRL.shape;

            particleShape.radius = player.playerAttributes.SwordRange / 2;

            //swordSlashRL.GetComponent<SphereCollider>().radius = particleShape.radius;
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
            //float targetAngle = 0;

            //if (swordTargetEnemy != null && Vector3.Distance(swordTargetEnemy.transform.position, transform.position) <= player.playerAttributes.SwordRange / 3)
            //{
            //    targetAngle = Mathf.Atan2((swordTargetEnemy.transform.position - transform.position).normalized.x, (swordTargetEnemy.transform.position - transform.position).normalized.z) * Mathf.Rad2Deg;

            //    transform.rotation = Quaternion.Euler(0f, targetAngle, 0f);
            //}
            //else if (swordTargetEnemy == null) 
            //{
            //}

            transform.forward = player.aimDirection;
            SwordAttack();

            //StartCoroutine(MoveTowardsAttack(0.25f, swordTargetEnemy));
            //isAttacking = true;

            //canAttack = false;
        }
    }

    //IEnumerator MoveTowardsAttack(float duration, GameObject target = null)
    //{
    //    isAttacking = true;

    //    StartCoroutine(CompleteAttackMovement(duration));

    //    //Uses lerp to give the player a small dash forward towards their attack
    //    float elaspedTime = 0;
    //    Vector3 endVelo = Vector3.zero;
    //    Vector3 startVelo = Vector3.zero;

    //    if (swordTargetEnemy == null)
    //    {
    //        startVelo = player.playerAttributes.Speed * player.aimDirection;
    //    }
    //    else
    //    {
    //        startVelo = player.playerAttributes.Speed * (swordTargetEnemy.transform.position - transform.position).normalized;
    //    }

    //    float targetAngle = Mathf.Atan2(startVelo.x, startVelo.z) * Mathf.Rad2Deg;

    //    transform.rotation = Quaternion.Euler(0f, targetAngle, 0f);

    //    while (elaspedTime < duration)
    //    {
    //        body.velocity = Vector3.Lerp(startVelo, endVelo, elaspedTime / duration);

    //        elaspedTime += Time.deltaTime;

    //        yield return null;
    //    }

    //    yield return null;
    //}

    IEnumerator CompleteAttack(float duration)
    {
        yield return new WaitForSeconds(duration);

        swordSlashLR.gameObject.SetActive(false);
        swordSlashRL.gameObject.SetActive(false);

        isAttacking = false;
    }

    public void SwordAttack()
    {
        isAttacking = true;
        body.velocity = Vector3.zero;
        canAttack = false;

        StartCoroutine(CompleteAttack(player.playerAttributes.SwordCooldown / 1.5f));
        ActivateSwordHitbox();

        //Chooses which animation to play based on which attack number they are in the current combo
        switch (currentAttackNum)
        {
            case 1:

                GetComponentInChildren<Animator>().SetTrigger("Attack1");
                swordSlashLR.gameObject.SetActive(true);
                SwordSFX.start();

                SetCanAttack(true, player.playerAttributes.SwordCooldown / 1.5f);
                break;

            case 2:

                GetComponentInChildren<Animator>().SetTrigger("Attack2");
                swordSlashRL.gameObject.SetActive(true);
                SwordSFX.keyOff();

                SetCanAttack(true, player.playerAttributes.SwordCooldown / 1.5f);
                break;

            case 3:

                GetComponentInChildren<Animator>().SetTrigger("Attack3");
                swordSlashRL.gameObject.SetActive(true);
                SwordSFX.keyOff();

                player.playerCombat.SetCanAttack(false, 0);
                SetCanAttack(true, player.playerAttributes.SwordCooldown);
                break;
        }

        ////////////////////////////////////////////////////////////////////
    }

    //Initializes the drawing of the bow
    public void BowAttack()
    {
        if (canAttack)
        {
            GetComponentInChildren<Animator>().SetTrigger("DrawBow");

            canAttack = false;
            isDrawingBow = true;
            DrawSFX.start();

            currentAttackNum = 1;
        }
    }

    void SpawnArrow(GameObject target = null)
    {
        GameObject arrow = ProjectilePool.Instance.GetArrowFromPool(transform.position + (Vector3.up + transform.forward) / 2);

        arrow.GetComponent<PlayerArrow>().SetArrowChargeMultiplier(currentBowChargeTime / player.playerAttributes.BowChargeTime);

        if (player.playerMarkings.markings[3] != null)
        {
            arrow.GetComponent<PlayerArrow>().SetMovement(player.playerMarkings.markings[3].usedElement, target);

            return;
        }

        arrow.GetComponent<PlayerArrow>().SetMovement(Elements.None, target);
    }

    void ActivateSwordHitbox()
    {
        Collider[] enemyColliders = null;
        List<EnemyData> enemiesHit = new List<EnemyData>();

        //Creates the correct layer mask for the colliders to hit the proper enemies at any given time
        int colliderLayerMask = (1 << defaultLayer);

        if (Form == Planes.Terrestrial)
        {
            colliderLayerMask |= (1 << enemyTerrestrialLayer);
            colliderLayerMask &= ~(1 << enemyAstralLayer);
        }
        else
        {
            colliderLayerMask |= (1 << enemyAstralLayer);
            colliderLayerMask &= ~(1 << enemyTerrestrialLayer);
        }

        enemyColliders = Physics.OverlapSphere(transform.position + (transform.forward * (player.playerAttributes.SwordRange / 2)), player.playerAttributes.SwordRange / 2, colliderLayerMask);

        //Loops through each hit collider and adds all of the enemies into a list
        if (enemyColliders.Length > 0)
        {
            for (int i = 0; i < enemyColliders.Length; i++)
            {
                if (enemyColliders[i].gameObject.TryGetComponent(out EnemyData enemy) && enemy.Form == Form)
                {
                    enemiesHit.Add(enemy);
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

                enemiesHit[i].ApplyKnockback(10, transform.forward);
                enemiesHit[i].TakeDamage(player.playerAttributes.SwordDamage);

                if (assassinTotemExists)
                {
                    t = (AssassinTotem)player.playerInventory.GetTotemFromList(typeof(AssassinTotem)).Totem;

                    t.RemoveEffect();
                }
            }
            else
            {
                enemiesHit[i].ApplyKnockback(10, transform.forward);
                enemiesHit[i].TakeDamage(player.playerAttributes.SwordDamage);
            }
        }

        if (enemiesHit.Count > 0)
        {
            EventManager.Instance.InvokeOnSwordHit(enemiesHit);

            if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("Tutorial"))
            {
                TutorialManager.Instance.TriggerTutorialSection(7, true);
            }
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
        if (!iFramesActive)
        {
            player.playerAttributes.Health -= 1;

            StartCoroutine(BeginIFrames());
            hitSFX.start();
            hitSFX.release();
        }
    }

    IEnumerator BeginIFrames()
    {
        iFramesActive = true;

        Shader originalShader = GetComponentInChildren<SkinnedMeshRenderer>().material.shader;
        LayerMask originalLayer = gameObject.layer;

        gameObject.layer = iFramesLayer;
        GetComponentInChildren<SkinnedMeshRenderer>().material.shader = iFrameShader;

        yield return new WaitForSeconds(iFrameTime);

        iFramesActive = false;

        gameObject.layer = originalLayer;
        GetComponentInChildren<SkinnedMeshRenderer>().material.shader = originalShader;
    }

    public void AddBowTarget(EnemyData enemy)
    {
        bowTargetsInView.Add(enemy);
    }

    public void RemoveBowTarget(EnemyData enemy)
    {
        if (bowTargetsInView.Contains(enemy))
        {
            bowTargetsInView.RemoveAt(bowTargetsInView.IndexOf(enemy));
        }
    }

    public void AddSwordTarget(EnemyData enemy)
    {
        swordTargetsInView.Add(enemy);
    }

    public void RemoveSwordTarget(EnemyData enemy)
    {
        if (swordTargetsInView.Contains(enemy))
        {
            swordTargetsInView.RemoveAt(swordTargetsInView.IndexOf(enemy));
        }
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

        BowSFX.start();
        if (isDrawn)
        {
            isDrawingBow = false;
        }
    }

    public void SetCanAttack(bool value, float cooldown/*, Weapons weaponChoice = Weapons.None*/)
    {
        if (value)
        {
            if (cooldown > 0)
            {
                //if (weaponChoice == Weapons.Sword)
                //{
                    StartCoroutine(BeginAttackCooldown(cooldown));
                //}

                //if (weaponChoice == Weapons.Bow)
                //{
                //    StartCoroutine(BeginAttackCooldown(cooldown));
                //}

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
    private void OnDestroy()
    {
        BowSFX.release();
        SwordSFX.release();
        ArrowSFX.release();
        DrawSFX.release();
    }
}

public enum Planes
{
    Terrestrial,
    Astral
}
