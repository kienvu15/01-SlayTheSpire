using UnityEngine;

[CreateAssetMenu(fileName = "GainManaEffect", menuName = "Cards/Effects/Gain Mana")]
public class GainManaEffect : EffectData
{
    public int amount = 1;

    public override bool Apply(Character self, Character target, ManaSystem manaSystem, Deck deck)
    {
        if (manaSystem == null) return false;

        int oldMana = manaSystem.currentMana;
        manaSystem.currentMana = Mathf.Min(manaSystem.currentMana + amount, manaSystem.maxMana);

        manaSystem.SendMessage("UpdateManaUI", SendMessageOptions.DontRequireReceiver);

        // nếu có thay đổi mana thì coi như thành công
        return manaSystem.currentMana > oldMana;
    }
}
