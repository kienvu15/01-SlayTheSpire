using UnityEngine;

[CreateAssetMenu(fileName = "ImpactVFXDatabase", menuName = "Cards/Impact VFX Database")]
public class ImpactVFXDatabase : ScriptableObject
{

    public GameObject meleeSlashPrefab;
    public GameObject rangedImpactPrefab;
    public GameObject defenseShieldPrefab;
    public GameObject magicExplosionPrefab;
    public GameObject specialEffectPrefab;

    public GameObject GetPrefab(CardType type)
    {
        switch (type)
        {
            case CardType.Mellee: return meleeSlashPrefab;
            case CardType.Ranged: return rangedImpactPrefab;
            case CardType.Defense: return defenseShieldPrefab;
            case CardType.Magic: return magicExplosionPrefab;
            case CardType.Special: return specialEffectPrefab;
        }
        return null;
    }
}
