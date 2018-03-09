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

    private Unit unit;

    public LevelPiece(float posX, float posZ, GameObject piece)
    {
        this.posX = posX;
        this.posZ = posZ;
        this.piece = piece;
        unit = null;
    }

    public GameObject Piece
    {
        get
        {
            return piece;
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

    public Unit Unit
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

