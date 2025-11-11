using UnityEngine;
using UnityEngine.UI;

public class TakeButton : MonoBehaviour
{
    public Button button;
    private Item targetItem;

    public void Setup(Item item)
    {
        targetItem = item;
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => targetItem.Take());
    }
}
