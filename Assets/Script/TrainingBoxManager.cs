using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TrainingBoxManager : MonoBehaviour {
   
	// Use this for initialization
	void Start () {
        MoveBoxCommand();
	}

    void MoveBoxCommand()
    {
        if (GestureManager.Instance.IsManipulating)
        {
            GestureManager.Instance.Transition(GestureManager.Instance.ManipulationRecognizer);
        }
    }
    
}
