using System.Collections.Generic;
using UnityEngine;

public class NewAttackImpactManager : MonoBehaviour
{
    public static NewAttackImpactManager Instance;

    [SerializeField] private ImpactDictionaryAsset dictionary;

    // Pool: Prefab → Queue Instance
    private Dictionary<GameObject, Queue<GameObject>> poolDict = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (dictionary != null)
            dictionary.Initialize();
    }

    public void PlayImpact(HitContext ctx)
    {
        if (dictionary.TryGetImpact(ctx, out var prefab))
        {
            GameObject vfxInstance = GetFromPool(prefab);

            // Position
            vfxInstance.transform.position = ctx.position;
            vfxInstance.transform.rotation = ctx.rotation;
            // Optionally attach:
            // if (ctx.target != null) vfxInstance.transform.SetParent(ctx.target);

            vfxInstance.SetActive(true);

            ImpactFadeOut ip = vfxInstance.GetComponentInChildren<ImpactFadeOut>();
            if (ip != null)
            {
                ip.PlayFadeOut(_ => ReturnToPool(prefab, vfxInstance));
            }
        }
        else
        {
            Debug.LogWarning($"Impact VFX not found for {ctx.cardType} / {ctx.effectKind}");
        }
    }

    private GameObject GetFromPool(GameObject prefabKey)
    {
        if (!poolDict.ContainsKey(prefabKey))
            poolDict[prefabKey] = new Queue<GameObject>();

        Queue<GameObject> pool = poolDict[prefabKey];

        if (pool.Count > 0)
        {
            GameObject obj = pool.Dequeue();
            if (obj != null)
                return obj;
        }

        return Instantiate(prefabKey);
    }

    private void ReturnToPool(GameObject prefabKey, GameObject instance)
    {
        if (instance == null) return;

        instance.SetActive(false);
        instance.transform.SetParent(transform);

        if (!poolDict.ContainsKey(prefabKey))
            poolDict[prefabKey] = new Queue<GameObject>();

        poolDict[prefabKey].Enqueue(instance);
    }
}
