using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GameModel;
using Unity.VisualScripting;
using UnityEngine;

public class BattleController : MonoBehaviour
{
    private readonly Vector3[] _startPositionsTeam1 =
    {
        new Vector3(-40, -25, 0),
        new Vector3(-30, 0, 0),
        new Vector3(-40, 25, 0),
    };
    
    private readonly Vector3[] _startPositionsTeam2 =
    {
        new Vector3(40, -25, 0),
        new Vector3(30, 0, 0),
        new Vector3(40, 25, 0),
    };

    [SerializeField] private ScoreUI scoreUI;
    [SerializeField] private BattleTimeUI battleTimeUI;
    
    [SerializeField] private int respawnTime;
    [SerializeField] private int battleTime;
    private List<ChampionObject> _champions;
    private List<ChampionObject> _championsTeam1;
    private List<ChampionObject> _championsTeam2;
    private int _scoreTeam1;
    private int _scoreTeam2;

    [SerializeField] private ChampionFactory factory;

    public delegate void OnBattleEnd();
    public event OnBattleEnd onBattleEnd;

    public void InitiateBattle(Champion[] championsTeam1, Champion[] championsTeam2)
    {
        _champions = new List<ChampionObject>();
        _championsTeam1 = new List<ChampionObject>();
        _championsTeam2 = new List<ChampionObject>();
        
        //Initiate Team 1
        for (int i = 0; i < championsTeam1.Length; i++)
        {
            var noob = factory.CreateChampionInBattle(championsTeam1[i], _startPositionsTeam1[i]);
            _champions.Add(noob);
            _championsTeam1.Add(noob);
        }
        
        //Initiate Team 2
        for (int i = 0; i < championsTeam2.Length; i++)
        {
            var noob = factory.CreateChampionInBattle(championsTeam2[i], _startPositionsTeam2[i]);
            _champions.Add(noob);
            _championsTeam2.Add(noob);
        }

        foreach (var champion in _champions)
        {
            champion.onDeath += HandleChampionDeath;
        }

        // Initialize Score
        _scoreTeam1 = 0;
        _scoreTeam2 = 0;
        scoreUI.ChangeScore(_scoreTeam1, _scoreTeam2);

        // Initialize Battle Time
        battleTimeUI.ChangeTimer(battleTime);
        StartCoroutine(StartBattleTimer());
    }

    private IEnumerator StartBattleTimer()
    {
        while (battleTime > 0)
        {
            yield return new WaitForSeconds(1);
            battleTime--;
            battleTimeUI.ChangeTimer(battleTime);
        }
        EndBattle();
    }

    public void ExecuteTurn()
    {
        DecideOnChampionActions();
    }
    
    private void DecideOnChampionActions()
    {
        foreach (var champion in _champions)
        {
            switch (champion.CurrentState)
            {
                case ChampionState.Attacking:
                case ChampionState.Dead:
                    break;
                case ChampionState.Ready:
                    HandleReadyState(champion);
                    break;
                case ChampionState.AttackCooldown:
                    HandleAttackCooldownState(champion);
                    break;
            }
        }
    }

    private void HandleReadyState(ChampionObject champion)
    {
        ChampionObject closestEnemy = GetClosestEnemyFromChampion(champion);
        float distanceToChampion = DistanceBetweenChampions(champion, closestEnemy);

        if (champion.AttackRange >= distanceToChampion)
        {
            var enemyPos = closestEnemy.transform.position;
            var directionToEnemy = (enemyPos - champion.transform.position).normalized;
            StartCoroutine(champion.AttackCoroutine(directionToEnemy,() =>
            {
                closestEnemy.LoseHealth(champion.AttackDamage);
            }));
        }
        else
        {
            MoveTowardsTarget(champion, closestEnemy);
        }
    }

    private void HandleAttackCooldownState(ChampionObject champion)
    {
        ChampionObject closestEnemy = GetClosestEnemyFromChampion(champion);
        float distanceToClosestEnemy = DistanceBetweenChampions(champion, closestEnemy);
        if (champion.AttackRange <= distanceToClosestEnemy)
        {
            MoveTowardsTarget(champion, closestEnemy);
            return;
        }
        
        List<ChampionObject> enemies = new List<ChampionObject>();
        if (_championsTeam1.Contains(champion)) enemies.AddRange(_championsTeam2);
        else enemies.AddRange(_championsTeam1);

        enemies = SortByDistanceFromChampion(champion, enemies);
        foreach (var enemy in enemies)
        {
            float distanceToChampion = DistanceBetweenChampions(champion, enemy);
            if (enemy.AttackRange >= distanceToChampion)
            {
                MoveAwayFromTarget(champion, enemy);
                return;
            }
        }

        
    }

    private List<ChampionObject> SortByDistanceFromChampion(ChampionObject champion, List<ChampionObject> targets)
    {
        List<KeyValuePair<ChampionObject, float>> distanceToTargets = new List<KeyValuePair<ChampionObject, float>>();

        foreach (var target in targets)
        {
            float distance = DistanceBetweenChampions(champion, target);
            distanceToTargets.Add(new KeyValuePair<ChampionObject, float>(target, distance));
        }

        var sortedList = distanceToTargets.OrderBy(pair => pair.Value).ToList();
        return sortedList.Select(pair => pair.Key).ToList();
    }

    private void MoveTowardsTarget(ChampionObject mover, ChampionObject target)
    {
        var targetPos = target.transform.position;
        var directionToMove = (targetPos - mover.transform.position).normalized;
        mover.MoveInDirection(directionToMove);
    }
    
    private void MoveAwayFromTarget(ChampionObject mover, ChampionObject target)
    {
        var targetPos = target.transform.position;
        var directionToMove = (mover.transform.position - targetPos).normalized;
        mover.MoveInDirection(directionToMove);
    }

    private ChampionObject GetClosestEnemyFromChampion(ChampionObject champion)
    {
        List<ChampionObject> allEnemies = new List<ChampionObject>();

        if (_championsTeam1.Contains(champion)) allEnemies.AddRange(_championsTeam2);
        if (_championsTeam2.Contains(champion)) allEnemies.AddRange(_championsTeam1);
        
        ChampionObject closestEnemy = null;
        float currentlyClosestDistance = float.MaxValue;
        
        foreach (var champ in allEnemies)
        {
            if (champ.IsDead) continue;
            var distance = DistanceBetweenChampions(champion, champ);
            if (distance < currentlyClosestDistance)
            {
                closestEnemy = champ;
                currentlyClosestDistance = distance;
            }
        }
        return closestEnemy;
    }
    
    private void HandleChampionDeath(ChampionObject champion)
    {
        IncreaseScore(champion);
        scoreUI.ChangeScore(_scoreTeam1, _scoreTeam2);
        StartCoroutine(StartRespawnTime(champion));
    }

    private void IncreaseScore(ChampionObject champion)
    {
        if (_championsTeam1.Contains(champion)) _scoreTeam1++;
        if (_championsTeam2.Contains(champion)) _scoreTeam2++;
    }

    private IEnumerator StartRespawnTime(ChampionObject champion)
    {
        yield return new WaitForSeconds(respawnTime);
        champion.Respawn();
    }
    
    private void EndBattle()
    {
        StopAllCoroutines();
        onBattleEnd?.Invoke();
    }

    private static float DistanceBetweenChampions(ChampionObject champion1, ChampionObject champion2)
    {
        float distance = Vector2.Distance(champion1.transform.position, champion2.transform.position);
        return distance;
    }
}
