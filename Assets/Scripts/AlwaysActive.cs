using UnityEngine;
using System.IO;
using System.Collections;

public class AlwaysActive : MonoBehaviour {

    private static AlwaysActive instance;

    private void Awake()
    {

        if (instance == null)
        {
            instance = this;
        }
            
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(this.gameObject);
    }
}



