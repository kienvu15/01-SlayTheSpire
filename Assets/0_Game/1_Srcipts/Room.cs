using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Room : MonoBehaviour
{
    [Header("Runtime")]
    public RoomType type;
    public GameObject checkImage;
    public bool visited;
    
    [Header("Button")]
    public Button roomButton;

    [Header("Visual")]
    [SerializeField] private Image roomImage;
    [SerializeField] private Sprite startSprite;
    [SerializeField] private Sprite battleSprite;
    [SerializeField] private Sprite eliteSprite;
    [SerializeField] private Sprite eventSprite;
    [SerializeField] private Sprite shopSprite;
    [SerializeField] private Sprite restSprite;
    [SerializeField] private Sprite bossSprite;

    [HideInInspector] public List<Room> outgoing = new();
    [HideInInspector] public List<Room> incoming = new();
    [HideInInspector] public Vector2Int gridPos;

    private void Start()
    {
        if (roomButton != null)
            roomButton.onClick.AddListener(OnRoomClicked);
    }

    #region SetData + Sprite
    public void SetRoomType(RoomType newType)
    {
        type = newType;

        if (roomImage == null) return;

        roomImage.sprite = GetSprite(newType);
    }

    private Sprite GetSprite(RoomType type)
    {
        return type switch
        {
            RoomType.Start => startSprite,
            RoomType.Battle => battleSprite,
            RoomType.Elite => eliteSprite,
            RoomType.Event => eventSprite,
            RoomType.Shop => shopSprite,
            RoomType.Rest => restSprite,
            RoomType.Boss => bossSprite,
            _ => null
        };
    }
    #endregion

    public void ConnectTo(Room target)
    {
        if (target == null || outgoing.Contains(target)) return;

        outgoing.Add(target);
        target.incoming.Add(this);
    }

    public void OnRoomClicked()
    {
        Debug.Log($"Room clicked: {type} at {gridPos}");
        if (type == RoomType.Start) return;

        Room currentRoom = PlayerMapController.Instance.currentRoom;
        if (!incoming.Contains(currentRoom)) return;
        MapUIManager.Instance.SelectRoom(this);

        //if (incoming.Contains(PlayerMapController.Instance.currentRoom))
        //{ 
        //    MapUIManager.Instance.SelectRoom(this);
        //}

        // SoundManager.Instance.Play("PencilDraw", null, 1);
        //if (selectSlider != null)
        //  StartCoroutine(FillSelectSlider());

        //if (anim != null)
        //anim.SetBool("Click", true);

        //if (goButton != null)
        //{
        // goButton.gameObject.SetActive(true);
        // goButton.transform.SetParent(buttonEnterParent.transform, false);
        //}

        //if (!hasMovedOnce)
        //{
        // hasMovedOnce = true;
        // StartCoroutine(MoveButtonParRight());
        //}
        //}
    }

    public void SetSelected(bool selected)
    {
        if (checkImage != null)
            checkImage.SetActive(selected);
    }

    public void ResetUI()
    {
        SetSelected(false);
    }
}