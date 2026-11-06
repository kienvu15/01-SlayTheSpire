using UnityEngine;

[System.Serializable]
public class RelicInstance
{
    public Relic relicData;
    public int level = 1;

    [System.NonSerialized]
    public bool isEquipped;

    public string GetDescription()
    {
        return level >= 2
            ? relicData.upgradeDescription
            : relicData.defaultDescription;
    }

    public bool CanUpgrade()
    {
        return relicData != null && level < relicData.maxLevel;
    }

    public void Equip(Character owner)
    {
        if (isEquipped || relicData == null) return;

        relicData.OnEquip(owner, level);
        isEquipped = true;
    }

    public void Unequip(Character owner)
    {
        if (!isEquipped || relicData == null) return;

        relicData.OnUnequip(owner, level);
        isEquipped = false;
    }

    public void Upgrade(Character owner)
    {
        if (!CanUpgrade()) return;

        // Nếu đang trang bị thì tháo hiệu ứng cấp cũ
        if (isEquipped)
        {
            relicData.OnUnequip(owner, level);
        }

        level++;

        // Áp dụng hiệu ứng cấp mới
        if (isEquipped)
        {
            relicData.OnEquip(owner, level);
        }
    }
}