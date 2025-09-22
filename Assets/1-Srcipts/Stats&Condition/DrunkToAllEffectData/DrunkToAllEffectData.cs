using UnityEngine;

[CreateAssetMenu(fileName = "DrunkToAllEffect", menuName = "Cards/Effects/Drunk To All")]
public class DrunkToAllEffectData : EffectData
{
    public int duration = 2;   // số lượt tồn tại mặc định
    public CardType vfxType = CardType.Magic; // set trong inspector

    public override bool Apply(Character self, Character target, ManaSystem manaSystem, Deck deck)
    {
        // tìm player và enemy trong scene
        Match match = GameObject.FindFirstObjectByType<Match>();
        if (match == null) return false;

        bool applied = false;

        // --- áp dụng cho Player ---
        if (match.player != null)
        {
            Condition drunk = new Condition
            {
                type = ConditionType.Drunk,
                duration = duration
            };
            match.player.AddCondition(drunk, vfxType);
            Debug.Log($"[DrunkToAllEffect] Applied Drunk x ({drunk.duration}) to {match.player.name}");
            applied = true;
        }

        // --- áp dụng cho toàn bộ Enemies ---
        EnemySystem enemySystem = Object.FindFirstObjectByType<EnemySystem>();
        if (enemySystem != null)
        {
            foreach (var enemy in enemySystem.enemies)
            {
                if (enemy == null) continue;
                Condition drunk = new Condition
                {
                    type = ConditionType.Drunk,
                    duration = duration
                };
                enemy.AddCondition(drunk, vfxType);
//                AttackImpactManager.Instance.ShowConditionImpact(enemy.transform, vfxType);

                Debug.Log($"[DrunkToAllEffect] Applied Drunk x ({drunk.duration}) to {enemy.name}");
                applied = true;
            }
        }

        return applied;
    }

    public override int GetIntentValue()
    {
        return duration;
    }
}
