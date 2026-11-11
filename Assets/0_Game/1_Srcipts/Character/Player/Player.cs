using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Player : Character
{
    public Image avatarImage;
    public Animator animator;
    public GameObject canvas;
    AudioSource audioSource;

    protected override void Start()
    {
        audioSource = GetComponent<AudioSource>();
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
            audioSource.Play();
        }
        Character_Info_UI.Instance.Update_InfoUI();
    }

    public override void AddShield(int amount, CardType vfxType = CardType.Special)
    {
        base.AddShield(amount);

        if (animator != null)
        {
            animator.SetTrigger("Up");
        }
    }
}

