using System;
using System.Collections.Generic;
using UnityEngine;

public class RelicManager : MonoBehaviour
{
    public static RelicManager Instance;

    [Header("Relic Inventory")]
    public List<RelicInstance> equippedRelics = new List<RelicInstance>();

    // UI Events
    public Action<RelicInstance> onRelicAdded;
    public Action<RelicInstance> onRelicRemoved;

    // Owner Character
    public Character owner;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // EQUIP / UNEQUIP
    public void EquipRelic(Relic relic)
    {
        if (relic == null) return;

        // Kiểm tra Relic đã sở hữu chưa
        foreach (var instance in equippedRelics)
        {
            if (instance.relicData == relic)
                return;
        }

        RelicInstance newInstance = new RelicInstance
        {
            relicData = relic,
            level = 1
        };

        equippedRelics.Add(newInstance);

        // Áp dụng hiệu ứng
        newInstance.Equip(owner);

        // Thông báo UI
        onRelicAdded?.Invoke(newInstance);
    }

    public void UnequipRelic(RelicInstance instance)
    {
        if (instance == null) return;

        if (equippedRelics.Contains(instance))
        {
            // Gỡ hiệu ứng trước
            instance.Unequip(owner);

            equippedRelics.Remove(instance);

            onRelicRemoved?.Invoke(instance);
        }
    }

    public void AddRelic(Relic relic)
    {
        EquipRelic(relic);
    }

    // UPGRADE
    public void UpgradeRelic(RelicInstance instance)
    {
        if (instance == null) return;

        if (!equippedRelics.Contains(instance)) return;

        if (!instance.CanUpgrade()) return;

        instance.Upgrade(owner);

        Debug.Log(
            $"Upgraded {instance.relicData.relicName} to Level {instance.level}"
        );
    }

    // BRIDGE HOOKS
    public int ApplyOnBeforeDealDamage(
        Character attacker,
        Character target,
        int baseDamage)
    {
        int modified = baseDamage;

        foreach (var instance in equippedRelics)
        {
            modified = instance.relicData.OnBeforeDealDamage(
                attacker,
                target,
                modified,
                instance.level
            );
        }

        return modified;
    }

    public void ApplyOnAfterDealDamage(
        Character attacker,
        Character target,
        int damageDealt)
    {
        foreach (var instance in equippedRelics)
        {
            instance.relicData.OnAfterDealDamage(
                attacker,
                target,
                damageDealt,
                instance.level
            );
        }
    }

    public void ApplyOnHeal(Character self, ref int amount)
    {
        foreach (var instance in equippedRelics)
        {
            instance.relicData.OnHeal(
                self,
                ref amount,
                instance.level
            );
        }
    }

    public void ApplyOnAddShield(Character self, ref int amount)
    {
        foreach (var instance in equippedRelics)
        {
            instance.relicData.OnAddShield(
                self,
                ref amount,
                instance.level
            );
        }
    }
}