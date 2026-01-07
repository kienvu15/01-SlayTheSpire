using UnityEngine;
using DG.Tweening;
using System;

public class Item : MonoBehaviour
{
    [HideInInspector] public RelicManager relicManager;
    public AllRelicDatabase allRelicDatabase;
    private Relic randomRelic;

    public bool isrelic = true;

    [Header("Optional Target for Non-Relic Items")]
    public Transform flyTarget;
    public GameObject mapIcon;
    public float flyDuration = 1.2f;  
    public float arcHeight = 2.5f;

    public event Action OnFlyCompleted;

    private void Start()
    {
        randomRelic = allRelicDatabase.GetRandomRelic();
    }

    private void OnDestroy()
    {
        if (ItemUIManager.HasInstance)
            ItemUIManager.Instance.UnregisterItem(this);
    }

    public void Take()
    {
        if (isrelic)
        {
            relicManager.AddRelic(randomRelic);
            Destroy(gameObject);
        }
        else
        {
            if (flyTarget == null)
            {
                Debug.LogWarning($"{name} không có flyTarget để bay tới. Vui lòng gán flyTarget trong Inspector.");
                return;
            }

            FlyToTarget();
        }
    }

    void FlyToTarget()
    {
        Vector3 startPos = transform.position;
        Vector3 endPos = flyTarget.position;

        Vector3 controlPoint = (startPos + endPos) / 2f + Vector3.up * arcHeight;

        float t = 0f;
        DOTween.To(() => t, x =>
        {
            t = x;

            Vector3 pos =
                Mathf.Pow(1 - t, 2) * startPos +
                2 * (1 - t) * t * controlPoint +
                Mathf.Pow(t, 2) * endPos;

            transform.position = pos;

            transform.Rotate(Vector3.forward * 360f * Time.deltaTime);
        }, 1f, flyDuration)
        .SetEase(Ease.InOutQuad)
        .OnComplete(() =>
        {
            Debug.Log($"{name} đã bay đến {flyTarget.name}");
            mapIcon.SetActive(true);
            if(mapIcon.activeSelf == true)
            {
                OnFlyCompleted?.Invoke();
                Destroy(gameObject);
            }
        });
    }


}
