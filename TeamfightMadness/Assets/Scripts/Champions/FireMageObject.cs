using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireMageObject : ChampionObject
{
    private const float SpacingThreshold = 0.2f;
    [SerializeField] private GameObject attackProjectile;

    public override void HandleAttackCooldown()
    {
        var validTargets = controller.GetLivingChampionsOfTeam(OpponentTeam);
        if (validTargets.Count == 0) return;
        var target = controller.GetClosestChampion(this, validTargets);
        var distanceToTarget = controller.GetDistanceToChampion(this, target);
        
        if (AttackRange < distanceToTarget)
        {
            MoveTowardsTarget(target);
            return;
        }
        
        if (distanceToTarget >= AttackRange - SpacingThreshold) return;
        
        if (target.AttackRange >= distanceToTarget) MoveAwayFromTarget(target);
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
                SpawnProjectile(transform.position, directionToEnemy);
            }));
        }
        else
        {
            MoveTowardsTarget(target);
        }
    }

    private void SpawnProjectile(Vector3 spawnPosition, Vector3 targetDirection)
    {
        GameObject noob = Instantiate(attackProjectile, spawnPosition, Quaternion.identity);
        var obj = noob.GetComponent<Projectile>();
        obj.Initialize(targetDirection, OpponentTeam, AttackDamage);
    }
}
