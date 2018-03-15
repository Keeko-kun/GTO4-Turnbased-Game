using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Unit", menuName = "Units/New Unit")]
public class UnitSO : ScriptableObject
{
    public string unitName;
    [SerializeField]
    private int level = 1;
    [SerializeField]
    private int experience;
    [SerializeField]
    private int health;
    private int currentHealth;
    [SerializeField]
    private int strength;
    [SerializeField]
    private int magic;
    [SerializeField]
    private int defense;
    [SerializeField]
    private int resistance;
    [SerializeField]
    private int speed;
    [SerializeField]
    private int luck;
    [SerializeField]
    private int skill;
    [SerializeField]
    private int movement;
    [SerializeField]
    private int[] growthPercentage = new int[8];

    private System.Random rnd = new System.Random();

    public void IncreaseExperience(int exp)
    {
        experience += exp;

        if (experience >= 100)
        {
            experience -= 100;
            IncreaseLevel();
        }
    }

    private void IncreaseLevel()
    {
        level++;

        bool[] determineGrowth = new bool[8];

        for (int i = 0; i < growthPercentage.Length; i++)
        {
            int randomNumber = rnd.Next(0, 100);
            if (randomNumber <= growthPercentage[i])
            {
                determineGrowth[i] = true;
            }
            else
            {
                determineGrowth[i] = false;
            }
        }

        if (determineGrowth[0])
        {
            health += rnd.Next(1, 3);
        }
        if (determineGrowth[1])
        {
            strength += rnd.Next(1, 4);
        }
        if (determineGrowth[2])
        {
            magic += rnd.Next(1, 4);
        }
        if (determineGrowth[3])
        {
            defense += rnd.Next(1, 3);
        }
        if (determineGrowth[4])
        {
            resistance += rnd.Next(1, 3);
        }
        if (determineGrowth[5])
        {
            speed += rnd.Next(1, 5);
        }
        if (determineGrowth[6])
        {
            luck += rnd.Next(1, 4);
        }
        if (determineGrowth[7])
        {
            skill += rnd.Next(1, 4);
        }
    }

    public void TakeDamage(AttackType type, int opponentAttack)
    {
        int damage = 0;

        switch (type)
        {
            case AttackType.Physical:
                damage = opponentAttack - defense;
                break;
            case AttackType.Magic:
                damage = opponentAttack - resistance;
                break;
        }

        if (damage <= 0)
        {
            damage = 1;
        }

        currentHealth--;
    }

    public void PerformAttack(UnitSO opponent, AttackType type)
    {
        int damage = 0;

        switch (type)
        {
            case AttackType.Physical:
                damage = strength;
                break;
            case AttackType.Magic:
                damage = magic;
                break;
        }

        int randomNumber = rnd.Next(0, 100);
        if (randomNumber <= skill)
        {
            damage = damage * 2;
        }

        opponent.TakeDamage(type, damage);
    }
}

public enum AttackType
{
    Physical,
    Magic
}
