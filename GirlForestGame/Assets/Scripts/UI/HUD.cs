using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class HUD : MonoBehaviour
{
    [Header("General")]
    [SerializeField] Canvas canvas;
    [SerializeField] float hiddenPanelVisibilityPercentage = 0.1f;

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

    [Header("Markings Panel")]
    [SerializeField] GameObject markingsPanel;
    [SerializeField] Image[] images = new Image[4]; // 0 = SwordAttribute | 1 = SwordElement | 2 = BowAttribute | 3 = BowElement
    bool markingsPanelActive = true;

    PlayerController player;

    Tween attributePanelTween = null;
    Tween markingsPanelTween = null;

    // Start is called before the first frame update
    void Start()
    {
        player = PlayerController.Instance;

        currentHeartImages = startingHearts;

        attributePanelActive = false;
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
        if (InputManager.Instance.OpenAttributes())
        {
            ToggleAttributePanel();
        }

        if (InputManager.Instance.OpenMarkings())
        {
            ToggleMarkingsPanel();
        }
    }

    public void ToggleHUD(bool hudOn)
    {
        if (hudOn)
        {
            gameObject.SetActive(true);

            return;
        }

        gameObject.SetActive(false);
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

    void UpdateMarkings()
    {

    }

    void ToggleAttributePanel()
    {
        if (!attributePanelActive && (attributePanelTween == null || !attributePanelTween.IsActive()))
        {
            attributePanelTween = attributePanel.transform.DOMove(attributePanel.transform.position +
                (Vector3.right * (attributePanel.GetComponent<RectTransform>().rect.width - attributePanel.GetComponent<RectTransform>().rect.width * hiddenPanelVisibilityPercentage)), 0.5f);

            attributePanelActive = !attributePanelActive;

            return;
        }

        UpdateAttributes();

        if (attributePanelTween == null || !attributePanelTween.IsActive())
        {
            attributePanelTween = attributePanel.transform.DOMove(attributePanel.transform.position +
                (Vector3.left * (attributePanel.GetComponent<RectTransform>().rect.width - attributePanel.GetComponent<RectTransform>().rect.width * hiddenPanelVisibilityPercentage)), 0.5f);

            attributePanelActive = !attributePanelActive;
        }
    }

    void ToggleMarkingsPanel()
    {
        if (!markingsPanelActive && (markingsPanelTween == null || !markingsPanelTween.IsActive()))
        {
            markingsPanel.transform.DOMove(markingsPanel.transform.position +
                (Vector3.left * (markingsPanel.GetComponent<RectTransform>().rect.width - markingsPanel.GetComponent<RectTransform>().rect.width * hiddenPanelVisibilityPercentage)), 0.5f);

            markingsPanelActive = !markingsPanelActive;

            return;
        }

        UpdateMarkings();

        if (markingsPanelTween == null || !markingsPanelTween.IsActive())
        {
            markingsPanel.transform.DOMove(markingsPanel.transform.position +
                (Vector3.right * (markingsPanel.GetComponent<RectTransform>().rect.width - markingsPanel.GetComponent<RectTransform>().rect.width * hiddenPanelVisibilityPercentage)), 0.5f);

            markingsPanelActive = !markingsPanelActive;
        }
    }
}
