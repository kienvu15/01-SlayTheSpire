using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class EnemyView : Enemy
{
    public List<EnemyAction> actionPattern;
    private int currentActionIndex = 0;
    private Animator animator;
    public Rigidbody2D rb;

    // public Transform target;
    public List<OverrideValues> overrideValuesPattern;
    public EnemyActionTypeIcon typeIcons;   
    public EnemyIntentUI intentUI;         
    public Spot currentSlot;            

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
    }

    protected override void Start()
    {
        base.Start();
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

        float speed = 50f;

        // Clear slot cũ
        if (currentSlot != null && currentSlot.occupant == this)
        {
            currentSlot.isOccupied = false;
            currentSlot.occupant = null;
        }

        // Convert world position của Slot sang local pos trong canvas của Enemy
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

    public void SnapToSlot(Spot slot)
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

        RectTransform enemyRect = (RectTransform)transform;
        RectTransform enemyCanvas = enemyRect.parent as RectTransform;

        enemyRect.anchoredPosition = WorldToLocalPos(slot.transform.position, enemyCanvas);
        enemyRect.localPosition = new Vector3(enemyRect.localPosition.x, enemyRect.localPosition.y, 0f);

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
        if (animator != null)
        {
            animator.SetTrigger("Up");
        }
    }

    public void PerformAction(Character target)
    {
        if (actionPattern.Count == 0) return;

        int startIndex = currentActionIndex;

        do
        {
            EnemyAction action = actionPattern[currentActionIndex];
            List<int> overrides = null;

            if (overrideValuesPattern != null && currentActionIndex < overrideValuesPattern.Count)
                overrides = overrideValuesPattern[currentActionIndex].values;

            if (action.currentCooldown <= 0)
            {
                action.Apply(this, target, overrides);

                if (animator != null && action.type == Type.Attack)
                    animator.SetTrigger("Attack");

                action.currentCooldown = action.cooldown;

                currentActionIndex = (currentActionIndex + 1) % actionPattern.Count;

                // Sau khi đánh xong → cập nhật intent mới
                ShowNextIntent();
                break;
            }
            else
            {
                action.currentCooldown--;
                currentActionIndex = (currentActionIndex + 1) % actionPattern.Count;
            }

        } while (currentActionIndex != startIndex);
    }

    private int PeekNextActionIndex()
    {
        int index = currentActionIndex;
        int start = index;

        do
        {
            EnemyAction action = actionPattern[index];
            if (action.currentCooldown <= 0)
            {
                return index; // đây mới là action sẽ chạy tiếp theo
            }
            else
            {
                index = (index + 1) % actionPattern.Count;
            }

        } while (index != start);

        return currentActionIndex; // fallback
    }

    private void ShowNextIntent()
    {
        if (intentUI == null || actionPattern.Count == 0) return;

        int nextIndex = PeekNextActionIndex();
        EnemyAction nextAction = actionPattern[nextIndex];

        // Lấy overrideValues (nếu có)
        List<int> overrides = null;
        if (overrideValuesPattern != null && nextIndex < overrideValuesPattern.Count)
            overrides = overrideValuesPattern[nextIndex].values;

        // Tính value hiển thị
        int displayValue = 0;

        for (int i = 0; i < nextAction.effects.Count; i++)
        {
            var effect = nextAction.effects[i];
            if (effect == null) continue;

            int overrideValue = (overrides != null && i < overrides.Count) ? overrides[i] : -1;

            if (effect is IOverrideValue o)
            {
                displayValue += (overrideValue > -1) ? overrideValue : o.GetIntentValue();
            }
            else
            {
                displayValue += effect.GetIntentValue(); // fallback cho effect thường
            }
        }

        // Icon dựa vào Type
        Sprite icon = typeIcons != null ? typeIcons.GetIcon(nextAction.type) : null;
        intentUI.SetIntent(icon, displayValue);
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
