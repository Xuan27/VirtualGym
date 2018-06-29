using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.WSA.Input;

public class GestureManager : MonoBehaviour {
    //Instance is also defined as an object
    //GazeGestureManager Instance Getter and Setter
    public static GestureManager Instance { get; private set; }

    /**************
     * GESTURES
     * 1. Selection
     * 2. Manipulation
     * **************/
    public GestureRecognizer SelectionRecognizer { get; private set; }

    public GestureRecognizer ManipulationRecognizer { get; private set; }


    //Gesture that holds the current active recognizer since only one
    //gesture recognizer should be active at a time.
    public GestureRecognizer ActiveRecognizer { get; private set; }

    //Flag that determines if the manipulation gesture has started
    public bool IsManipulating { get; private set; }

    //Vector with the position coordinates of the moving object
    public Vector3 ManipulationPosition { get; private set; }

    //Gameobject of the moving element
    public GameObject ManipulatingObject { get; private set; }

    //Gameobject of the selected Training Box Light parent
    public GameObject ManipulatingObject_LightParent { get; private set; }

    public GameObject TrainingBoxRoot { get; private set; }

    public List<GameObject> UnfocusedTrainingBoxes { get; private set; }

    public GameObject LineRender;

    void Start()
    {
        Instance = this;
        LineRender.SetActive(false);
    }

    // Use this for initialization
    void Awake () {
        //Selection Event
        SelectionRecognizer = new GestureRecognizer();
        SelectionRecognizer.SetRecognizableGestures(GestureSettings.Tap);
        SelectionRecognizer.Tapped += SelectionRecognizer_Tapped;
        

        //Manipulation Event
        ManipulationRecognizer = new GestureRecognizer();
        ManipulationRecognizer.SetRecognizableGestures(GestureSettings.ManipulationTranslate);
        ManipulationRecognizer.ManipulationStarted += ManipulationRecognizer_ManipulationStarted;
        ManipulationRecognizer.ManipulationUpdated += ManipulationRecognizer_ManipulationUpdated;
        ManipulationRecognizer.ManipulationCompleted += ManipulationRecognizer_ManipulationCompleted;
        ManipulationRecognizer.ManipulationCanceled += ManipulationRecognizer_ManipulationCanceled;
       
        //Resets gesture recognition to Selection Recognizer gesture
        ResetGestureRecognizers();
    }
    /*****************************
     * FUNCTIONS & EVENT FUNCTIONS
     * 1.ResetGestureRecognizers (function)
     * 2.Transition (function)
     * 3.ManipulationRecognizer_ManipulationStarted (function event)
     * 4.ManipulationRecognizer_ManipulationUpdated (function event)
     * 5.ManipulationRecognizer_ManipulationCompleted (function event)
     * 6.ManipulationRecognizer_ManipulationCanceled (function event)
     * 7.SelectionRecognizer_Tapped (function event)
     * ****************/


    //1
    //*************************************************
    // ResetGestureRecognizers function
    // This function resets gesture recognition to 
    // Selection Recognizer gesture
    //
    // Return Value
    // ------------
    // None
    //
    // Parameters
    // ------------
    // None
    //*************************************************

    public void ResetGestureRecognizers()
    {
        // Default to the navigation gestures.
        Transition(SelectionRecognizer, false);
    }

    //2
    //*************************************************
    // Transition function
    // This function transitions between the Selection
    // gesture and Manipulation gesture by turning on 
    // one of the gesture and turning off the other.
    //
    // Return Value
    // ------------
    // None
    //
    // Parameters
    // ------------
    // GestureRecognizer newRecognizer  Gesture Recognizer that will be turn on
    // bool              Manipulation   Sets the manipulation boolean to true or false
    //*************************************************
    public void Transition(GestureRecognizer newRecognizer, bool Manipulation)
    {
        if (newRecognizer == null)
        {
            return;
        }

        /*TURN OFF*/
        //If active recognizer is not the same as the new recognizer the active 
        //recognizer is turn off
        if (ActiveRecognizer != null)
        {
            if (ActiveRecognizer == newRecognizer)
            {
                return;
            }

            ActiveRecognizer.CancelGestures();
            ActiveRecognizer.StopCapturingGestures();
        }

        /*TURN ON*/
        //New recognizer is turn on
        newRecognizer.StartCapturingGestures();
        IsManipulating = Manipulation;
        ActiveRecognizer = newRecognizer;
    }

    //3
    //*************************************************
    // ManipulationRecognizer_ManipulationStarted function
    // This function is called when the manipulation event is started
    //
    // Return Value
    // ------------
    // None
    //
    // Parameters
    // ------------
    // ManipulationStartedEventArgs obj  Manipulation event arguments
    //*************************************************
    private void ManipulationRecognizer_ManipulationStarted(ManipulationStartedEventArgs obj)
    {
        if (GazeManager.Instance.FocusedTrainingBox != null && IsManipulating == true)
        {
            IsManipulating = true;

            ManipulationPosition = GazeManager.Instance.Position;

            ManipulatingObject.SendMessageUpwards("PerformManipulationStart", ManipulationPosition);
            //Sets the tag of the selection lights parent to selected
            //GazeManager.Instance.TrainingBoxLightParent.tag = "selectionLight";
            //Sets the line renderer active
            //LineRender.SetActive(true);
        }
    }

    //4
    //*************************************************
    // ManipulationRecognizer_ManipulationUpdated function
    // This function is called when the manipulation event is updated
    //
    // Return Value
    // ------------
    // None
    //
    // Parameters
    // ------------
    // ManipulationStartedEventArgs obj  Manipulation event arguments
    //*************************************************
    private void ManipulationRecognizer_ManipulationUpdated(ManipulationUpdatedEventArgs obj)
    {

        if (GazeManager.Instance.FocusedTrainingBox != null && IsManipulating == true)
        {
            IsManipulating = true;
            ManipulationPosition = GazeManager.Instance.Position;

            ManipulatingObject.SendMessageUpwards("PerformManipulationUpdate", ManipulationPosition);
        }
    }

    //5
    //*************************************************
    // ManipulationRecognizer_ManipulationCompleted function
    // This function is called when the manipulation event gets completed
    //
    // Return Value
    // ------------
    // None
    //
    // Parameters
    // ------------
    // ManipulationStartedEventArgs obj  Manipulation event arguments
    //*************************************************
    private void ManipulationRecognizer_ManipulationCompleted(ManipulationCompletedEventArgs obj)
    {
        IsManipulating = false;
        //Sets the tag of the selection lights parent to unselected
        GazeManager.Instance.TrainingBoxLightParent.tag = "unselectedLight";
        //Deselects the manipulating object and the transiton to the tap selection gesture is initiated
        GazeManager.Instance.TrainingBoxLightParent.SendMessageUpwards("Deselect");
    }

    //6
    //*************************************************
    // ManipulationRecognizer_ManipulationCanceled function
    // This function is called when the manipulation event gets canceled
    //
    // Return Value
    // ------------
    // None
    //
    // Parameters
    // ------------
    // ManipulationStartedEventArgs obj  Manipulation event arguments
    //*************************************************
    private void ManipulationRecognizer_ManipulationCanceled(ManipulationCanceledEventArgs obj)
    {
        IsManipulating = false;
    }

    //7
    //*************************************************
    // SelectionRecognizer_Tapped function
    // This function is called when the selection gesture recognizes a tap event
    //
    // Return Value
    // ------------
    // None
    //
    // Parameters
    // ------------
    // TappedEventArgs obj  Tap event arguments
    //*************************************************
    private void SelectionRecognizer_Tapped(TappedEventArgs obj)
    {
        //Default color of the selection lights
        Color startingColor = Color.red;

        //Defines the object that will be manipulated
        ManipulatingObject = GazeManager.Instance.FocusedTrainingBox;

        //Selected Training Box Lights Parent object
        ManipulatingObject_LightParent = GazeManager.Instance.TrainingBoxLightParent;

        TrainingBoxRoot = GazeManager.Instance.FocusedTrainingBox.transform.root.gameObject;

        int trainingBoxCount = TrainingBoxRoot.transform.childCount;

        List<GameObject> UnfocusedTrainingBoxes = new List<GameObject>();

        for (int boxes = 0; boxes < trainingBoxCount; boxes++)
        {
            if (TrainingBoxRoot.transform.GetChild(boxes).gameObject != ManipulatingObject)
                UnfocusedTrainingBoxes.Add(TrainingBoxRoot.transform.GetChild(boxes).gameObject);
        }



        //If the focused object is not null and object selection lights are red and the cursor hits an object then the object gets
        //selected and the gesture transition to manipulation initiated on the SelectionCommand.cs OnSelect function
        if (ManipulatingObject != null && SelectionCommand.Instance.SelectionColor == startingColor && GazeManager.Instance.Hit)
           GazeManager.Instance.TrainingBoxLightParent.SendMessageUpwards("OnSelect");
    }
}

