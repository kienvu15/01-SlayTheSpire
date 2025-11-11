using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RelicUIManager : MonoBehaviour
{
    public RelicManager relicManager;
    public GameObject relicHolderPrefab;  // prefab có sẵn RelicDisplay
    public Transform relicPanel;          // panel có Grid Layout Group
    public Transform relicDiscriptionTransform;

    public TextMeshProUGUI relicCountText; // Hiển thị số lượng relic

    private Dictionary<Relic, RelicDisplay> relicToUI = new Dictionary<Relic, RelicDisplay>();

    private void Start()
    {
        // Khởi tạo UI cho relic hiện có
        foreach (var relic in relicManager.equippedRelics)
        {
            AddRelicUI(relic);
        }
        UpdateRelicCount();
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
        UpdateRelicCount();
    }

    private void RemoveRelicUI(Relic relic)
    {
        if (!relicToUI.ContainsKey(relic)) return;

        RelicDisplay display = relicToUI[relic];
        if (display != null)
            Destroy(display.gameObject);

        relicToUI.Remove(relic);
        UpdateRelicCount();
    }

    public void UpdateRelicCount()
    {
        int count = relicManager.equippedRelics.Count;
        relicCountText.text = $"Relic{count}/6";
    }
}
