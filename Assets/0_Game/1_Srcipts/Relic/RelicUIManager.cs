using System.Collections.Generic;
using UnityEngine;

public class RelicUIManager : MonoBehaviour
{
    public RelicManager relicManager;
    public GameObject relicHolderPrefab;  
    public Transform relicPanel;         
    public Transform relicDiscriptionTransform;

    private Dictionary<Relic, RelicDisplay> relicToUI 
        = new Dictionary<Relic, RelicDisplay>();

    private void OnEnable()
    {
        relicManager.onRelicAdded += AddRelicUI;
        relicManager.onRelicRemoved += RemoveRelicUI;
    }

    void Start()
    {
        foreach (var relic in relicManager.equippedRelics)
        {
            AddRelicUI(relic);
        }
    }

    private void OnDisable()
    {
        relicManager.onRelicAdded -= AddRelicUI;
        relicManager.onRelicRemoved -= RemoveRelicUI;
    }

    public void AddRelicUI(Relic relic)
    {
        if (relicToUI.ContainsKey(relic)) return;
        GameObject holder = Instantiate(relicHolderPrefab, relicPanel);
        RelicDisplay display = holder.GetComponent<RelicDisplay>();
        if (display != null)
        {
            display.Setup(relic, false); 
            display.descriptionPanel.transform.SetParent(relicDiscriptionTransform, false);
            var rt = display.descriptionPanel.GetComponent<RectTransform>();
            rt.SetParent(relicDiscriptionTransform, false);
            rt.anchoredPosition = Vector2.zero;
            rt.localRotation = Quaternion.identity;
            rt.localScale = Vector3.one;

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
