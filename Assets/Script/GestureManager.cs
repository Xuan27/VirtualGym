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
        Transition(SelectionRecognizer, false);
    }

    public void Transition(GestureRecognizer newRecognizer, bool Manipulation)
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
        IsManipulating = Manipulation;
        ActiveRecognizer = newRecognizer;
    }

    private void ManipulationRecognizer_ManipulationStarted(ManipulationStartedEventArgs obj)
    {
        if (GazeManager.Instance.FocusedObject != null && IsManipulating == true)
        {
            IsManipulating = true;
            ManipulationPosition = GazeManager.Instance.Position;

            ManipulatingObject.SendMessageUpwards("PerformManipulationStart", ManipulationPosition);
            GazeManager.Instance.TrainingBoxLight.tag = "selectionLight";
            
            GameObject[] unselectedLightParent = GameObject.FindGameObjectsWithTag("unselectedLight");
            for(int i = 0; i < unselectedLightParent.Length; i++)
            {
                Debug.Log("Unselected Light parent name: " + unselectedLightParent[i].name);
                Debug.Log("Unselected Light Training Box name: " + unselectedLightParent[i].transform.parent.name);
            }
        }
    }

    private void ManipulationRecognizer_ManipulationUpdated(ManipulationUpdatedEventArgs obj)
    {

        if (GazeManager.Instance.FocusedObject != null && IsManipulating == true)
        {
            IsManipulating = true;
            ManipulationPosition = GazeManager.Instance.Position;

            ManipulatingObject.SendMessageUpwards("PerformManipulationUpdate", ManipulationPosition);
        }
    }
    
    private void ManipulationRecognizer_ManipulationCompleted(ManipulationCompletedEventArgs obj)
    {
        IsManipulating = false;
        GazeManager.Instance.TrainingBoxLight.tag = "unselectedLight";
        GazeManager.Instance.TrainingBoxLight.SendMessageUpwards("Deselect");
    }

    private void ManipulationRecognizer_ManipulationCanceled(ManipulationCanceledEventArgs obj)
    {
        IsManipulating = false;
    }


    private void SelectionRecognizer_Tapped(TappedEventArgs obj)
    {

        //Default color of the selection lights
        Color startingColor = Color.red;

        ManipulatingObject = GazeManager.Instance.FocusedObject;



        if (ManipulatingObject != null && SelectionCommand.Instance.SelectionColor == startingColor && GazeManager.Instance.Hit)
        {
           GazeManager.Instance.TrainingBoxLight.SendMessageUpwards("OnSelect");
        }
    }
}

