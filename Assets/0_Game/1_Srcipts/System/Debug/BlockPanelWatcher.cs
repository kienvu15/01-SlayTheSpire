using UnityEngine;

public class BlockPanelProxy : MonoBehaviour
{
    public CanvasGroup target;

    public bool BlocksRaycasts
    {
        get => target.blocksRaycasts;
        set
        {
            Debug.Log($"[BlockPanelProxy] Set blocksRaycasts = {value}\n{System.Environment.StackTrace}");
            target.blocksRaycasts = value;
        }
    }
}
