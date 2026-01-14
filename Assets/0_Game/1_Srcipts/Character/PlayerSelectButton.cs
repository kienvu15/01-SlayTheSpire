using UnityEngine;
using UnityEngine.UI;

public class PlayerSelectButton : MonoBehaviour
{
    public PlayerData playerData;
    public PlayerSelectionUI selectionUI;

    [Header("Visual")]
    [SerializeField] private Outline outline; // object viền sáng

    [Header("Config")]
    public bool startCharacter = false;

    private Button button;

    void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);

        SetSelected(false);
    }

    private void Start()
    {
        if (startCharacter)
        {
            Select();
        }
    }

    void OnClick()
    {
        Select();
    }

    void Select()
    {
        PlayerSelectionManager.Instance.SelectButton(this);

        selectionUI.Show(playerData);
        PlayerSelectionManager.Instance.SelectPlayer(playerData);
    }

    public void SetSelected(bool selected)
    {
        if (outline != null)
            outline.enabled = selected;
    }
}
