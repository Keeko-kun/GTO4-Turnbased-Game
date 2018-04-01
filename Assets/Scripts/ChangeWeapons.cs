using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeWeapons : MonoBehaviour {

    public List<Text> buttons;

    public void UpdatUI(List<AttackMove> attacks)
    {
        for (int i = 0; i < buttons.Count; i++)
        {
            if (i >= attacks.Count)
            {
                buttons[i].text = "";
            }
            else
            {
                buttons[i].text = attacks[i].attackName;
            }
        }
    }

}
