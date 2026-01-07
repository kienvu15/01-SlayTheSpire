using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class RoomUI : MonoBehaviour
{
    public Animator anim;

    public Button goButton;
    public GameObject onRoomClick;
    public Room room;
    private GameObject buttonEnterParent;

    private Canvas panelCanvas;
    public Slider selectSlider;
    public Outline outline;
    private static bool hasMovedOnce = false;

    [Header("Room Flags")]
    public bool isStartPanel = false;
    public bool isStartRoom = false;

    private void Awake()
    {
        room = GetComponentInParent<Room>();

        anim = GetComponent<Animator>();
        selectSlider = GetComponentInChildren<Slider>();
        panelCanvas = GetComponent<Canvas>();

        if (panelCanvas != null)
        {
            panelCanvas.overrideSorting = true;
            panelCanvas.sortingLayerName = "UI";
            panelCanvas.sortingOrder = 602;
        }
    }

    void OnEnable()
    {
        if (isStartRoom) { SetAsVisited(); return;  } 

        if(room.visited == true)
        {
            return;
        }

        hasMovedOnce = false;
        selectSlider.value = 0f;

        if (GameFlowManager.Instance.isOnBattle)
        {
           return;
        }

        if (PlayerMapController.Instance != null && room.incoming.Contains(PlayerMapController.Instance.currentRoom))
        {
            PlayIdleHighlight();
        }
    }

    public void PlayIdleHighlight()
    {
        if (anim != null)
        {
            anim.Play("Idle");   
        }
    }

    public void RsesetUI()
    {
        if (anim != null)
        {
            anim.Play("Hide", 0, 0);   // Ẩn mọi thứ
        }
    }

    void OnDisable()
    {
        if (room.visited == true) return;

        if (outline != null)
            outline.enabled = false;
    }

    void Start()
    {
        if (anim != null)
        {
            AnimatorStateInfo state = anim.GetCurrentAnimatorStateInfo(0);
            anim.Play(state.fullPathHash, 0, Random.Range(0f, 1f));
        }
        MapUIManager.Instance.RegisterRoomUI(this);

        buttonEnterParent = GameObject.Find("ButtonPar");

        hasMovedOnce = false;
    }

    public void SetAsVisited()
    {
        if (goButton != null)
            goButton.gameObject.SetActive(false);
        if (anim != null)
            anim.SetBool("Stay", true);
        selectSlider.value = 1f;
    }

    public void OnRoomClicked()
    {
        if (isStartRoom) return; 

        if (room.incoming.Contains(PlayerMapController.Instance.currentRoom))
        {
            MapUIManager.Instance.SelectRoom(this);

            SoundManager.Instance.Play("PencilDraw", null, 1);
            if (selectSlider != null)
                StartCoroutine(FillSelectSlider());

            if (anim != null)
                anim.SetBool("Click", true);

            if (goButton != null)
            {
                goButton.gameObject.SetActive(true);
                goButton.transform.SetParent(buttonEnterParent.transform, false);
            }

            if (!hasMovedOnce)
            {
                hasMovedOnce = true;
                StartCoroutine(MoveButtonParRight());
            }
        }
    }

    private IEnumerator MoveButtonParRight()
    {
        if (buttonEnterParent == null) yield break;

        RectTransform rect = buttonEnterParent.GetComponent<RectTransform>();
        if (rect == null) yield break;

        Vector2 startPos = rect.anchoredPosition;
        Vector2 targetPos = startPos + new Vector2(250f, 0f);

        float duration = 0.7f;
        float elapsed = 0f;
        SoundManager.Instance.Play("RockMove", null, 1);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            rect.anchoredPosition = Vector2.Lerp(startPos, targetPos, t);
            yield return null;
        }
        rect.anchoredPosition = targetPos;
    }

    private IEnumerator MoveButtonParLeft()
    {
        if (buttonEnterParent == null) yield break;

        RectTransform rect = buttonEnterParent.GetComponent<RectTransform>();
        if (rect == null) yield break;

        Vector2 startPos = rect.anchoredPosition;
        Vector2 targetPos = startPos - new Vector2(250f, 0f);

        float duration = 0.7f;
        float elapsed = 0f;
        SoundManager.Instance.Play("RockMove", null, 1);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            rect.anchoredPosition = Vector2.Lerp(startPos, targetPos, t);
            yield return null;
        }
        rect.anchoredPosition = targetPos;
    }

    private IEnumerator FillSelectSlider()
    {
        float duration = 0.5f;
        float elapsed = 0f;

        float startValue = 0f;
        float endValue = 1f;

        selectSlider.value = startValue;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            selectSlider.value = Mathf.Lerp(startValue, endValue, t);
            yield return null;
        }

        selectSlider.value = endValue;
    }

    public void OnEnterRoomClicked()
    {
        if (isStartRoom) return;
        StartCoroutine(MoveButtonParLeft());
        UIManager.Instance.FillOverTime();
        UIManager.Instance.OnTransitionFilled -= HandleTransitionComplete;
        UIManager.Instance.OnTransitionFilled += HandleTransitionComplete;
    }

    private void HandleTransitionComplete()
    {
        PlayerMapController.Instance.MoveTo(room);

        if (room.type == RoomType.Battle)
        {
            room.visited = true;
            SetAsVisited();
            GameSystem.Instance.isBattlePhase = true;
            UIManager.Instance.FadeOutOverTime();

            bool sysExists = GameSystem.Instance != null;
            bool isBattle = sysExists && GameSystem.Instance.isBattlePhase;
            bool deckExists = Deck.instance != null;
            bool needDraw = deckExists && !Deck.instance.hasDrawFisrtRow;

            if (sysExists && isBattle && needDraw)
            {
                Deck.instance.hasDrawFisrtRow = true;
                StartCoroutine(Deck.instance.DrawHand(Match.instance.handSize));
            }
            MapUIManager.Instance.ShowBattleCanvas();
        }
        else if (room.type == RoomType.Shop)
        {
            GameSystem.Instance.isBattlePhase = true;
            room.visited = true;
            SetAsVisited();
            UIManager.Instance.FadeOutOverTime();
            MapUIManager.Instance.OpenShop();
        }
        else if (room.type == RoomType.Event)
        {
            room.visited = true;
            SetAsVisited();
            UIManager.Instance.HandPlayerBottom.SetActive(false);
            UIManager.Instance.ToolImage.SetActive(false);
            UIManager.Instance.MapOBB.SetActive(false);
            UIManager.Instance.MapStuffs.SetActive(false);

            UIManager.Instance.FadeOutOverTime();
            MapUIManager.Instance.ShowEventUI();
        }
        else if (room.type == RoomType.Rest)
        {
            room.visited = true;
            SetAsVisited();

            GameSystem.Instance.isBattlePhase = false;

            UIManager.Instance.HandPlayerBottom.SetActive(false);
            UIManager.Instance.MapStuffs.SetActive(false);
            UIManager.Instance.ToolImage.SetActive(false);
            UIManager.Instance.FadeOutOverTime();
            MapUIManager.Instance.ShowRestUI();
        }

        UIManager.Instance.CrawlerFloor();
        UIManager.Instance.OnTransitionFilled -= HandleTransitionComplete;
    }

    public void SetAsCurrent()
    {
        if (goButton != null)
            goButton.gameObject.SetActive(false);
    }

    public void ResetUI()
    {
        if (isStartRoom)
        {
            SetAsVisited();
            return;
        }

        if (room != null)
        {
            if (room == PlayerMapController.Instance.currentRoom)
            {
                SetAsCurrent();
                return;
            }
            else if (room.visited)
            {
                SetAsVisited();
                return;
            }
        }

        if (anim != null)
            anim.SetBool("Click", false);

        selectSlider.value = 0f;

        if (goButton != null)
            goButton.gameObject.SetActive(false);
    }
}
