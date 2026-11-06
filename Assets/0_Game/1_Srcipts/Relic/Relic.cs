using UnityEngine;

public abstract class Relic : ScriptableObject
{
    [Header("Basic Info")]
    public string relicName;
    public Sprite icon;

    [Header("Description")]
    [TextArea(3, 5)]
    public string defaultDescription;

    [TextArea(3, 5)]
    public string upgradeDescription;

    [Header("Shop")]
    public int gold_Buy = 10;
    public int gold_Sell = 10;

    [Header("Upgrade")]
    public int maxLevel = 2;
    public int upgradeCost = 50;

    // Damage
    public virtual int OnBeforeDealDamage(
        Character attacker, Character target, int baseDamage
    ) => baseDamage;

    public virtual int OnBeforeDealDamage(
        Character attacker, Character target, int baseDamage, int level
    ) => OnBeforeDealDamage(attacker, target, baseDamage);

    public virtual void OnAfterDealDamage(
        Character attacker, Character target, int damageDealt
    )
    { }

    public virtual void OnAfterDealDamage(
        Character attacker, Character target, int damageDealt, int level
    ) => OnAfterDealDamage(attacker, target, damageDealt);

    // Heal
    public virtual void OnHeal(Character self, ref int amount) { }

    public virtual void OnHeal(Character self, ref int amount, int level)
        => OnHeal(self, ref amount);

    // Shield
    public virtual void OnAddShield(Character self, ref int amount) { }

    public virtual void OnAddShield(Character self, ref int amount, int level)
        => OnAddShield(self, ref amount);


    // Equip
    public virtual void OnEquip(Character self) { }

    public virtual void OnEquip(Character self, int level)
        => OnEquip(self);

    public virtual void OnUnequip(Character self) { }

    public virtual void OnUnequip(Character self, int level)
        => OnUnequip(self);
}