using UnityEngine;

public class GameSystem : MonoBehaviour
{
    public static GameSystem Instance { get; private set; }
    
    [SerializeField] public GameObject BattleUICanvas;
    [SerializeField] private PanCamera panCamera;
    [SerializeField] public GameObject gameCanvas
        ;
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this; 
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        if (BattleUICanvas != null)
        {
            bool active = BattleUICanvas.activeSelf;
            if (isBattlePhase != active)
            {
                isBattlePhase = active;
                Debug.Log($"[GameSystem] BattlePhase = {isBattlePhase}");
            }
        }

        if(gameCanvas.activeSelf == false)
        {
            if (panCamera != null)
            {
                panCamera.enabled = false;
            }
        }
        else
        {
            if (panCamera != null)
            {
                panCamera.enabled = true;
            }
        }

    }

    public bool IsGamePaused { get; private set; } = false;

    [SerializeField] private bool isBattlePhase = false;   // hiện trong Inspector
    public bool IsBattlePhase => isBattlePhase;
}
