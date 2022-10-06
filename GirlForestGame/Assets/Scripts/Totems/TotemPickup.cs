using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TotemPickup : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        ChooseTotem();
    }

    void ChooseTotem()
    {
        List<System.Type> possibleTotems = new List<System.Type>();

        //Adds all the existing totems into a list of all of them together (3 different loops for 3 different types of Totems)
        var permanentTotems = System.AppDomain.CurrentDomain.GetAllDerivedTypes(typeof(PermanentTotem));

        for (int i = 0; i < permanentTotems.Length; i++)
        {
            possibleTotems.Add(permanentTotems[i]);
        }

        var onTriggerTotems = System.AppDomain.CurrentDomain.GetAllDerivedTypes(typeof(OnTriggerTotem));

        for (int i = 0; i < onTriggerTotems.Length; i++)
        {
            possibleTotems.Add(onTriggerTotems[i]);
        }

        var constantTotems = System.AppDomain.CurrentDomain.GetAllDerivedTypes(typeof(ConstantTotem));

        for (int i = 0; i < constantTotems.Length; i++)
        {
            possibleTotems.Add(constantTotems[i]);
        }

        //Selects a random totem to become from the existing pool of totems
        int totemChoice = Random.Range(0, possibleTotems.Count);

        gameObject.AddComponent(possibleTotems[totemChoice]);
    }
}
