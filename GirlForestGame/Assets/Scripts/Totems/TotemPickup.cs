using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TotemPickup : MonoBehaviour
{
    [SerializeField] float colliderDelay = 1;

    float timeElasped;

    TotemObject chosenTotem;

    // Start is called before the first frame update
    void Start()
    {
        ChooseTotem();
    }

    private void OnEnable()
    {
        GetComponent<Collider>().enabled = false;

        timeElasped = 0;
    }

    private void Update()
    {
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
        TotemObject[] possibleTotems = TypeHandler.GetAllInstances<TotemObject>("Totems");
        int randomIndex = Random.Range(0, possibleTotems.Length);

        chosenTotem = possibleTotems[randomIndex];

        chosenTotem.Totem.Init();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && !PlayerController.Instance.playerInventory.IsChoosingWeapon)
        {
            PlayerController.Instance.playerInventory.AddTotemToList(chosenTotem);

            gameObject.SetActive(false);
        }
    }
}
