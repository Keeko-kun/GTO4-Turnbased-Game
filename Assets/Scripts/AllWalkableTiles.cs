using cakeslice;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllWalkableTiles
{

    public LevelPiece[,] Map { get; set; }
    public GameObject Unit { get; set; }

    public AllWalkableTiles()
    {

    }

    public void ColorTiles()
    {
        int movement = Unit.GetComponent<Unit>().stats.Movement;

        LevelPiece currentTile = Unit.GetComponent<Movement>().currentTile;

        HashSet<LevelPiece> reachableTiles = new HashSet<LevelPiece>();

        for (int x = -movement; x <= movement; x++)
        {
            int mZ = (Mathf.Abs(x) * -1) + movement;
            for (int z = -mZ; z <= mZ; z++)
            {
                Vector2Int tile = new Vector2Int((int)(currentTile.PosX + x), (int)(currentTile.PosZ + z));
                if (tile.x < Map.GetLength(0) &&
                    tile.y < Map.GetLength(1) &&
                    tile.x >= 0 && tile.y >= 0)
                {
                    if (Map[tile.x, tile.y].Walkable)
                    {
                        List<LevelPiece> path = Unit.GetComponent<Movement>().pathfinder.FindPath(new Node((int)currentTile.PosX, (int)currentTile.PosZ, true), new Node(tile.x, tile.y, true));
                        if (path.Count <= movement)
                        {
                            if (Map[tile.x, tile.y].Unit == null || Map[tile.x, tile.y].Unit == Unit)
                                reachableTiles.Add(Map[tile.x, tile.y]);
                        }
                    }
                }
            }
        }

        for (int i = 0; i < Map.GetLength(0); i++)
        {
            for (int j = 0; j < Map.GetLength(1); j++)
            {
                Map[i, j].Piece.GetComponent<Outline>().enabled = true;
                Map[i, j].Piece.GetComponent<Outline>().color = (int)SelectColors.OutOfRange;
            }
        }

        foreach (LevelPiece l in reachableTiles)
        {
            l.Piece.GetComponent<Outline>().enabled = true;
            l.Piece.GetComponent<Outline>().color = (int)SelectColors.InRange;
        }
    }

    public void DecolorTiles()
    {
        for (int i = 0; i < Map.GetLength(0); i++)
        {
            for (int j = 0; j < Map.GetLength(1); j++)
            {
                Map[i, j].Piece.GetComponent<Outline>().enabled = false;
            }
        }
    }
}
