using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarriorObject : ChampionObject
{
    public override void HandleAttackCooldown()
    {
        var validTargets = controller.GetLivingChampionsOfTeam(OpponentTeam);
        if (validTargets.Count == 0) return;
        var target = controller.GetClosestChampion(this, validTargets);
        var distanceToTarget = controller.GetDistanceToChampion(this, target);
        if (distanceToTarget < AttackRange / 2f) return;
        MoveTowardsTarget(target);
    }

    public override void HandleReadyState()
    {
        var validTargets = controller.GetLivingChampionsOfTeam(OpponentTeam);
        if (validTargets.Count == 0) return;
        var target = controller.GetClosestChampion(this, validTargets);
        var distanceToTarget = controller.GetDistanceToChampion(this, target);
        
        if (AttackRange >= distanceToTarget)
        {
            var enemyPos = target.transform.position;
            var directionToEnemy = (enemyPos - transform.position).normalized;
            StartCoroutine(AttackCoroutine(directionToEnemy,() =>
            {
                target.LoseHealth(AttackDamage);
            }));
        }
        else
        {
            MoveTowardsTarget(target);
        }
    }
}