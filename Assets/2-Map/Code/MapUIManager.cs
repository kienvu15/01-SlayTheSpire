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

    [Header("Hide On")]
    public List<GameObject> hideOnBattle;
    public List<GameObject> hideOnShop;
    public List<GameObject> hideOnEvent;

    [Header("Shop UI")]
    public GameObject shopUI;

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
        var go = Instantiate(prefab, battleCanvasHolder);
        go.transform.localPosition = Vector3.zero;

        // EnemyFm
        StartCoroutine(RegisterEnemiesNextFrame(go));

        //deck
        deck.spawner = go.GetComponentInChildren<EnemySpawner>();
        deck.discard = go.GetComponentInChildren<Discard>();
        deck.handParent = go.transform.Find("HandPanel");
        deck.deckTransform = go.transform.Find("Deck");
        deck.CacheDeckCountText();
        deck.CacheDeckDiscard();

        //playerSeflCast
        playerSelfCast.discard = go.GetComponentInChildren<Discard>();
        CanvasGroup PlaySelfCast = playerSelfCast.GetComponent<CanvasGroup>();
        if (PlaySelfCast != null)
        {
            PlaySelfCast.blocksRaycasts = false; 
        }

        //match
        match.discard = go.GetComponentInChildren<Discard>();
        match.enemySystem = go.GetComponentInChildren<EnemySystem>();

        GameFlowManager.Instance.spawner = go.GetComponentInChildren<EnemySpawner>();
        GameFlowManager.Instance.discard = go.GetComponentInChildren<Discard>();
        GameFlowManager.Instance.player = player;

        //manaSystem + object player
        GameObject panel = go.GetComponentInChildren<ManaPanel>().gameObject;
        if (panel != null)
        {
            manaSystem.manaContainer = panel.transform;
            manaSystem.UpdateManaUI();

            //player.canvas.SetActive(true);
            player.animator.gameObject.SetActive(true);
            player.StartCoroutine(player.EnableCanvasAfterAnimation("Enter"));
            PlaySelfCast.blocksRaycasts = true;
            EndButton.SetActive(true);
        }

        GameSystem.Instance.BattleUICanvas = go;

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
