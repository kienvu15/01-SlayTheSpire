//using System;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class GameStage_Manager : MonoBehaviour
//{
//    public static GameStage_Manager Instance { get; private set; }

//    public Player player;
//    public Deck deck;
//    public Discard discard;
//    public MidMatch_Manager match;
//    public CoinManager coinManager;
//    public ManaSystem manasystem;
//    public List<GameObject> hideOnBattle;

//    public bool isOnBattle;
//    public bool isFirstBattle = true;

//    //===== Event =====//
//    public event Action OnBattleEnded;

//    private void Awake()
//    {
//        if(Instance == null)
//        {
//            Instance = this;
//            DontDestroyOnLoad(gameObject);
//        }
//        else
//        {
//            Destroy(gameObject);
//        }
//    }

    

//}
