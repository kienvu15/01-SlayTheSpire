using UnityEngine;

[CreateAssetMenu(fileName = "EventDatabase", menuName = "Game/EventDatabase")]
public class EventDatabase : ScriptableObject
{
    public RoomEventData[] events;
}
