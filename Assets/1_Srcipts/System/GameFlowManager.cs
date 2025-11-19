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

    private void Start()
    {
        
    }

    public void HideAllG()
    {
        StartCoroutine(OnAllEncountersCleared());

        player.ClearAllConditionsAndSkills();
        manasystem.ResetManaToMax();
        coinManager.AddCoins(Random.Range(14, 17));
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
}
