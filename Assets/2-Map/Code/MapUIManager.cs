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
        yield return null; // đợi 1 frame

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

        //deck
        // deck.spawner = battleRoom.GetComponentInChildren<EnemySpawner>();
        //deck.discard = battleRoom.GetComponentInChildren<Discard>();
        //deck.handParent = battleRoom.transform.Find("HandPanel");
        //deck.deckTransform = battleRoom.transform.Find("Deck");
        deck.CacheDeckCountText();
        deck.CacheDeckDiscard();

        //playerSeflCast
        //playerSelfCast.discard = battleRoom.GetComponentInChildren<Discard>();
        CanvasGroup PlaySelfCast = playerSelfCast.GetComponent<CanvasGroup>();
        if (PlaySelfCast != null)
        {
            PlaySelfCast.blocksRaycasts = false; 
        }

        //match
        //match.discard = battleRoom.GetComponentInChildren<Discard>();
        match.enemySystem = battleRoom.GetComponentInChildren<EnemySystem>();

        GameFlowManager.Instance.spawner = battleRoom.GetComponentInChildren<EnemySpawner>();
        GameFlowManager.Instance.discard = battleRoom.GetComponentInChildren<Discard>();
        GameFlowManager.Instance.player = player;
       // GameFlowManager.Instance.manasystem = manaSystem;

        //manaSystem + object player
        GameObject manaPanel = FindFirstObjectByType<ManaPanel>().gameObject;
        if (manaPanel != null)
        {
            manaSystem.manaContainer = manaPanel.transform;
            manaSystem.UpdateManaUI();

            //player.canvas.SetActive(true);
            player.animator.gameObject.SetActive(true);
            player.StartCoroutine(player.EnableCanvasAfterAnimation("Enter"));
            PlaySelfCast.blocksRaycasts = true;
            EndButton.SetActive(true);
        }

        GameSystem.Instance.BattleUICanvas = battleRoom;

        // hide UI khác
        foreach (var obj in hideOnBattle)
        {
            if (obj != null) obj.SetActive(false);
        }
    }

    public void OpenShop()
    {
        shopUI.SetActive(true);
        // hide UI khác
        foreach (var obj in hideOnShop)
        {
            if (obj != null) obj.SetActive(false);
        }
    }

    public void ShowEventUI()
    {
        eventUI.SetActive(true);
        // hide UI khác
        foreach (var obj in hideOnEvent)
        {
            if (obj != null) obj.SetActive(false);
        }
    }
    public void HideBattleCanvas()
    {
        // clear all children trong holder
        for (int i = battleCanvasHolder.childCount - 1; i >= 0; --i)
        {
            Destroy(battleCanvasHolder.GetChild(i).gameObject);
        }

        // show lại UI
        foreach (var obj in hideOnBattle)
        {
            if (obj != null) obj.SetActive(true);
        }

        // CLEAR ENEMIES TRONG BATTLE MANAGER
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
        // reset tất cả
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
