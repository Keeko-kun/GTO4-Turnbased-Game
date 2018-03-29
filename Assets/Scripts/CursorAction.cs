using cakeslice;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorAction : MonoBehaviour
{

    public ActionMode mode;
    public GameObject statsPanel;

    private bool performingAction;
    private MoveCursor cursor;
    private GameObject unit;
    private AllWalkableTiles walkableTiles;
    private fadePanel unitStats;
    private ChangeStats updatePanel;

    private void Start()
    {
        mode = ActionMode.SelectTile;
        performingAction = false;
        cursor = GetComponent<MoveCursor>();
        walkableTiles = new AllWalkableTiles();
        walkableTiles.Map = cursor.map.GetMap();
        unitStats = statsPanel.GetComponent<fadePanel>();
        updatePanel = statsPanel.GetComponent<ChangeStats>();
    }

    // Update is called once per frame
    void Update()
    {
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
            }
        }
        else if (Input.GetKeyDown("joystick button 1") && mode != ActionMode.LevelUp)
        {
            ResetToSelectTile();
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
                unit.GetComponent<Unit>().IncreaseExperience(100, this);
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
        walkableTiles.ColorTiles();
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
    LevelUp
}