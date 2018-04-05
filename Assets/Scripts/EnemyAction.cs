using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAction {

    public Unit Unit { get; private set; }
    public EnemyActionType ActionType { get; private set; }
    public LevelPiece TargetTile { get; private set; }
    public Unit TargetUnit { get; private set; }

    private static AttackSequence attackSequence;

    public EnemyAction()
    {

    }

    public static EnemyAction GetNewAction(Unit unit, CursorAction cursorAction)
    {
        EnemyAction action = new EnemyAction();
        action.Unit = unit;

        cursorAction.WalkableTiles.Unit = unit.gameObject;

        WithinRange(unit, cursorAction, action);

        if (action.ActionType == EnemyActionType.Attack || action.ActionType == EnemyActionType.WalkAttack)
            SetWeapon(action.Unit, action.TargetUnit, cursorAction);

        return action;
    }

    private static bool WithinRange(Unit unit, CursorAction cursorAction, EnemyAction action)
    {
        LevelPiece currentTile = unit.GetComponent<Movement>().currentTile;
        HashSet<LevelPiece> reachableTiles = cursorAction.WalkableTiles.ReachableTiles(true, true);


        Hashtable hitableUnitsFromTile = new Hashtable();

        foreach (LevelPiece tile in reachableTiles)
        {
            unit.GetComponent<Movement>().currentTile = tile;
            foreach (AttackMove weapon in unit.stats.Attacks)
            {
                unit.Weapon = weapon;
                HashSet<LevelPiece> reachableFromCurrent = cursorAction.WalkableTiles.ReachableTiles(false, true);
                foreach (LevelPiece weaponRange in reachableFromCurrent)
                {
                    if (weaponRange.Unit != null)
                    {
                        if (weaponRange.Unit.GetComponent<Unit>().playerUnit)
                        {
                            hitableUnitsFromTile.Add(weaponRange.Unit, tile);
                        }
                    }
                }
            }

        }

        unit.GetComponent<Movement>().currentTile = currentTile;

        List<LevelPiece> keys = (List<LevelPiece>)hitableUnitsFromTile.Keys;

        if (keys.Count == 1)
        {
            action.TargetUnit = keys[0].Unit.GetComponent<Unit>();
            action.TargetTile = (LevelPiece)hitableUnitsFromTile[keys[0]];
            DecideType(action);
        }
        else if (keys.Count > 1)
        {
            List<bool> willDieFromAttack = new List<bool>();
            foreach (LevelPiece tile in keys)
            {
                willDieFromAttack.Add(SetWeapon(unit, tile.Unit.GetComponent<Unit>(), cursorAction));
            }

            for (int i = 0; i < willDieFromAttack.Count; i++)
            {
                if (!willDieFromAttack.Contains(true) || !willDieFromAttack.Contains(false))
                {
                    int random = Random.Range(0, keys.Count);
                    action.TargetUnit = keys[random].Unit.GetComponent<Unit>();
                    action.TargetTile = (LevelPiece)hitableUnitsFromTile[keys[random]];
                    DecideType(action);
                    break;
                }
                keys.Shuffle();
                if (willDieFromAttack[i] == false)
                {
                    action.TargetUnit = keys[i].Unit.GetComponent<Unit>();
                    action.TargetTile = (LevelPiece)hitableUnitsFromTile[keys[i]];
                    DecideType(action);
                }
            }
        }
        else if (keys.Count == 0)
        {
            action.ActionType = EnemyActionType.Walk;
            action.TargetUnit = null;
        }

        return true;
    }

    private static void DecideType(EnemyAction action)
    {
        if (action.Unit.GetComponent<Movement>().currentTile == action.TargetTile && action.TargetUnit != null)
        {
            action.ActionType = EnemyActionType.Attack;
        }
        else
        {
            action.ActionType = EnemyActionType.WalkAttack;
        }
    }

    private static bool SetWeapon(Unit unit, Unit target, CursorAction cursorAction)
    {
        cursorAction.WalkableTiles.Unit = target.gameObject;
        AttackMove weapon = cursorAction.WalkableTiles.DefenderCanHit(unit);

        if (weapon != null)
        {
            unit.Weapon = weapon;
        }
        else
        {
            unit.Weapon = unit.stats.Attacks[0];
        }

        bool willDie = attackSequence.PredictOutcome(unit, target, weapon, cursorAction.updatePrediction);

        return willDie;

    }



}

public enum EnemyActionType
{
    Walk,
    Attack,
    WalkAttack
}