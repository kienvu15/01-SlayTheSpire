using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomCardSystem : MonoBehaviour
{
    public static RandomCardSystem Instance;

    [Header("Card")]
    public AllCardData allCardDatabase;
    public Transform shopSlotsParent;
    public GameObject cardDisplayPrefab;
    public CardHolder cardHolder;

    [Header("VFX")]
    public GameObject disappearVFXPrefab;

    private List<CardData> shopCards = new List<CardData>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
       // GeneratePickMeCard(null);
       // GeneratePickMeCard(null);
        //RandomCardSystem.Instance.GeneratePickMeCard(this);

    }

    private LootItemUI callingLootButton;
    public void GeneratePickMeCard(LootItemUI callingButton)
    {
        callingLootButton = callingButton;
        foreach (Transform child in shopSlotsParent)
            Destroy(child.gameObject);

        shopCards.Clear();

        for (int i = 0; i < 3; i++)
        {
            CardData randomCard = allCardDatabase.GetRandomCard();
            if (randomCard == null) continue;

            shopCards.Add(randomCard);

            GameObject slot = Instantiate(cardDisplayPrefab, shopSlotsParent);
            CardDisplay display = slot.GetComponent<CardDisplay>();

            display.useScaleAnimation = true;
            display.LoadCard(randomCard);

            display.Init(randomCard, cardHolder);
        }
    }

   

    public IEnumerator OnCardSelected(CardDisplay pickedCard)
    {
        // --- 1. Xử lý các lá bài KHÔNG được chọn (VFX và Hủy) ---
        // Giữ nguyên logic này, nhưng sử dụng UIManager để đợi VFX xong trước khi hủy UI

        // Tạo danh sách các card không được chọn để xử lý VFX và hủy
        List<Transform> cardsToDestroy = new List<Transform>();
        foreach (Transform child in shopSlotsParent)
        {
            if (child != pickedCard.transform)
            {
                cardsToDestroy.Add(child);
            }
        }

        foreach (Transform child in cardsToDestroy)
        {
            // Chơi VFX tại vị trí card không được chọn
            if (disappearVFXPrefab != null)
            {
                GameObject vfx = Instantiate(disappearVFXPrefab, child.position, Quaternion.identity, transform);
                ParticleSystem ps = vfx.GetComponent<ParticleSystem>();
                if (ps != null)
                {
                    ps.Play();
                    Destroy(vfx, ps.main.duration);
                }
                else
                {
                    Destroy(vfx, 0.5f);
                }
            }

            // Ẩn Visual của lá bài không được chọn
            for (int i = 0; i < child.childCount; i++)
            {
                child.GetChild(i).gameObject.SetActive(false);
            }

            // Đợi một chút rồi hủy
            yield return new WaitForSeconds(0.2f);
            Destroy(child.gameObject);
        }

        // --- 2. Xử lý lá bài ĐƯỢC CHỌN (Animation Bay) ---

        // Đợi thêm một chút để VFX hoàn thành
        yield return new WaitForSeconds(0.3f);

        // GỌI ANIMATION BAY BÀI VÀ TRUYỀN CALLBACK VÀO
        // Khi animation bay xong, callback sẽ được gọi
        UIManager.Instance.AnimateCardFly(
            pickedCard.GetComponent<RectTransform>(),
            UIManager.Instance.deckIcon, // Target là Deck Icon
            () => // Đây là callback (onComplete)
            {
                // 3. LOGIC XỬ LÝ KHI ANIMATION BAY XONG

                // Thêm bài đã chọn vào bộ bài của người chơi
                // Giả định bạn có hàm AddCardToDeck
                // PlayerDeck.Instance.AddCard(pickedCard.CardData); 

                // Hủy lá bài đã chọn (nó đã bay đi rồi)
                Destroy(pickedCard.gameObject);

                // Ẩn toàn bộ panel chọn bài
                gameObject.SetActive(false);

                // 4. HỦY NÚT LOOT GỐC
                if (callingLootButton != null)
                {
                    // Gọi hàm hoàn tất loot của nút đã kích hoạt panel này
                    callingLootButton.FinalizeLooting();
                }
            });
    }

    public void SkipButton()
    {
        SoundManager.Instance.Play("SelectButton");
        gameObject.SetActive(false);
    }
}