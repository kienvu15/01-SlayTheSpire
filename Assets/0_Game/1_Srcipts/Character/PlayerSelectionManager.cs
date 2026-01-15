using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSelectionManager : MonoBehaviour
{
    public static PlayerSelectionManager Instance;

    private PlayerData selectedPlayer;
    private PlayerSelectButton currentButton;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SelectButton(PlayerSelectButton button)
    {
        if (currentButton != null)
            currentButton.SetSelected(false);

        currentButton = button;
        currentButton.SetSelected(true);
    }

    public void SelectPlayer(PlayerData data)
    {
        selectedPlayer = data;
    }

    public PlayerData GetSelectedPlayer()
    {
        return selectedPlayer;
    }

    public void ConfirmSelection(string nextScene)
    {
        SceneManager.LoadScene(nextScene);
    }
}
