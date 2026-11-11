
using System.Collections.Generic;
using UnityEngine;

public class DamagePopupManager : MonoBehaviour
{
    public static DamagePopupManager Instance;

    [Header("References")]
    public DamagePopup popupPrefab;
    public Canvas canvas;

    [Header("Pool")]
    public int initialPoolSize = 10;

    private Queue<DamagePopup> pool = new Queue<DamagePopup>();
    private RectTransform canvasRect;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        canvasRect = canvas.GetComponent<RectTransform>();

        for (int i = 0; i < initialPoolSize; i++)
        {
            CreatePopup();
        }
    }

    private DamagePopup CreatePopup()
    {
        DamagePopup popup = Instantiate(popupPrefab, canvasRect);
        popup.gameObject.SetActive(false);

        pool.Enqueue(popup);

        return popup;
    }

    private DamagePopup GetPopup()
    {
        if (pool.Count > 0)
            return pool.Dequeue();

        DamagePopup popup = Instantiate(popupPrefab, canvasRect);
        popup.gameObject.SetActive(false);

        return popup;
    }

    public void ShowPopup(
        Vector3 worldPosition,
        int damageAmount,
        bool isMiss = false)
    {
        Camera worldCamera = Camera.main;

        if (canvas.renderMode == RenderMode.ScreenSpaceCamera ||
            canvas.renderMode == RenderMode.WorldSpace)
        {
            worldCamera = canvas.worldCamera != null
                ? canvas.worldCamera
                : Camera.main;
        }

        Vector2 screenPosition =
            RectTransformUtility.WorldToScreenPoint(
                worldCamera,
                worldPosition
            );

        Camera uiCamera =
            canvas.renderMode == RenderMode.ScreenSpaceOverlay
                ? null
                : canvas.worldCamera;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            screenPosition,
            uiCamera,
            out Vector2 localPosition
        );

        DamagePopup popup = GetPopup();

        popup.transform.SetAsLastSibling();

        popup.Setup(
            localPosition,
            damageAmount,
            isMiss,
            ReturnToPool
        );
    }

    private void ReturnToPool(DamagePopup popup)
    {
        if (popup == null) return;

        popup.ResetPopup();
        popup.gameObject.SetActive(false);

        pool.Enqueue(popup);
    }
}
