using UnityEngine;

public static class DamageCalculator
{
    public static int CalculateDamage(Character attacker, Character target, int baseDamage)
    {
        if (attacker == null || target == null) return 0;

        CharacterStats aStats = attacker.stats;
        CharacterStats tStats = target.stats;

        // Crit check
        bool isCrit = Random.value < aStats.critChance;
        float multiplier = isCrit ? aStats.critDamage : 1f;

        // Defense giảm damage
        int finalDamage = Mathf.Max(0, Mathf.RoundToInt(baseDamage * multiplier) - tStats.defense);

        // Apply damage
        target.TakeDamage(finalDamage);

        

        Debug.Log($"[DamageCalculator] {attacker.name} dealt {finalDamage} damage to {target.name} (Crit: {isCrit})");

        return finalDamage;
    }
}
