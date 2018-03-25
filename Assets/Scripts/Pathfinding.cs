using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour {

    List<LevelPiece> path;

    public LevelPiece[,] GetMap()
    {
        return GetComponent<Generate>().GetMap();
    }

    public List<LevelPiece> FindPath(Node a, Node b)
    {
        aStarPath aStar = new aStarPath(GetMap());
        aStar.FindPath(a, b);
        path = aStar.GetPath();
        return path;
    }
}
