using UnityEngine;
using UnityEngine.UI;

public class TutorialCanvas : MonoBehaviour
{
    public DIALOGUE dialogue;          
    public GameObject tutorialCanvas;  

    void Start()
    {
        dialogue.OnItemFlyCompleted += ShowTutorialPanel;
    }

    void ShowTutorialPanel()
    {
        Debug.Log("Nhận tín hiệu từ DIALOGUE → Hiện tutorial panel!");
        tutorialCanvas.SetActive(true);
    }
}