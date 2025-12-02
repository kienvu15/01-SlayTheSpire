using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameFlowManager : MonoBehaviour
{
    public static GameFlowManager Instance { get; private set; }

    public Player player;
    public Deck deck;
    public Discard discard;
    public Match match;
    public CoinManager coinManager;
    public ManaSystem manasystem;
    public List<GameObject> hideOnBattle;

    public bool isOnBattle;
    public bool isFirstBattle = true;

    //===== Event =====//
    public event Action OnBattleEnded;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public IEnumerator OnAllEncountersCleared()
    {
        yield return new WaitForSeconds(0f);
        foreach (var obj in hideOnBattle)
        {
            if (obj != null) obj.SetActive(false);
        }
        deck.BuildDeck();
        discard.Clear();
        deck.hasDrawFisrtRow = false;
    }

    public IEnumerator AfterBattleEvent()
    {
        player.canvas.SetActive(false);
        GameStage.Instance.SetBusy(true);
        deck.StartCoroutine(deck.AnimateClearHand());
        player.ClearAllConditionsAndSkills();
        manasystem.ResetManaToMax();

        yield return new WaitForSeconds(1f);
        UIManager.Instance.StartCoroutine(UIManager.Instance.MoveHandToBottom());
        GameStage.Instance.SetBusy(false);
        UIManager.Instance.PlayPartical();
        OnBattleEnded?.Invoke();
        GameSystem.Instance.BattleUICanvas = null;

        discard.Clear();
        deck.BuildDeck();
        deck.hasDrawFisrtRow = false;

    }

}
