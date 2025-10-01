using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EventManager : MonoBehaviour
{
    public static EventManager Instance;

    [Header("UI References")]
    public GameObject eventPanel;
    public TMP_Text titleText;
    public TMP_Text descriptionText;

    public Image eventImageUI;    
    public Image backgroundImageUI;

    public Transform choiceButtonHolder;
    public GameObject choiceButtonPrefab;

    [Header("Data")]
    public EventDatabase eventDatabase;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        ShowRandomEvent();
    }
    public void ShowRandomEvent()
    {
        // Lấy random event
        var data = eventDatabase.events[Random.Range(0, eventDatabase.events.Length)];
        ShowEvent(data);
    }

    public void ShowEvent(RoomEventData data)
    {
        eventPanel.SetActive(true);

        titleText.text = data.eventTitle;
        descriptionText.text = data.description;

        if (eventImageUI != null)
            eventImageUI.sprite = data.eventImage;

        if (backgroundImageUI != null)
            backgroundImageUI.sprite = data.BackgroundImage;

        // Xóa button cũ
        foreach (Transform child in choiceButtonHolder)
            Destroy(child.gameObject);

        // Tạo button theo choice
        foreach (var choice in data.choices)
        {
            var btnObj = Instantiate(choiceButtonPrefab, choiceButtonHolder);

            // text
            var btnText = btnObj.GetComponentInChildren<TMP_Text>();
            btnText.text = choice.choiceText;

            // ảnh button
            var btnImg = btnObj.GetComponent<Image>();
            if (btnImg != null && choice.choiceImage != null)
                btnImg.sprite = choice.choiceImage;

            btnObj.GetComponent<Button>().onClick.AddListener(() =>
            {
                ApplyChoice(choice);
            });
        }
    }


    private void ApplyChoice(EventChoice choice)
    {
        Debug.Log($"Player chọn: {choice.choiceText}");

        // Nếu có event tiếp theo → nhảy sang event khác luôn
        if (choice.nextEvent != null)
        {
            ShowEvent(choice.nextEvent);
            return;
        }

        // Nếu có outcome → random 1 kết quả
        if (choice.outcomes.Count > 0)
        {
            var outcome = GetRandomOutcome(choice.outcomes);
            Debug.Log($"Outcome: {outcome.resultType} ({outcome.value})");

            // Xử lý kết quả
            switch (outcome.resultType)
            {
                case EventResultType.Leave:
                    gameObject.SetActive(false);
                    break;
                case EventResultType.Gold:
                    // Player.Instance.gold += outcome.value;
                    break;
                case EventResultType.Damage:
                    // Player.Instance.hp -= outcome.value;
                    break;
                case EventResultType.Heal:
                    // Player.Instance.hp += outcome.value;
                    break;
                case EventResultType.Combat:
                    MapUIManager.Instance.ShowBattleCanvas();
                    break;
                case EventResultType.Shop:
                    MapUIManager.Instance.OpenShop();
                    break;
                case EventResultType.Relic:
                    // Inventory.AddRelic(outcome.value);
                    break;
            }

            // hiện text outcome lên UI
            descriptionText.text = outcome.resultText;
            eventImageUI.sprite = null;
        }

        // Sau khi xong thì có thể đóng panel hoặc để player bấm "Continue"
        // eventPanel.SetActive(false);
    }

    private EventOutcome GetRandomOutcome(List<EventOutcome> list)
    {
        float totalWeight = 0f;
        foreach (var o in list) totalWeight += o.weight;

        float roll = Random.value * totalWeight;
        float cumulative = 0f;

        foreach (var o in list)
        {
            cumulative += o.weight;
            if (roll <= cumulative)
                return o;
        }
        return list[list.Count - 1]; // fallback
    }


}
