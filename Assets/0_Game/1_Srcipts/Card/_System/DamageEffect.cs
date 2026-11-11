using UnityEngine;

[CreateAssetMenu(menuName = "Cards/Effects/Damage")]
public class DamageEffect : EffectData
{
    public int damage = 5;
    //public CardType vfxType = CardType.Mellee;

    [Header("Impact VFX Override")]
    public ImpactProfile impactProfile;

    [Header("Bonus theo giá trị động")]
    public ValueSourceData bonusSource;   // để trống = không có bonus
    public int bonusPerUnit = 1;          // +1 damage mỗi đơn vị

    public override bool RequiresTarget => true;
    public override bool CountsAsIntend => true;

    private int GetTotalDamage(EffectContext ctx)
    {
        int total = ctx.GetValue(damage);
        if (bonusSource != null)
            total += bonusSource.GetValue(ctx) * bonusPerUnit;
        return total;
    }

    public override bool Apply(EffectContext ctx)
    {
        if (ctx.target == null) return false;
        ImpactProfile selectedProfile = impactProfile != null ? impactProfile : ctx.impactProfile;
        bool hit = ctx.self.DealDamage(ctx.target, GetTotalDamage(ctx), selectedProfile);

        ctx.lastHit = hit;
        if (hit) ctx.hitTargets.Add(ctx.target);
        return true;
    }

    public override int GetIntentValue(EffectContext ctx) => GetTotalDamage(ctx);
}