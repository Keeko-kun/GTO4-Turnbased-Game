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

    public void CalculateTiles(bool movement)
    {
        int iterator = 0;

        if (movement)
            iterator = Unit.GetComponent<Unit>().stats.Movement;
        else
            iterator = Unit.GetComponent<Unit>().Weapon.range;

        LevelPiece currentTile = Unit.GetComponent<Movement>().currentTile;

        HashSet<LevelPiece> reachableTiles = new HashSet<LevelPiece>();

        for (int x = -iterator; x <= iterator; x++)
        {
            int mZ = (Mathf.Abs(x) * -1) + iterator;
            for (int z = -mZ; z <= mZ; z++)
            {
                Vector2Int tile = new Vector2Int((int)(currentTile.PosX + x), (int)(currentTile.PosZ + z));
                if (tile.x < Map.GetLength(0) &&
                    tile.y < Map.GetLength(1) &&
                    tile.x >= 0 && tile.y >= 0)
                {
                    if (Map[tile.x, tile.y].Walkable)
                    {
                        if (movement)
                        {
                            List<LevelPiece> path = Unit.GetComponent<Movement>().pathfinder.FindPath(new Node((int)currentTile.PosX, (int)currentTile.PosZ, true), new Node(tile.x, tile.y, true));
                            if (path.Count <= iterator)
                            {
                                if (Map[tile.x, tile.y].Unit == null || Map[tile.x, tile.y].Unit == Unit)
                                    reachableTiles.Add(Map[tile.x, tile.y]);
                            }
                        }
                        else
                        {
                            if (Map[tile.x, tile.y].Unit != null && Map[tile.x, tile.y].Unit != Unit)
                            {
                                reachableTiles.Add(Map[tile.x, tile.y]);
                            }
                        }

                    }
                }
            }
        }

        ColorTiles(reachableTiles);
    }

    public AttackMove DefenderCanHit(Unit target)
    {
        LevelPiece currentTile = target.GetComponent<Movement>().currentTile;

        foreach (AttackMove weapon in target.stats.Attacks)
        {
            for (int x = -weapon.range; x <= weapon.range; x++)
            {
                int mZ = (Mathf.Abs(x) * -1) + weapon.range;
                for (int z = -mZ; z <= mZ; z++)
                {
                    Vector2Int tile = new Vector2Int((int)(currentTile.PosX + x), (int)(currentTile.PosZ + z));
                    if (tile.x < Map.GetLength(0) &&
                        tile.y < Map.GetLength(1) &&
                        tile.x >= 0 && tile.y >= 0)
                    {
                        if (Map[tile.x, tile.y].Unit == Unit)
                        {
                            return weapon;
                        }
                    }
                }
            }
        }

        return null;
    }

        private void ColorTiles(HashSet<LevelPiece> reachableTiles)
        {
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
