//using UnityEngine;
//using UnityEngine.UI;

//public class CardUI : MonoBehaviour
//{
//    [TextArea] public string description;  // nội dung thẻ
//    private Button button;

//    private CardData cardData;
//    private void Awake()
//    {
//        button = GetComponent<Button>();
//        button.onClick.AddListener(OnCardClick);
//    }

//    public void OnCardClick()
//    {
//        Debug.Log("Card clicked: " + description);
//        if (CardDescriptionUI.Instance.IsVisible)
//            CardDescriptionUI.Instance.Hide();
//        else
//            CardDescriptionUI.Instance.Show(description);
//    }
//}
