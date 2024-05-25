using System;
using System.Collections;
using System.Collections.Generic;
using GameModel;
using UnityEngine;

public class ChampionObject : MonoBehaviour
{
    private Champion _champion;
    
    public int Health { get; private set; }
    public int Speed { get; private set; }
    public int AttackRange { get; private set; }
    public int AttackDamage { get; private set; }
    public float AttackSpeed { get; private set; }
    public bool CanDoAction { get; private set; }
    
    void Start()
    {
        Health = _champion.MaxHealth;
        Speed = _champion.Speed;
        AttackRange = _champion.AttackRange;
        AttackDamage = _champion.AttackDamage;
        AttackSpeed = _champion.AttackSpeed;
        CanDoAction = true;
    }

    public void Attack(ChampionObject championToAttack)
    {
        //
        // Implement Attack Animation and time until attack hits
        //
        
        StartCoroutine(DisableAnyActionsForTime(10 / AttackSpeed));
    }
    
    public void LoseHealth(int health)
    {
        Health -= health;
        
        print(health);

        if (Health <= 0)
        {
            print("Champion is dead");
        }
    }
    
    public void MoveInDirection(Vector3 direction)
    {
        Vector3 newPosition = transform.position + direction * Speed * Time.deltaTime;
        transform.position = newPosition;
    }

    private IEnumerator DisableAnyActionsForTime(float time)
    {
        CanDoAction = false;
        var timeRemaining = time;
        while (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            yield return null;
        }

        CanDoAction = true;
    }

    public void AssignChampionModel(Champion championModel)
    {
        _champion = championModel;
    }
}
