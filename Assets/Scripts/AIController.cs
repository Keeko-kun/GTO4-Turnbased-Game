using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour {

    public List<GameObject> enemyPrefabs;

    public List<GameObject> enemyUnits;

    public void RaiseLevel(List<GameObject> playerUnits)
    {
        int totalLevel = 0;
        foreach (GameObject unit in playerUnits)
        {
            totalLevel += unit.GetComponent<Unit>().Level;
        }

        

        foreach (GameObject unit in enemyUnits)
        {
            int averageLevel = (int)Math.Ceiling((double)totalLevel / (double)playerUnits.Count) + UnityEngine.Random.Range(-1, 1);
            for (int i = 0; i < averageLevel; i++)
            {
                unit.GetComponent<Unit>().IncreaseExperience(100, GetComponent<CursorAction>());
            }          
        }
    }

}
