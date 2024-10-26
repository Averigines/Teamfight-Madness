using System.Collections.Generic;
using UnityEngine;

namespace GameModel
{
    public abstract class Champion
    {
        public int MaxHealth { get; private set; }
        public int Speed { get; private set; }
        public int AttackRange { get; private set; }
        public int AttackDamage { get; private set; }
        
        // Number represents how many times a character can attack every second
        public float AttackSpeed { get; private set; }
        public float AttackCooldown { get; private set; }

        protected Champion(int maxHealth, int speed, int attackRange, int attackDamage, float attackSpeed, float attackCooldown)
        {
            MaxHealth = maxHealth;
            Speed = speed;
            AttackRange = attackRange;
            AttackDamage = attackDamage;
            AttackSpeed = attackSpeed;
            AttackCooldown = attackCooldown;
        }
    }

    public class FireMage : Champion
    {
        public FireMage(int maxHealth, int speed, int attackRange, int attackDamage, float attackSpeed, float attackCooldown) : base(
            maxHealth, speed, attackRange, attackDamage, attackSpeed, attackCooldown)
        {
            
        }
    }
    
    public class Warrior : Champion
    {
        public Warrior(int maxHealth, int speed, int attackRange, int attackDamage, float attackSpeed, float attackCooldown) : base(
            maxHealth, speed, attackRange, attackDamage, attackSpeed, attackCooldown)
        {
            
        }
    }
    
    public class TreeStump : Champion
    {
        public TreeStump(int maxHealth, int speed, int attackRange, int attackDamage, float attackSpeed, float attackCooldown) : base(
            maxHealth, speed, attackRange, attackDamage, attackSpeed, attackCooldown)
        {
            
        }
    }
}

