using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour {

    public UnitSO stats;

    private void Start()
    {
        stats.Initialize();
    }

}
