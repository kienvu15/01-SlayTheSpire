
using UnityEngine;

public enum ImpactEffectKind
{
    Damage,
    Crit,
    HitArmor,
    Miss,
    Dodge,
    Heal,
    Shield,
    Condition
}

public enum ImpactSpawnAnchor
{
    Target,
    Attacker,
    WorldPosition
}

[System.Serializable]
public class ImpactVFXEntry
{
    [Header("Impact Type")]
    public ImpactEffectKind effectKind;

    [Header("Prefab")]
    public GameObject prefab;

    [Header("Spawn Position")]
    public ImpactSpawnAnchor spawnAnchor = ImpactSpawnAnchor.Target;

    // Offset theo local position của anchor
    public Vector3 positionOffset;

    [Header("Spawn Rotation")]
    public Vector3 rotationOffset;

    [Header("Scale")]
    public Vector3 scale = Vector3.one;

    [Header("Attachment")]
    public bool attachToAnchor = false;
}

[CreateAssetMenu(fileName = "NewImpactProfile",
    menuName = "Cards/Impact Profile")]
public class ImpactProfile : ScriptableObject
{
    [Header("Default Impact")]
    public ImpactVFXEntry defaultImpact;

    [Header("Specific Impacts")]
    public ImpactVFXEntry[] impacts;

    public ImpactVFXEntry GetImpact(ImpactEffectKind kind)
    {
        if (impacts != null)
        {
            foreach (var impact in impacts)
            {
                if (impact != null &&
                    impact.effectKind == kind &&
                    impact.prefab != null)
                {
                    return impact;
                }
            }
        }

        return defaultImpact != null &&
               defaultImpact.prefab != null
            ? defaultImpact
            : null;
    }
}
