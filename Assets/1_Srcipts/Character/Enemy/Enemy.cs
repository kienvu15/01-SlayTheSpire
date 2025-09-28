using UnityEngine;

public class Enemy : Character
{
    [Header("Randomize HP")]
    public bool randomizeHP = false;
    public int randomRange = 3;

    protected override void Start()
    {
        if (randomizeHP)
        {
            int offset = Random.Range(-randomRange, randomRange + 1);
            stats.maxHP = Mathf.Max(1, stats.maxHP + offset);
        }

        base.Start();
        UpdateUI();
    }
}
