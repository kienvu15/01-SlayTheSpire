using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class RoomUI : MonoBehaviour
{
    private Animator anim;
    public Button goButton;
    private Room room;
    private GameObject ButtonPar;

    private Canvas panelCanvas;
    public Slider selectSlider;

    private Coroutine moveButtonCoroutine;
    private static bool hasMovedOnce = false;

    public bool isStartPanel = false;

    void OnEnable()
    {
        hasMovedOnce = false;
    }

    void Start()
    {
        anim = GetComponent<Animator>();
        selectSlider = GetComponentInChildren<Slider>();

        if (anim != null) { AnimatorStateInfo state = anim.GetCurrentAnimatorStateInfo(0); anim.Play(state.fullPathHash, 0, Random.Range(0f, 1f)); }

        room = GetComponentInParent<Room>();
        MapUIManager.Instance.RegisterRoomUI(this);

        ButtonPar = GameObject.Find("ButtonPar");
        if(ButtonPar != null)
        {
            
        }

        panelCanvas = GetComponent<Canvas>();
        if (panelCanvas != null)
        {
            panelCanvas.overrideSorting = true;
            panelCanvas.sortingLayerName = "UI";
            panelCanvas.sortingOrder = 602;
        }

        hasMovedOnce = false;
    }

    private void Update()
    {
        if (!room.incoming.Contains(PlayerMapController.Instance.currentRoom))
        {
            if (anim != null)
            {
                anim.SetBool("Click", false);
                anim.Play("Hide", 0, 0f);
            }
        }

    }

    public void SetAsVisited()
    {

        if (goButton != null)
            goButton.gameObject.SetActive(false);

        if(isStartPanel == false)
        {
            anim.Play("Stay");
        }
        
    }

    public void OnRoomClicked()
    {
        if (room.incoming.Contains(PlayerMapController.Instance.currentRoom))
        {
            MapUIManager.Instance.SelectRoom(this);

            SoundManager.Instance.Play("PencilDraw", null, 1);
            if (selectSlider != null)
                StartCoroutine(FillSelectSlider());

            if (anim != null)
                anim.SetBool("Click", true);

            goButton.gameObject.SetActive(true);
            goButton.transform.SetParent(ButtonPar.transform, false);

            if (!hasMovedOnce)
            {
                hasMovedOnce = true;
                moveButtonCoroutine = StartCoroutine(MoveButtonParRight());
            }
        }

    }

    private IEnumerator MoveButtonParRight()
    {
        if (ButtonPar == null) yield break;

        RectTransform rect = ButtonPar.GetComponent<RectTransform>();
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

    public void OnGoClicked()
    {
        PlayerMapController.Instance.MoveTo(room);

        if (room.type == RoomType.Battle)
        {
            MapUIManager.Instance.ShowBattleCanvas();
        }

        if (room.type == RoomType.Shop)
            MapUIManager.Instance.OpenShop();

        if (room.type == RoomType.Event)
            MapUIManager.Instance.ShowEventUI();
    }

    public void SetAsCurrent()
    {
        if (goButton != null)
            goButton.gameObject.SetActive(false);
    }

    public void ResetUI()
    {
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
                anim.SetBool("Stay", true);
                return;
            }
        }

        anim.SetBool("Click", false);
        selectSlider.value = 0f;
        if (goButton != null)
            goButton.gameObject.SetActive(false);
    }
}
