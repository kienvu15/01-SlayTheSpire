using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RelicUIManager : MonoBehaviour
{
    public RelicManager relicManager;
    public GameObject relicHolderPrefab;  
    public Transform relicPanel;         
    public Transform relicDiscriptionTransform;

    private Dictionary<Relic, RelicDisplay> relicToUI = new Dictionary<Relic, RelicDisplay>();

    private void Start()
    {
        foreach (var relic in relicManager.equippedRelics)
        {
            AddRelicUI(relic);
        }
    }

    private void OnEnable()
    {
        relicManager.onRelicAdded += AddRelicUI;
        relicManager.onRelicRemoved += RemoveRelicUI;
    }

    private void OnDisable()
    {
        relicManager.onRelicAdded -= AddRelicUI;
        relicManager.onRelicRemoved -= RemoveRelicUI;
    }

    private void AddRelicUI(Relic relic)
    {
        if (relicToUI.ContainsKey(relic)) return;

        // Tạo slot UI
        GameObject holder = Instantiate(relicHolderPrefab, relicPanel);
        RelicDisplay display = holder.GetComponent<RelicDisplay>();

        if (display != null)
        {
            display.Setup(relic, false); // false = player đang sở hữu, không phải shop
            // if no shop mode, want set description panel parent
            display.descriptionPanel.transform.SetParent(relicDiscriptionTransform, false);
        }

        relicToUI[relic] = display;
    }

    private void RemoveRelicUI(Relic relic)
    {
        if (!relicToUI.ContainsKey(relic)) return;

        RelicDisplay display = relicToUI[relic];
        if (display != null)
            Destroy(display.gameObject);

        relicToUI.Remove(relic);
    }

}
