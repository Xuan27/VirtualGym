using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionCommand : MonoBehaviour {
    // Called by GazeGestureManager when the user performs a Select gesture

    public GameObject[] selectionLights;
    void OnSelect()
    {
        for(int i = 0; i < selectionLights.Length; i++)
        {
            selectionLights[i].GetComponent<Renderer>().material.color = Color.yellow;
           
        }
        
        
        //GetComponent<Renderer>().material.color = Color.green;

        //GameObject selectionLights = this.gameObject.transform.GetChild(0).gameObject;

        //Debug.Log("Function argument: " + argument);
        //Debug.Log(selectionLights.name);

        // 2.d: Uncomment the below line to highlight the material when gaze enters.
        //Debug.Log(selectMaterial[i].GetType);
        //Debug.Log(this.get);

        //for(int i = 0; i < selectMaterial.Length; i++)
        //{
        //   selectMaterial[i].color = Color.green;
        //}
        //

        /* If the sphere has no Rigidbody component, add one to enable physics.
        if (!this.GetComponent<Rigidbody>())
        {
            var rigidbody = this.gameObject.AddComponent<Rigidbody>();
            rigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;
        }*/
    }
}
