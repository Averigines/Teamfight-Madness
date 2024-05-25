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
    public Vector3 StartPosition { get; private set; }
    public bool IsDead { get; private set; }
    
    public delegate void OnDeath(ChampionObject champion);
    public event OnDeath onDeath;
    
    void Start()
    {
        Health = _champion.MaxHealth;
        Speed = _champion.Speed;
        AttackRange = _champion.AttackRange;
        AttackDamage = _champion.AttackDamage;
        AttackSpeed = _champion.AttackSpeed;
        CanDoAction = true;
        StartPosition = transform.position;
        IsDead = false;
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
            IsDead = true;
            CanDoAction = false;
            gameObject.SetActive(false);
            StopAllCoroutines();
            onDeath?.Invoke(this);
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
        /*var timeRemaining = time;
        while (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            yield return null;
        }*/
        
        yield return new WaitForSeconds(time);

        CanDoAction = true;
    }

    public void AssignChampionModel(Champion championModel)
    {
        _champion = championModel;
    }

    public void Respawn()
    {
        IsDead = false;
        CanDoAction = true;
        Health = _champion.MaxHealth;
        transform.position = StartPosition;
        gameObject.SetActive(true);
    }
}
