public interface IOverrideValue
{
    void ApplyWithOverride(Character self, Character target, int overrideValue);
    int GetIntentValue(int? overrideValue = null);
}
