using System.Collections.Generic;
using UnityEngine;

public class LootPanel : MonoBehaviour
{
    public GameObject lootItemPrefab; 
    public Transform container;      

    public void Show(List<LootData> loots)
    {
        gameObject.SetActive(true);

        foreach (Transform child in container)
        {
            Destroy(child.gameObject);
        }

        foreach (LootData loot in loots)
        {
            GameObject obj = Instantiate(lootItemPrefab, container);

            LootItemUI ui = obj.GetComponent<LootItemUI>();
            if (ui != null)
            {
                ui.Init(loot);
            }
        }
    }

}