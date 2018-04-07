using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Unit : MonoBehaviour {

    public UnitSO stats;
    public GameObject LevelUpDisplay;
    public GameObject levelUpTextObject;
    public GameObject levelUpParticles;
    public GameObject damageText;
    public bool playerUnit;
    public bool maySpawn = false;
    public GameObject[] deathParticles = new GameObject[2];

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
    private int points;

    private AttackMove weapon;

    private System.Random rnd = new System.Random();

    private bool hasBeenInitialized;

    private GameObject levelUpScreen;

    public bool HasMoved { get; set; }
    public bool HasAttacked { get; set; }

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
    public int Points { get { return points; } }
    public AttackMove Weapon { get { return weapon; } set { weapon = value; } }

    private void Awake()
    {
        if (!Globals.initialSpawn || !playerUnit)
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
        }
        else
        {
            unitName = PlayerPrefs.GetString(stats.Name + "_name");
            _class = PlayerPrefs.GetString(stats.Name + "_class");
            level = PlayerPrefs.GetInt(stats.Name + "_level");
            experience = PlayerPrefs.GetInt(stats.Name + "_experience");
            health = PlayerPrefs.GetInt(stats.Name + "_health");
            currentHealth = PlayerPrefs.GetInt(stats.Name + "_currentHealth");
            strength = PlayerPrefs.GetInt(stats.Name + "_strength");
            magic = PlayerPrefs.GetInt(stats.Name + "_magic");
            defense = PlayerPrefs.GetInt(stats.Name + "_defense");
            resistance = PlayerPrefs.GetInt(stats.Name + "_resistance");
            speed = PlayerPrefs.GetInt(stats.Name + "_speed");
            luck = PlayerPrefs.GetInt(stats.Name + "_luck");
            skill = PlayerPrefs.GetInt(stats.Name + "_skill");
            maySpawn = PlayerPrefsX.GetBool(stats.Name + "_maySpawn");
        }

        movement = stats.Movement;
        points = stats.Points;

        weapon = stats.Attacks[0];

        hasBeenInitialized = true;
    }

    public void HealAfterVictory()
    {
        int heal = (int)Math.Round((decimal)health / (decimal)4);
        currentHealth += heal;

        if (currentHealth > health)
        {
            currentHealth = health;
        }
    }

    public void IncreaseExperience(int exp, CursorAction cursor)
    {
        experience += exp;

        if (experience >= 100)
        {
            experience -= 100;
            IncreaseLevel(cursor);
        }
    }

    private void IncreaseLevel(CursorAction cursor)
    {
        level++;
        List<string> grownStats = GrowStats();

        if (playerUnit)
        {
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
            StartCoroutine(DismissLevelUp(cursor));
        }


    }

    public IEnumerator DismissLevelUp(CursorAction cursor)
    {
        yield return new WaitForSeconds(3.75f);
        levelUpScreen.transform.GetChild(0).GetComponent<Animator>().SetBool("dismissed", true);
        yield return new WaitForSeconds(1.1f);
        Destroy(levelUpScreen);
    }

    public void TakeDamage(int damage, bool crit, GameObject cursor)
    {
        currentHealth -= damage;

        GameObject canvas = GetComponentInChildren<Canvas>().gameObject;
        GameObject text = Instantiate(damageText, canvas.transform);

        text.GetComponent<SetDamagePosition>().Set(transform.position);
        if (crit) text.GetComponentInChildren<Text>().text = "Crit! " + damage.ToString();
        else if (damage == 0) text.GetComponentInChildren<Text>().text = "Miss";
        else text.GetComponentInChildren<Text>().text = damage.ToString();


        if (currentHealth <= 0) // This means dead
        {
            currentHealth = 0;
            StartCoroutine(InitiateDeath(cursor));
        }
    }

    private IEnumerator InitiateDeath(GameObject cursor)
    {
        if (playerUnit)
        {
            cursor.GetComponent<PlayerSession>().playerUnits.Remove(gameObject);
            PlayerPrefsX.SetBool(stats.Name + "_maySpawn", false);
        }
        else
        {
            cursor.GetComponent<AIController>().enemyUnits.Remove(gameObject);
        }

        GetComponent<Movement>().currentTile.Unit = null;

        GetComponent<Animator>().SetBool("death", true);

        yield return new WaitForSecondsRealtime(2);
        GameObject smoke = Instantiate(deathParticles[0], transform);

        yield return new WaitForSecondsRealtime(.2f);
        GetComponentInChildren<SkinnedMeshRenderer>().enabled = false;

        var sh = deathParticles[1].GetComponent<ParticleSystem>().shape;
        sh.skinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        GameObject red = Instantiate(deathParticles[1], transform);

        yield return new WaitForSecondsRealtime(4.5f);
        Destroy(gameObject);
    }

    public List<string> GrowStats()
    {
        List<string> grownStats = new List<string>();

        while (grownStats.Count == 0)
        {
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
        }

        return grownStats;
    }

}
