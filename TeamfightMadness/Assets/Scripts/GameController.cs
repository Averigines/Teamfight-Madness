using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameModel;

public class GameController : MonoBehaviour
{
    [Serializable]
    private struct ChampionEntry
    {
        public int health;
        public int speed;
        public int attackRange;
        public int attackDamage;
        public float attackSpeed;
    }

    [SerializeField] private BattleController battleController;

    [SerializeField] private List<ChampionEntry> championEntries;

    private void Start()
    {
        foreach (var champion in championEntries)
        {
            Champions.CreateChampion(champion.health, champion.speed, champion.attackRange, champion.attackDamage, champion.attackSpeed);
        }
        var allChampions = Champions.AllChampions;
        
        var selectedChampionsTeam1 = new[]
        {
            allChampions[0]//, allChampions[1], allChampions[2]
        };
        var selectedChampionsTeam2 = new[]
        {
            allChampions[1]//, allChampions[4], allChampions[5]
        };
        battleController.InitiateBattle(selectedChampionsTeam1, selectedChampionsTeam2);
    }

    void Update()
    {
        battleController.ExecuteTurn();
    }
}
