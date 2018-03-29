using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Generate : MonoBehaviour {

    public List<GameObject> grass;
    public GameObject water;
    public GameObject bridge;
    public int mapSize;
    public GameObject unit;
    public GameObject unit2;

    private LevelPiece[,] map;
    private System.Random random = new System.Random();
    private float spacing = 2.5f;
    private List<LevelPiece> path;
    private List<GameObject> physicalPath = new List<GameObject>();


    void Awake () {
        GenerateGrass();
        SpawnUnit();
	}

    private void GenerateGrass()
    {
        map = new LevelPiece[mapSize, mapSize];
        for (int x = 0; x < mapSize; x++)
        {
            for (int z = 0; z < mapSize; z++)
            {
                GameObject piece = grass[random.Next(0, grass.Capacity)];
                map[x, z] = new LevelPiece(x, z);
                GameObject g = Instantiate(piece, GetVector3(map[x, z]), piece.transform.rotation);
                map[x, z].Piece = g;
                map[x, z].Walkable = g.GetComponent<TerrainProperties>().walkable;
                g.transform.Rotate(RandomRotation());

            }
        }
        GenerateRiver();
    }

    private void GenerateRiver()
    {
        bool randomDirection = false;
        int randomZ = random.Next(0, 1) * 2 - 1;
        int safeZone = mapSize / 4;
        int startPosZ = random.Next(safeZone + 1, mapSize - safeZone);
        int oldZ = 0;
        int doubleX = 0;
        for (int i = 0; i < mapSize; i++)
        {
            Destroy(map[i, startPosZ].Piece);
            map[i, startPosZ] = new LevelPiece(i, startPosZ);
            GameObject g = Instantiate(water, GetVector3(map[i, startPosZ]), water.transform.rotation);
            map[i, startPosZ].Piece = g;
            map[i, startPosZ].Walkable = g.GetComponent<TerrainProperties>().walkable;

            if (random.Next(0,100) >= 80 && !randomDirection && i != 0)
            {
                randomDirection = true;
                oldZ = startPosZ;
                doubleX = i;
                i--;
                startPosZ += randomZ;
            }
        }
        GenerateBridges(randomDirection, oldZ, startPosZ, doubleX);
    }

    private void GenerateBridges(bool changedDirection, int oldZ, int currentZ, int doubleX)
    {
        int bridges = (int)Math.Ceiling((double)(mapSize / 6));

        for (int i = 0; i < bridges; i++)
        {
            int bridgeX = random.Next(0, mapSize);
            int bridgeZ;
            if (bridgeX == doubleX && changedDirection)
            {
                bridgeX += random.Next(0, 1) * 2 - 1;
            }

            if (bridgeX > doubleX || !changedDirection)
            {
                bridgeZ = currentZ;
            }
            else
            {
                bridgeZ = oldZ;
            }

            Destroy(map[bridgeX, bridgeZ].Piece);
            map[bridgeX, bridgeZ] = new LevelPiece(bridgeX, bridgeZ);
            GameObject g = Instantiate(bridge, GetVector3(map[bridgeX, bridgeZ]), bridge.transform.rotation);
            map[bridgeX, bridgeZ].Piece = g;
            map[bridgeX, bridgeZ].Walkable = g.GetComponent<TerrainProperties>().walkable;
        }
    }

    private void SpawnUnit()
    {
        GameObject u = Instantiate(unit, unit.transform.position, unit.transform.rotation);
        SetUnit(0, 0, u);
        u.GetComponent<Movement>().pathfinder = GetComponent<Pathfinding>();
        u.GetComponent<Movement>().currentTile = map[0, 0];

        GameObject u2 = Instantiate(unit2, new Vector3(unit2.transform.position.x -2.5f, unit2.transform.position.y, unit2.transform.position.z), unit2.transform.rotation);
        SetUnit(1, 0, u2);
        u2.GetComponent<Movement>().pathfinder = GetComponent<Pathfinding>();
        u2.GetComponent<Movement>().currentTile = map[1, 0];
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

    public void SetUnit(int x, int z, GameObject unit)
    {
        map[x, z].Unit = unit;
    }
}
