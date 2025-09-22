using System.Collections.Generic;
using UnityEngine;

public class RelicManager : MonoBehaviour
{
    public List<Relic> equippedRelics = new List<Relic>();
    public Relic boneKnife;
    // UI events
    public System.Action<Relic> onRelicAdded;
    public System.Action<Relic> onRelicRemoved;

    void Start()
    {
        EquipRelic(boneKnife);
    }

    public void EquipRelic(Relic relic)
    {
        if (!equippedRelics.Contains(relic))
        {
            equippedRelics.Add(relic);
            onRelicAdded?.Invoke(relic);
        }
    }

    public void UnequipRelic(Relic relic)
    {
        if (equippedRelics.Contains(relic))
        {
            equippedRelics.Remove(relic);
            onRelicRemoved?.Invoke(relic);
        }
    }

    // Cho Shop gọi
    public void AddRelic(Relic relic)
    {
        EquipRelic(relic);
        Debug.Log("Đã thêm relic: " + relic.relicName);
    }

    // ===== Bridge hooks để Character gọi =====
    public int ApplyOnBeforeDealDamage(Character attacker, Character target, int baseDamage)
    {
        int modified = baseDamage;
        foreach (var relic in equippedRelics)
            modified = relic.OnBeforeDealDamage(attacker, target, modified);
        return modified;
    }

    public void ApplyOnAfterDealDamage(Character attacker, Character target, int damageDealt)
    {
        foreach (var relic in equippedRelics)
            relic.OnAfterDealDamage(attacker, target, damageDealt);
    }

    public void ApplyOnHeal(Character self, ref int amount)
    {
        foreach (var relic in equippedRelics)
            relic.OnHeal(self, ref amount);
    }

    public void ApplyOnAddShield(Character self, ref int amount)
    {
        foreach (var relic in equippedRelics)
            relic.OnAddShield(self, ref amount);
    }
}
