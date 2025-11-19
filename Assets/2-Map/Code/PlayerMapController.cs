using UnityEngine;

public class PlayerMapController : MonoBehaviour
{
    public static PlayerMapController Instance;
    public Room currentRoom;  

    private void Awake()
    {
        Instance = this;
    }

    public void MoveTo(Room targetRoom)
    {
        if (targetRoom == null) return;

        if (targetRoom.incoming.Contains(currentRoom))
        {
            currentRoom.visited = true;
            currentRoom = targetRoom;
            MapUIManager.Instance.UpdateRoomHighlights();
        }
    }


}
