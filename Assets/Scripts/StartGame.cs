using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour {

    public fadePanel blackness;

    private bool clickedA = false;

    private void Start()
    {
        Globals.initialSpawn = false;
    }

    // Update is called once per frame
    void Update () {
		if (Input.GetAxis("A Button") == 1 && !clickedA)
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
