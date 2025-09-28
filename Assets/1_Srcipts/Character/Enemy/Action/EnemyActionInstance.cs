using System.Collections.Generic;

[System.Serializable]
public class EnemyActionInstance
{
    public EnemyAction baseAction;
    public int baseCooldown;       // cooldown gốc sau khi override
    public int currentCooldown;

    public EnemyActionInstance(EnemyAction action, int? overrideCooldown = null)
    {
        baseAction = action;
        baseCooldown = overrideCooldown.HasValue ? overrideCooldown.Value : action.cooldown;
        currentCooldown = 0; // bắt đầu sẵn sàng
    }

    public void Apply(EnemyView owner, Character target, List<int> overrides)
    {
        baseAction.Apply(owner, target, overrides);
        currentCooldown = baseCooldown; // dùng cooldown đã override
    }

    public Type Type => baseAction.type;

    public List<EffectData> Effects => baseAction.effects;
}

