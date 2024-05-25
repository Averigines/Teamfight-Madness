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
    
    [SerializeField] private GameObject championPreFab;
    private List<ChampionObject> _champions;
    private List<ChampionObject> _championsTeam1;
    private List<ChampionObject> _championsTeam2;

    public void InitiateBattle(Champion[] championsTeam1, Champion[] championsTeam2)
    {
        _champions = new List<ChampionObject>();
        _championsTeam1 = new List<ChampionObject>();
        _championsTeam2 = new List<ChampionObject>();
        
        //Initiate Team 1
        for (int i = 0; i < championsTeam1.Length; i++)
        {
            var go = Instantiate(championPreFab, _startPositionsTeam1[i], Quaternion.identity);
            var championObject = go.GetComponent<ChampionObject>();
            _champions.Add(championObject);
            _championsTeam1.Add(championObject);

            championObject.AssignChampionModel(championsTeam1[i]);
        }
        
        //Initiate Team 2
        for (int i = 0; i < championsTeam2.Length; i++)
        {
            var go = Instantiate(championPreFab, _startPositionsTeam2[i], Quaternion.identity);
            var championObject = go.GetComponent<ChampionObject>();
            _champions.Add(championObject);
            _championsTeam2.Add(championObject);

            championObject.AssignChampionModel(championsTeam2[i]);
        }
    }

    public void ExecuteTurn()
    {
        DecideOnChampionActions();
    }
    
    private void DecideOnChampionActions()
    {
        foreach (var champion in _champions)
        {
            DecideOnActionOfChampion(champion);
        }
    }
    
    private void DecideOnActionOfChampion(ChampionObject champion)
    {
        if (!champion.CanDoAction) return;
        
        ChampionObject closestEnemy = GetClosestEnemyFromChampion(champion);
        float distanceToChampion = DistanceBetweenChampions(champion, closestEnemy);

        if (champion.AttackRange >= distanceToChampion)
        {
            champion.Attack(closestEnemy);
            closestEnemy.LoseHealth(champion.AttackDamage);
        }
        else
        {
            var closestEnemyPosition = closestEnemy.transform.position;
            var directionToMove = (closestEnemyPosition - champion.transform.position).normalized;
            champion.MoveInDirection(directionToMove);
        }
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
            var distance = DistanceBetweenChampions(champion, champ);
            if (distance < currentlyClosestDistance)
            {
                closestEnemy = champ;
                currentlyClosestDistance = distance;
            }
        }
        return closestEnemy;
    }

    private static float DistanceBetweenChampions(ChampionObject champion1, ChampionObject champion2)
    {
        float distance = Vector2.Distance(champion1.transform.position, champion2.transform.position);
        return distance;
    }
}
