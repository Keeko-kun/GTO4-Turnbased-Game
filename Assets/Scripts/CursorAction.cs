using cakeslice;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorAction : MonoBehaviour
{

    public ActionMode mode;
    public GameObject statsPanel;
    public GameObject buttonsPanel;
    public GameObject predictionsPanel;

    private bool performingAction;
    private MoveCursor cursor;
    private GameObject unit;
    private GameObject target;
    private AllWalkableTiles walkableTiles;
    private fadePanel unitStats;
    private fadePanel buttonsFade;
    private fadePanel predictionsFade;
    private ChangeStats updatePanel;
    private ChangeWeapons updateWeapons;
    private ChangePrediction updatePrediction;
    private AttackSequence attackSequence;

    public AllWalkableTiles WalkableTiles { get { return walkableTiles; } }

    private void Start()
    {
        mode = ActionMode.SelectTile;
        performingAction = false;
        cursor = GetComponent<MoveCursor>();
        attackSequence = GetComponent<AttackSequence>();
        walkableTiles = new AllWalkableTiles();
        walkableTiles.Map = cursor.map.GetMap();
        unitStats = statsPanel.GetComponent<fadePanel>();
        buttonsFade = buttonsPanel.GetComponent<fadePanel>();
        predictionsFade = predictionsPanel.GetComponent<fadePanel>();
        updatePanel = statsPanel.GetComponent<ChangeStats>();
        updateWeapons = buttonsPanel.GetComponent<ChangeWeapons>();
        updatePrediction = predictionsPanel.GetComponent<ChangePrediction>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("joystick button 1") && mode != ActionMode.LevelUp)
        {
            ResetToSelectTile();
        }
        if (mode == ActionMode.SelectWeapon)
        {
            SelectWeapon();
            return;
        }
        if (Input.GetKeyDown("joystick button 0"))
        {
            switch (mode)
            {
                case ActionMode.MoveUnit:
                    StartCoroutine(MoveUnit((int)cursor.GetCurrentTile.PosX, (int)cursor.GetCurrentTile.PosZ));
                    break;
                case ActionMode.SelectTile:
                    SelectTile();
                    break;
                case ActionMode.SelectAction:
                    SelectAction();
                    break;
                case ActionMode.LevelUp:
                    StartCoroutine(unit.GetComponent<Unit>().DismissLevelUp(this));
                    break;
                case ActionMode.SelectTarget:
                    SelectTarget();
                    break;
                case ActionMode.ConfirmBattle:
                    StartCoroutine(ConfirmBattle());
                    break;
            }
        }
    }

    private void SelectWeapon()
    {
        if (Input.GetKeyDown("joystick button 0") && updateWeapons.buttons[0].text != "")
        {
            unit.GetComponent<Unit>().Weapon = unit.GetComponent<Unit>().stats.Attacks[0];
            SelectWeaponToAttackWith();
        }
        else if (Input.GetKeyDown("joystick button 2") && updateWeapons.buttons[1].text != "")
        {
            unit.GetComponent<Unit>().Weapon = unit.GetComponent<Unit>().stats.Attacks[1];
            SelectWeaponToAttackWith();
        }
        else if (Input.GetKeyDown("joystick button 3") && updateWeapons.buttons[2].text != "")
        {
            unit.GetComponent<Unit>().Weapon = unit.GetComponent<Unit>().stats.Attacks[2];
            SelectWeaponToAttackWith();
        }
    }

    private void SelectTile()
    {
        if (cursor.GetCurrentTile.Unit != null)
        {
            unit = cursor.GetCurrentTile.Unit;
            if (!unit.GetComponent<Movement>().walking)
            {
                mode = ActionMode.SelectAction;
                cursor.chooseAction.GetComponent<fadePanel>().visible = true;
                updatePanel.UpdateUI(unit.GetComponent<Unit>());
                unitStats.visible = true;
            }
        }
    }

    private void SelectTarget()
    {
        if (cursor.GetCurrentTile.Piece.GetComponent<Outline>().color == (int)SelectColors.OutOfRange)
        {
            ResetToSelectTile();
            return;
        }
        if (cursor.GetCurrentTile.Unit != null)
        {
            mode = ActionMode.ConfirmBattle;
            target = cursor.GetCurrentTile.Unit;
            attackSequence.PredictOutcome(unit.GetComponent<Unit>(), target.GetComponent<Unit>(), unit.GetComponent<Unit>().Weapon, updatePrediction);
            predictionsFade.visible = true;
            unit.GetComponentInChildren<Outline>().enabled = false;
            walkableTiles.DecolorTiles();
        }
    }

    private IEnumerator ConfirmBattle() //Coroutine?
    {
        mode = ActionMode.WaitForBattle;
        List<AttackTurn> turns = new List<AttackTurn>();
        if (int.Parse(updatePrediction.hitAttacker.text) > 0) turns.Add(new AttackTurn(unit.GetComponent<Unit>(), target.GetComponent<Unit>()));
        if (int.Parse(updatePrediction.hitDefender.text) > 0) turns.Add(new AttackTurn(target.GetComponent<Unit>(), unit.GetComponent<Unit>()));
        if (updatePrediction.twiceAttacker.enabled) turns.Add(new AttackTurn(unit.GetComponent<Unit>(), target.GetComponent<Unit>()));
        if (updatePrediction.twiceDefender.enabled) turns.Add(new AttackTurn(target.GetComponent<Unit>(), unit.GetComponent<Unit>()));

        predictionsFade.visible = false;

        yield return StartCoroutine(attackSequence.ExecuteBattle(turns));

        ResetToSelectTile();
    }

    private void SelectAction()
    {
        switch (cursor.chooseAction.currentAction)
        {
            case CurrentAction.Move:
                SelectActionMoveUnit();
                break;
            case CurrentAction.Back:
                ResetToSelectTile();
                break;
            case CurrentAction.Attack:
                SelectActionAttack();
                break;
            case CurrentAction.EndTurn:
                ResetToSelectTile();
                break;
        }

        unitStats.visible = false;
        cursor.chooseAction.GetComponent<fadePanel>().visible = false;
        cursor.chooseAction.currentAction = CurrentAction.Move;
    }

    private void SelectActionMoveUnit()
    {
        walkableTiles.Unit = unit;
        walkableTiles.CalculateTiles(true);
        unit.GetComponentInChildren<Outline>().enabled = true;
        mode = ActionMode.MoveUnit;
    }

    private IEnumerator MoveUnit(int x, int z)
    {
        GameObject copyOfUnit = unit;
        ResetToSelectTile();

        if (cursor.GetCurrentTile.Piece.GetComponent<Outline>().color == (int)SelectColors.OutOfRange ||
            copyOfUnit.GetComponent<Movement>().walking)
        {
            ResetToSelectTile();
            yield break;
        }

        cursor.map.SetUnit((int)copyOfUnit.GetComponent<Movement>().currentTile.PosX, (int)copyOfUnit.GetComponent<Movement>().currentTile.PosZ, null);
        cursor.map.SetUnit(x, z, copyOfUnit);
        yield return StartCoroutine(copyOfUnit.GetComponent<Movement>().StartMovement(cursor.GetCurrentTile));

    }

    private void SelectActionAttack()
    {
        buttonsFade.visible = true;
        updateWeapons.UpdatUI(unit.GetComponent<Unit>().stats.Attacks);
        mode = ActionMode.SelectWeapon;
    }

    private void SelectWeaponToAttackWith()
    {
        mode = ActionMode.SelectTarget;
        buttonsFade.visible = false;
        unit.GetComponentInChildren<Outline>().enabled = true;
        walkableTiles.Unit = unit;
        walkableTiles.CalculateTiles(false);
    }

    public void ResetToSelectTile()
    {
        switch (mode)
        {
            case ActionMode.MoveUnit:
                walkableTiles.DecolorTiles();
                unit.GetComponentInChildren<Outline>().enabled = false;
                unit = null;
                break;
            case ActionMode.SelectTile:
                break;
            case ActionMode.SelectWeapon:
                buttonsFade.visible = false;
                break;
            case ActionMode.SelectTarget:
                unit.GetComponentInChildren<Outline>().enabled = false;
                walkableTiles.DecolorTiles();
                predictionsFade.visible = false;
                break;
            case ActionMode.ConfirmBattle:
                predictionsFade.visible = false;
                break;
            case ActionMode.LevelUp:
                return;
            default:
                break;
        }

        cursor.chooseAction.GetComponent<fadePanel>().visible = false;
        cursor.chooseAction.currentAction = CurrentAction.Move;
        unitStats.visible = false;
        mode = ActionMode.SelectTile;
    }
}

public enum ActionMode
{
    MoveUnit,
    SelectTile,
    SelectAction,
    LevelUp,
    SelectWeapon,
    SelectTarget,
    ConfirmBattle,
    WaitForBattle
}