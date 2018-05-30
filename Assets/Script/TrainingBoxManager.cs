using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TrainingBoxManager : MonoBehaviour {
    public static TrainingBoxManager Instance { get; private set; }

    public GameObject[] Connectors { get; private set; }

    public float tempDistance { get; private set; }

    public GameObject movingTrainingBox;

    public List<GameObject> staticTrainingBoxes;

    public List<GameObject> unselectedLights;
   
	// Use this for initialization
	void Start () {
        Instance = this;
        MoveBoxCommand();
	}

    void MoveBoxCommand()
    {
        if (GestureManager.Instance.IsManipulating)
        {
            GestureManager.Instance.Transition(GestureManager.Instance.ManipulationRecognizer, true);
        }
    }
}
