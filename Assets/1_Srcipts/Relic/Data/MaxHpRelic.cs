using UnityEngine;

[CreateAssetMenu(fileName = "MaxHpRelic", menuName = "Relics/Max HP +8")]
public class MaxHpRelic : Relic
{
    public int bonusHP = 8;

    public override void OnEquip(Character self)
    {
        self.stats.maxHP += bonusHP;
        self.stats.currentHP += bonusHP; // hồi thêm máu
        self.UpdateUI();
    }

    public override void OnUnequip(Character self)
    {
        self.stats.maxHP -= bonusHP;
        self.stats.currentHP = Mathf.Min(self.stats.currentHP, self.stats.maxHP);
        self.UpdateUI();
    }
}
