using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourcesManager : MonoBehaviour {

    public int gold;
    public int wood;

    public GameObject goldText;
    public GameObject woodText;

    private void Start()
    {
        UpdateText();
    }

    public void AddGold(int amount)
    {
        gold += amount;
        UpdateText();
    }

    public void AddWood(int amount)
    {
        wood += amount;
        UpdateText();
    }

    public void BuyWeapon()
    {
        gold -= 100;
        wood -= 50;
        UpdateText();
    }

    void UpdateText()
    {
        goldText.GetComponent<Text>().text = "Gold: " + gold.ToString();
        woodText.GetComponent<Text>().text = "Wood: " + wood.ToString();
    }

}
