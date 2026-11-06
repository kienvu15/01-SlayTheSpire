using TMPro;
using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.UI;

public enum CardMode
{
    Shop,       
    Reward,     
    ViewOnly    
}

public class CardDisplay : MonoBehaviour
{
    [Header("Mode")]
    public CardMode currentMode = CardMode.ViewOnly;

    [Header("UI References")]
    public TMP_Text card_Name;
    public Image artworkImage;
    public GameObject outline_Glow;
    public GameObject mana_Container;
    public TMP_Text mana_ConsumeText;  
    public TMP_Text descriptionText;
    public Canvas canvas;

    [Header("Button")]
    public Button card_Button;

    [Header("Shop UI")]
    public GameObject goldContainer;
    public TMP_Text goldText;
    public NewShopSystem newShopSystem;

    [Header("Animation Settings")]
    public bool useScaleAnimation = false; 
    public float scaleMultiplier = 1.15f;
    public float animDuration = 0.2f;

    [Header("Hand Selection")]
    private HandCardManager handManager;

    [HideInInspector] public CanvasGroup blockerGroup;
    [HideInInspector] public CardHolder cardholder;
    [HideInInspector] public CardData cardData;
    private static CardDisplay currentlyOpen;
    private Vector3 originalScale;

    private void Awake()
    {
        originalScale = transform.localScale;
        handManager = GetComponentInParent<HandCardManager>();
        cardholder = FindAnyObjectByType<CardHolder>();
    }

    void Start()
    {
        if (cardData == null) return;
        canvas.sortingLayerName = "UI";
        canvas.sortingOrder = 601;

        mana_ConsumeText.text = cardData.manaCost.ToString();

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
    }

    public void LoadCard(CardData data)
    {
        cardData = data;
        // Tên và mana
        card_Name.text = data.cardName;
        mana_ConsumeText.text = data.manaCost.ToString();
        // Artwork
        artworkImage.sprite = data.artwork;
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
        // Description text
        if (descriptionText != null)
            descriptionText.text = DescriptionFormatter.Format(data.description);
    }

    public CardData GetCardData()
    {
        return cardData;
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
            //if (currentlyOpen == this && descriptionPanel.activeSelf)
            //{
            //    ConfirmPickReward();
            //    return;
            //}
        }

        if (handManager != null)
        {
            handManager.SelectCard(gameObject);
        }
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

            //HideDescription();
        }
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

    public void ToggleDescription()
    {
        //if (descriptionPanel == null) return;

        // Đóng card khác
        //if (currentlyOpen != null && currentlyOpen != this)
          //  currentlyOpen.HideDescription();

        //bool isOpening = !descriptionPanel.activeSelf;
        //descriptionPanel.SetActive(isOpening);

        //if (isOpening)
        //{
        //    currentlyOpen = this;
        //    ShowBlocker();

        //    // Hiệu ứng Scale (áp dụng cho cả Shop lẫn Reward nếu muốn)
        //    if (useScaleAnimation)
        //    {
        //        transform.DOKill();
        //        transform.DOScale(originalScale * scaleMultiplier, animDuration).SetEase(Ease.OutBack);
        //        if (canvas != null) canvas.sortingOrder = 10;
        //    }
        //}
        //else
        //{
        //    // Đóng lại
        //    currentlyOpen = null;
        //    HideBlocker();
        //    ResetScaleAnim();
        //}
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
