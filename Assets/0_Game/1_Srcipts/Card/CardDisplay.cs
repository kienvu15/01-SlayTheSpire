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

    [Header("Hand Selection")]
    private HandCardManager handManager;

    public CardInstance cardInstance;
    public CardData cardData => cardInstance?.data;
    public CardData GetCardData() => cardData;
    public CardInstance GetCardInstance() => cardInstance;

    [HideInInspector] public CanvasGroup blockerGroup;
    [HideInInspector] public CardHolder cardholder;
    private static CardDisplay currentlyOpen;

    private void Awake()
    {
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

        UpdateManaColor();
    }

    public void UpdateManaColor()
    {
        if (mana_ConsumeText == null || cardInstance == null) return;

        bool canPlay = ManaSystem.instance != null &&
                       ManaSystem.instance.CanPlayCard(cardInstance);

        mana_ConsumeText.color = canPlay ? Color.white : Color.red;
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

        }

        if (handManager != null)
        {
            handManager.SelectCard(gameObject);
        }
    }

    //Reward
    public void Init(CardData data, CardHolder holder)
    {
        currentMode = CardMode.Reward;
        cardholder = holder;
        LoadCard(data);
    }

}