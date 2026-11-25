using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using NaughtyAttributes;

public class EnemyView : Character
{
    [Header("Enemy Data")]
    public EnemyData enemyData;

    [Header("Components")]
    public Animator animator;

    [Header("Intent_Helper")]
    public EnemyActionTypeIcon typeIcons;
    public EnemyIntentUI intentUI;

    [Header("Helper")]
    public Transform avatarImage;
    public Image EavatarImage;
    public GameObject canvas;
    public RectTransform cardDrop_Helper;
    public CanvasGroup canvasGroup;
    
    public EnemySystem system;
    public Spot currentSlot;

    [Header("Enemy Type")]
    public bool isBoss = false;
    private bool isActive = true;
    private int nextActionIndex = 0; 

    private List<EnemyActionInstance> actionPattern;
    private int currentActionIndex = 0;

    protected override void Start()
    {
        base.Start();
        system = GetComponentInParent<EnemySystem>();
    }

    private void OnDestroy()
    {
        if (currentSlot != null)
        {
            if (currentSlot.occupant == this)
            {
                currentSlot.isOccupied = false;
                currentSlot.occupant = null;
            }
            currentSlot = null;
        }
    }

    [NaughtyAttributes.Button]
    public void Initialize()
    {
        stats.maxHP = Random.Range(enemyData.maxHP.x, enemyData.maxHP.y + 1);
        stats.currentHP = stats.maxHP;
        // Enemy Type
        isBoss = enemyData.isBoss;
        // Visual
        transform.localScale = enemyData.local_Scale;
        EavatarImage.sprite = enemyData.enemySprite;
        avatarImage.localScale = enemyData.avatar_localScale;
        RectTransform rect = transform as RectTransform;
        rect.sizeDelta = new Vector2(rect.sizeDelta.x, enemyData.CanvasHieght);
        hpBar.GetComponent<RectTransform>().sizeDelta = new Vector2(enemyData.hpSlider_Hieght, 10);
        cardDrop_Helper.sizeDelta = enemyData.CardDrop;
        cardDrop_Helper.anchoredPosition = enemyData.CardDropPosition;
        // Action Pattern
        actionPattern = new List<EnemyActionInstance>();
        foreach (EnemyAction action in enemyData.actionPatternAssets)
        {
            if (action == null) continue;

            actionPattern.Add(
                new EnemyActionInstance(action)
            );
        }
        // Reset Action Index
        currentActionIndex = 0;
        nextActionIndex = 0;


        UpdateUI();
        ShowNextIntent();
    }

    public void Initialize(EnemyData data)
    {
        enemyData = data;
        stats.maxHP = Random.Range(enemyData.maxHP.x, enemyData.maxHP.y + 1);
        stats.currentHP = stats.maxHP;
        // Enemy Type
        isBoss = enemyData.isBoss;
        // Visual
        transform.localScale = enemyData.local_Scale;
        EavatarImage.sprite = enemyData.enemySprite;
        avatarImage.localScale = enemyData.avatar_localScale;
        avatarImage.localEulerAngles = enemyData.rotation;
        RectTransform rect = transform as RectTransform;
        rect.sizeDelta = new Vector2(rect.sizeDelta.x, enemyData.CanvasHieght);
        hpBar.GetComponent<RectTransform>().sizeDelta = new Vector2(enemyData.hpSlider_Hieght, 10);
        cardDrop_Helper.sizeDelta = enemyData.CardDrop;
        cardDrop_Helper.anchoredPosition = enemyData.CardDropPosition;
        // Action Pattern
        actionPattern = new List<EnemyActionInstance>();
        foreach (EnemyAction action in enemyData.actionPatternAssets)
        {
            if (action == null) continue;

            actionPattern.Add(
                new EnemyActionInstance(action)
            );
        }
        // Reset Action Index
        currentActionIndex = 0;
        nextActionIndex = 0;


        UpdateUI();
        ShowNextIntent();
    }

    private IEnumerator Die()
    {
        float duration = 0.8f;
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, t / duration);
            canvasGroup.alpha = alpha;
            yield return null;
        }
        canvasGroup.alpha = 0f;
        Destroy(gameObject);
    }

    public override void TakeDamage(int amount)
    {
        base.TakeDamage(amount);
        if (animator != null)
        {
            animator.SetTrigger("Hit");
        }
        if (stats.currentHP <= 0 && isActive)
        {
            canvas.SetActive(false);
            if (system != null)
            {
                system.OnEnemyDied(this);
            }
            isActive = false;
            StartCoroutine(Die());
        }
    }

    private List<int> GetOverrides(int index)
    {
        if (enemyData == null ||
            enemyData.overrideValuesPattern == null ||
            index < 0 ||
            index >= enemyData.overrideValuesPattern.Count)
            return null;

        return enemyData.overrideValuesPattern[index].values;
    }

    public void PerformAction(Character target)
    {
        if (actionPattern == null || actionPattern.Count == 0) return;

        if (isBoss)
        {
            int startIndex = currentActionIndex;
            do
            {
                EnemyActionInstance action = actionPattern[currentActionIndex];

                if (action.currentCooldown <= 0)
                {
                    action.Apply(this, target, GetOverrides(currentActionIndex));

                    if (animator != null)
                    {
                        if (action.Type == Type.Attack || action.Type == Type.BadBuff)
                            animator.SetTrigger("Attack");

                        else if (action.Type == Type.Buff)
                            animator.SetTrigger("Up");
                    }

                    for (int i = 0; i < actionPattern.Count; i++)
                    {
                        if (i != currentActionIndex &&
                            actionPattern[i].currentCooldown > 0)
                        {
                            actionPattern[i].currentCooldown--;
                        }
                    }

                    currentActionIndex = (currentActionIndex + 1) % actionPattern.Count;

                    ShowNextIntent();
                    break;
                }

                currentActionIndex = (currentActionIndex + 1) % actionPattern.Count;

            } while (currentActionIndex != startIndex);
        }
        else
        {
            if (nextActionIndex < 0 || nextActionIndex >= actionPattern.Count)
            {
                ShowNextIntent();
                return;
            }

            EnemyActionInstance action = actionPattern[nextActionIndex];

            action.Apply(this, target, GetOverrides(nextActionIndex));

            if (animator != null)
            {
                if (action.Type == Type.Attack || action.Type == Type.BadBuff)
                    animator.SetTrigger("Attack");

                else if (action.Type == Type.Buff)
                    animator.SetTrigger("Up");
            }

            for (int i = 0; i < actionPattern.Count; i++)
            {
                if (i != nextActionIndex &&
                    actionPattern[i].currentCooldown > 0)
                {
                    actionPattern[i].currentCooldown--;
                }
            }

            ShowNextIntent();
        }
    }

    private void ShowNextIntent()
    {
        if (intentUI == null || actionPattern == null || actionPattern.Count == 0)
            return;

        if (isBoss)
        {
            nextActionIndex = PeekNextActionIndexBoss();
        }
        else
        {
            List<int> availableIndices = new List<int>();

            for (int i = 0; i < actionPattern.Count; i++)
            {
                if (actionPattern[i].currentCooldown == 0)
                    availableIndices.Add(i);
            }

            if (availableIndices.Count == 0)
            {
                nextActionIndex = -1;
            }
            else
            {
                nextActionIndex = availableIndices[
                    Random.Range(0, availableIndices.Count)
                ];
            }
        }
        if (nextActionIndex < 0 || nextActionIndex >= actionPattern.Count)
            return;
        EnemyActionInstance nextAction = actionPattern[nextActionIndex];

        List<int> overrides = GetOverrides(nextActionIndex);

        var ctx = new EffectContext { self = this, level = 1 };
        int displayValue = 0;

        for (int i = 0; i < nextAction.Effects.Count; i++)
        {
            var effect = nextAction.Effects[i];
            if (effect == null) continue;

            int ov = (overrides != null && i < overrides.Count) ? overrides[i] : -1;
            ctx.hasOverride = ov >= 0;
            ctx.overrideValue = ov;

            if (!effect.CountsAsIntend) continue;

            displayValue += effect.GetIntentValue(ctx);
        }

        Sprite icon = typeIcons != null
            ? typeIcons.GetIcon(nextAction.Type)
            : null;

        intentUI.SetIntent(icon, displayValue);
    }

    private int PeekNextActionIndexBoss()
    {
        int index = currentActionIndex;
        int start = index;
        do
        {
            EnemyActionInstance action = actionPattern[index];
            if (action.currentCooldown <= 0)
                return index;
            else
                index = (index + 1) % actionPattern.Count;
        } while (index != start);
        return currentActionIndex;
    }

    public bool CanBeMoved
    {
        get
        {
            if (enemyData == null) return true;
            if (isBoss) return false;
            return enemyData.canBeMoved;
        }
    }

    #region Movement
    private Coroutine moveVisualRoutine;
    public void PlayMoveVisual(Spot targetSlot)
    {
        if (targetSlot == null || !gameObject.activeInHierarchy) return;
        if (moveVisualRoutine != null) StopCoroutine(moveVisualRoutine);
        moveVisualRoutine = StartCoroutine(MoveVisualRoutine(targetSlot));
    }
    private IEnumerator MoveVisualRoutine(Spot targetSlot)
    {
        RectTransform enemyRect = (RectTransform)transform;
        RectTransform enemyCanvas = enemyRect.parent as RectTransform;

        Vector2 targetPos = WorldToLocalPos(targetSlot.transform.position, enemyCanvas);
        float speed = 1940f;

        while (Vector2.Distance(enemyRect.anchoredPosition, targetPos) > 1f)
        {
            enemyRect.anchoredPosition = Vector2.MoveTowards(
                enemyRect.anchoredPosition, targetPos, speed * Time.deltaTime);
            enemyRect.localPosition = new Vector3(
                enemyRect.localPosition.x, enemyRect.localPosition.y, 0f);
            yield return null;
        }

        enemyRect.anchoredPosition = targetPos;
        enemyRect.localPosition = new Vector3(
            enemyRect.localPosition.x, enemyRect.localPosition.y, 0f);
        moveVisualRoutine = null;
    }
    public void AssginToSlot(Spot slot)
    {
        if (slot == null) return;
        if (currentSlot != null && currentSlot.occupant == this)
        {
            currentSlot.isOccupied = false;
            currentSlot.occupant = null;
        }
        currentSlot = slot;
        currentSlot.isOccupied = true;
        currentSlot.occupant = this;
    }
    private Vector2 WorldToLocalPos(Vector3 worldPos, RectTransform canvas)
    {
        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(null, worldPos);
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas, screenPoint, null, out localPoint);
        return localPoint;
    }
    #endregion

}
