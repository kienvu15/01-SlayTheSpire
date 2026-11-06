using UnityEngine;

[CreateAssetMenu(fileName = "MaxHpRelic", menuName = "Relics/Max HP")]
public class MaxHpRelic : Relic
{
    [Header("HP Bonus")]
    public int defaultBonusHP = 8;
    public int upgradeBonusHP = 16;

    public override void OnEquip(Character self, int level)
    {
        int bonus = level >= 2 ? upgradeBonusHP : defaultBonusHP;

        self.stats.maxHP += bonus;
        self.stats.currentHP += bonus;

        self.UpdateUI();
    }

    public override void OnUnequip(Character self, int level)
    {
        int bonus = level >= 2 ? upgradeBonusHP : defaultBonusHP;

        self.stats.maxHP -= bonus;
        self.stats.currentHP = Mathf.Min(
            self.stats.currentHP,
            self.stats.maxHP
        );

        self.UpdateUI();
    }
}