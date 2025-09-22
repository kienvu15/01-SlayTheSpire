using UnityEngine;
using UnityEngine.UI;

public class RoomUI : MonoBehaviour
{
    public Image circleHighlight;   // vòng tròn quanh room
    public Button goButton;         // nút Go

    private Room room;
    public GameObject ButtonPar;

    void Start()
    {
        room = GetComponentInParent<Room>();   // ✅ thêm dòng này

        MapUIManager.Instance.RegisterRoomUI(this);

        if (circleHighlight != null)
            circleHighlight.color = Color.clear;

        ButtonPar = GameObject.Find("ButtonPar");
    }

    public void SetAsVisited()
    {
        if (circleHighlight != null)
            circleHighlight.color = Color.green; // vẫn xanh
        if (goButton != null)
            goButton.gameObject.SetActive(false);
    }

    public void OnRoomClicked()
    {
        // chỉ cho click nếu có đường đi hợp lệ
        if (room.incoming.Contains(PlayerMapController.Instance.currentRoom))
        {
            MapUIManager.Instance.SelectRoom(this); // ✅ báo cho manager

            circleHighlight.color = Color.yellow;
            goButton.gameObject.SetActive(true);
            goButton.transform.SetParent(ButtonPar.transform, false);
        }
    }


    public void OnGoClicked()
    {
        PlayerMapController.Instance.MoveTo(room);

        if (room.type == RoomType.Battle)
        {
            MapUIManager.Instance.ShowBattleCanvas();
        }

        if(room.type == RoomType.Shop)
        {
            // mở shop UI
            MapUIManager.Instance.OpenShop();
        }
    }


    public void SetAsCurrent()
    {
        if (circleHighlight != null)
            circleHighlight.color = Color.green; // player đang ở đây
        if (goButton != null)
            goButton.gameObject.SetActive(false);
    }

    public void ResetUI()
    {
        if (room != null)
        {
            if (room == PlayerMapController.Instance.currentRoom)
            {
                SetAsCurrent(); // nếu là current thì xanh dương
                return;
            }
            else if (room.visited)
            {
                SetAsVisited(); // nếu visited thì xanh lá
                return;
            }
        }

        // chưa đi qua
        if (circleHighlight != null)
            circleHighlight.color = Color.clear;
        if (goButton != null)
            goButton.gameObject.SetActive(false);
    }

}
