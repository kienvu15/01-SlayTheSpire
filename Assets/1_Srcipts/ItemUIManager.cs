using System.Collections.Generic;
using UnityEngine;

public class ItemUIManager : MonoBehaviour
{
    public static ItemUIManager Instance { get; private set; }
    public static bool HasInstance => Instance != null;

    [Header("Setup")]
    public Canvas mainCanvas;
    public GameObject takeButtonPrefab;
    public int poolSize = 20;

    private Camera cam;
    private Queue<TakeButton> pool = new Queue<TakeButton>();
    private Dictionary<Item, TakeButton> activeButtons = new Dictionary<Item, TakeButton>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        cam = Camera.main;
        CreatePool();
    }

    private void CreatePool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject btnObj = Instantiate(takeButtonPrefab, mainCanvas.transform);
            //btnObj.transform.parent = transform;
            btnObj.SetActive(false);
            pool.Enqueue(btnObj.GetComponent<TakeButton>());
        }
    }

    public void RegisterItem(Item item)
    {
        if (activeButtons.ContainsKey(item)) return;

        TakeButton btn = GetFromPool();
        btn.Setup(item);
        activeButtons[item] = btn;
    }

    public void UnregisterItem(Item item)
    {
        if (activeButtons.TryGetValue(item, out TakeButton btn))
        {
            ReturnToPool(btn);
            activeButtons.Remove(item);
        }
    }

    private void Update()
    {
        foreach (var kvp in activeButtons)
        {
            Item item = kvp.Key;
            TakeButton btn = kvp.Value;

            if (item == null || !btn.gameObject.activeSelf)
                continue;

            Vector3 screenPos = cam.WorldToScreenPoint(item.transform.position + Vector3.up * 1.5f);

            // Nếu item nằm sau camera → ẩn
            bool isBehind = screenPos.z < 0;
            btn.gameObject.SetActive(!isBehind);

            if (!isBehind)
                btn.transform.position = screenPos;
        }
    }

    private TakeButton GetFromPool()
    {
        if (pool.Count > 0)
        {
            var btn = pool.Dequeue();
            btn.gameObject.SetActive(true);
            return btn;
        }

        GameObject btnObj = Instantiate(takeButtonPrefab, mainCanvas.transform);
        return btnObj.GetComponent<TakeButton>();
    }

    private void ReturnToPool(TakeButton btn)
    {
        btn.gameObject.SetActive(false);
        pool.Enqueue(btn);
    }
}
