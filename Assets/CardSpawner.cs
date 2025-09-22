//using UnityEngine;

//public class CardSpawner : MonoBehaviour
//{
//    public GameObject cardPrefab;   // prefab có CardDisplay + CardHolder
//    public Transform parent;        // UI parent để chứa các lá card
//    public CardHolder cardHolder;   // nơi giữ danh sách cardData

//    void Start()
//    {
//        SpawnAllCards();
//    }

//    public void SpawnAllCards()
//    {
//        foreach (CardData data in cardHolder.cardDataList)
//        {
//            GameObject card = Instantiate(cardPrefab, parent);
//            card.GetComponent<CardDisplay>().LoadCard(data);
//        }
//    }
//}
