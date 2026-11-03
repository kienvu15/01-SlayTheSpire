using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Character_Deck_Setup", menuName = "Scriptable Objects/Character_Deck_Setup")]
public class Character_Deck_Setup : ScriptableObject
{
    [Header("Character_Deck_Info")]
    public List<CardData> allCardData = new List<CardData>();
}
