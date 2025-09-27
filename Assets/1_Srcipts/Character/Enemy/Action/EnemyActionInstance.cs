using System.Collections.Generic;

[System.Serializable]
public class EnemyActionInstance
{
    public EnemyAction baseAction;
    public int currentCooldown;

    public EnemyActionInstance(EnemyAction action)
    {
        baseAction = action;
        currentCooldown = 0; // bắt đầu sẵn sàng
    }

    public void Apply(EnemyView owner, Character target, List<int> overrides)
    {
        baseAction.Apply(owner, target, overrides);
        currentCooldown = baseAction.cooldown;
    }

    public Type Type => baseAction.type;

    public List<EffectData> Effects => baseAction.effects;
}
