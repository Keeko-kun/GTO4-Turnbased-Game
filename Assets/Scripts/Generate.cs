using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Generate : MonoBehaviour {

    public List<GameObject> pieces;
    public int mapSize;
    public Material aap64;

    private LevelPiece[,] map;
    private System.Random random = new System.Random();
    private float spacing = 2.5f;
    private List<LevelPiece> path;
    private List<GameObject> physicalPath;


    void Awake () {
        //Generate Map
        map = new LevelPiece[mapSize, mapSize];
        for (int x = 0; x < mapSize; x++)
        {
            for (int z = 0; z < mapSize; z++)
            {
                GameObject piece = pieces[random.Next(0, pieces.Capacity)];
                map[x, z] = new LevelPiece(x, z);
                GameObject g = Instantiate(piece, GetVector3(map[x,z]), piece.transform.rotation);
                map[x, z].Piece = g;
                map[x, z].Walkable = true; //Set all tiles Walkable true, for now
                g.GetComponent<Renderer>().material = aap64;
                g.transform.Rotate(RandomRotation());
                
            }
        }
	}

    private Vector3 GetVector3(LevelPiece piece)
    {
        float x = (piece.PosX * spacing) * -1;
        float y = 0;
        float z = piece.PosZ * spacing;
        return new Vector3(x, y, z);
    }

    private Vector3 RandomRotation()
    {
        float z = 90 * (random.Next(0, 4));
        return new Vector3(0, 0, z);
    }

    public LevelPiece[,] GetMap()
    {
        return map;
    }

    public void SetUnit(int x, int z, Unit unit)
    {
        map[x, z].Unit = unit;
    }

    public void FindPath(Node a, Node b, GameObject block)
    {
        physicalPath = new List<GameObject>();
        Debug.Log(b);
        aStarPath aStar = new aStarPath(map);
        aStar.FindPath(a, b);
        path = aStar.GetPath();
        Debug.Log(path.Count);
        foreach (LevelPiece tile in path)
        {
            GameObject g = Instantiate(block, new Vector3(tile.PosX * spacing * -1, block.transform.position.y, tile.PosZ * spacing), block.transform.rotation);
            physicalPath.Add(g);
        }
    }

    public void DestroyPath()
    {
        foreach(GameObject block in physicalPath)
        {
            Destroy(block);
        }
    }
}
