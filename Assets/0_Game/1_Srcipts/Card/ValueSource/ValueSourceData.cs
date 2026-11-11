using UnityEngine;

public abstract class ValueSourceData : ScriptableObject
{
    // Trả về số lượng đếm được (ví dụ số lá Melee trên tay)
    public abstract int GetValue(EffectContext ctx);
}