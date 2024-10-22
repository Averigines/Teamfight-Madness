using System;
using System.Collections;
using System.Collections.Generic;
using GameModel;
using UnityEngine;

public class ChampionObject : MonoBehaviour
{
    [SerializeField] private HealthBar healthBar;
    
    private Champion _champion;
    private Animator _animator;
    private static readonly int AttackAnim = Animator.StringToHash("Attack");
    private static readonly int AttackAnimDuration = Animator.StringToHash("AttackDuration");

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

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    void Start()
    {
        Health = _champion.MaxHealth;
        healthBar.Initialize(_champion.MaxHealth);
        healthBar.UpdateHealthBar(Health);
        Speed = _champion.Speed;
        AttackRange = _champion.AttackRange;
        AttackDamage = _champion.AttackDamage;
        AttackSpeed = _champion.AttackSpeed;
        CanDoAction = true;
        StartPosition = transform.position;
        IsDead = false;
    }

    public void LoseHealth(int health)
    {
        Health -= health;
        healthBar.UpdateHealthBar(Health);

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

    public IEnumerator Attack(Action onAttackComplete)
    {
        CanDoAction = false;

        float duration = 1 / AttackSpeed;
        _animator.SetFloat(AttackAnimDuration, AttackSpeed);
        _animator.SetBool(AttackAnim, true);

        yield return new WaitForSeconds(duration);

        onAttackComplete?.Invoke();
        _animator.SetBool(AttackAnim, false);
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
        healthBar.UpdateHealthBar(Health);
        transform.position = StartPosition;
        gameObject.SetActive(true);
    }
}

public class FireMageObject : ChampionObject
{
    
}

public class WarriorObject : ChampionObject
{
    
}
