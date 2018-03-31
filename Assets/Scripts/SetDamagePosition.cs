using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetDamagePosition : MonoBehaviour {

    private Vector3 location;

    public void Set(Vector3 position)
    {
        location = new Vector3(position.x + Random.Range(-.2f, .2f), position.y * 4, position.z);
    }

    public void Update()
    {
        Vector2 ViewportPosition = Camera.main.WorldToScreenPoint(location);
        transform.position = ViewportPosition;

        if (GetComponentInChildren<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime > 1)
        {
            Destroy(GetComponentInParent<RectTransform>().gameObject);
        }
    }
}
