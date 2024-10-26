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
        new Vector3(-10, 0, 0),
        new Vector3(-15, -10, 0),
        new Vector3(-15, 10, 0),
    };
    
    private readonly Vector3[] _startPositionsTeam2 =
    {
        new Vector3(10, 0, 0),
        new Vector3(15, -10, 0),
        new Vector3(15, 10, 0),
    };

    [SerializeField] private ScoreUI scoreUI;
    [SerializeField] private BattleTimeUI battleTimeUI;
    [SerializeField] private GameObject arena;
    
    [SerializeField] private int respawnTime;
    [SerializeField] private int battleTime;
    private List<ChampionObject> _champions;
    private int _scoreTeam1;
    private int _scoreTeam2;

    [SerializeField] private ChampionFactory factory;

    public delegate void OnBattleEnd();
    public event OnBattleEnd onBattleEnd;

    public void InitiateBattle(Champion[] championsTeam1, Champion[] championsTeam2)
    {
        _champions = new List<ChampionObject>();

        //Initiate Team 1
        for (int i = 0; i < championsTeam1.Length; i++)
        {
            var noob = factory.CreateChampionInBattle(championsTeam1[i], _startPositionsTeam1[i]);
            noob.Initialize(this, Team.Blue, championsTeam1[i], arena);
            _champions.Add(noob);
        }
        
        //Initiate Team 2
        for (int i = 0; i < championsTeam2.Length; i++)
        {
            var noob = factory.CreateChampionInBattle(championsTeam2[i], _startPositionsTeam2[i]);
            noob.Initialize(this, Team.Red, championsTeam2[i], arena);
            _champions.Add(noob);
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
        champion.HandleReadyState();
    }

    private void HandleAttackCooldownState(ChampionObject champion)
    {
        champion.HandleAttackCooldown();
    }

    private List<KeyValuePair<ChampionObject,float>> SortByDistanceFromChampion(ChampionObject champion, List<ChampionObject> targets)
    {
        List<KeyValuePair<ChampionObject, float>> targetsWithDistance = new List<KeyValuePair<ChampionObject, float>>();

        foreach (var target in targets)
        {
            float distance = GetDistanceToChampion(champion, target);
            targetsWithDistance.Add(new KeyValuePair<ChampionObject, float>(target, distance));
        }

        var sortedList = targetsWithDistance.OrderBy(pair => pair.Value).ToList();
        return sortedList;
    }

    public ChampionObject GetClosestChampion(ChampionObject source, List<ChampionObject> targets)
    {
        ChampionObject closestChampion = null;
        float currentlyClosestDistance = float.MaxValue;
        
        foreach (var target in targets)
        {
            var distance = GetDistanceToChampion(source, target);
            if (distance < currentlyClosestDistance)
            {
                closestChampion = target;
                currentlyClosestDistance = distance;
            }
        }
        return closestChampion;
    }
    
    private void HandleChampionDeath(ChampionObject champion)
    {
        IncreaseScore(champion.Team);
        scoreUI.ChangeScore(_scoreTeam1, _scoreTeam2);
        StartCoroutine(StartRespawnTime(champion));
    }

    private void IncreaseScore(Team team)
    {
        if (team == Team.Blue) _scoreTeam1++;
        if (team == Team.Red) _scoreTeam2++;
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

    public float GetDistanceToChampion(ChampionObject source, ChampionObject target)
    {
        float distance = Vector2.Distance(source.transform.position, target.transform.position);
        return distance;
    }

    public List<ChampionObject> GetChampionsOfTeam(Team team, bool excludeDead)
    {
        List<ChampionObject> champs = new List<ChampionObject>();

        if (excludeDead)
        {
            foreach (var champ in _champions.Where(champ => champ.Team == team && champ.CurrentState != ChampionState.Dead))
            {
                champs.Add(champ);
            }
        }
        else
        {
            foreach (var champ in _champions.Where(champ => champ.Team == team))
            {
                champs.Add(champ);
            }
        }
        
        return champs;
    }
}
