using System.Collections.Generic;
using UnityEngine;

public class RelicUIManager : MonoBehaviour
{
    [Header("References")]
    public bool isInBag = false;
    public RelicManager relicManager;
    public GameObject relicHolderPrefab;
    public Transform relicPanel;

    private Dictionary<RelicInstance, RelicDisplay> relicToUI
        = new Dictionary<RelicInstance, RelicDisplay>();
    
    private void OnEnable()
    {
        if (relicManager == null) return;
        relicManager.onRelicAdded += AddRelicUI;
        relicManager.onRelicRemoved += RemoveRelicUI;
    }
    private void OnDisable()
    {
        if (relicManager == null) return;
        relicManager.onRelicAdded -= AddRelicUI;
        relicManager.onRelicRemoved -= RemoveRelicUI;
    }

    private void Start()
    {
        foreach (var relic in relicManager.equippedRelics)
        {
            AddRelicUI(relic);
        }
    }

    public void AddRelicUI(RelicInstance instance)
    {
        if (instance == null || relicToUI.ContainsKey(instance))
            return;
        GameObject holder = Instantiate(relicHolderPrefab, relicPanel);
        RelicDisplay display = holder.GetComponent<RelicDisplay>();
        if(isInBag == true)
        {
            display.OnClickAddLisent();
        }
        if (display == null)
        {
            Destroy(holder);
            return;
        }
        display.Init_Data(instance);
        relicToUI.Add(instance, display);
    }

    private void RemoveRelicUI(RelicInstance instance)
    {
        if (instance == null || !relicToUI.ContainsKey(instance))
            return;

        RelicDisplay display = relicToUI[instance];

        if (display != null)
            Destroy(display.gameObject);

        relicToUI.Remove(instance);
    }

    public void RefreshRelicUI(RelicInstance instance)
    {
        if (instance == null) return;

        if (relicToUI.TryGetValue(instance, out RelicDisplay display))
        {
            display.UpdateRelicUI();
        }
    }
}