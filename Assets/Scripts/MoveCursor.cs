using cakeslice;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCursor : MonoBehaviour {

    public Generate map;
    public Vector2Int startingTile;
    public MoveSelectedAction chooseAction;

    private LevelPiece currentTile;
    private float tileSize = Globals.spacing;

    private bool canMove;

    private CursorAction action;

    void Start () {

        canMove = true;

        currentTile = map.GetMap()[startingTile.x, startingTile.y];

        transform.position = new Vector3(-currentTile.PosX * tileSize, transform.position.y, currentTile.PosZ * tileSize);

        action = GetComponent<CursorAction>();
	}
	
	void Update () {
        if (canMove && action.mode != ActionMode.SelectAction && action.mode != ActionMode.LevelUp && action.mode != ActionMode.SelectWeapon && action.mode != ActionMode.ConfirmBattle && action.mode != ActionMode.WaitForBattle)
        {
            if (Input.GetAxisRaw("HorizontalL") == 1)
            {
                MoveArrowOnMap(1, 0);                
            }
            else if (Input.GetAxisRaw("HorizontalL") == -1)
            {
                MoveArrowOnMap(-1, 0);
            }
            else if (Input.GetAxisRaw("VerticalL") == 1)
            {
                MoveArrowOnMap(0, 1);
            }
            else if (Input.GetAxisRaw("VerticalL") == -1)
            {
                MoveArrowOnMap(0, -1);
            }
        }
        else if (canMove && action.mode == ActionMode.SelectAction)
        {
            if (Input.GetAxisRaw("VerticalL") == 1)
            {
                MoveActionSelect(false);
            }
            else if (Input.GetAxisRaw("VerticalL") == -1)
            {
                MoveActionSelect(true);
            }
        }

        if (Input.GetAxisRaw("VerticalL") == 0 && Input.GetAxisRaw("HorizontalL") == 0)
        {
            canMove = true;
        }

        if (currentTile.Unit != null)
        {
            if(currentTile.Unit.GetComponent<Movement>().currentTile == currentTile)
                transform.position = new Vector3(transform.position.x, 6, transform.position.z);
        }
        else
            transform.position = new Vector3(transform.position.x, 3, transform.position.z);

    }

    void MoveArrowOnMap(int z, int x)
    {
        if (action.mode == ActionMode.ViewEnemyStats)
        {
            action.ResetToSelectTile();
        }

        canMove = false;
        Vector2Int tile = new Vector2Int((int)(currentTile.PosX - x), (int)(currentTile.PosZ + z));

        if (tile.x < map.GetMap().GetLength(0) &&
            tile.y < map.GetMap().GetLength(1) &&
            tile.x >= 0 && tile.y >= 0)
        {
            transform.Translate(tileSize * x, 0, tileSize * z);
            currentTile = map.GetMap()[tile.x, tile.y];
        }
        else
        {
            Debug.Log("Can't move");
            return;
        }
    }

    void MoveActionSelect(bool up)
    {
        canMove = false;
        switch (chooseAction.currentAction)
        {
            case CurrentAction.Move:
                if (up) break;
                else chooseAction.currentAction = CurrentAction.Attack;
                break;
            case CurrentAction.Attack:
                if (up) chooseAction.currentAction = CurrentAction.Move;
                else chooseAction.currentAction = CurrentAction.Back;
                break;
            case CurrentAction.Back:
                if (up) chooseAction.currentAction = CurrentAction.Attack;
                else chooseAction.currentAction = CurrentAction.EndTurn;
                break;
            case CurrentAction.EndTurn:
                if (up) chooseAction.currentAction = CurrentAction.Back;
                else break ;
                break;
        }
    }

    public LevelPiece GetCurrentTile
    {
        get
        {
            return currentTile;
        }
    }
}
