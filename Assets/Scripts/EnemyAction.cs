using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAction {

    public Unit Unit { get; private set; }
    public EnemyActionType ActionType { get; private set; }
    public LevelPiece TargetTile { get; private set; }
    public Unit TargetUnit { get; private set; }

    public EnemyAction()
    {

    }

    public static EnemyAction GetNewAction(Unit unit, CursorAction cursorAction)
    {
        LevelPiece currentTile = unit.GetComponent<Movement>().currentTile;
        EnemyAction action = new EnemyAction();
        action.Unit = unit;

        cursorAction.WalkableTiles.Unit = unit.gameObject;

        WithinRange(unit, cursorAction, action);

        cursorAction.GetComponent<MoveCursor>().map.SetUnit((int)currentTile.PosX, (int)currentTile.PosZ, null);
        cursorAction.GetComponent<MoveCursor>().map.SetUnit((int)action.TargetTile.PosX, (int)action.TargetTile.PosZ, action.Unit.gameObject);

        if (action.ActionType == EnemyActionType.Attack || action.ActionType == EnemyActionType.WalkAttack)
            SetWeapon(action.Unit, action.TargetUnit, cursorAction, action.TargetTile);

        return action;
    }

    private static void WithinRange(Unit unit, CursorAction cursorAction, EnemyAction action)
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
                            if (!hitableUnitsFromTile.ContainsKey(weaponRange))
                                hitableUnitsFromTile.Add(weaponRange, tile);
                        }
                    }
                }
            }

        }

        unit.GetComponent<Movement>().currentTile = currentTile;

        List<LevelPiece> keys = hitableUnitsFromTile.Keys.Cast<LevelPiece>().ToList(); ;

        if (keys.Count == 1)
        {
            Debug.Log("hier (1)");
            action.TargetUnit = keys[0].Unit.GetComponent<Unit>();
            action.TargetTile = (LevelPiece)hitableUnitsFromTile[keys[0]];
            DecideType(action);
        }
        else if (keys.Count > 1)
        {
            Debug.Log("hier (meerkat)");
            List<bool> willDieFromAttack = new List<bool>();
            foreach (LevelPiece tile in keys)
            {
                willDieFromAttack.Add(SetWeapon(unit, tile.Unit.GetComponent<Unit>(), cursorAction, tile));
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

            Unit currentTarget = null;

            foreach (GameObject unitObject in cursorAction.GetComponent<PlayerSession>().playerUnits)
            {
                if (currentTarget == null || currentTarget.CurrentHealth > unitObject.GetComponent<Unit>().CurrentHealth)
                {
                    currentTarget = unitObject.GetComponent<Unit>();
                }
            }

            List<LevelPiece> path = action.Unit.GetComponent<Movement>().pathfinder.FindPath(new Node((int)currentTarget.GetComponent<Movement>().currentTile.PosX, (int)currentTarget.GetComponent<Movement>().currentTile.PosZ, true),
                new Node((int)action.Unit.GetComponent<Movement>().currentTile.PosX, (int)action.Unit.GetComponent<Movement>().currentTile.PosZ, true));

            for (int i = 0; i < path.Count; i++)
            {
                foreach (LevelPiece tile in reachableTiles)
                {
                    if (path[i] == tile)
                    {
                        action.TargetTile = tile;
                        return;
                    }
                }
            }
        }
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

    private static bool SetWeapon(Unit unit, Unit target, CursorAction cursorAction, LevelPiece piece)
    {
        LevelPiece currentTile = unit.GetComponent<Movement>().currentTile;
        unit.GetComponent<Movement>().currentTile = piece;
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
        Debug.Log(weapon);
        bool willDie = cursorAction.attackSequence.PredictOutcome(unit, target, weapon, cursorAction.updatePrediction);
        unit.GetComponent<Movement>().currentTile = currentTile;
        return willDie;

    }



}

public enum EnemyActionType
{
    Walk,
    Attack,
    WalkAttack
}