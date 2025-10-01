using System.Collections.Generic;
using UnityEngine;

public enum EventResultType
{
    None,
    Leave,
    Gold,
    Damage,
    Heal,
    Relic,
    Combat,
    Shop
}

[System.Serializable]
public class EventOutcome
{
    public string resultText;          
    public EventResultType resultType;  
    public int value;                  
    [Range(0f, 1f)] public float weight = 1f; 
}

[System.Serializable]
public class EventChoice
{
    public string choiceText;

    [Header("UI Settings")]
    public Sprite choiceImage;

    [Header("Chaining")]
    public RoomEventData nextEvent;

    [Header("Outcomes (nếu có nhiều thì random)")]
    public List<EventOutcome> outcomes = new List<EventOutcome>();

    public EventOutcome GetRandomOutcome()
    {
        if (outcomes == null || outcomes.Count == 0) return null;

        // Nếu chỉ có 1 thì return luôn
        if (outcomes.Count == 1) return outcomes[0];

        // Weighted random
        float totalWeight = 0f;
        foreach (var outcome in outcomes)
            totalWeight += outcome.weight;

        float roll = Random.value * totalWeight;
        foreach (var outcome in outcomes)
        {
            if (roll < outcome.weight)
                return outcome;
            roll -= outcome.weight;
        }

        return outcomes[0]; // fallback
    }

}

[CreateAssetMenu(fileName = "NewEvent", menuName = "Game/Event")]
public class RoomEventData : ScriptableObject
{
    public string eventTitle;
    [TextArea] public string description;

    [Header("UI Settings")]
    public Sprite eventImage;
    public Sprite BackgroundImage;

    public EventChoice[] choices;
}
