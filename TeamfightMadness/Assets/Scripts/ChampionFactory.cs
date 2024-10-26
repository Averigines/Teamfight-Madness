using System;
using System.Collections;
using System.Collections.Generic;
using GameModel;
using UnityEngine;

public class ChampionFactory : MonoBehaviour
{
    [SerializeField] private GameObject fireMagePrefab;
    [SerializeField] private GameObject warriorPrefab;
    [SerializeField] private GameObject treeStumpPrefab;
    
    public static Champion CreateChampion(GameController.ChampionEntry entry)
    {
        switch (entry.name)
        {
            case ChampionName.FireMage:
                return new FireMage(entry.health, entry.speed, entry.attackRange, entry.attackDamage, entry.attackSpeed, entry.attackCooldown);
            case ChampionName.Warrior:
                return new Warrior(entry.health, entry.speed, entry.attackRange, entry.attackDamage, entry.attackSpeed, entry.attackCooldown);
            case ChampionName.TreeStump:
                return new TreeStump(entry.health, entry.speed, entry.attackRange, entry.attackDamage, entry.attackSpeed, entry.attackCooldown);
            default:
                throw new ArgumentException($"Unknown champion type: {entry.name}");
        }
    }

    public ChampionObject CreateChampionInBattle(Champion entry, Vector3 spawnPosition)
    {
        GameObject go = null;
        switch (entry)
        {
            case FireMage:
                go = Instantiate(fireMagePrefab, spawnPosition, Quaternion.identity);
                break;
            case Warrior:
                go = Instantiate(warriorPrefab, spawnPosition, Quaternion.identity);
                break;
            case TreeStump:
                go = Instantiate(treeStumpPrefab, spawnPosition, Quaternion.identity);
                break;
            default:
                throw new ArgumentException($"Unknown champion: {entry}");
        }

        ChampionObject obj = go.GetComponent<ChampionObject>();
        return obj;
    }
}
