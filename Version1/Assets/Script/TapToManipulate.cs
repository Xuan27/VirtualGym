using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.WSA.Input;

public class TapToManipulate : MonoBehaviour {

    public bool IsManipulating { get; private set; }

    public Vector3 ManipulationPosition { get; private set; }

    public static GazeGestureManager Instance { get; private set; }

    public GameObject SelectedObject { get; private set; }

    GestureRecognizer ManipulationRecognizer;

    bool placing = false;
    int counter = 0;

	// Use this for initialization
	void OnSelect () {
        if(counter%2 == 1)
        {
            placing = !placing;
            
            if (placing)
            {
                SelectedObject = this.gameObject;

                ManipulationRecognizer = new GestureRecognizer();
                ManipulationRecognizer.SetRecognizableGestures(GestureSettings.ManipulationTranslate);
                // Register for the Manipulation events on the ManipulationRecognizer.
                ManipulationRecognizer.ManipulationStarted += ManipulationRecognizer_ManipulationStarted;
                ManipulationRecognizer.ManipulationUpdated += ManipulationRecognizer_ManipulationUpdated;
                ManipulationRecognizer.ManipulationCompleted += ManipulationRecognizer_ManipulationCompleted;
                ManipulationRecognizer.ManipulationCanceled += ManipulationRecognizer_ManipulationCanceled;


                /*manipulation.HoldStarted += (args) =>
                {
                    Debug.Log("Arguments? " + args);
                    SelectedObject = this.gameObject;
                    Debug.Log(SelectedObject.transform.position);
                    //if (SelectedObject != null)
                    //{
                       // SelectedObject.SendMessageUpwards("Manipulate", SendMessageOptions.DontRequireReceiver);
                    //}
                };*/
                ManipulationRecognizer.StartCapturingGestures();
            }
        }
            
        counter++;
        ManipulationRecognizer.StartCapturingGestures();
    }
	
	// Update is called once per frame
	void Update () {
        if (placing)
        {
            ManipulationRecognizer.CancelGestures();
            ManipulationRecognizer.StartCapturingGestures();
            // Do a raycast into the world that will only hit the Spatial Mapping mesh.
            //var headPosition = Camera.main.transform.position;
            //var gazeDirection = Camera.main.transform.forward;

            //RaycastHit hitInfo;
            //if (Physics.Raycast(headPosition, gazeDirection, out hitInfo,
            //30.0f))
            //{
            //Debug.Log("Manipulation name: " + this.transform.name);
            // Move this object's parent object to
            // where the raycast hit the Spatial Mapping mesh.
            //this.transform.position = hitInfo.point;

            // Rotate this object's parent object to face the user.
            //Quaternion toQuat = Camera.main.transform.localRotation;
            //toQuat.x = 0;
            //toQuat.z = 0;
            //this.transform.rotation = toQuat;
            //}
        }
    }

    private void ManipulationRecognizer_ManipulationStarted(ManipulationStartedEventArgs obj)
    {
        if (this.gameObject != null)
        {
            IsManipulating = true;

            ManipulationPosition = Vector3.zero;

            this.gameObject.SendMessageUpwards("PerformManipulationStart", ManipulationPosition);
        }
    }

    private void ManipulationRecognizer_ManipulationUpdated(ManipulationUpdatedEventArgs obj)
    {
        if (this.gameObject != null)
        {
            IsManipulating = true;

            ManipulationPosition = obj.cumulativeDelta;

            this.gameObject.SendMessageUpwards("PerformManipulationUpdate", ManipulationPosition);
        }
    }

    private void ManipulationRecognizer_ManipulationCompleted(ManipulationCompletedEventArgs obj)
    {
        IsManipulating = false;
    }

    private void ManipulationRecognizer_ManipulationCanceled(ManipulationCanceledEventArgs obj)
    {
        IsManipulating = false;
    }
}


