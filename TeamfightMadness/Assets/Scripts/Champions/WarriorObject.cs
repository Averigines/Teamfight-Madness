using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarriorObject : ChampionObject
{
    public override void HandleAttackCooldown(List<KeyValuePair<ChampionObject,float>> enemiesWithDistance)
    {
        if (enemiesWithDistance[0].Value < 0.1) return;
        MoveTowardsTarget(enemiesWithDistance[0].Key);
    }
}