using UnityEngine;

public class AttackImpactManager : MonoBehaviour
{
    public static AttackImpactManager Instance;
    public ImpactVFXDatabase database;

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

        Vector3 spawnPos = target.position + Vector3.up * 0.5f; // hơi lệch trên cho đẹp
        GameObject vfx = Instantiate(prefab, spawnPos, Quaternion.identity, target);

        Destroy(vfx, 1.5f); // hủy sau khi animation chạy xong
    }

    public void ShowConditionImpact(Transform target, CardType type)
    {
        if (target == null) return;

        // mặc định condition sẽ show impact loại Special
        ShowImpact(type, target);
    }

}
