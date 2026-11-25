using UnityEngine;
using System.Collections;

public class GameStart_Init : MonoBehaviour
{
    [SerializeField] PlayerSpawner playerSpawner;

    private IEnumerator Start()
    {
        yield return null;
        playerSpawner.OnGameStart_Init();
    }
}
