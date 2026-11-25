using System;
using UnityEngine;
using System.Collections;

public class GameStage : MonoBehaviour
{
    public static GameStage Instance;

    public Player player;
    public Deck deck;
    public Discard discard;
    public MidMatch_Manager match;
    public CoinManager coinManager;
    public ManaSystem manasystem;

    public bool isOnBattle;
    public bool isFirstBattle = true;

    //===== Event =====//
    public event Action OnBattleEnded;

    // game bool
    public bool isBusy { get; private set; }
    public void SetBusy(bool value) => isBusy = value;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Helper
    public IEnumerator OnAllEncountersCleared()
    {
        yield return new WaitForSeconds(0f);

        //deck.BuildDeck();
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
        //GameSystem.Instance.BattleUICanvas = null;

        discard.Clear();
        //deck.BuildDeck();
        deck.hasDrawFisrtRow = false;
    }

}
