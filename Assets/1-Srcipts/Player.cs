using System.Collections;
using UnityEngine;

public class Player : Character
{
    private Animator animator;
    private GameObject canvas;
    protected override void Start()
    {
        animator = GetComponentInChildren<Animator>();
        canvas = GetComponentInChildren<Canvas>().gameObject;
        canvas.SetActive(false);

        base.Start();
        UpdateUI();
        StartCoroutine(EnableCanvasAfterAnimation("Enter"));
    }

    private IEnumerator EnableCanvasAfterAnimation(string stateName)
    {
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName(stateName));

        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

        canvas.SetActive(true);
        animator.SetBool("Enter", false);
    }

    public override void TakeDamage(int amount)
    {
        // Gọi hàm gốc để trừ máu, xử lý shield, update UI
        base.TakeDamage(amount);

        // Gọi animation riêng của Player
        if (animator != null)
        {
            animator.SetTrigger("Hit");  // trigger "Hit" trong Animator
        }
    }

    public override void AddShield(int amount)
    {
        base.AddShield(amount);

        if (animator != null)
        {
            animator.SetTrigger("Up");  // trigger "ShieldUp" trong Animator
        }
    }
}

