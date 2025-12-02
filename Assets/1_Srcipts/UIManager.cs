using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public GameObject Map;

    [Header("UI Panels")]
    public GameObject battleHolder;
    public GameObject Bg;
    public GameFlowManager GameFlowManager;

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
        if (animationLayer == null) animationLayer = transform;
    }

    void Start()
    {
        originEnterButtonAnchoredPos = buttonEnterParent.anchoredPosition;
        originalHandPos = hand.anchoredPosition;
        originalEndButtonPos = endButton.anchoredPosition;
        originalManaHolderPos = manaHolder.anchoredPosition;
    }

    #region MapPanel
    public void BackButton()
    {
        SoundManager.Instance.Play("SelectButton", null, 1);

        foreach (GameObject button in backMap)
        {
            button.SetActive(!button.activeSelf);
        }

        if (Mathf.Abs(buttonEnterParent.anchoredPosition.x - 380.33f) > 0.01f)
        {
            buttonEnterParent.anchoredPosition = new Vector2(originEnterButtonAnchoredPos.x, buttonEnterParent.anchoredPosition.y);
        }
        else
        {
            buttonEnterParent.anchoredPosition = new Vector2(380.33f, buttonEnterParent.anchoredPosition.y);
        }

        if (GameFlowManager.Instance.isOnBattle == true)
        {
            buttonEnterParent.gameObject.SetActive(false);
        }
    }

    public void MapIcon()
    {
        if (GameFlowManager.Instance.isOnBattle)
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

        if (GameFlowManager.Instance.isOnBattle)
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

    public void mapAfterBattle()
    {

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
        if (GameFlowManager.Instance.isFirstBattle == true)
        {
            UIManager.Instance.removeBattleCanvas();
            GameFlowManager.Instance.isFirstBattle = false;
        }
        else
        {
            Destroy(
            BattlleUIHolder.transform.GetChild(0).gameObject);
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
    public void AnimateCardFly(RectTransform startCard, RectTransform targetRect, System.Action onComplete = null)
    {
        GameObject dummyCard = Instantiate(startCard.gameObject, animationLayer);

        Destroy(dummyCard.GetComponent<CardDisplay>());
        Destroy(dummyCard.GetComponent<Button>());

        RectTransform dummyRect = dummyCard.GetComponent<RectTransform>();

        Vector3 screenPos = Camera.main.WorldToScreenPoint(startCard.position);
        dummyRect.position = screenPos;

        dummyRect.rotation = Quaternion.identity; 
        dummyRect.localScale = Vector3.one;     
        
        Sequence seq = DOTween.Sequence();

        seq.Join(dummyRect.DOMove(targetRect.position, 0.8f).SetEase(Ease.InBack));

        seq.Join(dummyRect.DOScale(Vector3.zero, 0.8f).SetEase(Ease.InBack));

        seq.Join(dummyRect.DORotate(new Vector3(0, 0, 360), 0.8f, RotateMode.FastBeyond360));

        seq.OnComplete(() =>
        {
            Destroy(dummyCard);
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
    public void EnterShop()
    {
        PlaySelftCast.SetActive(true);
        Shop.SetActive(true);
    }

    public void LevaveShop()
    {
        newShopSystem.DeselectCurrentCard();
        if (serviceButton.anchoredPosition != newShopSystem.buyButtonOriginalPos)
        {
            newShopSystem.StartCoroutine(newShopSystem.ButtonMovetoLeft());
        }
        StartCoroutine(LeaveDialugue());
        StartCoroutine(LeaveButton());
        StartCoroutine(LeaveShopBGImage());

    }

    public IEnumerator LeaveButton()
    {
        Vector2 startPos = newShopSystem.leaveButtonRect.anchoredPosition;
        Vector2 targetPos = startPos - new Vector2(325f, 0f);

        float duration = 0.4f;
        float elapsed = 0f;


        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            newShopSystem.leaveButtonRect.anchoredPosition = Vector2.Lerp(startPos, targetPos, t);
            yield return null;
        }

        newShopSystem.leaveButtonRect.anchoredPosition = targetPos;
    }

    public IEnumerator LeaveDialugue()
    {
        Vector2 startPos = newShopSystem.dialogBoxRect.anchoredPosition;
        Vector2 targetPos = startPos - new Vector2(0f, 300f);

        float duration = 0.4f;
        float elapsed = 0f;


        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            newShopSystem.dialogBoxRect.anchoredPosition = Vector2.Lerp(startPos, targetPos, t);
            yield return null;
        }

        newShopSystem.dialogBoxRect.anchoredPosition = targetPos;
    }

    public IEnumerator LeaveShopBGImage()
    {
        Vector2 startPos = newShopSystem.bgImageRect.anchoredPosition;
        Vector2 targetPos = startPos + new Vector2(0f, 1100f);

        float duration = 0.4f;
        float elapsed = 0f;


        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            newShopSystem.bgImageRect.anchoredPosition = Vector2.Lerp(startPos, targetPos, t);
            yield return null;
        }

        newShopSystem.bgImageRect.anchoredPosition = targetPos;
        Shop.SetActive(false);
    }

    public void RemoveCardService()
    {
        LevaveShop();
        removeService.SetActive(true);
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
}
