using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour {

    public fadePanel blackness;

    private bool clickedA = false;

	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown("joystick button 0") && !clickedA)
        {
            StartCoroutine(ClickedA());
        }
	}

    private IEnumerator ClickedA()
    {
        clickedA = true;
        blackness.visible = true;
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene("BasicScene");
    }
}
