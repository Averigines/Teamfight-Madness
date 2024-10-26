using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vector2 = System.Numerics.Vector2;

public class Projectile : MonoBehaviour
{
    private Vector3 _startPosition;
    private Vector3 _targetDirection;
    private Team _targetTeam;
    private int _damage;

    [SerializeField] private int speed;
    [SerializeField] private int range;

    public void Initialize(Vector3 targetDirection, Team opponentTeam, int damage)
    {
        _targetDirection = targetDirection;
        _targetTeam = opponentTeam;
        _startPosition = transform.position;
        _damage = damage;
    }
    
    void Update()
    {
        transform.position += _targetDirection * speed * Time.deltaTime;

        if (Vector3.Distance(transform.position, _startPosition) >= range)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (collision.TryGetComponent<ChampionObject>(out ChampionObject champ))
            {
                if (champ.Team == _targetTeam)
                {
                    champ.LoseHealth(_damage);
                    Destroy(gameObject);
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Arena"))
        {
            Destroy(gameObject);
        }
    }
}
