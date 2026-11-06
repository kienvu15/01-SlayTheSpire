using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class HandLayout : MonoBehaviour
{
    [Header("Layout Settings")]
    public float maxCardSpacing = 170f;
    public float cardOverlap = 0.85f;
    public float extraGap = 0f;

    [Header("Circular Arc")]
    public float curveRadius = 2500f;
    public float maxFanAngle = 12f;
    public float baseYOffset = 0f;

    [Header("Selection Animation")]
    public float selectionLift = 100f;
    public float selectionScale = 1.08f;
    public float selectionDuration = 0.2f;

    [Header("Selection Spacing")]
    [Range(0.1f, 1f)]
    public float selectedCardOverlap = 0.65f;

    [Header("Animation")]
    public float animationDuration = 0.25f;

    [Header("Layout Mode")]
    public bool useCurve = false;

    private GameObject selectedCard;
    private List<GameObject> currentCards;

    private Dictionary<GameObject, Vector3> originalScales =
        new Dictionary<GameObject, Vector3>();

    // ARRANGE
    public void ArrangeCards(List<GameObject> cards, bool animate = true)
    {
        if (cards == null) return;
        cards.RemoveAll(card => card == null);
        currentCards = cards;
        foreach (GameObject card in cards)
        {
            if (!originalScales.ContainsKey(card))
                originalScales.Add(card, card.transform.localScale);
        }
        ApplyLayout(animate);
    }

    // SELECT CARD
    public void SelectCard(List<GameObject> cards, GameObject card)
    {
        if (cards == null || card == null) return;

        currentCards = new List<GameObject>(cards);
        currentCards.RemoveAll(c => c == null);

        if (!currentCards.Contains(card)) return;

        foreach (GameObject c in currentCards)
        {
            if (!originalScales.ContainsKey(c))
                originalScales.Add(c, c.transform.localScale);
        }

        if (selectedCard == card)
        {
            ClearSelection();
            return;
        }
        selectedCard = card;
        UpdateSelectedGlow();
        ApplyLayout(true);
    }

    private void UpdateSelectedGlow()
    {
        if (currentCards == null) return;

        foreach (GameObject card in currentCards)
        {
            if (card == null) continue;

            CardDisplay display = card.GetComponent<CardDisplay>();

            if (display != null && display.outline_Glow != null)
            {
                display.outline_Glow.SetActive(card == selectedCard);
            }
        }
    }

    // CLEAR SELECTION
    public void ClearSelection()
    {
        if (selectedCard == null) return;

        selectedCard = null;

        UpdateSelectedGlow();
        ApplyLayout(true);
    }

    // LAYOUT CALCULATION
    private void ApplyLayout(bool animate)
    {
        if (currentCards == null) return;

        int count = currentCards.Count;
        if (count == 0) return;

        RectTransform parentRect = GetComponent<RectTransform>();

        float cardWidth =
            currentCards[0].GetComponent<RectTransform>().rect.width;

        int selectedIndex = currentCards.IndexOf(selectedCard);

        // Khoảng cách cố định giữa các lá bài
        float spacing = Mathf.Min(
            maxCardSpacing,
            cardWidth * cardOverlap
        ) + extraGap;

        if (count > 1)
        {
            float totalIntervals = selectedIndex >= 0
                ? count
                : count - 1;

            // Giới hạn trong chiều rộng vùng bài
            float availableSpacing =
                Mathf.Max(0f, parentRect.rect.width - cardWidth)
                / totalIntervals;

            spacing = Mathf.Min(spacing, availableSpacing);

            // Giới hạn độ rộng khi sử dụng Curve
            if (useCurve)
            {
                float maxHalfWidth =
                    curveRadius * Mathf.Sin(maxFanAngle * Mathf.Deg2Rad);

                float halfWidth = totalIntervals * spacing * 0.5f;

                if (halfWidth > maxHalfWidth && halfWidth > 0f)
                    spacing *= maxHalfWidth / halfWidth;
            }
        }

        // Căn giữa nhóm bài
        float startX = -(count - 1) * spacing * 0.5f;

        for (int i = 0; i < count; i++)
        {
            GameObject card = currentCards[i];
            if (card == null) continue;

            RectTransform rect = card.GetComponent<RectTransform>();

            float x = startX + i * spacing;

            // Tạo khoảng trống cho lá đang được chọn
            if (selectedIndex >= 0 && i != selectedIndex)
            {
                if (i < selectedIndex)
                    x -= spacing * 0.5f;
                else
                    x += spacing * 0.5f;
            }

            float y = baseYOffset;
            float rotation = 0f;

            // Tính toán đường cong
            if (useCurve)
            {
                float safeRadius = Mathf.Max(0.01f, curveRadius);

                float sin = Mathf.Clamp(
                    x / safeRadius,
                    -0.99f,
                    0.99f
                );

                float angle = Mathf.Asin(sin);

                y = safeRadius * Mathf.Cos(angle)
                    - safeRadius + baseYOffset;

                rotation = -angle * Mathf.Rad2Deg;
            }

            bool isSelected = card == selectedCard;

            SetCardOnTop(card, isSelected);

            if (isSelected)
                y += selectionLift;

            Vector2 targetPosition = new Vector2(x, y);

            Quaternion targetRotation =
                Quaternion.Euler(0f, 0f, rotation);

            Vector3 baseScale = originalScales[card];

            Vector3 targetScale = isSelected
                ? baseScale * selectionScale
                : baseScale;

            if (animate)
            {
                rect.DOKill();
                card.transform.DOKill();

                rect.DOAnchorPos(targetPosition, animationDuration)
                    .SetEase(Ease.OutCubic);

                rect.DOLocalRotateQuaternion(
                    targetRotation,
                    animationDuration
                ).SetEase(Ease.OutCubic);

                card.transform.DOScale(
                    targetScale,
                    animationDuration
                ).SetEase(Ease.OutCubic);
            }
            else
            {
                rect.anchoredPosition = targetPosition;
                rect.localRotation = targetRotation;
                card.transform.localScale = targetScale;
            }
        }
    }

    // GET TARGET POSITION
    public bool TryGetCardTarget(
        List<GameObject> cards,
        int cardIndex,
        out Vector2 targetPosition,
        out Quaternion targetRotation)
    {
        targetPosition = Vector2.zero;
        targetRotation = Quaternion.identity;
        if (cards == null) return false;
        cards.RemoveAll(c => c == null);
        if (cardIndex < 0 || cardIndex >= cards.Count)
            return false;
        int count = cards.Count;
        RectTransform parentRect = GetComponent<RectTransform>();
        float cardWidth =
            cards[0].GetComponent<RectTransform>().rect.width;
        float spacing = Mathf.Min(maxCardSpacing, cardWidth * cardOverlap)
                        + extraGap;
        if (count > 1)
        {
            float availableSpacing =
                Mathf.Max(0f, parentRect.rect.width - cardWidth)
                / (count - 1);
            spacing = Mathf.Min(spacing, availableSpacing);
            if (useCurve)
            {
                float maxHalfWidth =
                    curveRadius * Mathf.Sin(maxFanAngle * Mathf.Deg2Rad);
                float halfWidth = (count - 1) * spacing * 0.5f;
                if (halfWidth > maxHalfWidth && halfWidth > 0f)
                    spacing *= maxHalfWidth / halfWidth;
            }
        }

        float x = -(count - 1) * spacing * 0.5f
                  + cardIndex * spacing;
        float y = baseYOffset;
        float rotation = 0f;
        if (useCurve)
        {
            float safeRadius = Mathf.Max(0.01f, curveRadius);
            float angle = Mathf.Asin(
                Mathf.Clamp(x / safeRadius, -0.99f, 0.99f));
            y = safeRadius * Mathf.Cos(angle)
                - safeRadius + baseYOffset;
            rotation = -angle * Mathf.Rad2Deg;
        }
        targetPosition = new Vector2(x, y);
        targetRotation = Quaternion.Euler(0f, 0f, rotation);
        return true;
    }

    public void RefreshLayout(bool animate = true)
    {
        ApplyLayout(animate);
    }

    private void SetCardOnTop(GameObject card, bool onTop)
    {
        //Canvas canvas = card.GetComponent<Canvas>();
        //canvas.overrideSorting = onTop;
        //if (onTop)
        //    canvas.sortingLayerName = "UI";
        //    canvas.sortingOrder = 574;
    }

}
