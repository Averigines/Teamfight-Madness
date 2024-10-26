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
    protected Animator Animator;
    private SpriteRenderer _renderer;
    private Rigidbody2D _rb;
    private PolygonCollider2D _arenaArea;
    protected BattleController Controller;
    public Team Team { private set; get; }
    [NonSerialized] protected Team OpponentTeam;
    
    protected static readonly int AttackAnim = Animator.StringToHash("Attack");
    protected static readonly int AttackAnimDuration = Animator.StringToHash("AttackDuration");

    private int _health;
    private int _speed;
    public int AttackRange { get; private set; }
    protected int AttackDamage;
    protected float AttackSpeed;
    private float _attackCooldown;
    [NonSerialized] public ChampionState CurrentState;
    private Vector3 _startPosition;

    private Vector3 _targetDirection;
    private bool _needsToMove;
    
    public delegate void OnDeath(ChampionObject champion);
    public event OnDeath onDeath;

    private void Awake()
    {
        Animator = GetComponent<Animator>();
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
        AttackSpeed = _champion.AttackSpeed;
        _attackCooldown = _champion.AttackCooldown;
        CurrentState = ChampionState.Ready;
        _startPosition = transform.position;
        OpponentTeam = Team == Team.Blue ? Team.Red : Team.Blue;

        _needsToMove = false;
    }

    public void Initialize(BattleController controller, Team team, Champion champion, GameObject arena)
    {
        Controller = controller;
        Team = team;
        _champion = champion;
        _arenaArea = arena.GetComponent<PolygonCollider2D>();
    }

    private void FixedUpdate()
    {
        if (_needsToMove && CurrentState != ChampionState.Attacking)
        {
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
        Animator.SetBool(AttackAnim, false);
        gameObject.SetActive(false);
        StopAllCoroutines();
        onDeath?.Invoke(this);
    }

    private void MoveInTargetDirection()
    {
        CheckForFlip(_targetDirection);
        var newPosition = transform.position + _targetDirection * _speed * Time.fixedDeltaTime;
        
        if (_arenaArea.OverlapPoint(newPosition)) _rb.MovePosition(newPosition);
        _needsToMove = false;
    }

    protected void CheckForFlip(Vector2 direction)
    {
        bool shouldFlip = direction.x > 0;
        if (_renderer.flipX != shouldFlip) _renderer.flipX = shouldFlip;
    }

    protected IEnumerator AttackCooldownCoroutine()
    {
        CurrentState = ChampionState.AttackCooldown;
        yield return new WaitForSeconds(_attackCooldown);
        CurrentState = ChampionState.Ready;
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
