using UnityEngine;

public abstract class RequirementData : ScriptableObject
{
    public abstract bool IsMet(RequirementContext ctx);

    public virtual string GetFailureMessage()
    {
        return "Không đủ điều kiện sử dụng lá bài!";
    }
}