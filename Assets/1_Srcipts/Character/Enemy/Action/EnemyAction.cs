using System.Collections.Generic;
using UnityEngine;

public enum Type
{
    Attack,
    Buff,
    AddShield,
    Heal,
    BadBuff,
    Charge,
    Unknown
}

public enum BuffTargetMode
{
    SelfOnly,       // luôn buff bản thân
    SelfOrAlly,     // random bản thân hoặc ally
    AllyOnly        // random ally khác (không buff bản thân)
}

[CreateAssetMenu(fileName = "NewEnemyAction", menuName = "Enemy/Action")]
public class EnemyAction : ScriptableObject
{
    public Type type;
    public BuffTargetMode buffTargetMode; // chỉ dùng khi type = Buff   
    public string actionName;
    public int cooldown; // số lượt chờ trước khi dùng

    [HideInInspector]
    public int currentCooldown;

    // List các effect của action
    public List<EffectData> effects;

    public void Apply(Character self, Character target = null, List<int> overrideValues = null)
    {
        // ====== Determine default target ======
        switch (type)
        {
            case Type.Buff:
                if (target == null || target.faction == Faction.Player)
                {
                    switch (buffTargetMode)
                    {
                        case BuffTargetMode.SelfOnly:
                            target = self;
                            break;

                        case BuffTargetMode.SelfOrAlly:
                            if (Random.value < 0.5f || BattleManager.Instance.enemies.Count <= 1)
                            {
                                target = self;
                            }
                            else
                            {
                                target = BattleManager.Instance.GetRandomAlly(self, false);
                            }
                            break;

                        case BuffTargetMode.AllyOnly:
                            target = BattleManager.Instance.GetRandomAlly(self, false);
                            break;
                    }
                }
                break;

            case Type.Heal:
                if (target == null || target.faction == Faction.Player)
                {
                    switch (buffTargetMode)
                    {
                        case BuffTargetMode.SelfOnly:
                            target = self;
                            break;

                        case BuffTargetMode.SelfOrAlly:
                            if (Random.value < 0.3f || BattleManager.Instance.enemies.Count <= 1)
                            {
                                target = self;
                            }
                            else
                            {
                                target = BattleManager.Instance.GetRandomAlly(self, false);
                            }
                            break;

                        case BuffTargetMode.AllyOnly:
                            target = BattleManager.Instance.GetRandomAlly(self, false);
                            break;
                    }
                }
                break;

            case Type.AddShield:
                if (target == null || target.faction == Faction.Player)
                {
                    switch (buffTargetMode)
                    {
                        case BuffTargetMode.SelfOnly:
                            target = self;
                            break;

                        case BuffTargetMode.SelfOrAlly:
                            if (Random.value < 0.5f || BattleManager.Instance.enemies.Count <= 1)
                            {
                                target = self;
                            }
                            else
                            {
                                target = BattleManager.Instance.GetRandomAlly(self, false);
                            }
                            break;

                        case BuffTargetMode.AllyOnly:
                            target = BattleManager.Instance.GetRandomAlly(self, false);
                            break;
                    }
                }
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

            int overrideValue = (overrideValues != null && i < overrideValues.Count)
                ? overrideValues[i]
                : -1;

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