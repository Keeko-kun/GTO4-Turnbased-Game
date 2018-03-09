using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnCubeUnit : MonoBehaviour
{

    public GameObject unit;

    private PlayerCursor cursor;
    private bool buttonClicked;
    private float spacing = 2.5f;

    private void Start()
    {
        buttonClicked = false;

        cursor = GetComponent<PlayerCursor>();
    }

    private void Update()
    {
        if (Input.GetAxisRaw("A Button") == 1)
        {
            if (!buttonClicked)
            {
                buttonClicked = true;
                if (cursor.GetCurrentTile.Unit == null)
                {
                    GameObject g = Instantiate(unit, new Vector3(cursor.GetCurrentTile.PosX * spacing * -1, unit.transform.position.y, cursor.GetCurrentTile.PosZ * spacing), unit.transform.rotation);
                    WriteUnitToTile(g);
                }
            }
        }
        else
        {
            buttonClicked = false;
        }
    }

    private void WriteUnitToTile(GameObject g)
    {
        Unit u = new Unit(g, "Blocky");
        cursor.map.SetUnit((int)cursor.GetCurrentTile.PosX, (int)cursor.GetCurrentTile.PosZ, u);
    }

}
