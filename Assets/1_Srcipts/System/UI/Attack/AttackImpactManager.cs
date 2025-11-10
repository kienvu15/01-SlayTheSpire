using System.Collections.Generic;
using UnityEngine;

public class AttackImpactManager : MonoBehaviour
{
    public static AttackImpactManager Instance;
    public ImpactVFXDatabase database;

    private Dictionary<CardType, Queue<GameObject>> poolDict = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void ShowImpact(CardType type, Transform target)
    {
        if (target == null) return;

        GameObject prefab = database.GetPrefab(type);
        if (prefab == null) return;

        GameObject vfx = GetFromPool(type, prefab);
        vfx.transform.SetParent(target);
        vfx.transform.position = target.position + Vector3.up * 0.5f;
        vfx.transform.rotation = Quaternion.identity;
        vfx.SetActive(true);

        StartCoroutine(DeactivateAfterDelay(vfx, type, 1.5f));
    }

    public void ShowConditionImpact(Transform target, CardType type)
    {
        if (target == null) return;
        ShowImpact(type, target); 
    }

    private GameObject GetFromPool(CardType type, GameObject prefab)
    {
        if (!poolDict.ContainsKey(type))
            poolDict[type] = new Queue<GameObject>();

        Queue<GameObject> pool = poolDict[type];

        if (pool.Count > 0)
        {
            GameObject obj = pool.Dequeue();
            return obj;
        }
        else
        {
            GameObject newObj = Instantiate(prefab);
            newObj.SetActive(false);
            return newObj;
        }
    }

    private System.Collections.IEnumerator DeactivateAfterDelay(GameObject obj, CardType type, float delay)
    {
        yield return new WaitForSeconds(delay);
        obj.SetActive(false);
        obj.transform.SetParent(transform); 
        poolDict[type].Enqueue(obj);
    }
}
