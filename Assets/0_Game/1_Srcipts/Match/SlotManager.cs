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


}
