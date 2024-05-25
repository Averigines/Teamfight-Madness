using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GameModel
{
    public class Champion
    {
        public Vector2 Position;
        public int Health;
        public float Speed;
        public float AttackRange;
        public int DamagePerAttack;

        public delegate void OnAttack(Champion attackedChampion);

        public event OnAttack onAttack;

        public Champion(Vector2 position, int health, float speed, float attackRange, int damagePerAttack)
        {
            Position = position;
            Health = health;
            Speed = speed;
            AttackRange = attackRange;
            DamagePerAttack = damagePerAttack;
        }

        public void Attack(Champion championToAttack)
        {
            onAttack?.Invoke(championToAttack);
        }

        public void LoseHealth(int damage)
        {
            Health -= damage;
        }

        public void MoveTowardsChampions(Champion closestChampion)
        {
            //Implement
        }
    }

    public static class Champions
    {
        public static List<Champion> AllChampions { get; private set; }

        static Champions()
        {
            AllChampions = new List<Champion>();
        }

        public static void CreateChampion(Vector2 startPosition, int health, float speed, float attackRange, int damagePerAttack)
        {
            Champion champ = new Champion(startPosition, health, speed, attackRange, damagePerAttack);
            AllChampions.Add(champ);
        }

        public static void DecideOnActions()
        {
            foreach (var champion in AllChampions)
            {
                DecideOnActionOfChampion(champion);
            }
        }

        private static void DecideOnActionOfChampion(Champion champion)
        {
            Champion closestChampion = GetClosestChampionFromChampion(champion);
            float distanceToChampion = DistanceBetweenChampions(champion, closestChampion);

            if (champion.AttackRange <= distanceToChampion)
            {
                champion.Attack(closestChampion);
                closestChampion.LoseHealth(champion.DamagePerAttack);
            }
            else
            {
                champion.MoveTowardsChampions(closestChampion);
            }
        }

        private static Champion GetClosestChampionFromChampion(Champion champion)
        {
            List<Champion> otherChampions = GetAllChampionsButOne(champion);

            Champion closestChampion = null;
            float currentlyClosestDistance = float.MaxValue;
            foreach (var champ in otherChampions)
            {
                var distance = DistanceBetweenChampions(champion, champ);
                if (distance < currentlyClosestDistance)
                {
                    closestChampion = champ;
                    currentlyClosestDistance = distance;
                }
            }

            return closestChampion;
        }

        private static List<Champion> GetAllChampionsButOne(Champion champion)
        {
            List<Champion> champions = new List<Champion>();
            champions.AddRange(AllChampions);
            champions.Remove(champion);

            return champions;
        }
        
        private static bool CanChampionAttackOtherChampion(Champion thisChampion, Champion otherChampion)
        {
            return DistanceBetweenChampions(thisChampion, otherChampion) <= thisChampion.AttackRange;
        }

        private static float DistanceBetweenChampions(Champion champion1, Champion champion2)
        {
            float distance = Vector2.Distance(champion1.Position, champion2.Position);
            return distance;
        }
    }
}

