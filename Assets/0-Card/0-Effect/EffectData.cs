using UnityEngine;

public abstract class EffectData : ScriptableObject
{
    [TextArea] public string description;

    // hàm này bắt buộc các effect phải implement
    public abstract bool Apply(Character self, Character target, ManaSystem manaSystem, Deck deck);

    public virtual int GetIntentValue()
    {
        return 0;
    }

}
