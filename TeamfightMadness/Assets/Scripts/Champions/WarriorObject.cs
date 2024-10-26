using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class WarriorObject : ChampionObject
{
    private ChampionObject _currentAttackTarget;

    public override void HandleAttackCooldown()
    {
        var validTargets = Controller.GetLivingChampionsOfTeam(OpponentTeam);
        if (validTargets.Count == 0) return;
        var target = Controller.GetClosestChampion(this, validTargets);
        var distanceToTarget = Controller.GetDistanceToChampion(this, target);
        if (distanceToTarget < AttackRange / 2f) return;
        MoveTowardsTarget(target);
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

        float duration = 1 / AttackSpeed;
        Animator.SetFloat(AttackAnimDuration, AttackSpeed);
        Animator.SetBool(AttackAnim, true);
    }

    [UsedImplicitly]
    public void HitEnemy()
    {
        _currentAttackTarget.LoseHealth(AttackDamage);
    }
    
    [UsedImplicitly]
    public void FinishAttack()
    {
        Animator.SetBool(AttackAnim, false);
        StartCoroutine(AttackCooldownCoroutine());
    }
}