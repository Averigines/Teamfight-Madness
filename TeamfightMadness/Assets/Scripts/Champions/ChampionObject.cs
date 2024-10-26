using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GameModel;
using Unity.VisualScripting;
using UnityEngine;

public abstract class ChampionObject : MonoBehaviour
{
    [SerializeField] private HealthBar healthBar;
    
    private Champion _champion;
    private Animator _animator;
    private SpriteRenderer _renderer;
    private Rigidbody2D _rb;

    [NonSerialized] public BattleController controller;
    [NonSerialized] public Team Team;
    [NonSerialized] protected Team OpponentTeam;
    
    private static readonly int AttackAnim = Animator.StringToHash("Attack");
    private static readonly int AttackAnimDuration = Animator.StringToHash("AttackDuration");

    private int _health;
    private int _speed;
    public int AttackRange { get; private set; }
    protected int AttackDamage;
    private float _attackSpeed;
    private float _attackCooldown;
    public ChampionState CurrentState { get; private set; }
    private Vector3 _startPosition;

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
        _health = _champion.MaxHealth;
        healthBar.Initialize(_champion.MaxHealth, _renderer);
        healthBar.UpdateHealthBar(_health);
        _speed = _champion.Speed;
        AttackRange = _champion.AttackRange;
        AttackDamage = _champion.AttackDamage;
        _attackSpeed = _champion.AttackSpeed;
        _attackCooldown = _champion.AttackCooldown;
        CurrentState = ChampionState.Ready;
        _startPosition = transform.position;
        OpponentTeam = Team == Team.Blue ? Team.Red : Team.Blue;

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

    public void LoseHealth(int health)
    {
        _health -= health;
        healthBar.UpdateHealthBar(_health);

        if (_health <= 0) HandleDeath();
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
        var newPosition = transform.position + _targetDirection * _speed * Time.fixedDeltaTime;
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
            //print("CollidedWithChamp");
        }
    }
    
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Arena"))
        {
        }
        if (collision.CompareTag("Player"))
        {
            //print("CollidedWithChamp");
        }
    }

    public IEnumerator AttackCoroutine(Vector3 direction, Action onAttackComplete)
    {
        CheckForFlip(direction);
        
        CurrentState = ChampionState.Attacking;

        float duration = 1 / _attackSpeed;
        _animator.SetFloat(AttackAnimDuration, _attackSpeed);
        _animator.SetBool(AttackAnim, true);

        yield return new WaitForSeconds(duration);

        onAttackComplete?.Invoke();
        _animator.SetBool(AttackAnim, false);
        
        StartCoroutine(AttackCooldownCoroutine());
    }

    private IEnumerator AttackCooldownCoroutine()
    {
        CurrentState = ChampionState.AttackCooldown;
        yield return new WaitForSeconds(_attackCooldown);
        CurrentState = ChampionState.Ready;
    }

    public void AssignChampionModel(Champion championModel)
    {
        _champion = championModel;
    }

    public void Respawn()
    {
        CurrentState = ChampionState.Ready;
        _health = _champion.MaxHealth;
        healthBar.UpdateHealthBar(_health);
        transform.position = _startPosition;
        gameObject.SetActive(true);
    }

    public abstract void HandleReadyState();
    public abstract void HandleAttackCooldown();

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

    
}
