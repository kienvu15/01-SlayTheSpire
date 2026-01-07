using System.Collections;
using UnityEngine;

public class KGameMenuManager : MonoBehaviour
{
    [SerializeField] GroupAppearAnimManager groupAppearAnimManager;

    void Start()
    {
        StartCoroutine(StartGame());
    }

    IEnumerator StartGame()
    {
        yield return new WaitForSeconds(0.5f);
        groupAppearAnimManager.PlayGroupByName("Button");
        groupAppearAnimManager.PlayGroupByName("Logo");
    }
}
