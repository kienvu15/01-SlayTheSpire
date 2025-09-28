using System.Collections.Generic;
using UnityEngine;

public enum Type
{
    Attack,
    Buff,
    BadBuff,
    Charge,
    Unknown
}

[CreateAssetMenu(fileName = "NewEnemyAction", menuName = "Enemy/Action")]
public class EnemyAction : ScriptableObject
{
    public Type type;
    public string actionName;
    public int cooldown; // số lượt chờ trước khi dùng
    [HideInInspector] public int currentCooldown;

    // List các effect của action
    public List<EffectData> effects;

    public void Apply(Character self, Character target = null, List<int> overrideValues = null)
    {
        // ====== Determine default target ======
        switch (type)
        {
            case Type.Buff:
                target = self; // buff bản thân
                break;

            case Type.Attack:
            case Type.BadBuff:
            case Type.Charge:
                if (target == null)
                {
                    target = BattleManager.Instance.player;
                }
                break;

            default:
                if (target == null)
                {
                    Debug.LogWarning($"[EnemyAction] {actionName} không có target!");
                }
                break;
        }

        // ====== Apply effects ======
        for (int i = 0; i < effects.Count; i++)
        {
            var effect = effects[i];
            if (effect == null) continue;

            int overrideValue = (overrideValues != null && i < overrideValues.Count) ? overrideValues[i] : -1;

            if (effect is IOverrideValue o && overrideValue > -1)
            {
                o.ApplyWithOverride(self, target, overrideValue);
            }
            else
            {
                effect.Apply(self, target, null, null);
            }
        }
    }
}
