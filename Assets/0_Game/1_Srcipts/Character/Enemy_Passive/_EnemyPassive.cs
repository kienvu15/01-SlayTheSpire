using UnityEngine;

public abstract class _EnemyPassive : ScriptableObject
{
    public virtual void OnAttack(EnemyView owner, Character target, EnemyActionInstance action) { }
    public virtual void OnTakeDamage(EnemyView owner, int amount) { }
    public virtual void OnDeath(EnemyView owner) { }
}