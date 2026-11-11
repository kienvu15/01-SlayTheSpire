using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SlotManager : MonoBehaviour
{

    public static SlotManager Instance;
    private Spot[] slots;

    private void Awake()
    {
        Instance = this;
        slots = GetComponentsInChildren<Spot>()
                .OrderBy(s => s.transform.rotation.z)
                .ToArray();
    }

    public Spot[] GetAllSlots()
    {
        return slots;
    }

    public Spot GetSlotByIndex(int index)
    {
        if (index >= 0 && index < slots.Length)
        {
            return slots[index];
        }
        return null;
    }

    public Spot GetFreeSlot()
    {
        return slots.FirstOrDefault(s => !s.isOccupied);
    }

    public Spot GetBackSlot(Spot currentSlot)
    {
        int index = System.Array.IndexOf(slots, currentSlot);
        if(index >= 0 && index < slots.Length - 1)
        {
            return slots[index + 1];
        }
        return null;
    }

    public Spot GetFrontSlot(Spot currentSlot)
    {
        int index = System.Array.IndexOf(slots, currentSlot);
        if(index > 0)
        {
            return slots[index - 1];
        }
        return null;
    }

    // Helper
    public List<EnemyView> GetEnemiesInOrder()
    {
        var list = new List<EnemyView>();
        foreach (var s in slots)
        {
            if (s.isOccupied && s.occupant != null && s.occupant.stats.currentHP > 0)
                list.Add(s.occupant);
        }
        return list;
    }

    public EnemyView GetFrontEnemy()
    {
        var list = GetEnemiesInOrder();
        return list.Count > 0 ? list[0] : null;
    }

    public EnemyView GetBackEnemy()
    {
        var list = GetEnemiesInOrder();
        return list.Count > 0 ? list[list.Count - 1] : null;
    }

    public EnemyView GetEnemyOffset(EnemyView from, int steps)
    {
        var list = GetEnemiesInOrder();
        int i = list.IndexOf(from);
        if (i < 0) return null;

        int j = i + steps;
        return (j >= 0 && j < list.Count) ? list[j] : null;
    }

    // steps > 0: đẩy ra sau (knock back), steps < 0: kéo lên trước (pull forward).
    // Trả về số slot thực sự đã di chuyển.
    public int MoveEnemy(EnemyView enemy, int steps)
    {
        if (enemy == null || enemy.currentSlot == null || steps == 0) return 0;
        if (!enemy.CanBeMoved) return 0;

        int dir = steps > 0 ? 1 : -1;
        int remaining = Mathf.Abs(steps);
        int moved = 0;
        var affected = new HashSet<EnemyView> { enemy };

        while (remaining > 0)
        {
            int cur = System.Array.IndexOf(slots, enemy.currentSlot);
            int next = cur + dir;
            if (cur < 0 || next < 0 || next >= slots.Length) break;   // chạm biên

            Spot to = slots[next];
            Spot from = slots[cur];

            // Slot đích có enemy còn sống không? (xác đang mờ dần coi như trống)
            EnemyView other = (to.isOccupied && to.occupant != null
                               && to.occupant != enemy && to.occupant.stats.currentHP > 0)
                ? to.occupant : null;

            if (other != null && !other.CanBeMoved) break;   // bị chặn bởi enemy không thể đẩy

            enemy.AssginToSlot(to);            // chiếm slot đích
            if (other != null)
            {
                other.AssginToSlot(from);      // đổi chỗ: enemy kia sang slot cũ
                affected.Add(other);
            }

            moved++;
            remaining--;
        }

        foreach (var e in affected)
            if (e != null) e.PlayMoveVisual(e.currentSlot);

        return moved;
    }

}
