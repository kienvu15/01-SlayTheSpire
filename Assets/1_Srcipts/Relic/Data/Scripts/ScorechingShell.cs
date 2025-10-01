using UnityEngine;

[CreateAssetMenu(fileName = "ScorechingShell", menuName = "Relics/ScoreChing")]
public class ScorechingShell : Relic
{
    public int bonusHP = 10;
    public int bonusMana = 2;

    public override void OnEquip(Character self)
    {
        self.stats.maxHP -= bonusHP;
        self.stats.currentHP -= bonusHP; // hồi thêm máu
        self.UpdateUI();

        ManaSystem mana = self.GetComponentInChildren<ManaSystem>();
        if (mana != null)
        {
            mana.maxMana += bonusMana;
            mana.currentMana += bonusMana;
            mana.StartTurn(); // refresh UI
        }
    }

    public override void OnUnequip(Character self)
    {
        self.stats.maxHP -= bonusHP;
        self.stats.currentHP = Mathf.Min(self.stats.currentHP, self.stats.maxHP);
        self.UpdateUI();

        ManaSystem mana = self.GetComponentInChildren<ManaSystem>();
        if (mana != null)
        {
            mana.maxMana -= bonusMana;
            mana.currentMana = Mathf.Min(mana.currentMana, mana.maxMana);
            mana.StartTurn(); // refresh UI
        }
    }
}
