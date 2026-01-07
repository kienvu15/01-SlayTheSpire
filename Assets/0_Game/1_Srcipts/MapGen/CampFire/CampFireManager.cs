using UnityEngine;

public class CampFireManager : MonoBehaviour
{
    private Player player;
    void Start()
    {
         player = FindFirstObjectByType<Player>();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RestButton()
    {

        UIManager.Instance.FillOverTime();
        UIManager.Instance.OnTransitionFilled -= HandleTransitionComplete;
        UIManager.Instance.OnTransitionFilled += HandleTransitionComplete;

        
    }

    private void HandleTransitionComplete()
    {
        UIManager.Instance.FadeOutOverTime();

        int amount = player.stats.maxHP * 20 / 100;
        player.Heal(amount);

        UIManager.Instance.OnTransitionFilled -= HandleTransitionComplete;

    }

}
