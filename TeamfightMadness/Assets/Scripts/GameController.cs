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
        public Vector2 startPosition;
        public int health;
        public float speed;
        public float attackRange;
        public int damagePerAttack;
    }

    [SerializeField] private List<ChampionEntry> champions;
    [SerializeField] private GameObject championPreFab;

    private Dictionary<Champion, ChampionObject> _championsMap;

    void Start()
    {
        _championsMap = new Dictionary<Champion, ChampionObject>();
        
        foreach (var champion in champions)
        {
            Champions.CreateChampion(champion.startPosition, champion.health, champion.speed, champion.attackRange, champion.damagePerAttack);
        }

        foreach (var champion in Champions.AllChampions)
        {
            var go = Instantiate(championPreFab, new Vector3(champion.Position.x, champion.Position.y, 0), Quaternion.identity);
            var championObject = go.GetComponent<ChampionObject>();
            _championsMap.Add(champion, championObject);

            championObject.Champion = champion;

            champion.onAttack += championObject.AttackChampion;
        }
        
    }
    
    void Update()
    {
        Champions.DecideOnActions();
    }
}
