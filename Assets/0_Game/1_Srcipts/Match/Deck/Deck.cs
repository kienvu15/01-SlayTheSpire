using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Deck : MonoBehaviour
{
    [Header("References")]
    public CardHolder cardHolder;
    public Discard discard;
    public GameObject cardPrefab;
    public Transform handParent;
    public Transform pileDeckTransform;  
    
    public Match match;

    [Header("UI Debug")]
    public bool hasDrawFisrtRow = false;
    public TextMeshProUGUI[] deckCountText;
    public TextMeshProUGUI discardCountText;
    public TextMeshProUGUI allCardCount;

    [Header("Animation Settings")]
    public float drawDuration = 0.6f;   
    public float drawDelay = 0.15f;     
    public float refillDelay = 3f;      

    private List<CardData> deck = new List<CardData>();
    private List<GameObject> currentHand = new List<GameObject>();

    public static Deck instance;
    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        BuildDeck();
        UpdateUI();
        UpdateAllCardCount();
    }

    public void DrawFirstHand(int count)
    {
        hasDrawFisrtRow = true;
        StartCoroutine(DrawHand(count));
    }

    public void CacheDeckCountText()
    {
        GameObject[] DeckCount = GameObject.FindGameObjectsWithTag("DeckCount");
        List<TextMeshProUGUI> foundTexts = new List<TextMeshProUGUI>();

        foreach (var go in DeckCount)
        {
            var tmp = go.GetComponent<TextMeshProUGUI>();
            if (tmp != null) foundTexts.Add(tmp);
        }

        deckCountText = foundTexts.ToArray();
        UpdateUI(); 
    }

    public void CacheDeckDiscard()
    {
        discardCountText = GetComponent<TextMeshProUGUI>();
    }

    public void RemoveCardFromHand(GameObject cardGO, Match matchRef)
    {
        if (cardGO == null) return;

        if (currentHand.Contains(cardGO))
            currentHand.Remove(cardGO);

        UpdateUI();
    }

    public IEnumerator DelayAndDraw()
    {
        yield return null;
        yield return StartCoroutine(DrawHand(match.handSize));
    }

    // Copy collection vào deck và shuffle
    public void BuildDeck()
    {
        deck.Clear();
        foreach (CardData data in cardHolder.allCardData)
            deck.Add(data);
        Shuffle(deck);
        UpdateUI();
    }

    // Giữ để backward compatibility nếu nơi khác gọi
    public IEnumerator DrawHand(int count)
    {
        SoundManager.Instance.Play("FirstHandDraw");
        yield return StartCoroutine(DrawCards(count));
    }

    /// Rút count lá (nếu deck hết giữa chừng sẽ dừng và trả về)
    public IEnumerator DrawCards(int count)
    {
        List<int> availableSlots = new List<int>();
        for (int i = 0; i < handParent.childCount; i++)
        {
            if (handParent.GetChild(i).childCount == 0)
                availableSlots.Add(i);
        }

        int drawCount = Mathf.Min(count, availableSlots.Count);

        for (int k = 0; k < drawCount; k++)
        {
            int slotIndex = availableSlots[k];
            yield return StartCoroutine(DrawOneCard(slotIndex));
            UpdateUI();
        }
    }


    private IEnumerator DrawOneCard(int handIndex)
    {
        if (deck.Count == 0) yield break;

        CardData drawnCard = deck[0];
        deck.RemoveAt(0);

        Transform targetSlot = handParent.GetChild(handIndex);

        GameObject cardGO = Instantiate(cardPrefab, pileDeckTransform.position, Quaternion.identity, pileDeckTransform.parent);
        currentHand.Add(cardGO);

        cardGO.GetComponent<CardDisplay>().LoadCard(drawnCard);

        RectTransform rect = cardGO.GetComponent<RectTransform>();

        rect.position = pileDeckTransform.position;  
        rect.localScale = Vector3.zero;
        rect.localRotation = Quaternion.Euler(0, 90, 0);

        Vector3 worldTargetPos = targetSlot.position;

        Sequence seq = DOTween.Sequence();
        seq.Append(rect.DOMove(worldTargetPos, drawDuration).SetEase(Ease.OutQuad));    
        seq.Join(rect.DOScale(Vector3.one, drawDuration).SetEase(Ease.OutBack));       
        seq.Join(rect.DORotate(Vector3.zero, drawDuration, RotateMode.Fast));        

        seq.OnComplete(() =>
        {
            rect.SetParent(targetSlot, false);
            rect.anchoredPosition = Vector2.zero;
            rect.localScale = Vector3.one;
            rect.localRotation = Quaternion.identity;
            cardGO.GetComponent<DraggableCard>().enabled = true;
        });

        yield return new WaitForSeconds(drawDelay);
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

    // Xóa và đồng thời Destroy GameObjects in hand (dùng khi cần xóa ngay)
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
        // 1. Clean list trước (xoá null nếu có)
        currentHand.RemoveAll(card => card == null);

        // 2. Animate bay về discard
        yield return StartCoroutine(discard.AnimateDiscardHand(currentHand));

        // 3. Khi animation xong, clear list
        currentHand.Clear();

        // 4. Cập nhật UI
        UpdateUI();
    }


    // CHỈ xóa tham chiếu trong deck (dùng khi objects đã bị destroy bởi animation khác)
    public void ClearHandReferencesOnly()
    {
        currentHand.Clear();
    }

    public List<GameObject> GetCurrentHandObjects()
    {
        // Bước 1: Dọn dẹp chính cái list gốc (xóa hết những thằng đã chết/null)
        // Lệnh này rất mạnh: Nó duyệt cả list và xóa sạch phần tử null
        currentHand.RemoveAll(card => card == null);

        // Bước 2: Trả về một bản COPY của danh sách đã sạch
        // Việc tạo "new List" giúp đảm bảo nếu bên ngoài có sửa list này 
        // thì cũng không làm hỏng list gốc trong Deck.
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

    public void AddCard(CardData cardData)
    {
        deck.Add(cardData);
        UpdateUI();
    }

    public void UpdateAllCardCount()
    {
        Debug.Log("Updating all card count UI");
        allCardCount.text = cardHolder.allCardData.Count.ToString();
    }

    public void UpdateUI()
    {
        if (deckCountText != null)
            foreach (var deckCountText in deckCountText)
                if (deckCountText != null)
                    deckCountText.text = deck.Count.ToString();

        if (discardCountText != null && discard != null)
            discardCountText.text = discard.Count.ToString();
    }


}
