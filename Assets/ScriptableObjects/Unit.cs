using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit {

    public GameObject model;
    public string name;

    public Unit(GameObject model, string name)
    {
        this.model = model;
        this.name = name;
    }

}
