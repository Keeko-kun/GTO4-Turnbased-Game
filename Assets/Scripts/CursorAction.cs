using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorAction : MonoBehaviour
{

    public ActionMode mode;

    private bool performingAction;
    private MoveCursor cursor;
    private GameObject unit;

    private void Start()
    {
        mode = ActionMode.SelectTile;
        performingAction = false;
        cursor = GetComponent<MoveCursor>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("joystick button 0") && !performingAction)
        {
            switch (mode)
            {
                case ActionMode.MoveUnit:
                    MoveUnit();
                    break;
                case ActionMode.SelectTile:
                    SelectTile();
                    break;
            }
        }
    }

    private void SelectTile()
    {
        if(GetComponent<MoveCursor>().GetCurrentTile.Unit != null)
        {
            unit = GetComponent<MoveCursor>().GetCurrentTile.Unit;
            mode = ActionMode.MoveUnit;
        }
    }

    private void MoveUnit()
    {
        performingAction = true;
        unit.GetComponent<Movement>().Move(GetComponent<MoveCursor>().GetCurrentTile);
    }
}


public enum ActionMode
{
    MoveUnit,
    SelectTile
}