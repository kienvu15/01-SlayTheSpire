
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewAttackImpactManager : MonoBehaviour
{
    public static NewAttackImpactManager Instance;

    private Dictionary<GameObject, Queue<GameObject>> poolDict = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }
    public int ActiveCount { get; private set; }

    public IEnumerator WaitForAllImpacts(float timeout = 2f)
    {
        float t = 0f;
        while (ActiveCount > 0 && t < timeout)   // timeout chống kẹt nếu callback không bao giờ gọi
        {
            t += Time.deltaTime;
            yield return null;
        }
    }

    public void PlayImpact(ImpactProfile profile, ImpactEffectKind effectKind, Transform attacker, Transform target, Vector3 worldPosition, System.Action onFinished = null)
    {
        if (profile == null) { onFinished?.Invoke(); return; }

        ImpactVFXEntry entry = profile.GetImpact(effectKind);
        if (entry == null || entry.prefab == null) { onFinished?.Invoke(); return; }

        Transform anchor = null;

        switch (entry.spawnAnchor)
        {
            case ImpactSpawnAnchor.Target:
                anchor = target;
                break;

            case ImpactSpawnAnchor.Attacker:
                anchor = attacker;
                break;
        }

        Vector3 spawnPosition = worldPosition;
        Quaternion spawnRotation = Quaternion.identity;

        if (anchor != null)
        {
            spawnPosition = anchor.TransformPoint(entry.positionOffset);
            spawnRotation = anchor.rotation *
                Quaternion.Euler(entry.rotationOffset);
        }
        else if (entry.spawnAnchor == ImpactSpawnAnchor.WorldPosition)
        {
            spawnPosition += entry.positionOffset;
            spawnRotation = Quaternion.Euler(entry.rotationOffset);
        }

        GameObject instance = GetFromPool(entry.prefab);

        // Luôn parent vào Canvas chứa Manager
        instance.transform.SetParent(transform, false);

        instance.transform.position = spawnPosition;
        instance.transform.rotation = spawnRotation;

        // Scale lấy trực tiếp từ ImpactProfile
        instance.transform.localScale = entry.scale;

        instance.SetActive(true);
        ActiveCount++;
        ImpactFadeOut fade = instance.GetComponentInChildren<ImpactFadeOut>();
        if (fade != null)
        {
            fade.PlayFadeOut(_ =>
            {
                ReturnToPool(entry.prefab, instance);
                ActiveCount--;
                onFinished?.Invoke();
            });
        }
        else
        {
            Debug.LogWarning($"Impact prefab {entry.prefab.name} has no ImpactFadeOut.");
            ReturnToPool(entry.prefab, instance);
            ActiveCount--;
            onFinished?.Invoke();
        }
    }

    private GameObject GetFromPool(GameObject prefab)
    {
        if (!poolDict.ContainsKey(prefab))
            poolDict[prefab] = new Queue<GameObject>();

        Queue<GameObject> pool = poolDict[prefab];

        while (pool.Count > 0)
        {
            GameObject obj = pool.Dequeue();

            if (obj != null)
                return obj;
        }

        return Instantiate(prefab, transform);
    }

    private void ReturnToPool(
        GameObject prefab,
        GameObject instance)
    {
        if (instance == null) return;

        instance.SetActive(false);
        instance.transform.SetParent(transform, false);
        instance.transform.localPosition = Vector3.zero;
        instance.transform.localRotation = Quaternion.identity;
        instance.transform.localScale = Vector3.one;

        if (!poolDict.ContainsKey(prefab))
            poolDict[prefab] = new Queue<GameObject>();

        poolDict[prefab].Enqueue(instance);
    }

}
