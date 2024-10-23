using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireMageObject : ChampionObject
{
    private const float SpacingThreshold = 0.1f;

    public override void HandleAttackCooldown(List<KeyValuePair<ChampionObject,float>> enemiesWithDistance)
    {
        if (AttackRange < enemiesWithDistance[0].Value)
        {
            MoveTowardsTarget(enemiesWithDistance[0].Key);
            return;
        }

        if (enemiesWithDistance[0].Value >= AttackRange - SpacingThreshold) return;
        
        foreach (var enemy in enemiesWithDistance)
        {
            if (enemy.Key.AttackRange >= enemy.Value)
            {
                MoveAwayFromTarget(enemy.Key);
                return;
            }
        }
    }
}
