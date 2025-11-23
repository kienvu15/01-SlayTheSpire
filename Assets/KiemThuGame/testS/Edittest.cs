using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class Edittest
{
    private CoinManager coinManager;
    // A Test behaves as an ordinary method
    [Test]
    public void EdittestSimplePasses()
    {
        var game = new GameObject("CoinManager");
        coinManager = game.AddComponent<CoinManager>();

        coinManager.AddCoins(50);
        //Assert.AreEqual(150, coinManager.GetCoins());


    }





















    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator EdittestWithEnumeratorPasses()
    {
        // Use the Assert class to test conditions.
        // Use yield to skip a frame.
        yield return null;
    }
}
