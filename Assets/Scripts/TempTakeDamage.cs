using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TempTakeDamage : MonoBehaviour {

    public GameObject DamageText;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.P))
        {
            GameObject canvas = GetComponentInChildren<Canvas>().gameObject;
            GameObject text = Instantiate(DamageText, canvas.transform);

            text.GetComponent<SetDamagePosition>().Set(transform.position);
            text.GetComponentInChildren<Text>().text = new System.Random().Next(1, 20).ToString();
        }
	}
}