using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class ManaSystem : MonoBehaviour
{
    [Header("Mana Settings")]
    public int maxMana = 10;          // Giới hạn mana tối đa
    public int currentMana = 0;       // Mana hiện tại
    public int manaPerTurn = 1;       // Mỗi lượt được refill thêm

    [Header("UI References")]
    public Transform manaContainer;   // nơi chứa các icon mana
    public GameObject manaImagePrefab;    // prefab icon vàng (đã có mana)
    public GameObject hollowManaImagePrefab; // prefab icon xám (hollow)

    private List<GameObject> manaIcons = new List<GameObject>();

    void Start()
    {
    }

    /// Reset mana mỗi lượt (gọi khi End Turn)
    public void StartTurn()
    {
        currentMana += manaPerTurn;
        currentMana = Mathf.Clamp(currentMana, 0, maxMana);
        UpdateManaUI();
    }

    /// Kiểm tra xem có đủ mana để chơi card không
    public bool CanPlayCard(CardData card)
    {
        return currentMana >= card.manaCost;
    }

    /// Trừ mana khi chơi card
    public void SpendMana(int amount)
    {
        currentMana -= amount;
        if (currentMana < 0) currentMana = 0;
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
        // Xóa icon cũ
        foreach (var icon in manaIcons)
        {
            Destroy(icon);
        }
        manaIcons.Clear();

        // Vẽ lại mana (vàng trước)
        for (int i = 0; i < currentMana; i++)
        {
            GameObject icon = Instantiate(manaImagePrefab, manaContainer);
            manaIcons.Add(icon);
        }

        // Vẽ hollow (xám) cho phần còn lại
        for (int i = currentMana; i < maxMana; i++)
        {
            GameObject icon = Instantiate(hollowManaImagePrefab, manaContainer);
            manaIcons.Add(icon);
        }
    }

}
