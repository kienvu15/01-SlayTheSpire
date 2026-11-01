using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class HandLayout : MonoBehaviour
{
    [Header("Layout Settings")]
    public float maxCardSpacing = 170f;
    public float cardOverlap = 0.85f;
    public float extraGap = 0f; // Khoảng trống cộng thêm giữa các lá

    [Header("Circular Arc")]
    public float curveRadius = 2500f;   // lớn = cung thoải (kiểu StS)
    public float maxFanAngle = 12f;     // góc tối đa ở lá ngoài cùng (độ)
    public float baseYOffset = 0f;      // nâng/hạ cả hand

    [Header("Animation")]
    public float animationDuration = 0.25f;

    [Header("Layout Mode")]
    public bool useCurve = false; // false = thẳng, true = cong

    public void ArrangeCards(List<GameObject> cards, bool animate = true)
    {
        cards.RemoveAll(card => card == null);

        int count = cards.Count;
        if (count == 0) return;

        RectTransform parentRect = GetComponent<RectTransform>();
        float cardWidth = cards[0].GetComponent<RectTransform>().rect.width;

        // 1. Khoảng cách giữa các lá
        float spacing = Mathf.Min(maxCardSpacing, cardWidth * cardOverlap)
                        + extraGap;
        if (count > 1)
        {
            // Không vượt quá bề rộng panel
            float availableSpacing =
                Mathf.Max(0, parentRect.rect.width - cardWidth) / (count - 1);
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

        float startX = -(count - 1) * spacing * 0.5f;

        for (int i = 0; i < count; i++)
        {
            // 2. x cách đều
            float x = startX + i * spacing;

            float y = baseYOffset;
            float rotation = 0f;

            if (useCurve)
            {
                float sin = Mathf.Clamp(x / curveRadius, -0.99f, 0.99f);
                float angle = Mathf.Asin(sin);

                y = curveRadius * Mathf.Cos(angle) - curveRadius + baseYOffset;
                rotation = -angle * Mathf.Rad2Deg;
            }

            RectTransform rect = cards[i].GetComponent<RectTransform>();
            Vector2 targetPosition = new Vector2(x, y);
            Quaternion targetRotation = Quaternion.Euler(0, 0, rotation);

            rect.SetSiblingIndex(i);

            if (animate)
            {
                rect.DOKill();
                rect.DOAnchorPos(targetPosition, animationDuration)
                    .SetEase(Ease.OutQuad);
                rect.DOLocalRotateQuaternion(targetRotation, animationDuration)
                    .SetEase(Ease.OutQuad);
            }
            else
            {
                rect.anchoredPosition = targetPosition;
                rect.localRotation = targetRotation;
            }
        }
    }

    public bool TryGetCardTarget(List<GameObject> cards, int cardIndex,
    out Vector2 targetPosition, out Quaternion targetRotation)
    {
        targetPosition = Vector2.zero;
        targetRotation = Quaternion.identity;

        if (cards == null || cardIndex < 0 || cardIndex >= cards.Count)
            return false;

        cards.RemoveAll(card => card == null);
        if (cardIndex >= cards.Count || cards.Count == 0)
            return false;

        RectTransform parentRect = GetComponent<RectTransform>();
        RectTransform firstRect = cards[0].GetComponent<RectTransform>();
        if (parentRect == null || firstRect == null)
            return false;

        int count = cards.Count;
        float cardWidth = firstRect.rect.width;
        float spacing = Mathf.Min(maxCardSpacing, cardWidth * cardOverlap)
                        + extraGap;
        if (count > 1)
        {
            float availableSpacing = Mathf.Max(0f, parentRect.rect.width - cardWidth) / (count - 1);
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

        float x = -(count - 1) * spacing * 0.5f + cardIndex * spacing;
        float safeRadius = Mathf.Max(0.01f, curveRadius);
        float y = baseYOffset;
        float rotation = 0f;

        if (useCurve)
        {
            float sin = Mathf.Clamp(x / safeRadius, -0.99f, 0.99f);
            float angle = Mathf.Asin(sin);

            y = safeRadius * Mathf.Cos(angle) - safeRadius + baseYOffset;
            rotation = -angle * Mathf.Rad2Deg;
        }

        targetPosition = new Vector2(x, y);
        targetRotation = Quaternion.Euler(0f, 0f, rotation);
        return true;
    }
}