using UnityEngine;
using System.Collections.Generic;

public class LootGenerator : MonoBehaviour
{
    public AllCardData allCardData;
    public AllRelicDatabase allRelicDatabase;
    public LootPanel lootPanel;

    private void OnEnable()
    {
        var loots = GenerateLoot();
        lootPanel.Show(loots);
    }

    public List<LootData> GenerateLoot()
    {
        List<LootData> results = new List<LootData>();

        // Always give gold
        LootData gold = new LootData();
        gold.lootType = LootType.Gold;
        gold.amount = Random.Range(12, 28);
        gold.icon = goldIcon; 
        gold.name = gold.amount + " Gold";
        results.Add(gold);

        
        LootData cardLoot = new LootData();
        cardLoot.lootType = LootType.Card;
        cardLoot.icon = cardIcon;
        cardLoot.name = "Choose a card";
        results.Add(cardLoot);

        
        Relic r = allRelicDatabase.GetRandomRelic();
        LootData relicLoot = new LootData();
        relicLoot.lootType = LootType.Relic;
        relicLoot.relic = r;
        relicLoot.icon = r.icon;
        relicLoot.name = r.relicName;
        results.Add(relicLoot);
        

        return results;
    }

    public Sprite goldIcon;
    public Sprite cardIcon;
}
