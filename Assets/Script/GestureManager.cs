using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.WSA.Input;

public class GestureManager : MonoBehaviour {
    //Instance is also defined as an object
    //GazeGestureManager Instance Getter and Setter
    public static GestureManager Instance { get; private set; }

    public GestureRecognizer SelectionRecognizer { get; private set; }

    public GestureRecognizer ManipulationRecognizer { get; private set; }

    public GestureRecognizer ActiveRecognizer { get; private set; }

    public bool IsManipulating { get; private set; }

    public Vector3 ManipulationPosition { get; private set; }

    public GameObject ManipulatingObject { get; private set; }

    public GameObject SelectionObject { get; private set; }

    void Start()
    {
        Instance = this;
    }

    // Use this for initialization
    void Awake () {
        //Selection Event
        SelectionRecognizer = new GestureRecognizer();
        SelectionRecognizer.SetRecognizableGestures(GestureSettings.Tap);
        // 2.b: Register for the Tapped with the SelectionRecognizer_Tapped function.
        SelectionRecognizer.Tapped += SelectionRecognizer_Tapped;
        

        //Manipulation Event
        ManipulationRecognizer = new GestureRecognizer();
        ManipulationRecognizer.SetRecognizableGestures(GestureSettings.ManipulationTranslate);
        ManipulationRecognizer.ManipulationStarted += ManipulationRecognizer_ManipulationStarted;
        ManipulationRecognizer.ManipulationUpdated += ManipulationRecognizer_ManipulationUpdated;
        ManipulationRecognizer.ManipulationCompleted += ManipulationRecognizer_ManipulationCompleted;
        ManipulationRecognizer.ManipulationCanceled += ManipulationRecognizer_ManipulationCanceled;
       
        ResetGestureRecognizers();
    }

    public void ResetGestureRecognizers()
    {
        // Default to the navigation gestures.
        Transition(SelectionRecognizer);
    }

    public void Transition(GestureRecognizer newRecognizer)
    {
        if (newRecognizer == null)
        {
            return;
        }

        if (ActiveRecognizer != null)
        {
            if (ActiveRecognizer == newRecognizer)
            {
                return;
            }

            ActiveRecognizer.CancelGestures();
            ActiveRecognizer.StopCapturingGestures();
        }

        newRecognizer.StartCapturingGestures();
        ActiveRecognizer = newRecognizer;
    }

    private void ManipulationRecognizer_ManipulationStarted(ManipulationStartedEventArgs obj)
    {
        if (GazeManager.Instance.FocusedObject != null && SelectionCommand.Instance.IsManipulating == true)
        {
            IsManipulating = true;
            ManipulationPosition = Vector3.zero;

            ManipulatingObject.SendMessageUpwards("PerformManipulationStart", ManipulationPosition);
        }
    }

    private void ManipulationRecognizer_ManipulationUpdated(ManipulationUpdatedEventArgs obj)
    {
        if (GazeManager.Instance.FocusedObject != null && SelectionCommand.Instance.IsManipulating == true)
        {
            IsManipulating = true;

            ManipulationPosition = obj.cumulativeDelta;
            ManipulationPosition = GazeManager.Instance.Position;

            ManipulatingObject.SendMessageUpwards("PerformManipulationUpdate", ManipulationPosition);
        }
    }

    private void ManipulationRecognizer_ManipulationCompleted(ManipulationCompletedEventArgs obj)
    {
        IsManipulating = false;
        GazeManager.Instance.FocusedObject.SendMessageUpwards("Deselect");
        ResetGestureRecognizers();
    }

    private void ManipulationRecognizer_ManipulationCanceled(ManipulationCanceledEventArgs obj)
    {
        IsManipulating = false;
    }


    private void SelectionRecognizer_Tapped(TappedEventArgs obj)
    {
        //Parent of the focused object (selectionLight || selectionBox)
        GameObject TransformParent = GazeManager.Instance.FocusedObject;

        //Default color of the selection lights
        Color startingColor = Color.red;

        //Define the Manipulating Object to trainingBox
        if (TransformParent.name == "selectionLight")
            ManipulatingObject = TransformParent.gameObject.transform.parent.gameObject;

        else
            ManipulatingObject = TransformParent.gameObject;

        if (ManipulatingObject != null && SelectionCommand.Instance.SelectionColor == startingColor)
        {
            GazeManager.Instance.FocusedObject.SendMessageUpwards("OnSelect");       
        }
    }
}

