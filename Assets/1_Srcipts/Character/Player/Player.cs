using System.Collections;
using UnityEngine;

public class Player : Character
{
    public Animator animator;
    public GameObject canvas;
    protected override void Start()
    {
        base.Start();
        UpdateUI();
    }

    public IEnumerator EnableCanvasAfterAnimation(string stateName)
    {
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName(stateName));

        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

        canvas.SetActive(true);
        animator.SetBool("Enter", false);
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
}

