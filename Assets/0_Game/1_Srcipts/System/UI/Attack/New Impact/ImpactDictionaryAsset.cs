using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ImpactDictionary", menuName = "VFX/Impact Dictionary")]
public class ImpactDictionaryAsset : ScriptableObject
{
    [System.Serializable]
    public class Entry
    {
        public CardType cardType;
        public ImpactEffectKind effectKind;
        public GameObject prefab;
    }

    [SerializeField] private List<Entry> entries = new();

    private Dictionary<(CardType, ImpactEffectKind), GameObject> dict;

    public void Initialize()
    {
        dict = new Dictionary<(CardType, ImpactEffectKind), GameObject>();

        foreach (var e in entries)
        {
            var key = (e.cardType, e.effectKind);
            if (!dict.ContainsKey(key))
            {
                dict.Add(key, e.prefab);
            }
        }
    }

    public bool TryGetImpact(HitContext ctx, out GameObject prefab)
    {
        return dict.TryGetValue((ctx.cardType, ctx.effectKind), out prefab);
    }
}
