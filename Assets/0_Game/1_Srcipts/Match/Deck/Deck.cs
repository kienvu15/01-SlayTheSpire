using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using NaughtyAttributes;
public enum DrawAnimationType
{
    Straight,   
    Arc       
}

public class Deck : MonoBehaviour
{
    public static Deck instance;

    [Header("References")]
    public CardHolder cardHolder;
    public Discard discard;
    public GameObject cardPrefab;
    public Transform handParent;
    public Transform pileDeckTransform;  
    
    public MidMatch_Manager match;

    [Header("UI Debug")]
    public bool hasDrawFisrtRow = false;
    public TextMeshProUGUI[] deckCountText;
    public TextMeshProUGUI discardCountText;
    public TextMeshProUGUI allCardCount;

    [Header("Animation Settings")]
    public float drawDuration = 0.6f;   
    public float drawDelay = 0.15f;     
    public float refillDelay = 3f;

    [Header("Draw Fly Animation")]
    public int spinTurns = 1;            
    public float arcHeightRatio = 0.25f;
    public float startScale = 0.5f;
    public float peakScale = 1.2f;      

    private List<CardData> deck = new List<CardData>();
    private List<CardData> allCards = new List<CardData>();
    private List<GameObject> currentHand = new List<GameObject>();

    [Header("NewSystem")]
    public HandLayout handLayout;
    
    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        //BuildDeck();
        UpdateUI();
        UpdateAllCardCount();
    }

    public List<CardData> GetAllCards()
    {
        return new List<CardData>(allCards);
    }

    public void RemoveCardFromHand(GameObject cardGO, MidMatch_Manager matchRef)
    {
        if (cardGO == null) return;

        if (currentHand.Contains(cardGO))
            currentHand.Remove(cardGO);

        handLayout.ArrangeCards(currentHand, true);
        UpdateUI();
    }

    public void BuildDeck(List<CardData> startingCards)
    {
        deck.Clear();
        allCards.Clear();

        if (startingCards == null || startingCards.Count == 0)
        {
            UpdateUI();
            return;
        }
        deck.AddRange(startingCards);
        allCards.AddRange(startingCards);
        Shuffle(deck);
        UpdateUI();
        UpdateAllCardCount();
    }

    public IEnumerator DrawHand(int count)
    {
        //SoundManager.Instance.Play("FirstHandDraw");
        yield return StartCoroutine(DrawCards(count));
    }

    public IEnumerator DrawCards(int count)
    {
        if (handParent == null || handLayout == null) yield break;

        int drawCount = Mathf.Min(count, deck.Count);
        for (int i = 0; i < drawCount; i++)
        {
            yield return StartCoroutine(DrawOneCard(currentHand.Count));
            UpdateUI();
        }
    }

    private IEnumerator DrawOneCard(int handIndex)
    {
        yield return DrawOneCard(handIndex, DrawAnimationType.Straight);
    }

    private IEnumerator DrawOneCard(int handIndex, DrawAnimationType animationType)
    {
        if (deck.Count == 0 || handParent == null ||
            pileDeckTransform == null || handLayout == null)
            yield break;

        CardData drawnCard = deck[0];
        deck.RemoveAt(0);

        GameObject cardGO = Instantiate(cardPrefab, handParent, false);
        currentHand.Add(cardGO);

        CardDisplay display = cardGO.GetComponent<CardDisplay>();
        if (display != null)
            display.LoadCard(drawnCard);

        display.card_Button.onClick.AddListener(() =>
        {
            display.OnCardClick();
        });

        RectTransform rect = cardGO.GetComponent<RectTransform>();
        if (rect == null)
        {
            currentHand.Remove(cardGO);
            Destroy(cardGO);
            yield break;
        }

        DraggableCard draggable = cardGO.GetComponent<DraggableCard>();
        if (draggable != null)
            draggable.enabled = false;

        int cardIndex = currentHand.IndexOf(cardGO);

        if (!handLayout.TryGetCardTarget(currentHand, cardIndex,
            out Vector2 targetPosition, out Quaternion targetRotation))
        {
            currentHand.Remove(cardGO);
            Destroy(cardGO);
            yield break;
        }

        handLayout.ArrangeCards(currentHand, true);

        Vector3 startPosition = pileDeckTransform.position;
        Vector3 targetWorld = handParent.TransformPoint(targetPosition);

        float targetZ = targetRotation.eulerAngles.z;
        if (targetZ > 180f) targetZ -= 360f;

        rect.DOKill();
        rect.position = startPosition;
        rect.localScale = Vector3.zero;
        rect.localRotation = Quaternion.identity;

        float duration = Mathf.Max(0.1f, drawDuration);

        Sequence sequence = DOTween.Sequence().SetLink(cardGO);

        // Chọn kiểu đường bay
        if (animationType == DrawAnimationType.Straight)
        {
            // Đường thẳng
            sequence.Append(
                rect.DOMove(targetWorld, duration)
                    .SetEase(Ease.InOutQuad)
            );
        }
        else
        {
            // Đường cong như animation cũ
            Vector3 midpoint = Vector3.Lerp(
                startPosition, targetWorld, 0.5f);

            midpoint.y += Vector3.Distance(
                startPosition, targetWorld) * arcHeightRatio;

            sequence.Append(
                rect.DOPath(
                    new[] { startPosition, midpoint, targetWorld },
                    duration,
                    PathType.CatmullRom,
                    PathMode.Full3D
                ).SetEase(Ease.OutCubic)
            );
        }

        // Xoay lá bài
        sequence.Join(
            rect.DOLocalRotate(
                new Vector3(0f, 0f, targetZ + 360f * spinTurns),
                duration,
                RotateMode.FastBeyond360
            ).SetEase(Ease.OutQuad)
        );

        // Phóng to khi bay vào Hand
        sequence.Join(
            rect.DOScale(Vector3.one, duration)
                .SetEase(Ease.OutBack)
        );

        yield return sequence.WaitForCompletion();

        if (cardGO == null) yield break;

        handLayout.ArrangeCards(currentHand, false);

        if (draggable != null)
            draggable.enabled = true;
    }

    private int GetFirstEmptySlot()
    {
        for (int i = 0; i < handParent.childCount; i++)
        {
            if (handParent.GetChild(i).childCount == 0)
                return i;
        }
        return -1;
    }

    public void DestroyAndClearHand()
    {
        foreach (GameObject card in currentHand)
        {
            if (card != null) Destroy(card);
        }
        currentHand.Clear();
    }

    public IEnumerator AnimateClearHand()
    {
        currentHand.RemoveAll(card => card == null);
        yield return StartCoroutine(discard.AnimateDiscardHand(currentHand));
        currentHand.Clear();
        UpdateUI();
    }

    public void ClearHandReferencesOnly()
    {
        currentHand.Clear();
    }

    public List<GameObject> GetCurrentHandObjects()
    {
        currentHand.RemoveAll(card => card == null);
        return new List<GameObject>(currentHand);
    }

    public int GetDeckCount()
    {
        return deck.Count;
    }

    // Nhận list cardData vào cuối deck (đáy)
    public void ReceiveCards(List<CardData> cards)
    {
        if (cards == null || cards.Count == 0) return;
        allCards.AddRange(cards);
        deck.AddRange(cards);
        UpdateUI();
    }

    public void ShuffleDeck()
    {
        Shuffle(deck);
        UpdateUI();
    }

    void Shuffle<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int r = Random.Range(i, list.Count);
            T tmp = list[i];
            list[i] = list[r];
            list[r] = tmp;
        }
    }

    public void RemoveFromDeck(CardData data)
    {
        if (deck.Contains(data))
            deck.Remove(data);
        UpdateUI();
    }

    public void RemoveCardFromCollection(CardData data)
    {
        if (data == null) return;

        // Xóa khỏi danh sách toàn bộ bài
        allCards.Remove(data);

        // Xóa khỏi Draw Pile nếu còn tồn tại
        deck.Remove(data);

        UpdateUI();
    }

    public void AddCard(CardData cardData)
    {
        deck.Add(cardData);
        UpdateUI();
    }

    public void UpdateAllCardCount()
    {
        //if (allCardCount == null) return;

        //int total = deck.Count + currentHand.Count;

        //if (discard != null)
        //    total += discard.Count;

        //allCardCount.text = total.ToString();
    }

    public void UpdateUI()
    {
        if (deckCountText != null)
        {
            foreach (var text in deckCountText)
            {
                if (text != null)
                    text.text = "Draw Pile - " + deck.Count;
            }
        }

        if (discardCountText != null && discard != null)
            discardCountText.text = "Discard Pile - " + discard.Count;

        //UpdateAllCardCount();
    }

}
