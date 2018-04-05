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

        CanAttackNow(unit, cursorAction, action);

        if (action.ActionType == EnemyActionType.Attack)
        {
            return action;
        }



        return action;
    }

    private static bool CanAttackNow(Unit unit, CursorAction cursorAction, EnemyAction action)
    {
        HashSet<LevelPiece> reachableTiles = cursorAction.WalkableTiles.ReachableTiles(false, true);
        List<LevelPiece> hitableUnits = new List<LevelPiece>();

        foreach (LevelPiece tile in reachableTiles)
        {
            if (tile.Unit != null)
            {
                if (tile.Unit.GetComponent<Unit>().playerUnit)
                {
                    hitableUnits.Add(tile);
                }
            }
        }

        LevelPiece targetTile = null;

        if (hitableUnits.Count == 0)
        {
            return false;
        }
        else if(hitableUnits.Count > 1)
        {
            List<bool> willDieFromAttack = new List<bool>();
            foreach (LevelPiece tile in hitableUnits)
            {
                willDieFromAttack.Add(WillDieFromAttack(unit, tile.Unit.GetComponent<Unit>(), cursorAction));
            }

            for (int i = 0; i < willDieFromAttack.Count; i++)
            {
                if (!willDieFromAttack.Contains(true) || !willDieFromAttack.Contains(false)/*Run away?*/)
                {
                    targetTile = hitableUnits[Random.Range(0, hitableUnits.Count)];
                    break;
                }
                hitableUnits.Shuffle();
                if (willDieFromAttack[i] == false)
                {
                    targetTile = hitableUnits[i];
                }
            }

        }
        else
        {
            targetTile = hitableUnits[0];
        }

        action.TargetUnit = targetTile.Unit.GetComponent<Unit>();

        action.ActionType = EnemyActionType.Attack;
        return true;
    }

    private static bool WillDieFromAttack(Unit unit, Unit target, CursorAction cursorAction)
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