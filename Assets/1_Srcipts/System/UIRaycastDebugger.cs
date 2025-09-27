using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

public class UIRaycastDebugger : MonoBehaviour
{
    public GraphicRaycaster raycaster;
    public EventSystem eventSystem;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            PointerEventData pd = new PointerEventData(eventSystem);
            pd.position = Input.mousePosition;

            List<RaycastResult> results = new List<RaycastResult>();
            raycaster.Raycast(pd, results);

            Debug.Log("---- UI Raycast Results ---- count: " + results.Count);
            for (int i = 0; i < results.Count; i++)
            {
                var r = results[i];
                Debug.Log($"{i}: {r.gameObject.name} (depth {r.depth})");

                var groups = r.gameObject.GetComponentsInParent<CanvasGroup>();
                if (groups.Length == 0) Debug.Log("  (no CanvasGroup in parents)");
                for (int j = 0; j < groups.Length; j++)
                {
                    var g = groups[j];
                    Debug.Log($"  CanvasGroup[{j}] on {g.gameObject.name}: blocks={g.blocksRaycasts} ignoreParent={g.ignoreParentGroups} interactable={g.interactable} alpha={g.alpha}");
                }
            }
        }
    }
}
