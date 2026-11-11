using UnityEngine;

[CreateAssetMenu(menuName = "Cards/Effects/DamageByShield")]
public class DamageByShieldEffect : EffectData
{
    public CardType vfxType = CardType.Mellee;

    [Header("Impact VFX Override")]
    public ImpactProfile impactProfile;

    public override bool CountsAsIntend => true;

    public override bool Apply(EffectContext ctx)
    {
        if (ctx.target == null || ctx.self == null)
            return false;

        int damage = ctx.self.stats.shield;

        if (damage <= 0)
            return true;

        ImpactProfile selectedProfile = impactProfile != null ? impactProfile : ctx.impactProfile;
        ctx.lastHit = ctx.self.DealDamage(
            ctx.target,
            damage,
            selectedProfile
        );

        return true;
    }

    public override int GetIntentValue(EffectContext ctx)
    {
        if (ctx.self == null)
            return 0;

        return ctx.self.stats.shield;
    }
}