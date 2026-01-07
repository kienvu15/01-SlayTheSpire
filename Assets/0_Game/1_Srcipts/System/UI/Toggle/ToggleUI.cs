using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class ToggleUI : MonoBehaviour
{
    public GameObject[] Tap;
    public GameObject[] Pop;

    // public PanCamera panCamera;

    [Header("Card Deck (Optional)")]
    // public CardCollectionPanel cardCollectionPanel; // kéo vào nếu panel này là deck
    public bool openInRemoveMode = false;           // gắn từ Inspector hoặc set bằng code trước khi PopUp

    public void PopUp()
    {
        // Toggle Pop
        //foreach (GameObject PPobject in Pop)
        //{
        //    bool newState = !PPobject.activeSelf;
        //    PPobject.SetActive(newState);

        //    // Nếu có CardCollectionPanel thì set mode trước khi mở
        //    if (newState && cardCollectionPanel != null)
        //    {
        //        cardCollectionPanel.Open(openInRemoveMode);
        //    }
        //}

        //// Toggle Tap
        //if (Tap == null || Tap.Length == 0) return;
        //foreach (GameObject button in Tap)
        //{
        //    button.SetActive(!button.activeSelf);
        //}

    }


}
