using UnityEngine;

public class GameSystem : MonoBehaviour
{
    public static GameSystem Instance { get; private set; }
    
    [SerializeField] public GameObject BattleUICanvas;
    [SerializeField] private PanCamera panCamera;
    [SerializeField] public GameObject gameCanvas;

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

    public bool IsGamePaused { get; private set; } = false;

    [SerializeField] public bool isBattlePhase = false;   

}
