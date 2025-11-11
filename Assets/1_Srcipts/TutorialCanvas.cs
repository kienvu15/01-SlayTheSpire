using UnityEngine;
using UnityEngine.UI;

public class TutorialCanvas : MonoBehaviour
{
    [Header("References")]
    public GameObject tutorialPanel;   // panel mờ phủ toàn màn hình
    public Button mapButton;           // nút map mà player được phép bấm

    private bool tutorialActive = false;

    void Start()
    {
        tutorialPanel.SetActive(false);
    }

    // Gọi hàm này khi player nhận được Map
    public void ActivateTutorial()
    {
        tutorialPanel.SetActive(true);
        tutorialActive = true;

        // Chặn toàn bộ click trừ MapButton
        // Tắt raycast của nút MapButton → để click xuyên qua panel
        mapButton.GetComponent<Image>().raycastTarget = false;
        mapButton.interactable = true;
    }

    // Gọi khi player bấm vào MapButton
    public void OnMapOpened()
    {
        if (!tutorialActive) return;

        tutorialActive = false;
        tutorialPanel.SetActive(false);

        // Bật lại raycast cho MapButton
        mapButton.GetComponent<Image>().raycastTarget = true;
    }
}
