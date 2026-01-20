using UnityEngine;

[CreateAssetMenu(fileName = "DrunkEffect", menuName = "Cards/Effects/Apply_Drunk")]
public class DrunkEffectData : EffectData
{
    [Tooltip("Số lượt tồn tại Drunk")]
    public int duration = 2;

    public CardType vfxType = CardType.Magic;

    public override bool Apply(Character self, Character target, ManaSystem manaSystem, Deck deck)
    {
        Character realTarget = target != null ? target : self;
        if (realTarget == null) return false;

        Condition drunk = new Condition
        {
            type = ConditionType.Drunk,
            duration = duration
        };

        bool isFromPlayer = self is Player;
        realTarget.AddCondition(drunk, isFromPlayer, vfxType);

        //Debug.Log($"[DrunkEffect] {self.name} applied Drunk({duration}) to {realTarget.name}");
        return true;
    }

    public override int GetIntentValue()
    {
        return duration;
    }
}
