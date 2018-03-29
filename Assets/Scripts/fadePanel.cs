﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class fadePanel : MonoBehaviour {


    [Range(0, 5)]
    public float speed;
    public bool visible;

    private GameObject panel;
    private Text[] textObjects;
    private Image[] imageObjects;
    private float alpha = 0;
    private bool joepie;
    private int fadeDir = 1;

    // Use this for initialization
    void Start () {
        panel = gameObject;
        textObjects = panel.GetComponentsInChildren<Text>();
        imageObjects = panel.GetComponentsInChildren<Image>();
	}

    void OnGUI()
    {
        alpha += fadeDir * speed * Time.deltaTime;
        alpha = Mathf.Clamp01(alpha);

        foreach (Text text in textObjects)
        {
            text.color = new Color(text.color.r, text.color.g, text.color.b, alpha);
        }
        foreach(Image image in imageObjects)
        {
            image.color = new Color(image.color.r, image.color.g, image.color.b, alpha);
        }

        panel.GetComponent<Image>().color = new Color(panel.GetComponent<Image>().color.r, panel.GetComponent<Image>().color.g, panel.GetComponent<Image>().color.b, alpha);
    }

    // Update is called once per frame
    void Update () {
        if (visible)
        {
            fadeDir = 1;
        }
        else
        {
            fadeDir = -1;
        }
    }
}