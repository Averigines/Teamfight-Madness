using System.Collections.Generic;
using UnityEngine;

namespace GameModel
{
    public class Champion
    {
        public int MaxHealth { get; private set; }
        public int Speed { get; private set; }
        public int AttackRange { get; private set; }
        public int AttackDamage { get; private set; }
        
        // Number represents how many times a character can attack every 10 seconds
        public float AttackSpeed { get; private set; }

        public Champion(int maxHealth, int speed, int attackRange, int attackDamage, float attackSpeed)
        {
            MaxHealth = maxHealth;
            Speed = speed;
            AttackRange = attackRange;
            AttackDamage = attackDamage;
            AttackSpeed = attackSpeed;
        }
    }

    public static class Champions
    {
        public static List<Champion> AllChampions { get; private set; }

        static Champions()
        {
            AllChampions = new List<Champion>();
        }

        public static void CreateChampion(int maxHealth, int speed, int attackRange, int attackDamage, float attackSpeed)
        {
            Champion champ = new Champion(maxHealth, speed, attackRange, attackDamage, attackSpeed);
            AllChampions.Add(champ);
        }
    }
}

