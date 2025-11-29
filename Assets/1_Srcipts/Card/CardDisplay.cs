using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using NUnit.Framework;

public enum CardMode
{
    Shop,       
    Reward,     
    ViewOnly    
}

public class CardDisplay : MonoBehaviour, IHasDescriptionPanel
{
    [Header("Mode")]
    public CardMode currentMode = CardMode.ViewOnly;

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
    public GameObject descriptionPanel;   
    public TMP_Text descriptionText;

    [Header("Shop UI")]
    public GameObject goldContainer;
    public TMP_Text goldText;
    public NewShopSystem newShopSystem;

    [Header("Animation Settings")]
    public bool useScaleAnimation = false; 
    public float scaleMultiplier = 1.15f;
    public float animDuration = 0.2f;

    private static CardDisplay currentlyOpen;
    [HideInInspector] public CanvasGroup blockerGroup;
    public CardHolder cardholder;

    public CardData cardData;

    private Canvas canvas;
    private Vector3 originalScale;

    void OnEnable() => DescriptionPanelManager.Instance.Register(this);
    void OnDisable() => DescriptionPanelManager.Instance.Unregister(this);

    public GameObject GetDescriptionPanel() => descriptionPanel;
    public GameObject GetRootObject() => gameObject;
    public void vHideDescription() => HideDescription();

    private void Awake()
    {
        originalScale = transform.localScale;

        if (descriptionPanel != null)
            descriptionPanel.SetActive(false);

        //GameObject blocker = GameObject.Find("Block-Panel");
        //if (blocker != null)
        //{
        //    blockerGroup = blocker.GetComponent<CanvasGroup>();
        //    if (blockerGroup == null)
        //        blockerGroup = blocker.AddComponent<CanvasGroup>();
        //}

        cardholder = FindFirstObjectByType<CardHolder>();
        goldContainer.SetActive(false);
        blockerGroup = DescriptionPanelManager.Instance.blockerGroup;
    }

    //SHOP
    public void InitShop(CardData data, NewShopSystem shop)
    {
        currentMode = CardMode.Shop;
        cardData = data;
        newShopSystem = shop;
        LoadCard(data);
        if (goldContainer != null)
        {
            goldContainer.SetActive(true);
            goldText.text = data.goldCost.ToString();
        }
        useScaleAnimation = true;
    }

    //Reward
    public void Init(CardData data, CardHolder holder)
    {
        currentMode = CardMode.Reward; // Đặt chế độ Reward
        cardData = data;
        cardholder = holder;

        LoadCard(data);

        // Bật animation scale cho đẹp
        useScaleAnimation = true;
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

        descriptionPanel.GetComponent<Canvas>().sortingLayerName = "UI";
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

    private void ConfirmPickReward()
    {
        if (cardholder != null)
        {
           
            if (RandomCardSystem.Instance != null)
            {
            }

            RectTransform target = UIManager.Instance.deckIcon; 

            UIManager.Instance.AnimateCardFly(
                this.GetComponent<RectTransform>(),
                target,
                () => {
                    RandomCardSystem.Instance.StartCoroutine(RandomCardSystem.Instance.OnCardSelected(this));

                }
            );
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(false);
            }

            HideDescription();
        }
    }

    public CardData GetCardData()
    {
        return cardData;
    }

    public void ToggleDescription()
    {
        if (descriptionPanel == null) return;

        // Đóng card khác
        if (currentlyOpen != null && currentlyOpen != this)
            currentlyOpen.HideDescription();

        bool isOpening = !descriptionPanel.activeSelf;
        descriptionPanel.SetActive(isOpening);

        if (isOpening)
        {
            currentlyOpen = this;
            ShowBlocker();

            // Hiệu ứng Scale (áp dụng cho cả Shop lẫn Reward nếu muốn)
            if (useScaleAnimation)
            {
                transform.DOKill();
                transform.DOScale(originalScale * scaleMultiplier, animDuration).SetEase(Ease.OutBack);
                if (canvas != null) canvas.sortingOrder = 10;
            }
        }
        else
        {
            // Đóng lại
            currentlyOpen = null;
            HideBlocker();
            ResetScaleAnim();
        }
    }

    public void OnCardClick()
    {
        SoundManager.Instance.Play("SelectButton");

        if (currentMode == CardMode.Shop)
        {
            if (newShopSystem != null)
            {
                newShopSystem.SelectCard(this);
                return;
            }
        }

        if (currentMode == CardMode.Reward)
        {
            if (currentlyOpen == this && descriptionPanel.activeSelf)
            {
                ConfirmPickReward();
                return;
            }
        }
        ToggleDescription();
    
    }

    public void HideDescription()
    {
        if (descriptionPanel != null)
            descriptionPanel.SetActive(false);

        if (currentlyOpen == this)
            currentlyOpen = null;

        HideBlocker();
        ResetScaleAnim();
    }

    private void ResetScaleAnim()
    {
        if (useScaleAnimation)
        {
            transform.DOKill();
            transform.DOScale(originalScale, animDuration).SetEase(Ease.OutQuad);
            if (canvas != null) canvas.sortingOrder = 0;
        }
    }


    private void ShowBlocker()
    {
        if (blockerGroup != null)
        {
            blockerGroup.alpha = 0; 
            blockerGroup.blocksRaycasts = true; 
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
