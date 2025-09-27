using UnityEngine;

public abstract class Relic : ScriptableObject
{
    public string relicName;
    [TextArea] public string description;
    public Sprite icon;   // để show trong UI

    // Hook events để các relic override tuỳ ý
    public virtual int OnBeforeDealDamage(Character attacker, Character target, int baseDamage) => baseDamage;
    public virtual void OnAfterDealDamage(Character attacker, Character target, int damageDealt) { }
    public virtual void OnHeal(Character self, ref int amount) { }
    public virtual void OnAddShield(Character self, ref int amount) { }

    //EQ
    public virtual void OnEquip(Character self) { }
    public virtual void OnUnequip(Character self) { }
}
