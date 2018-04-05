using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeActions : MonoBehaviour {

    public Text move;
    public Text attack;

    private float alphaNormal = 1f;
    private float alphaTransparent = .4f;

    public void UpdateUI(bool hasMoved, bool hasAttacked)
    {
        if (hasAttacked)
        {
            move.color = new Color(move.color.r, move.color.g, move.color.b, alphaTransparent);
            attack.color = new Color(move.color.r, move.color.g, move.color.b, alphaTransparent);
        }
        else if (hasMoved && !hasAttacked)
        {
            move.color = new Color(move.color.r, move.color.g, move.color.b, alphaTransparent);
        }
        else
        {
            move.color = new Color(move.color.r, move.color.g, move.color.b, alphaNormal);
            attack.color = new Color(move.color.r, move.color.g, move.color.b, alphaNormal);
        }
    }

}
