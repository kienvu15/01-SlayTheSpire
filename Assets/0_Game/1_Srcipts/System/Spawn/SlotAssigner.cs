using System.Collections.Generic;
using UnityEngine;

public static class SlotAssigner
{
    public static List<(EnemyType type, Spot slot)> AssignSlots(
        EnemyType[] enemyTypes,
        Spot[] slots)
    {
        List<(EnemyType, Spot)> result = new List<(EnemyType, Spot)>();
        HashSet<Spot> used = new HashSet<Spot>();

        Spot lastFrontline = null; // frontline = Tank hoặc Fighter

        foreach (var type in enemyTypes)
        {
            Spot chosen = null;

            if (type == EnemyType.Tank)
            {
                // Tank ưu tiên slot 0 nếu trống
                if (slots.Length > 0 && !used.Contains(slots[0]))
                    chosen = slots[0];
            }
            else if (type == EnemyType.Boss)
            {
                // Boss ưu tiên slot cuối (chừa chỗ cho support nếu có)
                int bossIdx = slots.Length - 1;
                if (System.Array.Exists(enemyTypes, t => t == EnemyType.Support))
                    bossIdx = Mathf.Max(0, slots.Length - 2);

                if (!used.Contains(slots[bossIdx]))
                    chosen = slots[bossIdx];
            }
            else if (type == EnemyType.Support)
            {
                // Support: tìm slot ngay sau frontline gần nhất
                if (lastFrontline != null)
                {
                    int idx = System.Array.IndexOf(slots, lastFrontline);
                    if (idx + 1 < slots.Length && !used.Contains(slots[idx + 1]))
                        chosen = slots[idx + 1];
                }
            }

            // nếu chưa chọn được thì lấy slot trống đầu tiên
            if (chosen == null)
            {
                foreach (var s in slots)
                {
                    if (!used.Contains(s))
                    {
                        chosen = s;
                        break;
                    }
                }
            }

            if (chosen != null)
            {
                result.Add((type, chosen));
                used.Add(chosen);

                // Tank + Fighter coi như frontline
                if (type == EnemyType.Tank || type == EnemyType.Fighter)
                    lastFrontline = chosen;
            }
        }

        return result;
    }
}
