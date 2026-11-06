using UnityEngine;

[CreateAssetMenu(fileName = "ScorechingShell", menuName = "Relics/ScoreChing")]
public class ScorechingShell : Relic
{
    [Header("Default - Level 1")]
    public int defaultBonusHP = 10;
    public int defaultBonusMana = 2;

    [Header("Upgrade - Level 2")]
    public int upgradeBonusHP = 15;
    public int upgradeBonusMana = 4;

    public override void OnEquip(Character self, int level)
    {
        int hp = level >= 2 ? upgradeBonusHP : defaultBonusHP;
        int manaBonus = level >= 2 ? upgradeBonusMana : defaultBonusMana;

        // HP
        self.stats.maxHP -= hp;
        self.stats.currentHP = Mathf.Max(1, self.stats.currentHP - hp);
        self.UpdateUI();

        // Mana
        ManaSystem mana = self.GetComponentInChildren<ManaSystem>();

        if (mana != null)
        {
            mana.maxMana += manaBonus;
            mana.currentMana += manaBonus;
            mana.StartTurn();
        }
    }

    public override void OnUnequip(Character self, int level)
    {
        int hp = level >= 2 ? upgradeBonusHP : defaultBonusHP;
        int manaBonus = level >= 2 ? upgradeBonusMana : defaultBonusMana;

        // HP
        self.stats.maxHP += hp;
        self.stats.currentHP = Mathf.Min(
            self.stats.currentHP,
            self.stats.maxHP
        );

        self.UpdateUI();

        // Mana
        ManaSystem mana = self.GetComponentInChildren<ManaSystem>();

        if (mana != null)
        {
            mana.maxMana -= manaBonus;
            mana.currentMana = Mathf.Min(
                mana.currentMana,
                mana.maxMana
            );

            mana.StartTurn();
        }
    }
}