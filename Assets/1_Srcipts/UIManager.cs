using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("UI MapPanel")]
    public List<GameObject> backMap;
    [SerializeField] private RectTransform buttonEnterParent;
    [SerializeField] private PanCamera panCamera;
    private Vector2 originEnterButtonAnchoredPos;

    [Header("Deck")]
    public GameObject deckUI;

    [Header("Event")]
    public GameObject tutorialCanvas;

    [Header("UI Layer")]
    public Image transitionImage;
    public event System.Action OnTransitionFilled;

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

    private void OnEnable()
    {

    }

    void Start()
    {
        originEnterButtonAnchoredPos = buttonEnterParent.anchoredPosition;
    }

    #region MapPanel
    public void BackButton()
    {
        SoundManager.Instance.Play("SelectButton", null, 1);

        foreach (GameObject button in backMap)
        {
            button.SetActive(!button.activeSelf);
        }

        if (Mathf.Abs(buttonEnterParent.anchoredPosition.x - 380.33f) > 0.01f)
        {
            buttonEnterParent.anchoredPosition = new Vector2(originEnterButtonAnchoredPos.x, buttonEnterParent.anchoredPosition.y);
        }
        else
        {
            buttonEnterParent.anchoredPosition = new Vector2(380.33f, buttonEnterParent.anchoredPosition.y);
        }

        if (GameFlowManager.Instance.isOnBattle == true)
        {
            buttonEnterParent.gameObject.SetActive(false);
        }
    }

    public void MapIcon()
    {
        SoundManager.Instance.Play("SelectButton", null, 1);

        deckUI.SetActive(false);

        foreach (GameObject button in backMap)
        {
            button.SetActive(!button.activeSelf);
        }

        if (GameFlowManager.Instance.isOnBattle == true)
        {
            buttonEnterParent.gameObject.SetActive(false);
        }
        else
        {
            buttonEnterParent.gameObject.SetActive(true);
        }

        buttonEnterParent.anchoredPosition = new Vector2(originEnterButtonAnchoredPos.x, buttonEnterParent.anchoredPosition.y);

        if (tutorialCanvas != null)
        {
            Destroy(tutorialCanvas);
            tutorialCanvas = null;
        }
    }
    #endregion

    #region DeckButton
    public void DeckButton()
    {
        SoundManager.Instance.Play("SelectButton", null, 1);
        foreach (GameObject button in backMap)
        {
            button.SetActive(false);
        }
        deckUI.SetActive(true);
    }
    public void BackDeckButton()
    {
        SoundManager.Instance.Play("SelectButton", null, 1);
        foreach (GameObject button in backMap)
        {
            button.SetActive(false);
        }
        deckUI.SetActive(false);
    }
    #endregion

    #region Transition
    public void FillOverTime()
    {
        transitionImage.gameObject.SetActive(true);
        StartCoroutine(FillCoroutine());
    }

    private IEnumerator FillCoroutine()
    {
        float elapsed = 0f;
        float duration = 1f;
        transitionImage.fillAmount = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            transitionImage.fillAmount = Mathf.Clamp01(elapsed / duration);
            yield return null;
        }

        transitionImage.fillAmount = 1f;
        OnTransitionFilled?.Invoke();
    }

    public void FadeOutOverTime()
    {
        StartCoroutine(FadeOutCoroutine());
    }

    private IEnumerator FadeOutCoroutine()
    {
        float elapsed = 0f;
        float duration = 1.5f;
        Color color = transitionImage.color;
        float startAlpha = color.a;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            color.a = Mathf.Lerp(startAlpha, 0f, t);
            transitionImage.color = color;
            yield return null;
        }

        transitionImage.gameObject.SetActive(false);
        color.a = 1f;
        transitionImage.color = color;
    }

    #endregion
}
