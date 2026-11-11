using UnityEngine;

[CreateAssetMenu(menuName = "Cards/Requirements/Only Melee In Hand")]
public class OnlyMeleeInHandRequirement : RequirementData
{
    public override bool IsMet(RequirementContext ctx)
    {
        if (ctx.remainingCards == null)
            return true;

        foreach (var card in ctx.remainingCards)
        {
            if (card == null || card.data == null)
                continue;

            if (card.data.cardType != CardType.Mellee)
                return false;
        }

        return true;
    }

    public override string GetFailureMessage()
    {
        return "Các lá bài còn lại phải đều là Melee!";
    }
}