using System.Collections;
using UnityEngine;

public class KGameMenuManager : MonoBehaviour
{
    [SerializeField] GroupAppearAnimManager groupAppearAnimManager;
    [SerializeField] GroupAnimManager groupAnimManager;

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

    //Button

    public void StartButtonClick() 
    {
        groupAnimManager.StartButtonClick();
        groupAppearAnimManager.PlayGroupByName("CharacterS");
    }
}
