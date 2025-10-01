using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameFlowManager : MonoBehaviour
{
    public static GameFlowManager Instance { get; private set; }

    public EnemySpawner spawner;
    public Player player;
    public GameObject victoryPanel;
    public Deck deck;
    public Discard discard;
    public Match match;
    public CoinManager coinManager;
    public ManaSystem manasystem;
    public List<GameObject> hideOnBattle;

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
        coinManager.AddCoins(Random.Range(14, 17));
        manasystem.ResetManaToMax();
    }

    /// Hàm này được EnemySystem gọi khi sóng cuối cùng clear
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
