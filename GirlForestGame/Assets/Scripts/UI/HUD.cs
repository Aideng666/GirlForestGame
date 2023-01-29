using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    [Header("Hearts")]
    [SerializeField] GameObject heartPanel;
    [SerializeField] Sprite[] heartImages = new Sprite[5];
    [SerializeField] List<GameObject> startingHearts = new List<GameObject>();

    List<GameObject> currentHeartImages;

    PlayerController player;

    // Start is called before the first frame update
    void Start()
    {
        player = PlayerController.Instance;

        currentHeartImages = startingHearts;
    }

    private void OnEnable()
    {
        EventManager.OnHealthChange += UpdateHealth;
    }

    private void OnDisable()
    {
        EventManager.OnHealthChange -= UpdateHealth;
    }

    // Update is called once per frame
    void Update()
    {
        //UpdateHealth();
    }

    void UpdateHealth()
    {
        int currentHealth = player.playerAttributes.Health;
        int maxHealth = player.playerAttributes.MaxHealth;

        int numHealthImages = (int)Mathf.Ceil(maxHealth / 2);

        ////If there is an extra health image that exists, remove it
        //if (numHealthImages > currentHeartImages.Count)
        //{
        //    currentHeartImages.RemoveAt(currentHeartImages.Count - 1);

        //    Destroy(currentHeartImages[currentHeartImages.Count - 1]);
        //}

        for (int i = 0; i < numHealthImages; i++)
        {
            GameObject heart = null;

            //If there is more health than the number of existing health images, it will create one more
            if (i + 1 > currentHeartImages.Count)
            {
                heart = Instantiate(new GameObject("Heart Container", typeof(Image)), heartPanel.transform);
                currentHeartImages.Add(heart);
            }
            else
            {
                heart = currentHeartImages[i];
            }

            //Full Heart
            if (currentHealth >= 2)
            {
                heart.GetComponent<Image>().sprite = heartImages[4];
            }
            //filled half heart container
            else if (currentHealth == 1 && maxHealth == 1)
            {
                heart.GetComponent<Image>().sprite = heartImages[3];
            }
            //Half filled full Heart
            else if (currentHealth == 1 && maxHealth >= 2)
            {
                heart.GetComponent<Image>().sprite = heartImages[2];
            }
            //Empty Half Heart
            else if (currentHealth <= 0 && maxHealth == 1)
            {
                heart.GetComponent<Image>().sprite = heartImages[1];
            }
            //empty full heart
            else if (currentHealth <= 0 && maxHealth >= 2)
            {
                heart.GetComponent<Image>().sprite = heartImages[0];
            }

            currentHealth -= 2;
            maxHealth -= 2;
        }
    }
}
