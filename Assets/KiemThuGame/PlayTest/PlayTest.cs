using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class PlayTest
{
    private string homeScene = "0_Menu";
    private string gameScene = "Game";


    [UnityTest]
    public IEnumerator OpenGame()
    {
        yield return SceneManager.LoadSceneAsync(homeScene);
        yield return null;

        Assert.AreEqual(homeScene, SceneManager.GetActiveScene().name);

        UnityEngine.UI.Button playButton = GameObject.Find("PlayButton").GetComponent<UnityEngine.UI.Button>();

        playButton.onClick.Invoke();

        float timeout = 5f;
        float timer = 0f;

        while (SceneManager.GetActiveScene().name != gameScene && timer < timeout)
        {
            yield return null;
            timer += Time.unscaledDeltaTime;
        }

        Assert.AreEqual(gameScene, SceneManager.GetActiveScene().name);
    }

    [UnityTest]
    public IEnumerator OpenMap()
    {
        yield return SceneManager.LoadSceneAsync(gameScene);
        yield return new WaitForSecondsRealtime(1f);
        UIManager ui = GameObject.FindFirstObjectByType<UIManager>();

        GameObject mapObj = ui.Map;


        GameObject icon = GameObject.Find("MapIcon");

        UnityEngine.UI.Button mapButton = icon.GetComponentInChildren<UnityEngine.UI.Button>();

        // Click
        mapButton.onClick.Invoke();

        float timeout = 3f;
        float timer = 0f;

        while (!mapObj.activeSelf && timer < timeout)
        {
            yield return null;
            timer += Time.unscaledDeltaTime;
        }

        Assert.IsTrue(mapObj.activeSelf, "Map should be active after clicking the Map button.");
    }

    [UnityTest]
    public IEnumerator OpenDeck()
    {
        yield return SceneManager.LoadSceneAsync(gameScene);
        UIManager ui = GameObject.FindFirstObjectByType<UIManager>();

        GameObject MyDeck = ui.deckUI;

        UnityEngine.UI.Button Deck = GameObject.Find("Deck").GetComponentInChildren<UnityEngine.UI.Button>();

        Deck.onClick.Invoke();
        
        float timeout = 3f;
        float t = 0f;

        while (!MyDeck.activeSelf && t < timeout)
        {
            yield return null;
            t += Time.unscaledDeltaTime;
        }

        Assert.IsTrue(MyDeck.activeSelf, "MyDeck should be active after clicking the Deck button.");
    }

    [UnityTest]
    public IEnumerator AddCoinPlay()
    {
        yield return SceneManager.LoadSceneAsync(gameScene);
        CoinManager coinManager = GameObject.FindFirstObjectByType<CoinManager>();
        coinManager.AddCoins(100);
        if(coinManager.currentCoins >= 100)
        {
            Assert.Pass("AddCoinPlay passed.");
        }
        else
        {
            Assert.Fail("AddCoinPlay failed.");
        }
    }

    [UnityTest]
    public IEnumerator SoundTest()
    {
        yield return SceneManager.LoadSceneAsync(gameScene);
        SoundManager soundManager = GameObject.FindFirstObjectByType<SoundManager>();
        soundManager.Play("PencilDraw");
        if(soundManager.GetComponentInChildren<AudioSource>().isPlaying)
        {
            Assert.Pass("SoundTest passed.");
        }
        else
        {
            Assert.Fail("SoundTest failed.");
        }
    }

    [UnityTest]
    public IEnumerator MoveTest()
    {
        yield return SceneManager.LoadSceneAsync(gameScene);
        // 1. Thiết lập Scene giả lập
        GameObject player = new GameObject("Player");
        var playerMove = player.AddComponent<CharacterController>();
        var sp = playerMove.MoveCharacter(5f, Vector3.right);


        // 3. Chờ 1 frame để simulation diễn ra (nếu cần)
        yield return null;

        // 4. Kiểm tra kết quả
        Assert.AreNotEqual(new Vector3(0,0,0), sp);

    }


    private string Scene = "Test";
    [UnityTest]
    public IEnumerator ShootTest()
    {
        yield return SceneManager.LoadSceneAsync(Scene);
        GameObject player = GameObject.Find("Player");
        GameObject box = GameObject.Find("Box");

        

        yield return new WaitForSeconds(5f);


        Assert.IsTrue(!box.activeSelf, "Box should be destroy after take damage");

    }

}
