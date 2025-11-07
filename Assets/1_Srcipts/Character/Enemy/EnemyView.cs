using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class EnemyView : Enemy
{
    [Header("Components")]
    public Animator animator;
    public Rigidbody2D rb;

    [Header("UI References")]
    public EnemyActionTypeIcon typeIcons;
    public EnemyIntentUI intentUI;
    public Spot currentSlot;

    [Header("Enemy Type")]
    public bool isBoss = false;
    private int nextActionIndex = 0; // lưu action sẽ thực hiện ở turn tới
    public EnemyType enemyType;

    [Header("EnemyView Specific")]
    public List<EnemyAction> actionPatternAssets; // kéo thả trong Inspector
    public List<OverrideValues> overrideValuesPattern;
    private List<EnemyActionInstance> actionPattern; // runtime copy

    private int currentActionIndex = 0;

    protected override void Start()
    {
        base.Start();

        // Tạo instance runtime cho từng enemy → cooldown tách biệt
        actionPattern = new List<EnemyActionInstance>();
        for (int i = 0; i < actionPatternAssets.Count; i++)
        {
            int? cdOverride = null;

            // Nếu có override cooldown
            if (overrideValuesPattern != null && i < overrideValuesPattern.Count)
            {
                int cd = overrideValuesPattern[i].overrideCooldown;
                if (cd >= 0) cdOverride = cd;
            }

            EnemyActionInstance instance = new EnemyActionInstance(actionPatternAssets[i], cdOverride);
            actionPattern.Add(instance);
        }

        UpdateUI();
        ShowNextIntent();
    }


    protected override void Update()
    {
        base.Update();

        if (stats.currentHP <= 0)
        {
            EnemySystem system = FindFirstObjectByType<EnemySystem>();
            if (system != null)
            {
                system.OnEnemyDied(this);
            }

            StartCoroutine(Die());
        }
    }

    private IEnumerator Die()
    {
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }

    public IEnumerator Move2Slot(Spot targetSlot)
    {
        if (targetSlot == null) yield break;

        RectTransform enemyRect = (RectTransform)transform;
        RectTransform enemyCanvas = enemyRect.parent as RectTransform;

        float speed = 1940f;

        // Clear slot cũ
        if (currentSlot != null && currentSlot.occupant == this)
        {
            currentSlot.isOccupied = false;
            currentSlot.occupant = null;
        }

        // Convert world position của Slot sang local pos trong canvas
        Vector2 targetPos = WorldToLocalPos(targetSlot.transform.position, enemyCanvas);

        // Di chuyển dần
        while (Vector2.Distance(enemyRect.anchoredPosition, targetPos) > 1f)
        {
            enemyRect.anchoredPosition = Vector2.MoveTowards(
                enemyRect.anchoredPosition,
                targetPos,
                speed * Time.deltaTime
            );
            enemyRect.localPosition = new Vector3(enemyRect.localPosition.x, enemyRect.localPosition.y, 0f);
            yield return null;
        }

        // Snap cuối cùng
        enemyRect.anchoredPosition = targetPos;
        enemyRect.localPosition = new Vector3(enemyRect.localPosition.x, enemyRect.localPosition.y, 0f);

        // Gán slot
        AssginToSlot(targetSlot);
    }

    private Vector2 WorldToLocalPos(Vector3 worldPos, RectTransform canvas)
    {
        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(null, worldPos);
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas, screenPoint, null, out localPoint);
        return localPoint;
    }

    private void OnDestroy()
    {
        if (currentSlot != null)
        {
            currentSlot.isOccupied = false;
            currentSlot.occupant = null;
            currentSlot = null;
        }
    }

    public override void TakeDamage(int amount)
    {
        base.TakeDamage(amount);
        if (animator != null)
        {
            animator.SetTrigger("Hit");
        }
    }

    public override void AddShield(int amount)
    {
        base.AddShield(amount);
    }

    public void PerformAction(Character target)
    {
        if (actionPattern.Count == 0) return;

        if (isBoss)
        {
            int startIndex = currentActionIndex;

            do
            {
                EnemyActionInstance action = actionPattern[currentActionIndex];
                List<int> overrides = null;

                if (overrideValuesPattern != null && currentActionIndex < overrideValuesPattern.Count)
                    overrides = overrideValuesPattern[currentActionIndex].values;

                if (action.currentCooldown <= 0)
                {
                    // Dùng action
                    action.Apply(this, target, overrides);

                    if (animator != null && (action.Type == Type.Attack || action.Type == Type.BadBuff))
                        animator.SetTrigger("Attack");


                    // Giảm cooldown tất cả action khác (ngoại trừ action vừa dùng)
                    for (int i = 0; i < actionPattern.Count; i++)
                    {
                        if (i != currentActionIndex && actionPattern[i].currentCooldown > 0)
                            actionPattern[i].currentCooldown--;
                    }

                    currentActionIndex = (currentActionIndex + 1) % actionPattern.Count;

                    ShowNextIntent();
                    break;
                }
                else
                {
                    currentActionIndex = (currentActionIndex + 1) % actionPattern.Count;
                }

            } while (currentActionIndex != startIndex);
        }
        else
        {
            if (nextActionIndex < 0 || nextActionIndex >= actionPattern.Count)
            {
                ShowNextIntent(); // random lại cho turn sau
                return;
            }

            EnemyActionInstance action = actionPattern[nextActionIndex];

            // Dùng action
            List<int> overrides = null;
            if (overrideValuesPattern != null && nextActionIndex < overrideValuesPattern.Count)
                overrides = overrideValuesPattern[nextActionIndex].values;

            action.Apply(this, target, overrides);

            if (animator != null && (action.Type == Type.Attack || action.Type == Type.BadBuff))
                animator.SetTrigger("Attack");

            if (animator != null && (action.Type == Type.Buff))
                animator.SetTrigger("Up");

            // Giảm cooldown tất cả action khác
            for (int i = 0; i < actionPattern.Count; i++)
            {
                if (i != nextActionIndex && actionPattern[i].currentCooldown > 0)
                    actionPattern[i].currentCooldown--;
            }

            // Random intent cho turn tiếp
            ShowNextIntent();
        }
    }

    private void ShowNextIntent()
    {
        if (intentUI == null || actionPattern.Count == 0) return;

        if (isBoss)
        {
            nextActionIndex = PeekNextActionIndexBoss();
        }
        else
        {
            List<int> availableIndices = new List<int>();
            for (int i = 0; i < actionPattern.Count; i++)
            {
                if (actionPattern[i].currentCooldown == 0) // chỉ lấy action hồi xong
                    availableIndices.Add(i);
            }

            if (availableIndices.Count == 0)
            {
                nextActionIndex = -1; // không có action khả dụng
            }
            else
            {
                nextActionIndex = availableIndices[Random.Range(0, availableIndices.Count)];
            }
        }

        if (nextActionIndex < 0 || nextActionIndex >= actionPattern.Count) return;

        EnemyActionInstance nextAction = actionPattern[nextActionIndex];

        // Lấy overrideValues (nếu có)
        List<int> overrides = null;
        if (overrideValuesPattern != null && nextActionIndex < overrideValuesPattern.Count)
            overrides = overrideValuesPattern[nextActionIndex].values;

        // Tính value hiển thị
        int displayValue = 0;
        for (int i = 0; i < nextAction.Effects.Count; i++)
        {
            var effect = nextAction.Effects[i];
            if (effect == null) continue;

            int overrideValue = (overrides != null && i < overrides.Count) ? overrides[i] : -1;

            if (effect is IOverrideValue o)
                displayValue += (overrideValue > -1) ? overrideValue : o.GetIntentValue();
            else
                displayValue += effect.GetIntentValue();
        }

        // Icon dựa vào Type
        Sprite icon = typeIcons != null ? typeIcons.GetIcon(nextAction.Type) : null;
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

    public void AssginToSlot(Spot slot)
    {
        if (slot == null) return;

        // clear slot cũ
        if (currentSlot != null && currentSlot.occupant == this)
        {
            currentSlot.isOccupied = false;
            currentSlot.occupant = null;
        }

        // gán slot mới
        currentSlot = slot;
        currentSlot.isOccupied = true;
        currentSlot.occupant = this;
    }
}
