using cakeslice;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generate : MonoBehaviour
{

    public GameObject cursor;
    public List<GameObject> grass;
    public GameObject water;
    public GameObject bridge;
    public int mapSize;
    public GameObject unit;
    public GameObject unit2;

    private LevelPiece[,] map;
    private System.Random random = new System.Random();
    private float spacing = Globals.spacing;
    private List<LevelPiece> path;
    private List<GameObject> physicalPath = new List<GameObject>();


    void Start()
    {
        GenerateGrass();
        SpawnUnits();
        Globals.initialSpawn = true;
    }

    private void GenerateGrass()
    {
        mapSize = random.Next(6, 12);
        map = new LevelPiece[mapSize, mapSize];
        for (int x = 0; x < mapSize; x++)
        {
            for (int z = 0; z < mapSize; z++)
            {
                GameObject piece = grass[random.Next(0, grass.Capacity)];
                piece.GetComponent<Outline>().enabled = false;
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

            if (random.Next(0, 100) >= 80 && !randomDirection && i != 0)
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

    private void SpawnUnits()
    {
        List<GameObject> playerUnits = cursor.GetComponent<PlayerSession>().playerUnits;

        for (int i = 0; i < playerUnits.Count; i++)
        {
            GameObject u = Instantiate(playerUnits[i],
                new Vector3(0 - (Globals.spacing * i), playerUnits[i].transform.position.y, 0),
                Quaternion.identity);
            SetUnit(i, 0, u);

            if (Globals.initialSpawn && !u.GetComponent<Unit>().maySpawn)
            {
                Destroy(u);
                continue;
            }

            u.GetComponent<Movement>().pathfinder = GetComponent<Pathfinding>();
            u.GetComponent<Movement>().currentTile = map[i, 0];

            cursor.GetComponent<PlayerSession>().playerUnits[i] = u;
        }

        AIController aiController = cursor.GetComponent<AIController>();

        for (int i = 0; i < 1; i++)
        {
            int rnd = random.Next(0, aiController.enemyPrefabs.Count);
            GameObject u = Instantiate(aiController.enemyPrefabs[rnd],
                new Vector3(-1 * (map.GetLength(0) * Globals.spacing) + (Globals.spacing * (i + 1)), aiController.enemyPrefabs[rnd].transform.position.y, map.GetLength(0) * Globals.spacing - Globals.spacing),
                Quaternion.Euler(0, 180, 0));
            SetUnit(map.GetLength(0) - 1 - i, map.GetLength(1) - 1, u);
            u.GetComponent<Movement>().pathfinder = GetComponent<Pathfinding>();
            u.GetComponent<Movement>().currentTile = map[map.GetLength(0) - 1 - i, map.GetLength(1) - 1];

            aiController.enemyUnits.Add(u);
        }

        aiController.RaiseLevel(playerUnits);
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
