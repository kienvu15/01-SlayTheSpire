using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapUIManager : MonoBehaviour
{
    public static MapUIManager Instance;
    private List<RoomUI> roomUIs = new List<RoomUI>();

    [Header("Room Selection")]
    private Room selectedRoom;
    public Button goButton;

    [Header("Battle UI")]
    public BattleCanvasDatabase battleDatabase;
    public Transform roomCanvasHolder;
    public GameObject handDeckCanvas;
     
    [Header("Shop UI")]
    public ShopRoomDatabase shopDatabase;

    [Header("Shop UI")]
    public GameObject shopUI;

    [Header("Event UI")]
    public GameObject eventUI;
    public GameObject restEnventUI;
    public GameObject RoomEventCanvas;

    [Header("Hide On")]
    public List<GameObject> hideOnBattle;
    public List<GameObject> hideOnShop;
    public List<GameObject> hideOnEvent;

    [Header("References")]
    public bool FirstTimeSelectedRoom = false;
    public GameObject enemyFM;
    public BattleManager battleManager;
    public Deck deck;
    public MidMatch_Manager match;
    public ManaSystem manaSystem;
    public PlayerSelfCast playerSelfCast;
    public Player player;
    public GameObject EndButton;
    public PanCamera panCamera;

    [Header("Debug / Stats")]
    public int battleRoomCount = 0;

    private void Awake()
    {
        Instance = this;
    }

    #region DROP
    public void OpenShop()
    {
        if (shopDatabase == null) return;

        var prefab = shopDatabase.GetRandomShopRoom();
        if (prefab == null) return;

        // spawn vào holder
        var shopRoom = Instantiate(prefab, roomCanvasHolder);
        shopRoom.transform.localPosition = Vector3.zero;
        
        UIManager.Instance.HandPlayerBottom.SetActive(false);
        UIManager.Instance.MapOBB.SetActive(false);
        UIManager.Instance.ToolImage.SetActive(false);
        UIManager.Instance.blackPanel.SetActive(false);
        UIManager.Instance.PlaySelftCast.SetActive(false);

        foreach (var obj in hideOnBattle)
        {
            if (obj != null) obj.SetActive(false);
        }

        Button enterbutton = shopRoom.GetComponent<TextType>().EnterButton.GetComponent<Button>();
        enterbutton.onClick.AddListener(() => UIManager.Instance.EnterShop());

        Button skipButton = shopRoom.GetComponent<TextType>().SkipShopButton.GetComponentInChildren<Button>();
        skipButton.onClick.AddListener(() => UIManager.Instance.SkipShop());

        GameSystem.Instance.BattleUICanvas = shopRoom;
    }

    public void ShowEventUI()
    {
        eventUI.SetActive(true);
        foreach (var obj in hideOnEvent)
        {
            if (obj != null) obj.SetActive(false);
        }
        GameSystem.Instance.BattleUICanvas = eventUI;
    }

    public void ShowRestUI()
    {
        var restEvent = Instantiate(restEnventUI, RoomEventCanvas.transform);
        restEvent.transform.localPosition = Vector3.zero;

        UIManager.Instance.MapOBB.SetActive(false);
        GameSystem.Instance.BattleUICanvas = restEvent;
    }
    public void HideBattleCanvas()
    {
        //for (int i = roomCanvasHolder.childCount - 1; i >= 0; --i)
        //{
        //    Destroy(roomCanvasHolder.GetChild(i).gameObject);
        //}

        if (BattleManager.Instance != null)
        {
            BattleManager.Instance.enemies.Clear();
        }
        MapAfterBattle();
    }

    public void RegisterRoomUI(RoomUI ui)
    {
        if (!roomUIs.Contains(ui)) roomUIs.Add(ui);
    }
    #endregion
    #region Drop 2
    public void UpdateRoomHighlights()
    {
        Room current = PlayerMapController.Instance.currentRoom;

        foreach (RoomUI ui in roomUIs)
        {
            bool isNextRoom = ui.room.incoming.Contains(current);

            if (isNextRoom)
            {
                //ui.PlayIdleHighlight();
            }
            else
            {
                //ui.ResetUI();
            }
        }

        RoomUI currentUI = current.GetComponentInChildren<RoomUI>();
        //if (currentUI != null)
          // currentUI.SetAsCurrent();
    }

    public IEnumerator UpdateHighlightsNextFrame()
    {
        yield return null;
        MapUIManager.Instance.UpdateRoomHighlights();
    }

    public void MapInBattleScene()
    {
        for (int i = 0;  i < roomUIs.Count; i++)
        {
            roomUIs[i].anim.Play("Hide", 0, 0);
            roomUIs[i].onRoomClick.SetActive(false);
        }
    }

    public void MapAfterBattle()
    {
        for (int i = 0; i < roomUIs.Count; i++)
        {
           // roomUIs[i].ResetUI();
            roomUIs[i].onRoomClick.gameObject.SetActive(true);
        }
    }
    #endregion

    public void SelectRoom(Room room)
    {
        if (selectedRoom == room) return;

        if (selectedRoom != null)
            selectedRoom.SetSelected(false);

        selectedRoom = room;

        if (selectedRoom != null)
            selectedRoom.SetSelected(true);

        if(FirstTimeSelectedRoom == false)
        {
            FirstTimeSelectedRoom = true;
            UIManager.Instance.Enter_Room_Button_Helper(true);

            goButton.onClick.RemoveListener(EnterSelectedRoom);
            goButton.onClick.AddListener(EnterSelectedRoom);
        }
    }

    public void EnterSelectedRoom()
    {
        if (selectedRoom == null) return;

        Room targetRoom = selectedRoom;

        //goButton.gameObject.SetActive(false);
        selectedRoom = null;

        PlayerMapController.Instance.MoveTo(targetRoom);
        if (targetRoom.type == RoomType.Battle)
        {
            targetRoom.visited = true;
            GameSystem.Instance.isBattlePhase = true;
            //UIManager.Instance.FadeOutOverTime();

            //bool sysExists = GameSystem.Instance != null;
            //bool isBattle = sysExists && GameSystem.Instance.isBattlePhase;
            //bool deckExists = Deck.instance != null;
            //bool needDraw = deckExists && !Deck.instance.hasDrawFisrtRow;

            //if (sysExists && isBattle && needDraw)
            //{
                Deck.instance.hasDrawFisrtRow = true;
                StartCoroutine(Deck.instance.DrawHand(MidMatch_Manager.instance.handSize));
            //}
            MapUIManager.Instance.ShowBattleCanvas();
        }
    }

    public void ShowBattleCanvas()
    {
        if (battleDatabase == null) return;
        var prefab = battleDatabase.GetRandomBattleCanvas();
        if (prefab == null) return;
        // battleRoomCount++;
        Debug.Log($"Player đã vào battle room {battleRoomCount} lần.");
        var battleRoom = Instantiate(prefab, roomCanvasHolder);
        //handDeckCanvas.SetActive(true);
        battleRoom.transform.localPosition = Vector3.zero;
        StartCoroutine(RegisterEnemiesNextFrame(battleRoom));
        //CanvasGroup PlaySelfCast = playerSelfCast.GetComponent<CanvasGroup>();
        //if (PlaySelfCast != null)
        //{
          //  PlaySelfCast.blocksRaycasts = false;
        //}
        match.enemySystem = battleRoom.GetComponentInChildren<EnemySystem>();
        GameStage_Manager.Instance.player = player;
        manaSystem.UpdateManaUI();
        player.canvas.SetActive(true);
        player.animator.gameObject.SetActive(true);
        player.StartCoroutine(player.EnableCanvasAfterAnimation("Enter"));
        //PlaySelfCast.blocksRaycasts = true;
        EndButton.SetActive(true);

        UIManager.Instance.HandPlayerBottom.SetActive(true);
        UIManager.Instance.PlaySelftCast.SetActive(true);
        GameSystem.Instance.BattleUICanvas = battleRoom;
        GameStage_Manager.Instance.isOnBattle = true;
        //foreach (var obj in hideOnBattle)
        //{
        //    if (obj != null) obj.SetActive(false);
        //}
    }

    private IEnumerator RegisterEnemiesNextFrame(GameObject go)
    {
        yield return null;

        if (enemyFM != null)
        {
            EnemyView[] enemyViews = enemyFM.GetComponentsInChildren<EnemyView>(true);
            Debug.Log($"Sau 1 frame: Tìm thấy {enemyViews.Length} EnemyView trong EnemyFM");
            BattleManager.Instance.enemies.Clear();
            BattleManager.Instance.enemies.AddRange(enemyViews);
        }
    }

    public void ClearSelection()
    {
        if (selectedRoom != null)
        {
            selectedRoom.ResetUI();
            selectedRoom = null;
        }
        FirstTimeSelectedRoom = false;
    }

}
