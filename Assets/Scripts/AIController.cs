using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{

    public List<GameObject> enemyPrefabs;

    public List<GameObject> enemyUnits;

    private List<EnemyAction> commands;

    public void GenerateCommands()
    {
        commands = new List<EnemyAction>();

        foreach (GameObject unit in enemyUnits)
        {
            commands.Add(EnemyAction.GetNewAction(unit.GetComponent<Unit>(), GetComponent<CursorAction>()));
        }
    }

    public IEnumerator ExecuteCommands()
    {
        EnemyAction cmd = null;
        for (int i = enemyUnits.Count - 1; i >= 0; i--)
        {
            cmd = EnemyAction.GetNewAction(enemyUnits[i].GetComponent<Unit>(), GetComponent<CursorAction>());

            if (cmd.ActionType == EnemyActionType.Attack || cmd.ActionType == EnemyActionType.WalkAttack)
            {
                if (cmd.TargetUnit == null)
                {
                    cmd = EnemyAction.GetNewAction(cmd.Unit, GetComponent<CursorAction>());
                }
            }

            if (cmd.ActionType == EnemyActionType.Walk)
            {
                yield return StartCoroutine(Walk(cmd));
            }
            else if (cmd.ActionType == EnemyActionType.Attack)
            {
                yield return StartCoroutine(Attack(cmd));
            }
            else if (cmd.ActionType == EnemyActionType.WalkAttack)
            {
                yield return StartCoroutine(Walk(cmd));
                yield return new WaitForFixedUpdate();
                yield return StartCoroutine(Attack(cmd));
            }
        }

    }

    private IEnumerator Walk(EnemyAction command)
    {
        GetComponent<MoveCursor>().map.SetUnit((int)command.Unit.GetComponent<Movement>().currentTile.PosX, (int)command.Unit.GetComponent<Movement>().currentTile.PosZ, null);
        GetComponent<MoveCursor>().map.SetUnit((int)command.TargetTile.PosX, (int)command.TargetTile.PosZ, command.Unit.gameObject);
        yield return StartCoroutine(command.Unit.GetComponent<Movement>().StartMovement(command.TargetTile));
    }

    private IEnumerator Attack(EnemyAction command)
    {
        yield return new WaitForSeconds(0.5f);
        GetComponent<CursorAction>().attackSequence.PredictOutcome(command.Unit, command.TargetUnit, command.Unit.Weapon, GetComponent<CursorAction>().updatePrediction);
        yield return StartCoroutine(GetComponent<CursorAction>().ConfirmBattle(command.Unit, command.TargetUnit));
    }

    public void RaiseLevel(List<GameObject> playerUnits)
    {
        int totalLevel = 0;
        foreach (GameObject unit in playerUnits)
        {
            totalLevel += unit.GetComponent<Unit>().Level;
        }



        foreach (GameObject unit in enemyUnits)
        {
            int averageLevel = (int)Math.Ceiling((double)totalLevel / (double)playerUnits.Count) + UnityEngine.Random.Range(-1, 1);
            for (int i = 0; i < averageLevel; i++)
            {
                unit.GetComponent<Unit>().IncreaseExperience(100, GetComponent<CursorAction>());
            }
        }
    }

}
