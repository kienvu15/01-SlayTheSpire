using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public static partial class CardEffectRunner
{
    public static IEnumerator RunRoutine(CardInstance card, Character self, Character target,
        ManaSystem mana, Deck deck, Character player, List<EnemyView> enemies,
        Action<bool> done, float gap = 0.15f)
    {
        bool allSuccess = true;

        if (card.data.effects != null)
        {
            var ctx = new EffectContext
            {
                self = self,
                target = target,
                manaSystem = mana,
                deck = deck,
                level = card.level,
                sourceCard = card,
                player = player,
                enemies = enemies
            };
            Character dragTarget = target;

            foreach (var wrapper in card.data.effects)
            {
                if (wrapper == null || wrapper.effect == null) continue;

                wrapper.FillContext(ctx);
                ctx.target = dragTarget;

                if (!wrapper.effect.RequiresTarget)
                {
                    if (!wrapper.ShouldRun(ctx)) continue;

                    bool ok = true;
                    yield return wrapper.effect.ApplyRoutine(ctx, r => ok = r);
                    if (!ok) allSuccess = false;
                    if (gap > 0) yield return new WaitForSeconds(gap);
                    continue;
                }

                foreach (var t in wrapper.ResolveTargets(ctx).ToArray())
                {
                    if (t != null && t != ctx.self && t.stats.currentHP <= 0) continue;

                    ctx.target = t;
                    if (!wrapper.ShouldRun(ctx)) continue;

                    bool ok = true;
                    yield return wrapper.effect.ApplyRoutine(ctx, r => ok = r);
                    if (!ok) allSuccess = false;
                    if (gap > 0) yield return new WaitForSeconds(gap);
                }
            }
        }

        done(allSuccess);
    }
}