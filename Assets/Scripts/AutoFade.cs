using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoFade : MonoBehaviour {

    private fadePanel panel;

	// Use this for initialization
	void Start () {
        panel = GetComponent<fadePanel>();
        StartCoroutine(Fade());
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private IEnumerator Fade()
    {
        while (true)
        {
            yield return new WaitForSeconds(1.5f);
            panel.visible = !panel.visible;
        }
    }
}
