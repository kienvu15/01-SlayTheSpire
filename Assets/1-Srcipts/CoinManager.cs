using UnityEngine;
using TMPro;

public class CoinManager : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text coinText;   // UI hiển thị số coin

    [Header("Data")]
    public int startCoins = 100; // coin ban đầu
    private int currentCoins;

    void Start()
    {
        currentCoins = startCoins;
        UpdateUI();
    }

    public bool SpendCoins(int amount)
    {
        if (currentCoins >= amount)
        {
            currentCoins -= amount;
            UpdateUI();
            return true;
        }
        else
        {
            Debug.Log("Không đủ coin!");
            return false;
        }
    }

    public void AddCoins(int amount)
    {
        currentCoins += amount;
        UpdateUI();
    }

    public int GetCoins()
    {
        return currentCoins;
    }

    private void UpdateUI()
    {
        if (coinText != null)
            coinText.text = currentCoins.ToString();
    }
}
