using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionCommand : MonoBehaviour {

    public GameObject[] selectionLights;

    // Called by GazeGestureManager when the user performs a Select gesture
    public static SelectionCommand Instance { get; private set; }

    public Color SelectionColor { get; private set; }

    public bool IsManipulating { get; private set; }

    void Awake()
    {
        SelectionColor = Color.red;
        IsManipulating = true;
    }

    void Start()
    {
        Instance = this;
    }

    void Update()
    {

    }

    private void OnSelect()
    {
        IsManipulating = true;
        for (int i = 0; i < selectionLights.Length; i++)
        {
            selectionLights[i].GetComponent<Renderer>().material.color = Color.blue;
            SelectionColor = selectionLights[i].GetComponent<Renderer>().material.color;
        }
        GestureManager.Instance.Transition(GestureManager.Instance.ManipulationRecognizer, IsManipulating);
    }

    private void Deselect()
    {
        IsManipulating = false;
        for (int i = 0; i < selectionLights.Length; i++)
        {
            //if(TrainingBoxManager.Instance.lockObject == 1)
            //{
               // if(TrainingBoxManager.Instance.movingNearLight.name != selectionLights[i].name)
               // {
                    //selectionLights[i].GetComponent<Renderer>().material.color = Color.red;
                    //SelectionColor = selectionLights[i].GetComponent<Renderer>().material.color;
                //}
            //}
            //else
            //{
                selectionLights[i].GetComponent<Renderer>().material.color = Color.red;
                SelectionColor = selectionLights[i].GetComponent<Renderer>().material.color;
            //}
            
        }
        GestureManager.Instance.Transition(GestureManager.Instance.SelectionRecognizer, IsManipulating);
    }
}
