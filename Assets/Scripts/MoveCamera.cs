using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{

    private float rotateSpeed = 60;
    private float zoomSpeed = 0.35f;
    private float moveSpeed = 10;
    private float orthographicSize;

    private Vector3 sw = new Vector3(13.5f, 31.31f, -6.5f);
    private Vector3 se = new Vector3(13.5f, 31.31f, 16);
    private Vector3 ne = new Vector3(-24, 31.31f, 16);
    private Vector3 nw = new Vector3(-24, 31.31f, -6.5f);
    private int swR = -60;
    private int seR = -120;
    private int neR = -240;
    private int nwR = -300;

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

            transform.RotateAround(Vector3.zero, Vector3.up, Input.GetAxis("Left Trigger") * rotateSpeed * Time.deltaTime); //Rotate Left

            transform.RotateAround(Vector3.zero, Vector3.down, Input.GetAxis("Right Trigger") * rotateSpeed * Time.deltaTime); //Rotate Right
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