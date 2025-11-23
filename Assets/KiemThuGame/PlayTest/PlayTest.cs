using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;

public class PlayTest
{
    // A Test behaves as an ordinary method
    [Test]
    public void PlayTestSimplePasses()
    {
        // Use the Assert class to test conditions
    }

    private string homeScene = "0_Menu";
    private string gameScene = "Game";

    [UnitySetUp]
    public IEnumerator SetUp()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(homeScene);
        yield return null;
    }
    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator PlayTestWithEnumeratorPasses()
    {

        Assert.AreEqual(homeScene, UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);

        Button playButton = GameObject.Find("PlayButton").GetComponent<Button>();

        playButton.onClick.Invoke();

        float timeout = 5f;
        float timer = 0f;
        while (SceneManager.GetActiveScene().name != gameScene && timer < timeout)
        {
            yield return null; // đợi 1 frame
            timer += Time.unscaledDeltaTime;
        }

        // === ASSERT ===
        // Sau khi bấm nút phải chuyển sang scene Map
        string currentScene = SceneManager.GetActiveScene().name;//SCENE = "MAP"
        Assert.AreEqual(gameScene, currentScene,
            $"Scene không chuyển sang Map! Hiện tại đang ở: {currentScene}");

        Debug.Log("Test thành công: Bấm Play Game → đã load scene Map!");
    }
}
