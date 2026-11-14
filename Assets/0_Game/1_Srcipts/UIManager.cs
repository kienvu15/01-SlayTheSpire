using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("PlayerHand_Content")]
    [SerializeField] private Button bag_Button;
    [SerializeField] public Button endTurn_Button;
    [SerializeField] private GameObject drawPile_Button;
    [SerializeField] private GameObject discardPile_Button;
    [SerializeField] private Button enterRoom_Button;
    [SerializeField] private Button backMap_Button;

    [Header("CardInfo_Help")]
    [SerializeField] private GameObject card_NameT;
    [SerializeField] private GameObject manaObject;
    [SerializeField] private GameObject manaTextT;
    [SerializeField] public GameObject default_Disciption;
    [SerializeField] private GameObject upgrade_Disciption;
    [SerializeField] private TextMeshProUGUI goldTxt;

    [Header("Bag Content Panels")]
    public GameObject[] contentPanels;
    [SerializeField] private Button Inventory_Button;
    [SerializeField] private Button Map_Button;
    [SerializeField] private Button Deck_Button;

    [Header("Context Visual")]
    [SerializeField] private GameObject Map_Visual_GameObject;
    [SerializeField] private GameObject Bag_Content_Object;
    [SerializeField] private GameObject Player_Hand;

    [Header("Ref")]
    public MidMatch_Manager midMatch_Manager;

    [Header("UI Panels")]
    public GameObject battleHolder;
    public GameObject Bg;
//    public GameStage_Manager GameStage_Manager;

    [Header("Floor")]
    private int floor;
    public TextMeshProUGUI floorCount;
    public GameObject BG;
    public GameObject BattlleUIHolder;

    [Header("UI MapPanel")]
    public List<GameObject> backMap;
    public GameObject MapOBB;
    public GameObject MapStuffs;
    public GameObject ToolImage;
    public GameObject blackPanel;
    [SerializeField] private RectTransform buttonEnterParent;
    [SerializeField] private PanCamera panCamera;
    private Vector2 originEnterButtonAnchoredPos;
    public MapUIManager mapUiManager;

    [Header("Deck")]
    public GameObject deckUI;
    public RectTransform deckIcon;

    [Header("PileDeck")]
    public GameObject pileDeck;
    public Deck deck;
    public ParticleSystem pileDeckParticle;

    [Header("Discard")]
    public GameObject discard;
    public Discard discardScript;
    public ParticleSystem discardParticle;

    [Header("Loot")]
    public GameObject lootHolder;
    public GameObject lootPanel;
    [SerializeField] private GameObject pickMeCardPanel;

    [Header("Shop")]
    public GameObject Shop;
    public GameObject ShopDialogue;
    public RectTransform serviceButton;
    public GameObject leaveShopButton;
    public GameObject removeService;
    public NewShopSystem newShopSystem;
    public GameObject PlaySelftCast;

    [Header("Event")]
    public GameObject tutorialCanvas;

    [Header("Button")]
    public GameObject HandPlayerBottom;
    public RectTransform endButton;
    public RectTransform manaHolder;
    public RectTransform hand;
    private Vector2 originalHandPos;
    private Vector2 originalEndButtonPos;
    private Vector2 originalManaHolderPos;

    [Header("Scale Icon")]
    [SerializeField] private Transform goldIcon;

    [Header("Animation Settings")]
    public GameObject flyingCardPrefab;
    public Transform animationLayer;

    [Header("UI Layer")]
    public Image transitionImage;
    public event System.Action OnTransitionFilled;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        //if (animationLayer == null) animationLayer = transform;
    }

    void Start()
    {
        backMap_Button.onClick.AddListener(BackButton);
        bag_Button.onClick.AddListener(BagButton_CallBack);

        Inventory_Button.onClick.AddListener(() => ShowContent(0));
        Map_Button.onClick.AddListener(() => ShowContent(1));
        Deck_Button.onClick.AddListener(() => ShowContent(2));
    }

    #region LeftHand_Content
    public void LeftHand_Content(bool group1, bool group2)
    {
        endTurn_Button.gameObject.SetActive(group1);
        drawPile_Button.SetActive(group1);
        discardPile_Button.SetActive(group1);

        enterRoom_Button.gameObject.SetActive(group2);
        backMap_Button.gameObject.SetActive(group2);
    }

    public void Enter_Room_Button_Helper(bool interact)
    {
        enterRoom_Button.interactable = interact;
        TextMeshProUGUI buttonText = enterRoom_Button.GetComponentInChildren<TextMeshProUGUI>();

        buttonText.alpha = interact ? 1f : 0.5f;
    }

    public void PlayerHand(bool stage)
    {
        Player_Hand.SetActive(stage);
    }

    //Button Call
    public void BackButton()
    {
        LeftHand_Content(true, false);
        Enter_Room_Button_Helper(false);
        Map_Visual_GameObject.SetActive(false);
    }
    #endregion
    #region CardInfo_Help
    public void CardInfo_Help(bool stage)
    {
        card_NameT.SetActive(stage);
        manaObject.SetActive(stage);
        manaTextT.SetActive(stage);
        default_Disciption.SetActive(stage);
        upgrade_Disciption.SetActive(stage);
    }
    public void Init_CardInfor(CardDisplay cardDisplay)
    {
        card_NameT.GetComponent<TextMeshProUGUI>().text = cardDisplay.card_Name.text;
        manaTextT.GetComponent<TextMeshProUGUI>().text = cardDisplay.mana_ConsumeText.text;
        default_Disciption.GetComponent<TextMeshProUGUI>().text = "Default: " + cardDisplay.descriptionText.text;
    }
    public void Init_CardInfor(CardDisplay cardDisplay, CardData data)
    {
        card_NameT.GetComponent<TextMeshProUGUI>().text = cardDisplay.card_Name.text;
        manaTextT.GetComponent<TextMeshProUGUI>().text = cardDisplay.mana_ConsumeText.text;
        default_Disciption.GetComponent<TextMeshProUGUI>().text = "Default: " + cardDisplay.descriptionText.text;
        upgrade_Disciption.GetComponent<TextMeshProUGUI>().text = "Upgrade: " + cardDisplay.descriptionText.text;
        goldTxt.text = data.goldCost.ToString();
    }
    #endregion
    #region RelicInfo_Help
    public void RelicInfo_Help(bool stage)
    {
        card_NameT.SetActive(stage);
        //manaObject.SetActive(stage);
        //manaTextT.SetActive(stage);
        default_Disciption.SetActive(stage);
        upgrade_Disciption.SetActive(stage);
    }
    public void Init_RelicInfor(RelicInstance instance)
    {
        if (instance == null || instance.relicData == null) return;

        Relic relic = instance.relicData;
        card_NameT.GetComponent<TextMeshProUGUI>().text = relic.relicName;
        default_Disciption.GetComponent<TextMeshProUGUI>().text =
            relic.defaultDescription;
        upgrade_Disciption.GetComponent<TextMeshProUGUI>().text =
            relic.upgradeDescription;
    }
    #endregion
    #region Bag
    public void BagButton_CallBack()
    {
        Bag_Content_Object.SetActive(!Bag_Content_Object.activeSelf);
        //LeftHand_Content(false, false);
    }
    public void ShowContent(int index)
    {
        for (int i = 0; i < contentPanels.Length; i++)
        {
            contentPanels[i].SetActive(i == index);
        }

        if(index == 1)
        {
            LeftHand_Content(false, true);
        }
    }
    #endregion
    #region Pile_Helper_Scroll View
    public void LeftHand_Content_Help(bool group1, bool group2)
    {
        endTurn_Button.gameObject.SetActive(group1);
        endTurn_Button.GetComponentInChildren<TextMeshProUGUI>().text = "Take".ToString();

        drawPile_Button.SetActive(group2);
        discardPile_Button.SetActive(group2);
        enterRoom_Button.gameObject.SetActive(group2);
        backMap_Button.gameObject.SetActive(group2);
    }
    public void Context(bool stage, bool stage2)
    {
        card_NameT.SetActive(stage2);
        manaObject.SetActive(stage2);
        manaTextT.SetActive(stage2);

        default_Disciption.SetActive(stage);
        default_Disciption.GetComponent<TextMeshProUGUI>().text = "Choose your path".ToString();

        upgrade_Disciption.SetActive(stage2);
    }
    #endregion
    #region Button Helper
    public void EndTurn_OnClick()
    {
        endTurn_Button.onClick.AddListener(() => midMatch_Manager.EndTurn());
    }
    #endregion






    #region MapPanel
    //public void BackButton()
    //{
    //    SoundManager.Instance.Play("SelectButton", null, 1);

    //    foreach (GameObject button in backMap)
    //    {
    //        button.SetActive(!button.activeSelf);
    //    }

    //    if (Mathf.Abs(buttonEnterParent.anchoredPosition.x - 380.33f) > 0.01f)
    //    {
    //        buttonEnterParent.anchoredPosition = new Vector2(originEnterButtonAnchoredPos.x, buttonEnterParent.anchoredPosition.y);
    //    }
    //    else
    //    {
    //        buttonEnterParent.anchoredPosition = new Vector2(380.33f, buttonEnterParent.anchoredPosition.y);
    //    }

    //    if (GameStage_Manager.Instance.isOnBattle == true)
    //    {
    //        buttonEnterParent.gameObject.SetActive(false);
    //    }
    //}

    public void MapIcon()
    {
        if (GameStage_Manager.Instance.isOnBattle)
        {
            mapUiManager.MapInBattleScene();
        }
        SoundManager.Instance.Play("SelectButton", null, 1);

        deckUI.SetActive(false);

        bool isMapOpening = !backMap[0].activeSelf; 

        foreach (GameObject button in backMap)
        {
            button.SetActive(isMapOpening); 
        }

        if (GameStage_Manager.Instance.isOnBattle)
        {
            buttonEnterParent.gameObject.SetActive(false);
        }
        else
        {
            buttonEnterParent.gameObject.SetActive(isMapOpening);

            if (isMapOpening)
            {
                buttonEnterParent.anchoredPosition =
                    new Vector2(originEnterButtonAnchoredPos.x, buttonEnterParent.anchoredPosition.y);
            }
        }

        if (tutorialCanvas != null)
        {
            Destroy(tutorialCanvas);
            tutorialCanvas = null;
        }
    }

    #endregion

    #region DeckButton
    public void DeckButton()
    {
        SoundManager.Instance.Play("SelectButton", null, 1);
        foreach (GameObject button in backMap)
        {
            button.SetActive(false);
        }
        deckUI.SetActive(true);
    }
    public void BackDeckButton()
    {
        SoundManager.Instance.Play("SelectButton", null, 1);
        foreach (GameObject button in backMap)
        {
            button.SetActive(false);
        }
        deckUI.SetActive(false);
    }
    #endregion

    #region Transition
    public void FillOverTime()
    {
        transitionImage.gameObject.SetActive(true);
        StartCoroutine(FillCoroutine());
    }

    private IEnumerator FillCoroutine()
    {
        float elapsed = 0f;
        float duration = 1f;
        transitionImage.fillAmount = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            transitionImage.fillAmount = Mathf.Clamp01(elapsed / duration);
            yield return null;
        }

        transitionImage.fillAmount = 1f;
        if (GameStage_Manager.Instance.isFirstBattle == true)
        {
            UIManager.Instance.removeBattleCanvas();
            GameStage_Manager.Instance.isFirstBattle = false;
        }
        else if (GameStage_Manager.Instance.isFirstBattle == false 
            && GameSystem.Instance.isBattlePhase == true)
        {
            Destroy(BattlleUIHolder.transform.GetChild(0).gameObject);
        }
            OnTransitionFilled?.Invoke();
    }

    public void FadeOutOverTime()
    {
        hand.anchoredPosition = originalHandPos;
        endButton.anchoredPosition = originalEndButtonPos;
        manaHolder.anchoredPosition = originalManaHolderPos;
        discard.SetActive(true); pileDeck.SetActive(true);

        StartCoroutine(FadeOutCoroutine());
    }

    private IEnumerator FadeOutCoroutine()
    {
        float elapsed = 0f;
        float duration = 1.5f;
        Color color = transitionImage.color;
        float startAlpha = color.a;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            color.a = Mathf.Lerp(startAlpha, 0f, t);
            transitionImage.color = color;
            yield return null;
        }

        transitionImage.gameObject.SetActive(false);
        color.a = 1f;
        transitionImage.color = color;
    }

    #endregion

    #region animation deck after battle
    public void PlayPartical()
    {
        pileDeckParticle.Play();
        discardParticle.Play();
        StartCoroutine(DestroyAfterAnimation());
    }

    private IEnumerator DestroyAfterAnimation()
    {
        yield return new WaitForSeconds(pileDeckParticle.main.duration);
        pileDeckParticle.Stop();
        discardParticle.Stop();
        pileDeck.SetActive(false);
        discard.SetActive(false);
        yield return new WaitForSeconds(0.5f);
        lootPanel.SetActive(true);
    }

    public IEnumerator AnimationButtonMoveAfterBattle()
    {
        Vector2 endButtonstartPos = endButton.anchoredPosition;
        Vector2 manaStartPos = manaHolder.anchoredPosition;

        Vector2 endButtontargetPos = endButtonstartPos + new Vector2(1153f, 0f);
        Vector2 manaTargetPos = manaStartPos + new Vector2(-355f, 0f);

        float duration = 0.5f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            endButton.anchoredPosition = Vector2.Lerp(endButtonstartPos, endButtontargetPos, t);
            manaHolder.anchoredPosition = Vector2.Lerp(manaStartPos, manaTargetPos, t);

            yield return null;
        }
        endButton.anchoredPosition = endButtontargetPos;
    }

    public IEnumerator MoveHandToBottom()
    {
        Vector2 handStartPos = hand.anchoredPosition;
        Vector2 handTargetPos = handStartPos + new Vector2(0f, -360f);

        float duration = 0.5f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            hand.anchoredPosition = Vector2.Lerp(handStartPos, handTargetPos, t);

            yield return null;
        }
        hand.anchoredPosition = handTargetPos;
    }

    #endregion

    #region loot
    public void DeactivePickMeCardPanel()
    {
        SoundManager.Instance.Play("SelectButton");
        lootPanel.SetActive(false);
        MapOBB.SetActive(true);
        foreach(var obj in backMap)
        {
            obj.gameObject.SetActive(true);
        }
    }
    public void DeactiveLootPanel()
    {
        if (lootPanel != null)
        {
            lootPanel.SetActive(false);
        }
    }

    public void CheckAndCloseLootPanel(Transform holder)
    {
        StartCoroutine(CheckAndCloseLootPanelCoroutine(holder));
    }

    private IEnumerator CheckAndCloseLootPanelCoroutine(Transform holder)
    {
        yield return null;

        if (holder != null && holder.childCount <= 0)
        {
            Debug.Log("Card loot finished. Loot panel closed.");
            DeactiveLootPanel();
        }
    }

    #endregion

    #region aniamtion pick me card
    public void ActivePickMeCardPanel()
    {
        pickMeCardPanel.SetActive(true);
    }
    public void AnimateCardFly(
    RectTransform startCard,
    RectTransform targetRect,
    System.Action onComplete = null)
    {
        if (startCard == null || targetRect == null) return;

        RectTransform cardRect = startCard;

        Vector3 startWorldPos = cardRect.position;
        Vector3 targetWorldPos = targetRect.position;

        // Đưa card lên Canvas animation
        cardRect.SetParent(animationLayer, true);
        cardRect.SetAsLastSibling();

        cardRect.position = startWorldPos;

        cardRect.DOKill();

        Sequence seq = DOTween.Sequence();

        // Bay đến Deck
        seq.Join(
            cardRect.DOMove(targetWorldPos, 0.8f)
                .SetEase(Ease.InBack)
        );

        // Thu nhỏ dần
        seq.Join(
            cardRect.DOScale(Vector3.zero, 0.8f)
                .SetEase(Ease.InBack)
        );

        seq.OnComplete(() =>
        {
            Destroy(cardRect.gameObject);
            onComplete?.Invoke();
        });
    }
    #endregion

    #region aniamtion gold collect
    public void AnimationGold()
    {
        goldIcon.DOKill();
        goldIcon.localScale = Vector3.one;
        goldIcon.DOScale(1.2f, 0.15f)
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                goldIcon.DOScale(1f, 0.15f).SetEase(Ease.InQuad);
            });
    }
    #endregion

    #region Shop

    public void SkipShop()
    {
        for(int i = 0; i< backMap.Count; i++)
        {
            backMap[i].SetActive(true);
        }
    }

    public void EnterShop()
    {
        PlaySelftCast.SetActive(true);
        Shop.SetActive(true);
    }

    public void LevaveShop()
    {
        newShopSystem.DeselectCurrentCard();

       

    }

    



    public void RemoveCardService()
    {
        LevaveShop();
        removeService.SetActive(true);
    }

    public void LeaveRemoveService()
    {
        removeService.SetActive(false);
    }

    #endregion

    #region BattleBg
    public void removeBattleCanvas()
    {
        BG.SetActive(false);
    }
    #endregion

    #region Floor
    public void CrawlerFloor()
    {
        floor++;
        floorCount.text = floor + "th Floor";
    }
    #endregion

    #region RestEvent

    #endregion
}
