using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MultiHitTargeting { SameTarget, RandomEnemy }
public enum MultiHitOverride { DamagePerHit, HitCount }

[CreateAssetMenu(menuName = "Cards/Effects/Multi Hit Damage")]
public class MultiHitDamageEffect : EffectData
{
    [Header("Hit")]
    public int damagePerHit = 2;
    public int hits = 5;

    [Header("Impact VFX Override")]
    public CardType vfxType = CardType.Mellee;
    public ImpactProfile impactProfile;

    [Header("Target")]
    public MultiHitTargeting targeting = MultiHitTargeting.SameTarget;
    public bool stopWhenTargetDies = true;   

    [Header("Override value của wrapper dùng cho")]
    public MultiHitOverride overrideApplies = MultiHitOverride.HitCount;

    [Header("Timing")]
    public float hitInterval = 0.12f;       

    public override bool RequiresTarget => targeting == MultiHitTargeting.SameTarget;
    public override bool CountsAsIntend => true;

    private int GetDamage(EffectContext ctx) =>
        overrideApplies == MultiHitOverride.DamagePerHit ? ctx.GetValue(damagePerHit) : damagePerHit;

    private int GetHits(EffectContext ctx) =>
        Mathf.Max(1, overrideApplies == MultiHitOverride.HitCount ? ctx.GetValue(hits) : hits);

    public override int GetIntentValue(EffectContext ctx) => GetDamage(ctx) * GetHits(ctx);

    public override bool Apply(EffectContext ctx)
    {
        if (ctx.self == null) return false;
        if (targeting == MultiHitTargeting.SameTarget && ctx.target == null)
            return false;

        int dmg = GetDamage(ctx);
        int count = GetHits(ctx);

        Character self = ctx.self;
        Character fixedTarget = ctx.target;
        List<EnemyView> enemies = ctx.enemies;

        // Ưu tiên ImpactProfile riêng của Effect
        ImpactProfile selectedProfile =
            impactProfile != null ? impactProfile : ctx.impactProfile;

        if (hitInterval <= 0f)
        {
            RunHitsInstant(
                ctx, self, fixedTarget, enemies,
                dmg, count, selectedProfile
            );
        }
        else
        {
            ctx.lastHit = true;

            self.StartCoroutine(
                RunHitsRoutine(
                    self, fixedTarget, enemies,
                    dmg, count, selectedProfile
                )
            );
        }

        return true;
    }

    private void RunHitsInstant(
        EffectContext ctx,
        Character self,
        Character fixedTarget,
        List<EnemyView> enemies,
        int dmg,
        int count,
        ImpactProfile selectedProfile)
    {
        bool anyHit = false;

        for (int i = 0; i < count; i++)
        {
            Character t = PickTarget(fixedTarget, enemies);

            if (t == null) break;

            if (self.DealDamage(t, dmg, selectedProfile))
            {
                anyHit = true;
                ctx.hitTargets.Add(t);
            }
        }

        ctx.lastHit = anyHit;
    }

    private IEnumerator RunHitsRoutine(
        Character self,
        Character fixedTarget,
        List<EnemyView> enemies,
        int dmg,
        int count,
        ImpactProfile selectedProfile)
    {
        GameStage.Instance.SetBusy(true);

        for (int i = 0; i < count; i++)
        {
            if (self == null) break;

            Character t = PickTarget(fixedTarget, enemies);

            if (t == null) break;

            self.DealDamage(t, dmg, selectedProfile);

            yield return new WaitForSeconds(hitInterval);
        }

        GameStage.Instance.SetBusy(false);
    }

    // null = hết target, dừng chuỗi đánh
    private Character PickTarget(Character fixedTarget, List<EnemyView> enemies)
    {
        if (targeting == MultiHitTargeting.SameTarget)
        {
            if (stopWhenTargetDies && fixedTarget.stats.currentHP <= 0) return null;
            return fixedTarget;
        }

        // Random: chọn lại trong số enemy còn sống ở MỖI hit
        if (enemies == null) return null;

        var alive = new List<EnemyView>();
        foreach (var e in enemies)
            if (e != null && e.stats.currentHP > 0) alive.Add(e);

        return alive.Count == 0 ? null : alive[Random.Range(0, alive.Count)];
    }
}