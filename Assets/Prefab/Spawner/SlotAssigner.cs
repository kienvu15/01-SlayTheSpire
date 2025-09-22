using System.Collections.Generic;
using UnityEngine;

public class SlotAssigner
{
    public static Dictionary<EnemyType, Spot> AssignSlots(EnemyType[] types, Spot[] slots)
    {
        Dictionary<EnemyType, Spot> assigned = new Dictionary<EnemyType, Spot>();
        List<int> usedSlots = new List<int>();

        int n = slots.Length;

        // --- 1. Tank luôn ở đầu
        int tankIndex = System.Array.IndexOf(types, EnemyType.Tank);
        if (tankIndex >= 0 && n > 0)
        {
            assigned[EnemyType.Tank] = slots[0];
            usedSlots.Add(0);
        }

        // --- 2. Boss xử lý
        int bossIndex = System.Array.IndexOf(types, EnemyType.Boss);
        if (bossIndex >= 0 && n > 0)
        {
            int bossSlotIndex = n - 1;

            // Nếu có Support và boss đang cuối -> dời boss lên 1 để chừa chỗ cho support
            if (System.Array.Exists(types, t => t == EnemyType.Support))
            {
                bossSlotIndex = Mathf.Max(0, n - 2);
            }

            assigned[EnemyType.Boss] = slots[bossSlotIndex];
            usedSlots.Add(bossSlotIndex);
        }

        // --- 3. Support: phải ở sau 1 enemy nào đó
        int supportCount = System.Array.FindAll(types, t => t == EnemyType.Support).Length;
        for (int i = 0; i < supportCount; i++)
        {
            // tìm 1 slot trống sau enemy
            for (int j = 1; j < n; j++) // support không bao giờ ở slot[0]
            {
                if (!usedSlots.Contains(j) && usedSlots.Contains(j - 1))
                {
                    assigned[EnemyType.Support] = slots[j];
                    usedSlots.Add(j);
                    break;
                }
            }
        }

        // --- 4. Fighter: điền vào chỗ trống còn lại
        int fighterCount = System.Array.FindAll(types, t => t == EnemyType.Fighter).Length;
        for (int i = 0; i < n; i++)
        {
            if (fighterCount <= 0) break;
            if (!usedSlots.Contains(i))
            {
                assigned[EnemyType.Fighter] = slots[i];
                usedSlots.Add(i);
                fighterCount--;
            }
        }

        return assigned;
    }
}
