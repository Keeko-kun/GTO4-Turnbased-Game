using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackSequence : MonoBehaviour
{

    public bool PredictOutcome(Unit attacker, Unit defender, AttackMove moveAttacker, ChangePrediction updatePrediction)
    {
        int hpAttacker, hpDefender;
        int hitAttacker, hitDefender;
        int critAttacker, critDefender;
        bool attackerTwice, defenderTwice, canDefenderHit;

        canDefenderHit = CanDefenderHit(defender);

        AttackMove moveDefender = defender.Weapon;

        critAttacker = PredictCrit(attacker, moveAttacker, true);
        critDefender = PredictCrit(defender, moveDefender, canDefenderHit);
        hitAttacker = PredictHit(defender, moveAttacker, true);
        hitDefender = PredictHit(attacker, moveDefender, canDefenderHit);
        attackerTwice = PredictStrikeTwice(attacker, defender, hitAttacker);
        defenderTwice = PredictStrikeTwice(defender, attacker, hitDefender);
        hpDefender = defender.CurrentHealth;
        hpAttacker = attacker.CurrentHealth;
        hpDefender = PredictHP(attacker, defender, moveAttacker, hitAttacker, hpDefender);
        if (hpDefender > 0) hpAttacker = PredictHP(defender, attacker, moveDefender, hitDefender, hpAttacker);
        if (attackerTwice && hpDefender > 0 && hpAttacker > 0) hpDefender = PredictHP(attacker, defender, moveAttacker, hitAttacker, hpDefender);
        if (defenderTwice && hpDefender > 0 && hpAttacker > 0) hpAttacker = PredictHP(defender, attacker, moveDefender, hitDefender, hpAttacker);

        if (attacker.playerUnit)
        {
            updatePrediction.UpdateUI(attacker, defender, hpAttacker.ToString(), hpDefender.ToString(), hitAttacker.ToString(), hitDefender.ToString(), critAttacker.ToString(), critDefender.ToString(), attackerTwice, defenderTwice);
            return true;
        }
        else
        {
            if (hpAttacker <= 0)
            {
                return true;
            }
            return false;
        }
            
    }

    private int PredictCrit(Unit unit, AttackMove move, bool canHit)
    {
        if (canHit)
        {
            return (int)Math.Ceiling(unit.Skill + unit.Skill * move.crit);
        }
        return 0;
    }

    private int PredictHit(Unit unit, AttackMove move, bool canHit)
    {
        if (canHit)
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

        if (defender.Speed + 5 <= attacker.Speed)
        {
            return true;
        }
        return false;
    }

    private int PredictHP(Unit attacker, Unit defender, AttackMove move, int hit, int health)
    {
        int currentHealth = health;

        if (hit == 0)
        {
            return currentHealth;
        }


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


        if (currentHealth < 0)
        {
            currentHealth = 0;
        }

        return currentHealth;
    }

    public bool CanDefenderHit(Unit defender)
    {
        AttackMove defenderWeapon = GetComponent<CursorAction>().WalkableTiles.DefenderCanHit(defender);

        if (defenderWeapon != null)
        {
            defender.Weapon = defenderWeapon;
            return true;
        }

        defender.Weapon = defender.stats.Attacks[0];

        return false;
    }

    public IEnumerator ExecuteBattle(List<AttackTurn> turns, Unit unit)
    {

        foreach (AttackTurn turn in turns)
        {
            turn.Attacker.GetComponent<Animator>().SetBool("attack", true);
            yield return new WaitForFixedUpdate();
            turn.Attacker.GetComponent<Animator>().SetBool("attack", false);

            for (int i = 0; i < 40; i++)
            {
                yield return new WaitForFixedUpdate();
            }

            turn.Defender.TakeDamage(turn.Damage, turn.Crit);

            if (turn.Defender.CurrentHealth <= 0)
            {
                if (turn.Attacker == unit && turn.Attacker.playerUnit)
                {
                    turn.Attacker.IncreaseExperience(turn.Defender.Points, GetComponent<CursorAction>());
                }               
                break;
            }

            yield return new WaitForSeconds(1.5f);
        }

        yield break;
    }
}

public class AttackTurn
{
    public Unit Attacker { get; private set; }
    public Unit Defender { get; private set; }
    public int Damage { get; private set; }
    public bool Crit { get; private set; }

    public AttackTurn(Unit attacker, Unit defender)
    {
        Attacker = attacker;
        Defender = defender;

        int damage = 0;

        switch (attacker.Weapon.type)
        {
            case AttackType.Physical:
                damage = attacker.Strength + attacker.Weapon.might;
                break;
            case AttackType.Magic:
                damage = attacker.Magic + attacker.Weapon.might;
                break;
        }

        switch (attacker.Weapon.type)
        {
            case AttackType.Physical:
                damage = damage - defender.Defense;
                break;
            case AttackType.Magic:
                damage = damage - defender.Resistance;
                break;
        }

        if (damage <= 0)
        {
            damage = 1;
        }

        int randomNumber = UnityEngine.Random.Range(1, 100);
        if (randomNumber <= Math.Ceiling(attacker.Skill + (attacker.Skill * attacker.Weapon.crit)))
        {
            Crit = true;
            damage = damage * 2;
        }

        randomNumber = UnityEngine.Random.Range(1, 100);
        if (randomNumber <= Math.Ceiling(defender.Luck + (defender.Luck * attacker.Weapon.hit * 0.75f)))
        {
            damage = 0;
        }

        Damage = damage;
    }
}
