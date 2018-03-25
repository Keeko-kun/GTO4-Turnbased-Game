using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class LevelPiece
{
    private float posX;
    private float posZ;
    private GameObject piece;

    private GameObject unit;

    public LevelPiece(float posX, float posZ)
    {
        this.posX = posX;
        this.posZ = posZ;
        unit = null;
    }

    public bool Walkable { get; set; }

    public GameObject Piece
    {
        get
        {
            return piece;
        }
        set
        {
            piece = value;
        }
    }

    public float PosX
    {
        get
        {
            return posX;
        }
    }

    public float PosZ
    {
        get
        {
            return posZ;
        }
    }

    public GameObject Unit
    {
        get
        {
            return unit;
        }
        set
        {
            unit = value;
        }
    }
}

