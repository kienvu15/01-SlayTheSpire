using System.Collections.Generic;
using UnityEngine;

public class DescriptionPanelManager : MonoBehaviour
{
    public static DescriptionPanelManager Instance;
    public CanvasGroup blockerGroup;

    private List<IHasDescriptionPanel> displays = new List<IHasDescriptionPanel>();

    void Awake()
    {
        Instance = this;
    }

    public void Register(IHasDescriptionPanel disp)
    {
        if (!displays.Contains(disp))
            displays.Add(disp);
    }

    public void Unregister(IHasDescriptionPanel disp)
    {
        if (displays.Contains(disp))
            displays.Remove(disp);
    }

    public void CloseEverythingOutside(GameObject clickedObj)
    {
        foreach (var disp in displays)
        {
            var panel = disp.GetDescriptionPanel();
            if (panel == null || !panel.activeSelf) continue;

            if (clickedObj != null)
            {
                if (clickedObj.transform.IsChildOf(panel.transform)) continue;
                if (clickedObj.transform.IsChildOf(disp.GetRootObject().transform)) continue;
            }

            disp.vHideDescription();
        }
    }
}

