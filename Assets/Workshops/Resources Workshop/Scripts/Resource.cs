using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Resource : MonoBehaviour {

    public int amount;
    public int startingAmount;

    public Text text;

    public void Awake()
    {
        amount = startingAmount;
        UpdateUI();
    }

    public void AddAmount(int value)
    {
        amount += value;
        UpdateUI();
    }

    public void UpdateUI()
    {
        text.text = amount.ToString();
    }

}
