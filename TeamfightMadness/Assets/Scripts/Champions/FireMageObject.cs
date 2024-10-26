using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class FireMageObject : ChampionObject
{
    private const float SpacingThreshold = 0.2f;
    [SerializeField] private GameObject attackProjectile;
    private ChampionObject _currentAttackTarget;

    public override void HandleAttackCooldown()
    {
        var validTargets = Controller.GetLivingChampionsOfTeam(OpponentTeam);
        if (validTargets.Count == 0) return;
        var target = Controller.GetClosestChampion(this, validTargets);
        var distanceToTarget = Controller.GetDistanceToChampion(this, target);
        
        if (AttackRange < distanceToTarget)
        {
            MoveTowardsTarget(target);
            return;
        }
        
        if (distanceToTarget >= AttackRange - SpacingThreshold) return;
        
        else MoveAwayFromTarget(target);
        //if (target.AttackRange >= distanceToTarget) MoveAwayFromTarget(target);
    }
    
    public override void HandleReadyState()
    {
        var validTargets = Controller.GetLivingChampionsOfTeam(OpponentTeam);
        if (validTargets.Count == 0) return;
        var target = Controller.GetClosestChampion(this, validTargets);
        var distanceToTarget = Controller.GetDistanceToChampion(this, target);
        
        if (AttackRange >= distanceToTarget)
        {
            var enemyPos = target.transform.position;
            var directionToEnemy = (enemyPos - transform.position).normalized;
            StartAttack(target, directionToEnemy);
        }
        else
        {
            MoveTowardsTarget(target);
        }
    }

    private void StartAttack(ChampionObject target, Vector3 directionToEnemy)
    {
        CheckForFlip(directionToEnemy);
        
        CurrentState = ChampionState.Attacking;
        _currentAttackTarget = target;
        
        Animator.SetFloat(AttackAnimDuration, AttackSpeed);
        Animator.SetBool(AttackAnim, true);
    }

    [UsedImplicitly]
    public void FinishAttack()
    {
        var directionToEnemy = (_currentAttackTarget.transform.position - transform.position).normalized;
        SpawnProjectile(transform.position, directionToEnemy);
        Animator.SetBool(AttackAnim, false);
        StartCoroutine(AttackCooldownCoroutine());
    }

    private void SpawnProjectile(Vector3 spawnPosition, Vector3 targetDirection)
    {
        GameObject noob = Instantiate(attackProjectile, spawnPosition, Quaternion.identity);
        var obj = noob.GetComponent<Projectile>();
        obj.Initialize(targetDirection, OpponentTeam, AttackDamage);
    }
}
