using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour {

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
        foreach (EnemyAction command in commands)
        {
            if (command.ActionType == EnemyActionType.Walk)
            {
                yield return StartCoroutine(Walk(command));
            }
            else if (command.ActionType == EnemyActionType.Attack)
            {
                yield return StartCoroutine(Attack(command));
            }
            else if (command.ActionType == EnemyActionType.WalkAttack)
            {
                yield return StartCoroutine(Walk(command));
                yield return StartCoroutine(Attack(command));
            }
        }
    }

    private IEnumerator Walk(EnemyAction command)
    {
        GetComponent<MoveCursor>().map.SetUnit((int)command.TargetTile.PosX, (int)command.TargetTile.PosZ, command.Unit.gameObject);
        yield return StartCoroutine(command.Unit.GetComponent<Movement>().StartMovement(command.TargetTile));
    }

    private IEnumerator Attack(EnemyAction command)
    {
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
