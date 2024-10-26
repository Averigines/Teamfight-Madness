using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GameModel;
using Unity.VisualScripting;
using UnityEngine;

public class ChampionObject : MonoBehaviour
{
    [SerializeField] private HealthBar healthBar;
    
    private Champion _champion;
    private Animator _animator;
    private SpriteRenderer _renderer;
    private Rigidbody2D _rb;
    private static readonly int AttackAnim = Animator.StringToHash("Attack");
    private static readonly int AttackAnimDuration = Animator.StringToHash("AttackDuration");

    public int Health { get; private set; }
    public int Speed { get; private set; }
    public int AttackRange { get; private set; }
    public int AttackDamage { get; private set; }
    public float AttackSpeed { get; private set; }
    public float AttackCooldown { get; private set; }
    public ChampionState CurrentState { get; private set; }
    public Vector3 StartPosition { get; private set; }

    private Vector3 _targetDirection;
    private bool _needsToMove;
    
    public delegate void OnDeath(ChampionObject champion);
    public event OnDeath onDeath;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _renderer = GetComponent<SpriteRenderer>();
        _rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        Health = _champion.MaxHealth;
        healthBar.Initialize(_champion.MaxHealth, _renderer);
        healthBar.UpdateHealthBar(Health);
        Speed = _champion.Speed;
        AttackRange = _champion.AttackRange;
        AttackDamage = _champion.AttackDamage;
        AttackSpeed = _champion.AttackSpeed;
        AttackCooldown = _champion.AttackCooldown;
        CurrentState = ChampionState.Ready;
        StartPosition = transform.position;

        _needsToMove = false;
    }

    private void FixedUpdate()
    {
        if (_needsToMove && CurrentState != ChampionState.Attacking)
        {
            print($"{_champion.GetType().Name} is moving!");
            MoveInTargetDirection();
        }
    }

    private void LoseHealth(int health)
    {
        Health -= health;
        healthBar.UpdateHealthBar(Health);

        if (Health <= 0) HandleDeath();
    }

    private void HandleDeath()
    {
        CurrentState = ChampionState.Dead;
        _animator.SetBool(AttackAnim, false);
        gameObject.SetActive(false);
        StopAllCoroutines();
        onDeath?.Invoke(this);
    }

    private void MoveInTargetDirection()
    {
        CheckForFlip(_targetDirection);
        var newPosition = transform.position + _targetDirection * Speed * Time.fixedDeltaTime;
        _rb.MovePosition(newPosition);
        _needsToMove = false;
    }

    private void CheckForFlip(Vector2 direction)
    {
        bool shouldFlip = direction.x > 0;
        if (_renderer.flipX != shouldFlip) _renderer.flipX = shouldFlip;
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Arena"))
        {
        }
        if (collision.CompareTag("Player"))
        {
            print("CollidedWithChamp");
        }
    }
    
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Arena"))
        {
        }
        if (collision.CompareTag("Player"))
        {
            print("CollidedWithChamp");
        }
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
        CurrentState = ChampionState.Ready;
        Health = _champion.MaxHealth;
        healthBar.UpdateHealthBar(Health);
        transform.position = StartPosition;
        gameObject.SetActive(true);
    }

    public virtual void HandleAttackCooldown(List<KeyValuePair<ChampionObject,float>> enemiesWithDistance)
    {
        
    }
    
    protected void MoveTowardsTarget(ChampionObject target)
    {
        var targetPos = target.transform.position;
        _targetDirection = (targetPos - transform.position).normalized;
        _needsToMove = true;
    }
    
    protected void MoveAwayFromTarget(ChampionObject target)
    {
        var targetPos = target.transform.position;
        _targetDirection = (transform.position - targetPos).normalized;
        _needsToMove = true;
    }

    public void HandleReadyState(KeyValuePair<ChampionObject, float> closestEnemyWithDistance)
    {
        if (AttackRange >= closestEnemyWithDistance.Value)
        {
            var enemyPos = closestEnemyWithDistance.Key.transform.position;
            var directionToEnemy = (enemyPos - transform.position).normalized;
            StartCoroutine(AttackCoroutine(directionToEnemy,() =>
            {
                closestEnemyWithDistance.Key.LoseHealth(AttackDamage);
            }));
        }
        else
        {
            MoveTowardsTarget(closestEnemyWithDistance.Key);
        }
    }
}
