using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class HUD : MonoBehaviour
{
    [Header("Hearts Panel")]
    [SerializeField] GameObject heartPanel;
    [SerializeField] Sprite[] heartImages = new Sprite[5];
    [SerializeField] List<GameObject> startingHearts = new List<GameObject>();
    List<GameObject> currentHeartImages;

    [Header("Attribute Panel")]
    [SerializeField] GameObject attributePanel;
    [SerializeField] TextMeshProUGUI speedText;
    [SerializeField] TextMeshProUGUI swdDmgText;
    [SerializeField] TextMeshProUGUI bowDmgText;
    [SerializeField] TextMeshProUGUI swdCdnText;
    [SerializeField] TextMeshProUGUI bowCdnText;
    [SerializeField] TextMeshProUGUI swdRangeText;
    [SerializeField] TextMeshProUGUI projSpdText;
    [SerializeField] TextMeshProUGUI critChanceText;
    [SerializeField] TextMeshProUGUI bowChargeSpdText;
    [SerializeField] TextMeshProUGUI luckText;
    bool attributePanelActive = false;
    

    PlayerController player;

    // Start is called before the first frame update
    void Start()
    {
        player = PlayerController.Instance;

        currentHeartImages = startingHearts;

        UpdateAttributes();
    }

    private void OnEnable()
    {
        EventManager.OnHealthChange += UpdateHealth;
        EventManager.OnAttributeChange += UpdateAttributes;
    }

    private void OnDisable()
    {
        EventManager.OnHealthChange -= UpdateHealth;
        EventManager.OnAttributeChange -= UpdateAttributes;
    }

    // Update is called once per frame
    void Update()
    {
        if (InputManager.Instance.OpenAttributes())
        {
            ToggleAttributePanel();
        }
    }

    void UpdateHealth()
    {
        int currentHealth = player.playerAttributes.Health;
        int maxHealth = player.playerAttributes.MaxHealth;

        int numHealthImages = (int)Mathf.Ceil(maxHealth / 2);

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

    void UpdateAttributes()
    {
        speedText.text = $"SPD: {player.playerAttributes.Speed}";
        swdDmgText.text = $"SWD DMG: {player.playerAttributes.SwordDamage}";
        bowDmgText.text = $"BOW DMG: {player.playerAttributes.BowDamage}";
        swdCdnText.text = $"SWD CDN: {player.playerAttributes.SwordCooldown}";
        bowCdnText.text = $"Bow CDN: {player.playerAttributes.BowCooldown}";
        swdRangeText.text = $"SWD RANGE: {player.playerAttributes.SwordRange}";
        projSpdText.text = $"PROJ SPD: {player.playerAttributes.ProjectileSpeed}";
        critChanceText.text = $"CRIT CHANCE: {player.playerAttributes.CritChance}";
        bowChargeSpdText.text = $"BOW CHARGE TIME: {player.playerAttributes.BowChargeTime}";
        luckText.text = $"LUCK: {player.playerAttributes.Luck}";
    }

    void ToggleAttributePanel()
    {
        attributePanelActive = !attributePanelActive;

        if (!attributePanelActive)
        {
            attributePanel.transform.DOMove(new Vector3(-150, 540, 0), 0.5f);

            return;
        }

        attributePanel.transform.DOMove(new Vector3(150, 540, 0), 0.5f);
    }
}
