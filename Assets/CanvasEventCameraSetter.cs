using UnityEngine;

[RequireComponent(typeof(Canvas))]
public class CanvasEventCameraSetter : MonoBehaviour
{
    public Canvas canvas;
    void Awake()
    {


    }

    private void Start()
    {
        canvas.overrideSorting = true; // đảm bảo canvas này được ưu tiên vẽ
    }
}
