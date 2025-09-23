using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MapUIManager : MonoBehaviour
{
    public static MapUIManager Instance;
    private List<RoomUI> roomUIs = new List<RoomUI>();

    private RoomUI selectedRoom;

    [Header("Battle UI")]
    public BattleCanvasDatabase battleDatabase;
    public Transform battleCanvasHolder;   // parent để spawn canvas

    [Header("Hide On")]
    public List<GameObject> hideOnBattle;
    public List<GameObject> hideOnShop;

    [Header("Shop UI")]
    public GameObject shopUI;

    [Header("References")]
    public Deck deck; // tham chiếu đến Deck để kiểm soát việc rút bài
    public Match match;
    public ManaSystem manaSystem;
    public PlayerSelfCast playerSelfCast;
    public Player player;
    public GameObject EndButton;
    public PanCamera panCamera;

    private void Awake()
    {
        Instance = this;
    }

    public void ShowBattleCanvas()
    {
        if (battleDatabase == null) return;

        var prefab = battleDatabase.GetRandomBattleCanvas();
        if (prefab == null) return;

        // spawn vào holder
        var go = Instantiate(prefab, battleCanvasHolder);
        go.transform.localPosition = Vector3.zero;
        //  Button endButton = go.GetComponentInChildren<Button>();
        //  endButton.onClick.AddListener(() => { match.EndTurn(); });

        deck.spawner = go.GetComponentInChildren<EnemySpawner>();
        deck.discard = go.GetComponentInChildren<Discard>(); // gán discard mới cho deck
        deck.handParent = go.transform.Find("HandPanel");
        deck.deckTransform = go.transform.Find("Deck");
        deck.CacheDeckCountText();
        deck.CacheDeckDiscard();

        playerSelfCast.discard = go.GetComponentInChildren<Discard>();
        CanvasGroup PlaySelfCast = playerSelfCast.GetComponent<CanvasGroup>();
        if (PlaySelfCast != null)
        {
            PlaySelfCast.blocksRaycasts = false; // tắt chặn raycast
        }

        match.discard = go.GetComponentInChildren<Discard>();
        match.enemySystem = go.GetComponentInChildren<EnemySystem>();

        GameFlowManager.Instance.spawner = go.GetComponentInChildren<EnemySpawner>();
        GameFlowManager.Instance.discard = go.GetComponentInChildren<Discard>();

       

        GameObject panel = go.GetComponentInChildren<ManaPanel>().gameObject;
        if (panel != null)
        {
            manaSystem.manaContainer = panel.transform;
            manaSystem.gameObject.SetActive(true);
            player.gameObject.SetActive(true);
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
        // mở shop UI
        shopUI.SetActive(true);
        // hide UI khác
        foreach (var obj in hideOnShop)
        {
            if (obj != null) obj.SetActive(false);
        }
        //panCamera.enabled = false;
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
            // tô xanh lá room hiện tại
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
