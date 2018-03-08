using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets;

public class PlayerCursor : MonoBehaviour {

    public Generate map;
    public Vector2Int startingTile;

    private LevelPiece currentTile;
    private float tileSize = 2.5f;
    private float baseY = 3.5f;

    private bool canMove;

    void Start () {
        canMove = true;

        currentTile = map.GetMap()[startingTile.x, startingTile.y];

        transform.position = new Vector3(-currentTile.PosX * tileSize, baseY, currentTile.PosZ * tileSize);
	}
	
	void Update () {
        if (canMove)
        {
            if (Input.GetAxis("HorizontalL") == 1)
            {
                MoveCursor(1, 0);                
            }
            else if (Input.GetAxis("HorizontalL") == -1)
            {
                MoveCursor(-1, 0);
            }
            else if (Input.GetAxis("VerticalL") == 1)
            {
                MoveCursor(0, 1);
            }
            else if (Input.GetAxis("VerticalL") == -1)
            {
                MoveCursor(0, -1);
            }
            StartCoroutine(WaitForMove());
        }
	}

    void MoveCursor(int z, int x)
    {
        Vector2Int tile = new Vector2Int((int)(currentTile.PosX - x), (int)(currentTile.PosZ + z));

        Debug.Log(tile);

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

    IEnumerator WaitForMove()
    {
        canMove = false;
        yield return new WaitForSeconds(.15f);
        canMove = true;
    }
}
