using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangePrediction : MonoBehaviour {

    public Text unitAttacker;
    public Text unitDefender;
    public Text currentHPAttacker;
    public Text currentHPDefender;
    public Text maxHPAttacker;
    public Text maxHPDefender;
    public Text hitAttacker;
    public Text hitDefender;
    public Text critAttacker;
    public Text critDefender;
    public Text twiceAttacker;
    public Text twiceDefender;

    public void UpdateUI(Unit attacker, Unit defender, string currentHPAttacker, string currentHPDefender, string hitAttacker, string hitDefender, string critAttacker, string critDefender, bool twiceAttacker, bool twiceDefender)
    {
        unitAttacker.text = attacker.Name;
        unitDefender.text = defender.Name;
        maxHPAttacker.text = attacker.Health.ToString();
        maxHPDefender.text = defender.Health.ToString();
        this.currentHPAttacker.text = currentHPAttacker;
        this.currentHPDefender.text = currentHPDefender;
        this.hitAttacker.text = hitAttacker;
        this.hitDefender.text = hitDefender;
        this.critAttacker.text = critAttacker;
        this.critDefender.text = critDefender;
        if (twiceAttacker)
            this.twiceAttacker.enabled = true;
        else this.twiceAttacker.enabled = false;
        if (twiceDefender)
            this.twiceDefender.enabled = true;
        else this.twiceDefender.enabled = false;
    }
}
