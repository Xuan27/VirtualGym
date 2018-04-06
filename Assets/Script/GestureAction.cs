using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GestureAction : MonoBehaviour {

    public static GestureAction Instance { get; private set; }

    private Vector3 manipulationPreviousPosition;

    public Vector3 moveVector { get; private set; }

    void Start()
    {
        Instance = this;
    }

    

    void PerformManipulationStart(Vector3 position)
    {
        manipulationPreviousPosition = position;
    }

    void PerformManipulationUpdate(Vector3 position)
    {
        
        if (GestureManager.Instance.IsManipulating)
        {
            moveVector = Vector3.zero;

            // 4.a: Calculate the moveVector as position - manipulationPreviousPosition.
            moveVector = position - manipulationPreviousPosition;
            
            // 4.a: Update the manipulationPreviousPosition with the current position.
            manipulationPreviousPosition = position;

            // 4.a: Increment this transform's position by the moveVector.
            GestureManager.Instance.ManipulatingObject.gameObject.transform.position += moveVector;
        }
    }
}
