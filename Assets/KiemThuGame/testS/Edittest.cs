using NUnit.Framework;
using System.Collections;
using System.Net.WebSockets;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.TextCore.Text;

public class Edittest
{
    private CoinManager coinManager;
    [Test]
    public void AddCoinTest()
    {
        var game = new GameObject("CoinManager");
        coinManager = game.AddComponent<CoinManager>();

        coinManager.AddCoins(50);
        Assert.AreEqual(50, 50);
    }

    private Character character;
    [Test]
    public void HealthChangedTest()
    {
        var cha = new GameObject("Character");
        character = cha.AddComponent<Character>();

        character.stats.maxHP = 100;
        character.stats.currentHP = 100;
        character.stats.defense = 0;
        character.stats.shield = 0;

        character.ClearAllConditionsAndSkills();

        character.TakeDamage(30);

        Assert.AreEqual(70, character.stats.currentHP);
    }

    [Test]
    public void MoveTest()
    {
        var pl = new GameObject().AddComponent<CharacterController>();
        Vector3 speed = pl.MoveCharacter(5f, Vector3.right);
        Assert.AreEqual(new Vector3(5,0,0), speed);
    }

    [Test]
    public void MoveTest2()
    {
        var pl = new GameObject().AddComponent<CharacterController>();
        Vector3 speed = pl.MoveCharacter(3f, Vector3.up);
        Assert.AreEqual(new Vector3(0,3,0), speed);
    }

    private Enemy enemy;
    [Test]
    public void DealDamege()
    {
        var en = new GameObject("Enemy");
        enemy = en.AddComponent<Enemy>();

        enemy.stats.currentHP = 100;

        var cha = new GameObject("Character");
        character = cha.AddComponent<Character>();

        character.stats.maxHP = 100;
        character.stats.currentHP = 100;
        character.stats.defense = 0;
        character.stats.shield = 0;

        character.ClearAllConditionsAndSkills();
        character.DealRawDamage(enemy, 40);
        Assert.AreEqual(60, enemy.stats.currentHP);
    }

}
