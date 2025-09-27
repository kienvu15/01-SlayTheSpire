using UnityEngine;

public class PlayerMapController : MonoBehaviour
{
    public static PlayerMapController Instance;
    public Room currentRoom;   // room hiện tại player đang ở

    private void Awake()
    {
        Instance = this;
    }

    public void MoveTo(Room targetRoom)
    {
        if (targetRoom == null) return;

        if (targetRoom.incoming.Contains(currentRoom))
        {
            // đánh dấu phòng cũ là visited
            currentRoom.visited = true;

            // cập nhật currentRoom
            currentRoom = targetRoom;

            // update UI
            MapUIManager.Instance.UpdateRoomHighlights();
        }
    }


}
