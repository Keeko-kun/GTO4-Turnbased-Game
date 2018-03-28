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
            }
        }
        else if (Input.GetKeyDown("joystick button 1"))
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
                SelectMoveUnit(); //Temp
                updatePanel.UpdateUI(unit.GetComponent<Unit>().stats);
                unitStats.visible = true;
            }

        }
    }

    private void SelectMoveUnit()
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
        walkableTiles.DecolorTiles();
        copyOfUnit.GetComponentInChildren<Outline>().enabled = false;

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

    private void ResetToSelectTile()
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

        unitStats.visible = false;
        mode = ActionMode.SelectTile;

    }


}


public enum ActionMode
{
    MoveUnit,
    SelectTile,
    SelectAction
}