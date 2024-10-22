using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using GameModel;

public class GameController : MonoBehaviour
{
    [Serializable]
    public struct ChampionEntry
    {
        public ChampionName name;
        public int health;
        public int speed;
        public int attackRange;
        public int attackDamage;
        public float attackSpeed;
        public float attackCooldown;
        public bool active;
    }

    [SerializeField] private BattleController battleController;

    [SerializeField] private List<ChampionEntry> championEntries;

    private bool _activeBattle;

    private void Start()
    {
        List<Champion> champions = new List<Champion>();
        foreach (var entry in championEntries)
        {
            if (!entry.active) continue;
            
            Champion noob = ChampionFactory.CreateChampion(entry);
            champions.Add(noob);
        }
        
        int midPoint = champions.Count / 2;
        
        var selectedChampionsTeam1 = champions.Take(midPoint).ToArray();
        var selectedChampionsTeam2 = champions.Skip(midPoint).ToArray();
        
        _activeBattle = true;
        battleController.InitiateBattle(selectedChampionsTeam1, selectedChampionsTeam2);
        battleController.onBattleEnd += () =>
        {
            _activeBattle = false;
        };
    }

    void Update()
    {
        if (_activeBattle)
        {
            battleController.ExecuteTurn();
        }
    }
}
