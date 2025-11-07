using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardDisplay : MonoBehaviour
{
    [Header("UI References")]
    public TMP_Text titleText;

    public GameObject manaContainer;
    public TMP_Text manaText;

    public GameObject statsBG;
    public GameObject statsContainer;
    public TMP_Text statsText;

    [Header("Icon Database")]
    public CardTypeIconDatabase typeIconDatabase;
    public Image iconImage;
    public Image artworkImage;

    [Header("Description Panel")]
    public GameObject descriptionPanel;   // Panel con
    public TMP_Text descriptionText;      // Text trong panel
    public GameObject buyButton;

    private static CardDisplay currentlyOpen;
    private CanvasGroup blockerGroup;
    public  ShopSystem shopSystem;

    public CardData cardData;

    private Canvas canvas;
    private void Awake()
    {

        //button = GetComponentInChildren<Button>();
        //if (button != null)
        //    button.onClick.AddListener(OnCardClick);

        shopSystem = GetComponentInParent<ShopSystem>();
        if(shopSystem == null)
            buyButton.SetActive(false);

        if (descriptionPanel != null)
            descriptionPanel.SetActive(false);

        // Tìm Block-Panel trong scene
        GameObject blocker = GameObject.Find("Block-Panel");
        if (blocker != null)
        {
            blockerGroup = blocker.GetComponent<CanvasGroup>();
            if (blockerGroup == null)
                blockerGroup = blocker.AddComponent<CanvasGroup>();

           // HideBlocker(); // ẩn ngay từ đầu
        }

        BGHold bghold = GetComponentInParent<BGHold>();
        if (bghold == null)
        {
            Transform TG = GameObject.Find("PositionShowDiscription")?.transform;
            descriptionPanel.transform.SetParent(TG, false);
        }
    }

    public void Init(CardData data, ShopSystem shop)
    {
        cardData = data;
        shopSystem = shop;
        LoadCard(data);

        if (buyButton != null)
        {
            if (shopSystem != null)
            {
                buyButton.SetActive(true);
                buyButton.GetComponentInChildren<Button>().onClick.RemoveAllListeners();

                buyButton.GetComponentInChildren<Button>().onClick.AddListener(
                    () =>
                    {
                        shopSystem.BuyCard(cardData);

                        Button buyButtonBB = buyButton.GetComponent<Button>();
                        buyButtonBB.onClick.RemoveAllListeners();
                        buyButtonBB.interactable = false;
                        buyButtonBB.GetComponentInChildren<TMP_Text>().text = "SOLD";
                    }
                );
            }
            else
            {
                buyButton.SetActive(false);
            }
        }
    }

    void Start()
    {
        if (cardData == null) return;

        canvas = GetComponentInChildren<Canvas>();
        if(canvas != null)
        {
            canvas.sortingLayerName = "UI";
        }

        if (cardData.manaCost == 0)
        {
            manaContainer.SetActive(false);
        }

        else
        {
            manaContainer.SetActive(true);
            manaText.text = cardData.manaCost.ToString();
        }

        // Tính tổng intent từ EffectData
        int intentSum = 0;
        if (cardData.effects != null)
        {
            foreach (var wrapper in cardData.effects)
            {
                if (wrapper == null || wrapper.effect == null) continue;

                if (wrapper.overrideValue && wrapper.effect is IOverrideValue overridable)
                {
                    intentSum += overridable.GetIntentValue(wrapper.valueOverride);
                }
                else
                {
                    intentSum += wrapper.effect.GetIntentValue();
                }
            }

        }

        // Nếu có giá trị thì hiện stats, không thì ẩn
        if (intentSum > 0)
        {
            statsBG.SetActive(true);
            statsContainer.SetActive(true);
            statsText.text = intentSum.ToString();
        }
        else
        {
            statsBG.SetActive(false);
            statsContainer.SetActive(false);
        }
    }

    public void LoadCard(CardData data)
    {
        cardData = data;

        // Tên và mana
        titleText.text = data.cardName;
        manaText.text = data.manaCost.ToString();

        // Artwork
        artworkImage.sprite = data.artwork;

        // Icon: tự động lấy từ database
        if (typeIconDatabase != null)
        {
            Sprite typeIcon = typeIconDatabase.GetIcon(data.cardType);
            if (typeIcon != null)
            {
                iconImage.sprite = typeIcon;
                iconImage.enabled = true;
            }
            else
            {
                iconImage.enabled = false;
            }
        }
        else
        {
            iconImage.enabled = false;
        }

        // Stats
        string stats = "";

        int intentSum = 0;
        if (data.effects != null)
        {
            foreach (var wrapper in cardData.effects)
            {
                if (wrapper == null || wrapper.effect == null) continue;

                if (wrapper.overrideValue && wrapper.effect is IOverrideValue overridable)
                {
                    intentSum += overridable.GetIntentValue(wrapper.valueOverride);
                }
                else
                {
                    intentSum += wrapper.effect.GetIntentValue();
                }
            }

        }

        if (intentSum > 0)
        {
            statsBG.SetActive(true);
            statsContainer.SetActive(true);
            statsText.text = intentSum.ToString();
        }
        else
        {
            statsBG.SetActive(false);
            statsContainer.SetActive(false);
        }

        statsText.text = stats.Trim();

        // Description text (panel riêng)
        if (descriptionText != null)
            descriptionText.text = data.description;
    }


    public CardData GetCardData()
    {
        return cardData;
    }

    public void OnCardClick()
    {
        if (descriptionPanel == null) return;

        // Nếu đang có card khác mở → đóng nó
        if (currentlyOpen != null && currentlyOpen != this)
            currentlyOpen.HideDescription();

        bool newState = !descriptionPanel.activeSelf;
        descriptionPanel.SetActive(newState);

        if (newState)
        {
            currentlyOpen = this;
            ShowBlocker();
        }
        else
        {
            currentlyOpen = null;
            HideBlocker();
        }
    }

    public void HideDescription()
    {
        if (descriptionPanel != null)
            descriptionPanel.SetActive(false);

        if (currentlyOpen == this)
            currentlyOpen = null;

        HideBlocker();
    }

    private void ShowBlocker()
    {
        if (blockerGroup != null)
        {
            blockerGroup.alpha = 0; // vẫn trong suốt
            blockerGroup.blocksRaycasts = true;  // chặn click
            blockerGroup.interactable = true;
        }
    }

    private void HideBlocker()
    {
        if (blockerGroup != null)
        {
            blockerGroup.blocksRaycasts = false; // cho click xuyên qua
            blockerGroup.interactable = false;
        }
    }
}
