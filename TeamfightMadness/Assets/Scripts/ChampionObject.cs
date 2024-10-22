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
    private SpriteRenderer _renderer;
    private static readonly int AttackAnim = Animator.StringToHash("Attack");
    private static readonly int AttackAnimDuration = Animator.StringToHash("AttackDuration");

    public int Health { get; private set; }
    public int Speed { get; private set; }
    public int AttackRange { get; private set; }
    public int AttackDamage { get; private set; }
    public float AttackSpeed { get; private set; }
    public float AttackCooldown { get; private set; }
    public ChampionState CurrentState { get; private set; }
    private ChampionState _previousState;
    public Vector3 StartPosition { get; private set; }
    public bool IsDead { get; private set; }
    
    public delegate void OnDeath(ChampionObject champion);
    public event OnDeath onDeath;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _renderer = GetComponent<SpriteRenderer>();
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
        AttackCooldown = _champion.AttackCooldown;
        CurrentState = ChampionState.Ready;
        _previousState = ChampionState.Ready;
        StartPosition = transform.position;
        IsDead = false;
    }

    void Update()
    {
        if (_previousState == CurrentState) return;

        if (CurrentState == ChampionState.AttackCooldown)
        {
            
        }

        _previousState = CurrentState;
    }

    public void LoseHealth(int health)
    {
        Health -= health;
        healthBar.UpdateHealthBar(Health);

        if (Health <= 0)
        {
            IsDead = true;
            CurrentState = ChampionState.Dead;
            gameObject.SetActive(false);
            StopAllCoroutines();
            onDeath?.Invoke(this);
        }
    }
    
    public void MoveInDirection(Vector3 direction)
    {
        CheckForFlip(direction);
        Vector3 newPosition = transform.position + direction * Speed * Time.deltaTime;
        transform.position = newPosition;
    }

    private void CheckForFlip(Vector3 direction)
    {
        bool shouldFlip = direction.x > 0;
        if (_renderer.flipX != shouldFlip) _renderer.flipX = shouldFlip;
    }

    public IEnumerator AttackCoroutine(Vector3 direction, Action onAttackComplete)
    {
        CheckForFlip(direction);
        
        CurrentState = ChampionState.Attacking;

        float duration = 1 / AttackSpeed;
        _animator.SetFloat(AttackAnimDuration, AttackSpeed);
        _animator.SetBool(AttackAnim, true);

        yield return new WaitForSeconds(duration);

        onAttackComplete?.Invoke();
        _animator.SetBool(AttackAnim, false);
        
        StartCoroutine(AttackCooldownCoroutine());
    }

    private IEnumerator AttackCooldownCoroutine()
    {
        CurrentState = ChampionState.AttackCooldown;
        yield return new WaitForSeconds(AttackCooldown);
        CurrentState = ChampionState.Ready;
    }

    public void AssignChampionModel(Champion championModel)
    {
        _champion = championModel;
    }

    public void Respawn()
    {
        IsDead = false;
        CurrentState = ChampionState.Ready;
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
