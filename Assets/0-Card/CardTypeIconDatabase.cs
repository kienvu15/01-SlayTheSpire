using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CardTypeIconDatabase", menuName = "Cards/CardTypeIconDatabase")]
public class CardTypeIconDatabase : ScriptableObject
{
    [System.Serializable]
    public class TypeIconEntry
    {
        public CardType cardType;
        public Sprite icon;
    }

    public List<TypeIconEntry> entries = new List<TypeIconEntry>();

    public Sprite GetIcon(CardType type)
    {
        var entry = entries.Find(e => e.cardType == type);
        return entry != null ? entry.icon : null;
    }
}
