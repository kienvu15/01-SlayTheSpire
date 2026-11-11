using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;

public class ManaSystem : MonoBehaviour
{
    public static ManaSystem instance;

    [Header("Mana Settings")]
    public int maxMana = 10;          
    public int currentMana = 0;       
    public int manaPerTurn = 1;

    public TextMeshProUGUI ManaText;
    public Image Mana_Icon;
    public Animator mana_Anim;
    public GameObject glow_Object;

    private void Awake()
    {
        instance = this;
    }

    public void InitMana(CharacterData playerData)
    {
        maxMana = playerData.maxMana;
        currentMana = playerData.startMana;
        manaPerTurn = playerData.manaPerTurn;
        UpdateManaUI();
    }

    /// Reset mana mỗi lượt (gọi khi End Turn)
    public void StartTurn()
    {
        currentMana += manaPerTurn;
        currentMana = Mathf.Clamp(currentMana, 0, maxMana);
        mana_Anim.SetTrigger("ReFill");
        UpdateManaUI();
    }

    public int GainMana(int amount)
    {
        if (amount <= 0) return 0;

        int old = currentMana;
        currentMana = Mathf.Min(currentMana + amount, maxMana);
        AnimHelper.Instance.PlayManaScale();
        UpdateManaUI();
        mana_Anim.SetTrigger("ReFill");
        return currentMana - old;
    }

    /// Kiểm tra đủ mana để chơi card (theo level của lá bài)
    public bool CanPlayCard(CardInstance card)
    {
        return card != null && currentMana >= card.ManaCost;
    }

    /// Overload cũ: dùng mana gốc (level 1)
    public bool CanPlayCard(CardData card)
    {
        return card != null && currentMana >= card.manaCost;
    }

    /// Trừ mana khi chơi card
    public void SpendMana(int amount)
    {
        currentMana -= amount;
        if (currentMana < 0) currentMana = 0;
        AnimHelper.Instance.PlayManaScale();
        UpdateManaUI();
    }

    // reset mana to max when player enters a battle
    public void ResetManaToMax()
    {
        currentMana = maxMana;
        UpdateManaUI();
    }

    /// Cập nhật UI Mana bằng icon
    public void UpdateManaUI()
    {
        ManaText.text = $"{currentMana} / {maxMana}";
        if (currentMana <= 0)
        {
            ManaText.color = Color.red;
            Mana_Icon.color = new Color32(198, 198, 198, 255);
            glow_Object.SetActive(false);
        }
        else
        {
            ManaText.color = Color.white;
            Mana_Icon.color = Color.white; glow_Object.SetActive(true);
        }
    }

}
