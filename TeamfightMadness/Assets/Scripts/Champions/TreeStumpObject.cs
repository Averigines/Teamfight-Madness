using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeStumpObject : ChampionObject
{
    [SerializeField] private GameObject attackObject;
    public override void HandleReadyState()
    {
        StartAttack();
    }

    public override void HandleAttackCooldown()
    {
        
    }

    private void StartAttack()
    {
        CurrentState = ChampionState.Attacking;
        Animator.SetFloat(AttackAnimDuration, AttackSpeed);
        Animator.SetBool(AttackAnim, true);
    }

    public void FinishAttack()
    {
        SpawnAttackObject(transform.position);
        var validTargets = Controller.GetChampionsOfTeam(Team, true);
        foreach (var champ in validTargets)
        {
            if (Controller.GetDistanceToChampion(this, champ) <= AttackRange) champ.GainHealth(AttackDamage);
            Animator.SetBool(AttackAnim, false);
            StartCoroutine(AttackCooldownCoroutine());
        }
    }

    private void SpawnAttackObject(Vector3 centerPosition)
    {
        var noob = Instantiate(attackObject, centerPosition, Quaternion.identity);
        noob.transform.localScale = new Vector2(AttackRange * 2, AttackRange * 2);
    }
}
