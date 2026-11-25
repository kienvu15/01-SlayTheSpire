using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardCollection_OnRun : MonoBehaviour
{
    [Header("References")]
    public Deck deck;
    public GameObject cardDisplayPrefab;
    public GameObject conditionPrefab;

    public Transform contentParent;

    public Transform conditionParent;
    public ConditionIconDatabase conditionDb;

    [Header("UI")]
    public Button removeButton; 

    private CardDisplay selectedCard;

    public static bool firstTimeOpen = false;
    private List<GameObject> spawnedCards = new List<GameObject>();
    private List<GameObject> spawnedConditions = new List<GameObject>();
    
    private void OnEnable()
    {
        RefreshPanel();
    }
    private void OnDisable() => ClearConditions();

    public void RefreshPanel()
    {
        foreach (var obj in spawnedCards)
        {
            if (obj != null)
                Destroy(obj);
        }
        spawnedCards.Clear();

        if (deck == null) return;

        List<CardInstance> allCards = deck.GetAllCards();

        foreach (CardInstance instance in allCards)
        {
            GameObject newCard = Instantiate(cardDisplayPrefab, contentParent);
            CardDisplay display = newCard.GetComponent<CardDisplay>();
            if (display == null) continue;

            Canvas canvas = newCard.GetComponent<Canvas>();
            if (canvas != null)
                canvas.overrideSorting = false;

            display.LoadCard(instance);   // truyền instance để giữ level

            Button btn = newCard.GetComponentInChildren<Button>();
            if (btn != null)
                btn.onClick.AddListener(() => OnCard_Clicked(display));

            spawnedCards.Add(newCard);
        }
    }

    public void RefreshPanel(Transform parent, System.Action<CardDisplay> onCardClickedCallback)
    {
        foreach (var obj in spawnedCards)
        {
            if (obj != null)
                Destroy(obj);
        }
        spawnedCards.Clear();

        if (deck == null) return;

        List<CardInstance> allCards = deck.GetAllCards();

        foreach (CardInstance instance in allCards)
        {
            GameObject newCard = Instantiate(cardDisplayPrefab, parent);
            CardDisplay display = newCard.GetComponent<CardDisplay>();
            if (display == null) continue;

            Canvas canvas = newCard.GetComponent<Canvas>();
            if (canvas != null)
                canvas.overrideSorting = false;

            display.LoadCard(instance);   // truyền instance để giữ level

            Button btn = newCard.GetComponentInChildren<Button>();
            if (btn != null)
            {
                btn.onClick.AddListener(() =>
                {
                    OnCard_Clicked(display); // Vẫn gọi hiển thị thông tin nếu muốn
                    onCardClickedCallback?.Invoke(display); // Gọi logic riêng (ví dụ: chọn để xóa)
                });
            }

            spawnedCards.Add(newCard);
        }
    }

    public void OnCard_Clicked(CardDisplay pickedCard)
    {
        UIManager.Instance.CardInfo_Help(true);
        firstTimeOpen = true;

        ShowConditions(pickedCard.cardInstance);
        UIManager.Instance.Init_CardInfor(pickedCard);
    }

    public void ShowConditions(CardInstance card)
    {
        ClearConditions();

        if (card == null || conditionPrefab == null || conditionParent == null || conditionDb == null)
            return;

        foreach (ConditionType type in card.data.GetKeywords(card.level))
        {
            if (!conditionDb.TryGet(type, out var entry)) continue;

            GameObject go = Instantiate(conditionPrefab, conditionParent);
            go.GetComponent<ConditionInfoItem>().Setup(
                conditionDb.GetDisplayName(type),
                entry.description,
                DescriptionFormatter.GetColor(type),
                entry.icon);

            spawnedConditions.Add(go);
        }
    }

    public void ClearConditions()
    {
        foreach (var go in spawnedConditions)
            if (go != null) Destroy(go);
        spawnedConditions.Clear();
    }

}
