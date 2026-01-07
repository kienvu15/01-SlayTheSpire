using UnityEngine;
using UnityEngine.UI;

public class PlayerSelectButton : MonoBehaviour
{
    public PlayerData playerData;
    public PlayerSelectionUI selectionUI;

    private Button button;
    public bool startCharacter = false;

    void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
    }

    private void Start()
    {
        if (startCharacter) 
        {
            selectionUI.Show(playerData);
            PlayerSelectionManager.Instance.SelectPlayer(playerData);
        }
    }

    void OnClick()
    {
        selectionUI.Show(playerData);
        PlayerSelectionManager.Instance.SelectPlayer(playerData);
    }
}
