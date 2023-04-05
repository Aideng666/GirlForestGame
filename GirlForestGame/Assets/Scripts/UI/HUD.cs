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
    [SerializeField] TextMeshProUGUI healthText;
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
    [SerializeField] Image[] markingImages = new Image[4]; // 0 = Sword Attribute | 1 = Bow Attribute | 2 = Sword Element | 3 = Bow Element
    bool markingsPanelActive = true;

    [Header("Totems Panel")]
    [SerializeField] GameObject totemPanel;
    [SerializeField] Image totemImage;
    [SerializeField] TextMeshProUGUI totemName;
    [SerializeField] TextMeshProUGUI totemDescription;

    [Header("Player Stuff")]
    [SerializeField] TextMeshProUGUI coinText;
    [SerializeField] Image planeImage;
    [SerializeField] Sprite terrestrialSprite;
    [SerializeField] Sprite astralSprite;

    PlayerController player;

    Tween attributePanelTween = null;
    Tween markingsPanelTween = null;

    public static HUD Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        player = PlayerController.Instance;

        currentHeartImages = startingHearts;

        attributePanelActive = false;

        planeImage.sprite = terrestrialSprite;
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

        // Displays player's current money
        coinText.text = player.playerInventory.GetMoneyAmount().ToString();

        // Displays the player's current plane they are in
        if (player.playerCombat.Form == Planes.Terrestrial)
        {
            planeImage.sprite = terrestrialSprite;
        }
        else if (player.playerCombat.Form == Planes.Astral)
        {
            planeImage.sprite = astralSprite;
        }

        UpdateAttributes();
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
        healthText.text = (player.playerAttributes.MaxHealth).ToString();
        speedText.text = (player.playerAttributes.Speed).ToString();
        swdDmgText.text = (player.playerAttributes.SwordDamage).ToString();
        bowDmgText.text = (player.playerAttributes.BowDamage).ToString();
        swdCdnText.text = (player.playerAttributes.SwordCooldown).ToString();
        bowCdnText.text = (player.playerAttributes.BowCooldown).ToString();
        swdRangeText.text = (player.playerAttributes.SwordRange).ToString();
        projSpdText.text = (player.playerAttributes.ProjectileSpeed).ToString();
        critChanceText.text = (player.playerAttributes.CritChance).ToString();
        bowChargeSpdText.text = (player.playerAttributes.BowChargeTime).ToString();
        luckText.text = (player.playerAttributes.Luck).ToString();
    }

    public void UpdateMarkingsPanel(Sprite markingSprite, int index)
    {
        markingImages[index].sprite = markingSprite;
    }

    public void UpdateTotemHUD(Sprite totemSprite, string totemName, string totemDesc)
    {
        Sequence sequence = DOTween.Sequence();

        totemImage.sprite = totemSprite;
        this.totemName.text = totemName;
        totemDescription.text = totemDesc;

        sequence.Append(totemPanel.transform.DOScale(1f, 0.8f)).AppendInterval(3f).Append(totemPanel.transform.DOScale(0f, 0.8f)); 
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

        //UpdateAttributes();

        if (attributePanelTween == null || !attributePanelTween.IsActive())
        {
            attributePanelTween = attributePanel.transform.DOMove(attributePanel.transform.position +
                (Vector3.left * (attributePanel.GetComponent<RectTransform>().rect.width - attributePanel.GetComponent<RectTransform>().rect.width * hiddenPanelVisibilityPercentage)), 0.5f);

            attributePanelActive = !attributePanelActive;
        }
    }

    void ToggleMarkingsPanel()
    {
        //UpdateMarkings();

        if (!markingsPanelActive && (markingsPanelTween == null || !markingsPanelTween.IsActive()))
        {
            markingsPanel.transform.DOMove(markingsPanel.transform.position +
                (Vector3.right * (markingsPanel.GetComponent<RectTransform>().rect.width - markingsPanel.GetComponent<RectTransform>().rect.width * hiddenPanelVisibilityPercentage)), 0.5f);

            markingsPanelActive = !markingsPanelActive;

            return;
        }

        if (markingsPanelTween == null || !markingsPanelTween.IsActive())
        {
            markingsPanel.transform.DOMove(markingsPanel.transform.position +
                (Vector3.left * (markingsPanel.GetComponent<RectTransform>().rect.width - markingsPanel.GetComponent<RectTransform>().rect.width * hiddenPanelVisibilityPercentage)), 0.5f);

            markingsPanelActive = !markingsPanelActive;
        }
    }
}
