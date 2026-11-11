using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(ScrollRect))]
public class CenterScaleScroll : MonoBehaviour, IBeginDragHandler, IEndDragHandler
{
    [Header("Scale")]
    [SerializeField] private float maxScale = 1.25f;
    [SerializeField] private float minScale = 0.7f;
    [SerializeField] private float falloffDistance = 400f;
    [SerializeField] private AnimationCurve curve = AnimationCurve.EaseInOut(0, 1, 1, 0);

    [Header("Spacing theo scale")]
    [Tooltip("Khoảng cách gốc giữa các card")]
    [SerializeField] private float spacing = 20f;
    [Tooltip("1 = card to ra bao nhiêu thì đẩy hàng xóm ra đúng bấy nhiêu. >1 = thoáng hơn")]
    [SerializeField] private float gapMultiplier = 1f;
    [Tooltip("Khoảng trống thêm ở hai bên card giữa")]
    [SerializeField] private float centerExtraGap = 0f;

    [Header("Depth")]
    [SerializeField] private bool dimFarCards = true;
    [SerializeField, Range(0f, 1f)] private float minBrightness = 0.6f;
    [SerializeField] private bool overrideSorting = true;

    [Header("Snap")]
    [SerializeField] private bool snap = true;
    [SerializeField] private float snapSpeed = 10f;
    [SerializeField] private float snapVelocityThreshold = 120f;

    private ScrollRect scrollRect;
    private RectTransform viewport;
    private RectTransform content;
    private bool isDragging;
    //private int baseSortingOrder;
    private CardDisplay highlightedCard;
    private int highlightedIndex = -1;


    private readonly List<RectTransform> cards = new();
    private readonly List<float> baseX = new();      
    private readonly List<float> baseY = new();
    private readonly List<float> baseCenterX = new(); 

    private void Awake()
    {
        scrollRect = GetComponent<ScrollRect>();
        content = scrollRect.content;
        viewport = scrollRect.viewport != null ? scrollRect.viewport : (RectTransform)transform;

        Canvas parentCanvas = GetComponentInParent<Canvas>();
        //baseSortingOrder = parentCanvas != null ? parentCanvas.sortingOrder + 1 : 1;
    }

    public CardDisplay GetHighlightedCard()
    {
        return highlightedCard;
    }

    public CardData GetHighlightedCardData()
    {
        return highlightedCard.cardData;
    }

    /// <summary>Gọi sau khi spawn xong card.</summary>
    public void Refresh()
    {
        cards.Clear(); baseX.Clear(); baseY.Clear(); baseCenterX.Clear();

        // 1. Content: anchor trái, pivot trái
        content.anchorMin = new Vector2(0f, 0f);
        content.anchorMax = new Vector2(0f, 1f);
        content.pivot = new Vector2(0f, 0.5f);

        // 2. Bật layout để nó xếp vị trí gốc
        if (!content.TryGetComponent(out HorizontalLayoutGroup layout))
            layout = content.gameObject.AddComponent<HorizontalLayoutGroup>();
        layout.enabled = true;
        layout.spacing = spacing;
        layout.childAlignment = TextAnchor.MiddleLeft;
        layout.childControlWidth = false;
        layout.childControlHeight = false;
        layout.childForceExpandWidth = false;
        layout.childForceExpandHeight = false;

        if (!content.TryGetComponent(out ContentSizeFitter fitter))
            fitter = content.gameObject.AddComponent<ContentSizeFitter>();
        fitter.enabled = true;
        fitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        fitter.verticalFit = ContentSizeFitter.FitMode.Unconstrained;

        // 3. Padding để card đầu/cuối vào được giữa
        if (content.childCount > 0)
        {
            float cardW = ((RectTransform)content.GetChild(0)).rect.width;
            int pad = Mathf.Max(0, Mathf.RoundToInt((viewport.rect.width - cardW) * 0.5f));
            layout.padding.left = pad;
            layout.padding.right = pad;
        }

        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(content);

        // 4. Lưu vị trí gốc rồi TẮT layout để script tự điều khiển vị trí
        foreach (RectTransform child in content)
        {
            if (!child.gameObject.activeSelf) continue;
            cards.Add(child);
            baseX.Add(child.anchoredPosition.x);
            baseY.Add(child.anchoredPosition.y);
            baseCenterX.Add(child.anchoredPosition.x + (0.5f - child.pivot.x) * child.rect.width);
        }

        float totalWidth = LayoutUtility.GetPreferredWidth(content);
        layout.enabled = false;
        fitter.enabled = false;
        content.sizeDelta = new Vector2(totalWidth, content.sizeDelta.y);

        scrollRect.velocity = Vector2.zero;
        scrollRect.horizontalNormalizedPosition = 0f;

        UpdateCards();
    }

    private void Update()
    {
        UpdateCards();

        if (snap && !isDragging && cards.Count > 0 &&
            scrollRect.velocity.magnitude < snapVelocityThreshold)
            SnapToNearest();
    }

    // khoảng cách (có dấu) từ tâm card gốc tới tâm viewport
    private float SignedDistToCenter(int i)
    {
        Vector3 world = content.TransformPoint(new Vector3(baseCenterX[i], 0f, 0f));
        float x = viewport.InverseTransformPoint(world).x;
        return x - viewport.rect.center.x;
    }

    private void UpdateCards()
    {
        int n = cards.Count;
        if (n == 0) return;

        // --- tính scale cho từng card ---
        var scales = new float[n];
        var ts = new float[n];
        int centerIdx = 0;
        float best = float.MaxValue;

        for (int i = 0; i < n; i++)
        {
            float d = Mathf.Abs(SignedDistToCenter(i));
            if (d < best) { best = d; centerIdx = i; }

            ts[i] = curve.Evaluate(Mathf.Clamp01(d / falloffDistance));
            scales[i] = Mathf.Lerp(minScale, maxScale, ts[i]);
        }
        if (centerIdx != highlightedIndex)
        {
            highlightedIndex = centerIdx;
            highlightedCard = cards[centerIdx].GetComponent<CardDisplay>();
        }
        // --- tính offset tích luỹ: card to đẩy hàng xóm ra ---
        var offset = new float[n];
        float run = 0f;
        for (int i = 0; i < n; i++)
        {
            float w = cards[i].rect.width;
            float extra = w * (scales[i] - 1f) * gapMultiplier;
            offset[i] = run + extra * 0.5f;
            run += extra;
        }

        // giữ card giữa đứng yên, phần còn lại dịch tương đối
        float anchorOffset = offset[centerIdx];
        for (int i = 0; i < n; i++)
        {
            float o = offset[i] - anchorOffset;
            if (i < centerIdx) o -= centerExtraGap;
            else if (i > centerIdx) o += centerExtraGap;

            cards[i].anchoredPosition = new Vector2(baseX[i] + o, baseY[i]);
            cards[i].localScale = Vector3.one * scales[i];

            if (dimFarCards)
            {
                float b = Mathf.Lerp(minBrightness, 1f, ts[i]);
              
            }
        }
    }

    private void SnapToNearest()
    {
        float bestDelta = 0f, bestDist = float.MaxValue;

        for (int i = 0; i < cards.Count; i++)
        {
            float delta = -SignedDistToCenter(i);
            if (Mathf.Abs(delta) < bestDist)
            {
                bestDist = Mathf.Abs(delta);
                bestDelta = delta;
            }
        }

        if (bestDist < 0.5f) return;

        scrollRect.velocity = Vector2.zero;
        Vector2 pos = content.anchoredPosition;
        pos.x += bestDelta * Mathf.Min(1f, Time.unscaledDeltaTime * snapSpeed);
        content.anchoredPosition = pos;
    }

    public void OnBeginDrag(PointerEventData e) => isDragging = true;
    public void OnEndDrag(PointerEventData e) => isDragging = false;
}