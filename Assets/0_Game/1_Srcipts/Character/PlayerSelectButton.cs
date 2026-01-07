using UnityEngine;
using UnityEngine.UI;

public class PlayerSelectButton : MonoBehaviour
{
    public PlayerData playerData;
    public PlayerSelectionUI selectionUI;

    private Button button;

    void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
    }

    void OnClick()
    {
        selectionUI.Show(playerData);
        PlayerSelectionManager.Instance.SelectPlayer(playerData);
    }
}
