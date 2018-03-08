using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets;

public class Generate : MonoBehaviour {

    public List<GameObject> pieces;
    public int mapSize;
    public Material aap64;

    private LevelPiece[,] map;
    private System.Random random = new System.Random();
    private float spacing = 2.5f;

	void Start () {
        //Generate Map
        map = new LevelPiece[mapSize, mapSize];
        for (int x = 0; x < mapSize; x++)
        {
            for (int z = 0; z < mapSize; z++)
            {
                map[x, z] = new LevelPiece(x, z, pieces[random.Next(0, pieces.Capacity)]);
                GameObject g = Instantiate(map[x, z].Piece, GetVector3(map[x,z]), map[x, z].Piece.transform.rotation);
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
}
