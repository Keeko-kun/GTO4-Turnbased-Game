using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{

    public GameObject cursor;

    private float rotateSpeed = 60;
    private float zoomSpeed = 0.35f;
    private float moveSpeed = 10;
    private float orthographicSize;

    void Update()
    {
        orthographicSize = GetComponent<Camera>().orthographicSize;

        JoystickStuff();
    }

    void JoystickStuff()
    {
        if (Input.GetAxis("Right Click") == 0)
        {
            Vector3 translation = new Vector3(Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime, -Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime, 0);
            transform.Translate(translation);

            transform.RotateAround(cursor.transform.position, Vector3.up, Input.GetAxis("Left Trigger") * rotateSpeed * Time.deltaTime); //Rotate Left

            transform.RotateAround(cursor.transform.position, Vector3.down, Input.GetAxis("Right Trigger") * rotateSpeed * Time.deltaTime); //Rotate Right
        }
        else
        {
            if (orthographicSize + Input.GetAxis("Vertical") * zoomSpeed > 0.5f && orthographicSize + Input.GetAxis("Vertical") * zoomSpeed < 15)
            {
                GetComponent<Camera>().orthographicSize += Input.GetAxis("Vertical") * zoomSpeed;
            }

        }

    }
}