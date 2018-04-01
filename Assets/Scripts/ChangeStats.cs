using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeStats : MonoBehaviour {

    public Text unitName;
    public Text _class;
    public Text level;
    public Text experience;
    public Text currentHP;
    public Text maxHp;
    public Text strength;
    public Text magic;
    public Text defense;
    public Text resistance;
    public Text speed;
    public Text luck;
    public Text skill;
    public Text movement;

    public void UpdateUI(Unit unit)
    {
        unitName.text = unit.Name;
        _class.text = unit.Class;
        level.text = unit.Level.ToString();
        experience.text = unit.Experience.ToString();
        currentHP.text = unit.CurrentHealth.ToString();
        maxHp.text = unit.Health.ToString();
        strength.text = unit.Strength.ToString();
        magic.text = unit.Magic.ToString();
        defense.text = unit.Defense.ToString();
        resistance.text = unit.Resistance.ToString();
        speed.text = unit.Speed.ToString();
        luck.text = unit.Luck.ToString();
        skill.text = unit.Skill.ToString();
        movement.text = unit.Movement.ToString();
    }
}
