using TMPro;
using UnityEngine;
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
    //public bool useScaleAnimation = false; 
    //public float scaleMultiplier = 1.15f;
    //public float animDuration = 0.2f;

    [Header("Hand Selection")]
    private HandCardManager handManager;

    public CardInstance cardInstance;
    public CardData cardData => cardInstance?.data;
    public CardData GetCardData() => cardData;
    public CardInstance GetCardInstance() => cardInstance;

    [HideInInspector] public CanvasGroup blockerGroup;
    [HideInInspector] public CardHolder cardholder;
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
        if (canvas != null)
        {
            canvas.sortingLayerName = "UI";
            canvas.sortingOrder = 601;
        }

        if (cardInstance != null)
            Refresh();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            TestUpgrade();
        }
    }

    private void TestUpgrade()
    {
        if (cardInstance == null)
        {
            Debug.LogWarning("CardInstance is null!");
            return;
        }

        if (!cardInstance.CanUpgrade)
        {
            Debug.LogWarning($"{cardInstance.Name} cannot upgrade! Current Level: {cardInstance.level}");
            return;
        }

        bool success = cardInstance.UpgradeTemp();

        if (success)
        {
            Refresh();

            Debug.Log($"UPGRADE SUCCESS: {cardInstance.Name} - Level {cardInstance.level}");
        }
    }

    public void LoadCard(CardData data) => LoadCard(new CardInstance(data));

    public void LoadCard(CardInstance instance)
    {
        cardInstance = instance;
        Refresh();
    }

    public void Refresh()
    {
        if (cardInstance == null) return;

        if (card_Name != null)
            card_Name.text = cardInstance.Name;

        if (mana_ConsumeText != null)
            mana_ConsumeText.text = cardInstance.ManaCost.ToString();

        if (artworkImage != null)
        {
            artworkImage.sprite = cardInstance.Artwork;

            RectTransform rect = artworkImage.rectTransform;
            rect.sizeDelta = cardData.artworkSize;
            rect.anchoredPosition = cardData.artworkPosition;
            rect.localRotation = Quaternion.Euler(
                0f, cardData.artworkRotation, 0f
            );
        }

        if (descriptionText != null)
            descriptionText.text = DescriptionFormatter.Format(cardInstance.Description);
    }

    public int GetIntentSum()
    {
        if (cardInstance == null || cardInstance.data.effects == null) return 0;

        var ctx = new EffectContext { level = cardInstance.level };

        int sum = 0;
        foreach (var wrapper in cardInstance.data.effects)
        {
            if (wrapper == null || wrapper.effect == null) continue;

            wrapper.FillContext(ctx);
            if (wrapper.effect.CountsAsIntend)
                sum += wrapper.effect.GetIntentValue(ctx);
        }
        return sum;
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
        newShopSystem = shop;
        LoadCard(data);

        if (goldContainer != null)
        {
            goldContainer.SetActive(true);
            goldText.text = data.goldCost.ToString();
        }
       // useScaleAnimation = true;
    }

    //Reward
    public void Init(CardData data, CardHolder holder)
    {
        currentMode = CardMode.Reward;
        cardholder = holder;
        LoadCard(data);
       // useScaleAnimation = true;
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
        //if (useScaleAnimation)
        //{
        //    transform.DOKill();
        //    transform.DOScale(originalScale, animDuration).SetEase(Ease.OutQuad);
        //    if (canvas != null) canvas.sortingOrder = 0;
        //}
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