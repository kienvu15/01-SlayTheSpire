using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapUIManager : MonoBehaviour
{
    public static MapUIManager Instance;
    private List<RoomUI> roomUIs = new List<RoomUI>();

    private RoomUI selectedRoom;

    [Header("Battle UI")]
    public BattleCanvasDatabase battleDatabase;
    public Transform battleCanvasHolder;
    public GameObject handDeckCanvas;

    [Header("Shop UI")]
    public GameObject shopUI;

    [Header("Event UI")]
    public GameObject eventUI;

    [Header("Hide On")]
    public List<GameObject> hideOnBattle;
    public List<GameObject> hideOnShop;
    public List<GameObject> hideOnEvent;

    [Header("References")]
    public GameObject enemyFM;
    public BattleManager battleManager;
    public Deck deck;
    public Match match;
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

    public void ShowBattleCanvas()
    {
        if (battleDatabase == null) return;

        var prefab = battleDatabase.GetRandomBattleCanvas();
        if (prefab == null) return;

        battleRoomCount++;
        Debug.Log($"Player đã vào battle room {battleRoomCount} lần.");

        // spawn vào holder
        var battleRoom = Instantiate(prefab, battleCanvasHolder);
        handDeckCanvas.SetActive(true);

        battleRoom.transform.localPosition = Vector3.zero;

        // EnemyFm
        StartCoroutine(RegisterEnemiesNextFrame(battleRoom));

        deck.CacheDeckCountText();
        deck.CacheDeckDiscard();


        CanvasGroup PlaySelfCast = playerSelfCast.GetComponent<CanvasGroup>();
        if (PlaySelfCast != null)
        {
            PlaySelfCast.blocksRaycasts = false; 
        }

        //match
        match.enemySystem = battleRoom.GetComponentInChildren<EnemySystem>();

        GameFlowManager.Instance.player = player;
        


        manaSystem.UpdateManaUI();

        player.canvas.SetActive(true);
        player.animator.gameObject.SetActive(true);
        player.StartCoroutine(player.EnableCanvasAfterAnimation("Enter"));
        PlaySelfCast.blocksRaycasts = true;
        EndButton.SetActive(true);
        
        UIManager.Instance.HandPlayerBottom.SetActive(true);

        GameSystem.Instance.BattleUICanvas = battleRoom;
        GameFlowManager.Instance.isOnBattle = true;

        // hide UI khác
        foreach (var obj in hideOnBattle)
        {
            if (obj != null) obj.SetActive(false);
        }
    }

    public void OpenShop()
    {
        shopUI.SetActive(true);
        foreach (var obj in hideOnShop)
        {
            if (obj != null) obj.SetActive(false);
        }
    }

    public void ShowEventUI()
    {
        eventUI.SetActive(true);
        foreach (var obj in hideOnEvent)
        {
            if (obj != null) obj.SetActive(false);
        }
    }

    public void HideBattleCanvas()
    {
        for (int i = battleCanvasHolder.childCount - 1; i >= 0; --i)
        {
            Destroy(battleCanvasHolder.GetChild(i).gameObject);
        }

        //foreach (var obj in hideOnBattle)
        //{
        //    if (obj != null) obj.SetActive(true);
        //}

        if (BattleManager.Instance != null)
        {
            BattleManager.Instance.enemies.Clear();
        }
    }

    public void RegisterRoomUI(RoomUI ui)
    {
        if (!roomUIs.Contains(ui)) roomUIs.Add(ui);
    }

    public void UpdateRoomHighlights()
    {
        foreach (var ui in roomUIs)
            ui.ResetUI();

        if (PlayerMapController.Instance.currentRoom != null)
        {
            var currentUI = PlayerMapController.Instance.currentRoom.GetComponentInChildren<RoomUI>();
            if (currentUI != null) currentUI.SetAsCurrent();
        }
    }

    public void SelectRoom(RoomUI ui)
    {
        foreach (var r in roomUIs)
        {
            if (r != ui) r.ResetUI();
        }

        selectedRoom = ui;
    }

    public void ClearSelection()
    {
        if (selectedRoom != null)
        {
            selectedRoom.ResetUI();
            selectedRoom = null;
        }
    }

}
