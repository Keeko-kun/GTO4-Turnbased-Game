﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Unit : MonoBehaviour {

    public UnitSO stats;
    public GameObject LevelUpDisplay;
    public GameObject levelUpTextObject;
    public GameObject levelUpParticles;
    public bool playerUnit;

    private string unitName;
    private string _class;
    private int level;
    private int experience;
    private int health;
    private int currentHealth;
    private int strength;
    private int magic;
    private int defense;
    private int resistance;
    private int speed;
    private int luck;
    private int skill;
    private int movement;
    private int range;

    private System.Random rnd = new System.Random();

    private bool hasBeenInitialized;

    private GameObject levelUpScreen;

    public string Name { get { return unitName; } }
    public string Class { get { return _class; } }
    public int Level { get { return level; } }
    public int Experience { get { return experience; } }
    public int Health { get { return health; } }
    public int CurrentHealth { get { return currentHealth; } }
    public int Strength { get { return strength; } }
    public int Magic { get { return magic; } }
    public int Defense { get { return defense; } }
    public int Resistance { get { return resistance; } }
    public int Speed { get { return speed; } }
    public int Luck { get { return luck; } }
    public int Skill { get { return skill; } }
    public int Movement { get { return movement; } }
    public int Range { get { return range; } }

    private void Start()
    {
        if (!hasBeenInitialized)
        {
            unitName = stats.Name;
            _class = stats.Class;
            level = stats.Level;
            experience = stats.Experience;
            health = stats.Health;
            currentHealth = health;
            strength = stats.Strength;
            magic = stats.Magic;
            defense = stats.Defense;
            resistance = stats.Resistance;
            speed = stats.Speed;
            luck = stats.Luck;
            skill = stats.Skill;
            movement = stats.Movement;
            range = stats.Range;
        }

        hasBeenInitialized = true;
    }

    public void IncreaseExperience(int exp, CursorAction cursor)
    {
        experience += exp;

        if (experience >= 100)
        {
            cursor.mode = ActionMode.LevelUp;
            experience -= 100;
            IncreaseLevel();
        }
    }

    private void IncreaseLevel()
    {
        level++;

        bool[] determineGrowth = new bool[8];

        for (int i = 0; i < stats.Growth.Length; i++)
        {
            int randomNumber = rnd.Next(0, 100);
            if (randomNumber <= stats.Growth[i])
            {
                determineGrowth[i] = true;
            }
            else
            {
                determineGrowth[i] = false;
            }
        }

        List<string> grownStats = new List<string>();

        if (determineGrowth[0])
        {
            health++;
            currentHealth++;
            grownStats.Add("HP");
        }
        if (determineGrowth[1])
        {
            strength++;
            grownStats.Add("Strength");
        }
        if (determineGrowth[2])
        {
            magic++;
            grownStats.Add("Magic");
        }
        if (determineGrowth[3])
        {
            defense++;
            grownStats.Add("Defense");
        }
        if (determineGrowth[4])
        {
            resistance++;
            grownStats.Add("Resistance");
        }
        if (determineGrowth[5])
        {
            speed++;
            grownStats.Add("Speed");
        }
        if (determineGrowth[6])
        {
            luck++;
            grownStats.Add("Luck");
        }
        if (determineGrowth[7])
        {
            skill++;
            grownStats.Add("Skill");
        }

        levelUpScreen = Instantiate(LevelUpDisplay);
        Instantiate(levelUpParticles, gameObject.transform);

        GameObject panel = levelUpScreen.GetComponent<Transform>().GetChild(0).GetChild(1).GetChild(0).gameObject;
        GameObject allStats = panel.transform.parent.gameObject;

        GameObject unitNameT = Instantiate(levelUpTextObject, allStats.transform);
        unitNameT.GetComponent<Text>().text = unitName + " - Lv " + level;

        foreach (string stat in grownStats)
        {
            GameObject text = Instantiate(levelUpTextObject, panel.transform);
            text.GetComponent<Text>().text = stat + " + 1";
        }
    }

    public IEnumerator DismissLevelUp(CursorAction cursor)
    {
        levelUpScreen.transform.GetChild(0).GetComponent<Animator>().SetBool("dismissed", true);
        yield return new WaitForSeconds(1.1f);
        Destroy(levelUpScreen);
        cursor.ResetToSelectTile();
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

    public void PerformAttack(Unit opponent, AttackType type)
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
