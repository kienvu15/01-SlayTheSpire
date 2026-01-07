using UnityEngine;

public class GameStage : MonoBehaviour
{
    public static GameStage Instance;

    public bool isBusy { get; private set; }
    public void SetBusy(bool value) => isBusy = value;

    private void Awake()
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
}
