﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets;

public class PlayerCursor : MonoBehaviour {

    public Generate map;
    public Vector2Int startingTile;

    private LevelPiece currentTile;
    private float tileSize = 2.5f;
    private float baseY;

    private bool canMove;

    void Start () {
        baseY = transform.position.y;

        canMove = true;

        currentTile = map.GetMap()[startingTile.x, startingTile.y];

        transform.position = new Vector3(-currentTile.PosX * tileSize, baseY, currentTile.PosZ * tileSize);
	}
	
	void Update () {
        if (canMove)
        {
            if (Input.GetAxisRaw("HorizontalL") == 1)
            {
                MoveCursor(1, 0);                
            }
            else if (Input.GetAxisRaw("HorizontalL") == -1)
            {
                MoveCursor(-1, 0);
            }
            else if (Input.GetAxisRaw("VerticalL") == 1)
            {
                MoveCursor(0, 1);
            }
            else if (Input.GetAxisRaw("VerticalL") == -1)
            {
                MoveCursor(0, -1);
            }
        }

        if (Input.GetAxisRaw("VerticalL") == 0 && Input.GetAxisRaw("HorizontalL") == 0)
        {
            canMove = true;
        }

        Debug.Log(Input.GetAxisRaw("HorizontalL") + ", " + Input.GetAxisRaw("VerticalL"));

    }

    void MoveCursor(int z, int x)
    {
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
}
