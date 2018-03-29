using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Unit", menuName = "Units/New Unit")]
public class UnitSO : ScriptableObject
{
    [SerializeField]
    private string unitName;
    [SerializeField]
    private int level;
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

    public string Name { get { return unitName; } }
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
    public int[] Growth { get { return growthPercentage; } }
}

public enum AttackType
{
    Physical,
    Magic
}
