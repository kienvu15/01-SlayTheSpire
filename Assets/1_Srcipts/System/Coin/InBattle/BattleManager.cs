using UnityEngine;
using System.Collections.Generic;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance;

    [Header("Player")]
    public Player player;

    [Header("Enemy")]
    public List<EnemyView> enemies = new List<EnemyView>();

    private void Awake()
    {
        Instance = this;
    }

    public Character GetRandomAlly(Character requester, bool includeSelf = false)
    {
        List<Character> candidates = new List<Character>();

        foreach (var e in enemies)
        {
            if (e == null) continue;
            if (!includeSelf && e == requester) continue;
            if (e.faction == Faction.Enemy) candidates.Add(e);
        }

        if (candidates.Count == 0) return requester;
        return candidates[Random.Range(0, candidates.Count)];
    }



}
