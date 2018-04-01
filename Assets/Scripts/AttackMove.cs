using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AttackMove {
    public string attackName;
    public AttackType type;
    public int range;
    public int might;
    [Range(0,1)]
    public float hit;
    [Range(0,1)]
    public float crit;
}
