using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TotemPickup : MonoBehaviour
{
    [SerializeField] float colliderDelay = 1;

    float timeElasped;

    Totem chosenTotem = null;


    // Start is called before the first frame update
    void Start()
    {
        if (chosenTotem == null)
        {
            ChooseTotem();
        }
    }

    private void OnEnable()
    {
        GetComponent<Collider>().enabled = false;

        timeElasped = 0;

    }

    private void Update()
    {
        if (chosenTotem == null)
        {
            ChooseTotem();
        }

        if (gameObject.activeInHierarchy)
        {
            timeElasped += Time.deltaTime;
        }

        if(timeElasped >= colliderDelay)
        {
            GetComponent<Collider>().enabled = true;
        }
    }

    void ChooseTotem()
    {
        Type[] possibleOnPickupTotems = AppDomain.CurrentDomain.GetAllDerivedTypes(typeof(OnPickupTotem));
        Type[] possibleOnTriggerTotems = AppDomain.CurrentDomain.GetAllDerivedTypes(typeof(OnTriggerTotem));
        Type[] possibleConstantTotems = AppDomain.CurrentDomain.GetAllDerivedTypes(typeof(ConstantTotem));

        List<Type> possibleTotems = new List<Type>();

        foreach (var totem in possibleOnPickupTotems)
        {
            possibleTotems.Add(totem);
        }

        foreach (var totem in possibleOnTriggerTotems)
        {
            possibleTotems.Add(totem);
        }

        foreach (var totem in possibleConstantTotems)
        {
            possibleTotems.Add(totem);
        }

        int randomIndex = UnityEngine.Random.Range(0, possibleTotems.Count);
        Type chosenType = possibleTotems[randomIndex];

        chosenTotem = (Totem)gameObject.AddComponent(chosenType);
        chosenTotem.Init();
    }

    public void ChooseTotem(Totem totem)
    {
        chosenTotem = totem;
        chosenTotem.Init();
    }

    public Totem GetRandomTotem()
    {
        Type[] possibleTotems = AppDomain.CurrentDomain.GetAllDerivedTypes(typeof(Totem));
        int randomIndex = UnityEngine.Random.Range(0, possibleTotems.Length);

        return (Totem)Activator.CreateInstance(possibleTotems[randomIndex]);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && !PlayerController.Instance.playerInventory.IsChoosingWeapon)
        {
            PlayerController.Instance.playerInventory.AddTotemToList(chosenTotem);

            HUD.Instance.UpdateTotemHUD(chosenTotem.totemObject.totemSprite, chosenTotem.totemObject.totemName, chosenTotem.totemObject.totemDescription);

            gameObject.SetActive(false);

            FMODUnity.RuntimeManager.PlayOneShot("event:/UI/Obtain");

            //if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("Tutorial"))
            //{
            //    TutorialManager.Instance.TriggerTutorialSection(14, true);
            //    TutorialManager.Instance.TriggerTutorialSection(21, true);
            //}
        }
    }

}
