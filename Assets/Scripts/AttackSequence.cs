using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackSequence : MonoBehaviour {

    public void PredictOutcome(Unit attacker, Unit defender, AttackMove moveAttacker, AttackMove moveDefender)
    {
        int hpAttacker, hpDefender;
        int hitAttacker, hitDefender;
        int critAttacker, critDefender;
        bool attackerTwice, defenderTwice;

        critAttacker = PredictCrit(attacker, moveAttacker, moveAttacker.range);
        critDefender = PredictCrit(defender, moveDefender, moveAttacker.range);
        hitAttacker = PredictHit(defender, moveAttacker, moveAttacker.range);
        hitDefender = PredictHit(attacker, moveDefender, moveAttacker.range);
        attackerTwice = PredictStrikeTwice(attacker, defender, hitAttacker);
        defenderTwice = PredictStrikeTwice(defender, attacker, hitDefender);
        hpDefender = PredictHP(attacker, defender, moveAttacker, hitAttacker, attackerTwice);
        hpAttacker = PredictHP(defender, attacker, moveDefender, hitDefender, defenderTwice);
    }

    private int PredictCrit(Unit unit, AttackMove move, int rangeToBeat)
    {
        if (move.range >= rangeToBeat)
        {
            return (int)Math.Ceiling(unit.Skill + unit.Skill * move.crit);
        }
        return 0;
    }

    private int PredictHit(Unit unit, AttackMove move, int rangeToBeat)
    {
        if (move.range >= rangeToBeat)
        {
            return 100 - (int)Math.Ceiling(unit.Luck + unit.Luck * move.hit);
        }
        return 0;
    }

    private bool PredictStrikeTwice(Unit attacker, Unit defender, int hit)
    {
        if (hit == 0)
        {
            return false;
        }

        if(defender.Speed + 5 <= attacker.Speed)
        {
            return true;
        }
        return false;
    }

    private int PredictHP(Unit attacker, Unit defender, AttackMove move, int hit, bool strikeTwice)
    {
        int currentHealth = defender.CurrentHealth;

        if (hit == 0)
        {
            return currentHealth;
        }

        int iterator = 1;
        if (strikeTwice)
            iterator++;

        for(int i = 0; i < iterator; i++)
        {
            int damage = 0;

            switch (move.type)
            {
                case AttackType.Physical:
                    damage = (attacker.Strength + move.might) - defender.Defense;
                    break;
                case AttackType.Magic:
                    damage = (attacker.Magic + move.might) - defender.Resistance;
                    break;
            }

            if (damage <= 0)
            {
                damage = 1;
            }

            currentHealth -= damage;
        }

        if (currentHealth < 0)
        {
            currentHealth = 0;
        }

        return currentHealth;
    }
}

public class AttackTurn
{
    public Unit attacker;
    public Unit defender;
    public AttackMove move;

    public AttackTurn(Unit attacker, Unit defender, AttackMove move)
    {
        this.attacker = attacker;
        this.defender = defender;
        this.move = move;
    }
}
