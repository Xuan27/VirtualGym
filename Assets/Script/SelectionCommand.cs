using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionCommand : MonoBehaviour {

    public GameObject[] selectionLights;

    // Called by GazeGestureManager when the user performs a Select gesture
    public static SelectionCommand Instance { get; private set; }

    public Color SelectionColor { get; private set; }

    public bool IsManipulating;



    void Start()
    {
        Instance = this;
        SelectionColor = Color.red;
        IsManipulating = true;
    }

    private void OnSelect()
    {
        IsManipulating = true;
        for (int i = 0; i < selectionLights.Length; i++)
        {
            selectionLights[i].GetComponent<Renderer>().material.color = Color.blue;
            SelectionColor = selectionLights[i].GetComponent<Renderer>().material.color;
        }
        GestureManager.Instance.Transition(GestureManager.Instance.ManipulationRecognizer); 
    }

    private void Deselect()
    {
        IsManipulating = false;
        for (int i = 0; i < selectionLights.Length; i++)
        {
            selectionLights[i].GetComponent<Renderer>().material.color = Color.red;
            SelectionColor = selectionLights[i].GetComponent<Renderer>().material.color;
        }
    }
}
