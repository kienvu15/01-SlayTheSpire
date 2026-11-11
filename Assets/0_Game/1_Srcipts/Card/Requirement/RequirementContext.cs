using System.Collections.Generic;

public class RequirementContext
{
    public Character self;
    public CardInstance currentCard;
    public List<CardInstance> remainingCards;

    public RequirementContext(
        Character self,
        CardInstance currentCard,
        List<CardInstance> remainingCards)
    {
        this.self = self;
        this.currentCard = currentCard;
        this.remainingCards = remainingCards;
    }
}