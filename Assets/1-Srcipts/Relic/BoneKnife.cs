using UnityEngine;

[CreateAssetMenu(fileName = "BoneKnife", menuName = "Relics/BoneKnife")]
public class BoneKnife : Relic
{
    public int bonusDamage = 1;

    public override int OnBeforeDealDamage(Character attacker, Character target, int baseDamage)
    {
        return baseDamage + bonusDamage;
    }
}
