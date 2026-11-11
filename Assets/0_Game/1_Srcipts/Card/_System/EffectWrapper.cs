using System.Collections.Generic;
using UnityEngine;

public enum EffectRunCondition { Always, IfPreviousHit, IfHandEmpty }

public enum EffectTargetMode
{
    Default,            // target được kéo thả
    Self,
    AllEnemies,
    RandomEnemies,      // n con ngẫu nhiên (targetCount)
    AllCharacters,

    // --- Theo vị trí ---
    FrontEnemy,
    BackEnemy,
    DefaultAndBehind,
    DefaultAndFront,
    DefaultAndAdjacent,
    OnlyBehindDefault
}

[System.Serializable]
public class EffectWrapper
{
    public EffectData effect;
    public EffectRunCondition runCondition = EffectRunCondition.Always;

    [Header("Target")]
    public EffectTargetMode targetMode = EffectTargetMode.Default;
    public int targetCount = 2;   // RandomEnemies: số con
    public int slotRange = 1;     // DefaultAndBehind/Front: số con lấy thêm

    [Header("Base")]
    public bool overrideValue;
    public int valueOverride;

    [Header("Upgrade")]
    public bool upgradeOverrideValue;
    public int upgradeValueOverride;

    public bool ShouldRun(EffectContext ctx)
    {
        switch (runCondition)
        {
            case EffectRunCondition.IfPreviousHit:
                return targetMode == EffectTargetMode.Default
                    ? ctx.lastHit
                    : ctx.hitTargets.Contains(ctx.target);

            case EffectRunCondition.IfHandEmpty:
                return ctx.deck != null && ctx.deck.GetCurrentHandObjects().Count == 0;

            default:
                return true;
        }
    }

    public void FillContext(EffectContext ctx)
    {
        if (ctx.level >= 2 && upgradeOverrideValue)
        {
            ctx.hasOverride = true;
            ctx.overrideValue = upgradeValueOverride;
        }
        else if (overrideValue)
        {
            ctx.hasOverride = true;
            ctx.overrideValue = valueOverride;
        }
        else
        {
            ctx.hasOverride = false;
        }
    }

    // ===== Xác định danh sách target =====
    public List<Character> ResolveTargets(EffectContext ctx)
    {
        var list = new List<Character>();

        switch (targetMode)
        {
            case EffectTargetMode.Self:
                list.Add(ctx.self);
                break;

            case EffectTargetMode.AllEnemies:
                AddAliveEnemies(ctx, list);
                break;

            case EffectTargetMode.RandomEnemies:
                AddAliveEnemies(ctx, list);
                for (int i = 0; i < list.Count; i++)
                {
                    int r = Random.Range(i, list.Count);
                    (list[i], list[r]) = (list[r], list[i]);
                }
                if (list.Count > targetCount)
                    list.RemoveRange(targetCount, list.Count - targetCount);
                break;

            case EffectTargetMode.AllCharacters:
                AddAliveEnemies(ctx, list);
                if (ctx.player != null) list.Add(ctx.player);
                break;

            case EffectTargetMode.FrontEnemy:
                AddIfNotNull(list, SlotManager.Instance.GetFrontEnemy());
                break;

            case EffectTargetMode.BackEnemy:
                AddIfNotNull(list, SlotManager.Instance.GetBackEnemy());
                break;

            case EffectTargetMode.DefaultAndBehind:
            case EffectTargetMode.DefaultAndFront:
            case EffectTargetMode.DefaultAndAdjacent:
            case EffectTargetMode.OnlyBehindDefault:
                {
                    var main = ctx.target as EnemyView;
                    if (main == null) break;

                    var slots = SlotManager.Instance;

                    if (targetMode != EffectTargetMode.OnlyBehindDefault)
                        list.Add(main);

                    bool behind = targetMode != EffectTargetMode.DefaultAndFront;
                    bool front = targetMode == EffectTargetMode.DefaultAndFront
                               || targetMode == EffectTargetMode.DefaultAndAdjacent;

                    int range = targetMode == EffectTargetMode.DefaultAndAdjacent
                        ? 1 : Mathf.Max(1, slotRange);

                    if (behind)
                        for (int s = 1; s <= range; s++)
                            AddIfNotNull(list, slots.GetEnemyOffset(main, s));

                    if (front)
                        for (int s = 1; s <= range; s++)
                            AddIfNotNull(list, slots.GetEnemyOffset(main, -s));
                    break;
                }

            default:
                list.Add(ctx.target);   
                break;
        }

        return list;
    }

    private static void AddAliveEnemies(EffectContext ctx, List<Character> list)
    {
        if (ctx.enemies == null) return;
        foreach (var e in ctx.enemies)
            if (e != null && e.stats.currentHP > 0) list.Add(e);
    }

    private static void AddIfNotNull(List<Character> list, Character c)
    {
        if (c != null && !list.Contains(c)) list.Add(c);
    }

}