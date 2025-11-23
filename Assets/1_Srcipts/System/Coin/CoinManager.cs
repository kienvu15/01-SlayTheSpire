using UnityEngine;
using TMPro;

public class CoinManager : MonoBehaviour
{
    public static CoinManager Instance; // Singleton

    [Header("UI")]
    public TMP_Text coinText;

    [Header("Data")]
    public int startCoins = 100;
    private int currentCoins;

    private void Awake()
    {
        // Setup Singleton
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

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
        Debug.Log("Không đủ coin!");
        return false;
    }

    public void AddCoins(int amount)
    {
        currentCoins += amount;
        UpdateUI();
        Debug.Log("Added " + amount + " coins. Total: " + currentCoins);
    }

    private void UpdateUI()
    {
        if (coinText != null)
            coinText.text = currentCoins.ToString();
    }
}