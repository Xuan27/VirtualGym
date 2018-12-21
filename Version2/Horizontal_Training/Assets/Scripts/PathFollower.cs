using UnityEngine;
using System.Collections.Generic;
using System.Diagnostics;

public class ProgramInfo
{
    //Files with values
    public string BoxPosition;
    public string PathPosition;
    public string Error;
    public string WindowError;
}

public class NodeList
{
    public NodeList Next;
    private ProgramInfo Value;

    public NodeList()
    {
        Value = new ProgramInfo();
    }

    public void SetInfo(string boxPosition, string error, string pathPosition, string windowError){
        Value = new ProgramInfo();
        Value.BoxPosition = boxPosition;
        Value.PathPosition = pathPosition;
        Value.Error = error;
        Value.WindowError = windowError;
    }

    public string GetBoxPosition()
    {
        return Value.BoxPosition;
    }

    public string GetErrors()
    {
        return Value.Error;
    }

    public string GetWindowErrors()
    {
        return Value.WindowError;
    }

    public string GetPathPosition()
    {
        return Value.PathPosition;
    }    
}

public class LinkedList
{
    private NodeList Head;
    private NodeList Current;
    public int Count;

    public LinkedList()
    {
        Head = new NodeList();
        Current = Head;
    }

    public void InsertInfo(string boxPosition, string error, string pathPosition, string windowError)
    {
        NodeList newNode = new NodeList();

        newNode.SetInfo(boxPosition, error, pathPosition, windowError);
              
        Current.Next = newNode;
        Current = newNode;
        Count++;
    }
    public void AddErrors2List(List<string> errorlist)
    {
        NodeList current = Head;
        while(current.Next != null) {
           errorlist.Add(current.GetErrors());
           current = current.Next;
        }
    }

    public void AddWindowErrors2List(List<string> windowErrorlist)
    {
        NodeList current = Head;
        while(current.Next != null)
        {
            windowErrorlist.Add(current.GetWindowErrors());
            current = current.Next;
        }
    }

    public void AddBoxPosition2List(List<string> boxPositionlist)
    {
        NodeList current = Head;
        while (current.Next != null)
        {
            
            boxPositionlist.Add(current.GetBoxPosition());
            current = current.Next;
        }
    }
    public void AddPathPosition2List(List<string> pathPositionlist)
    {
        NodeList current = Head;
        while (current.Next != null)
        {
            pathPositionlist.Add(current.GetPathPosition());
            current = current.Next;
        }
    }

    public void ResetNodeList()
    {
        NodeList newNodeList = new NodeList();
        Head = newNodeList;
        Current = Head;
    }
}

//https://forum.unity.com/threads/move-gameobject-along-a-given-path.455195/
public class PathFollower : MonoBehaviour {
    public static PathFollower Instance;

    Stopwatch stopwatch = new Stopwatch();

    Node[] PathNode;
    NodeError[] ErrorNode;

    [HideInInspector]
    public List<string> errorList;
    public List<string> boxPositonList;
    public List<string> pathPositionList;
    public List<string> windowErrorList;

    public GameObject Player;
    public GameObject WindowFrames;
    private LineRenderer PlayerLine;
    private LineRenderer [] FramesLines;

    private int MoveSpeed;
    public int MoveSpeedForward;
    public int MoveSpeedBackward;


    static Vector3 CurrentPositionHolder;

    Vector3 OriginalPosition;
   

    int CurrentNode;
    int Count = 0;
    bool ForwardDir = true;
    GameObject DraggingObject;
    float Timer;
    float Error = 0;
    float Angle;
    //float[] ErrorArray;
    float frameXPosition;
    Vector3 StartPosition;
    Vector3 framePosition0;
    Vector3 framePosition1;
    public float framesDistance;
    // Use this for initialization
    LinkedList errorObject = new LinkedList();

    void Start () {
        Instance = this;
        PathNode = GetComponentsInChildren<Node>();
        ErrorNode = GetComponentsInChildren<NodeError>();
        PlayerLine = Player.GetComponent<LineRenderer>();
        FramesLines = WindowFrames.GetComponentsInChildren<LineRenderer>();
        errorList = new List<string>();
        if (AzureServices.instance.ForwardSpeed != 0 && AzureServices.instance.BackwardSpeed != 0)
        {
            MoveSpeedForward = AzureServices.instance.ForwardSpeed;
            MoveSpeedBackward = AzureServices.instance.BackwardSpeed;
        }
        CheckNode();
	}
	
    void CheckNode()
    {
        Timer = 0;
        StartPosition = Player.transform.position;
        CurrentPositionHolder = PathNode[CurrentNode].transform.position;
        
        if (ForwardDir)
        {
            
            if(CurrentNode != 0)
            {
                Error = Vector3.Distance(ErrorNode[CurrentNode].transform.position, DraggingObject.transform.position);
                Angle = Vector3.Angle(DraggingObject.transform.position, ErrorNode[CurrentNode].transform.position);
                
            }
                
        }
        else
        {
            Error = Vector3.Distance(ErrorNode[CurrentNode + 2].transform.position, DraggingObject.transform.position);
            Angle = Vector3.Angle(DraggingObject.transform.position, ErrorNode[CurrentNode + 2].transform.position);
        }
        string objPosition;
        string nodePosition;
        string windowError;
        string objError;
        if (DraggingObject != null)
        {
            objPosition = CurrentNode + ", " + ForwardDir + ", " + DraggingObject.transform.position.x + ", " + DraggingObject.transform.position.y + ", " + DraggingObject.transform.position.z + ", " + HoloToolkit.Unity.InputModule.CollisionBound.Instance.FinalCount + ", " + stopwatch.Elapsed + System.Environment.NewLine ;
            nodePosition = CurrentNode + ", " + ForwardDir + ", " + CurrentPositionHolder.x + ", " + (CurrentPositionHolder.y - 0.5).ToString() + ", " + CurrentPositionHolder.z + ", " + HoloToolkit.Unity.InputModule.CollisionBound.Instance.FinalCount + ", " + stopwatch.Elapsed + System.Environment.NewLine;
            windowError = CurrentNode + ", " + ForwardDir + ", " + WindowErrorBound.Instance.FrameErrorValue.ToString() + ", " + HoloToolkit.Unity.InputModule.CollisionBound.Instance.FinalCount + ", " + stopwatch.Elapsed + System.Environment.NewLine;
            objError = CurrentNode + ", " + ForwardDir + ", " + Error.ToString() + ", " + HoloToolkit.Unity.InputModule.CollisionBound.Instance.FinalCount + ", " + stopwatch.Elapsed + System.Environment.NewLine;
            errorObject.InsertInfo(objPosition, objError, nodePosition, windowError);         
        }
    }

    void Forward()
    {
        if (HoloToolkit.Unity.InputModule.CollisionBound.Instance.ResetTraining)
        {
            if (CurrentNode != 0 && CurrentNode > 0)
            {
                CurrentNode--;
                MoveSpeed = MoveSpeedBackward;
                CheckNode();
                ForwardDir = false;
            }
            else
            {
                CurrentNode = 0;
                MoveSpeed = MoveSpeedBackward;
                CheckNode();
                ForwardDir = false;
                HoloToolkit.Unity.InputModule.CollisionBound.Instance.ResetTraining = false;
            }
        }

        else
        {
            CurrentNode++;
            MoveSpeed = MoveSpeedForward;
            CheckNode();
        }
    }

    void Backward()
    {
        CurrentNode--;
        MoveSpeed = MoveSpeedBackward;
        CheckNode();
    }

    void SwitchDir(ref bool Direction)
    {
        Direction = !Direction;
    }

    
	// Update is called once per frame
	void Update () {
        MoveSpeedForward = AzureServices.instance.ForwardSpeed;
        MoveSpeedBackward = AzureServices.instance.BackwardSpeed;
        Vector3 lastPosition = new Vector3(Player.transform.position.x, -1, Player.transform.position.z);
        PlayerLine.SetPosition(0, Player.transform.position);
        PlayerLine.SetPosition(1, lastPosition);

        for(int line = 0; line < FramesLines.Length; line++)
        {
            if (line % 2 == 1)
                frameXPosition = (float)(Player.transform.position.x + 0.5); 
            else
                frameXPosition = (float)(Player.transform.position.x - 0.5);

            framePosition0 = new Vector3(frameXPosition, Player.transform.position.y, Player.transform.position.z);
            framePosition1 = new Vector3(frameXPosition, -1, Player.transform.position.z);

            FramesLines[line].SetPosition(0, framePosition0);
            FramesLines[line].SetPosition(1, framePosition1);
        }

        //Distance between frame to use to scale the window error box collider
       
        framesDistance = Vector3.Distance(FramesLines[0].transform.GetComponent<Renderer>().bounds.center, FramesLines[1].transform.GetComponent<Renderer>().bounds.center);
        
        if (HoloToolkit.Unity.InputModule.HandDraggable.Instance.HostTransform.gameObject != null)
            DraggingObject = HoloToolkit.Unity.InputModule.HandDraggable.Instance.HostTransform.gameObject;


        if (HoloToolkit.Unity.InputModule.CollisionBound.Instance.StartTraining && HoloToolkit.Unity.InputModule.HandDraggable.Instance.isDragging && HoloToolkit.Unity.InputModule.CollisionBound.Instance.FinalCount < AzureServices.instance.Repetitions)
        {
            WindowErrorBound.Instance.WindowError.transform.position = ErrorNode[CurrentNode].transform.position;

            //Start stopwatch object to retrieve time elapsed

            if(!stopwatch.IsRunning)
                stopwatch.Start();

            Timer += Time.deltaTime * MoveSpeed;

            if (Player.transform.position != CurrentPositionHolder)
            {
                
                    Player.transform.position = Vector3.Lerp(StartPosition, CurrentPositionHolder, Timer);
            }
                

            else
            {
                if (Count == 0)
                {
                    OriginalPosition = Player.transform.position;
                }

                if ((PathNode.Length - 1 == CurrentNode || CurrentNode == 0) && Count != 0)
                    SwitchDir(ref ForwardDir);

                if (ForwardDir)
                    Forward();
                else
                    Backward();
            }
            Count++;
        }
        
        else if (HoloToolkit.Unity.InputModule.CollisionBound.Instance.FinalCount == AzureServices.instance.Repetitions && HoloToolkit.Unity.InputModule.CollisionBound.Instance.FinalCount != 0)
        {
            resetTraining();
            AzureServices.instance.azureStatusText.text = ConditonalTraining.Instance.TrainingType +  " Completed ...";
            HoloToolkit.Unity.InputModule.CollisionBound.Instance.StartTraining = false;
        }
    }

    async void createUploadList()
    {
        errorObject.AddErrors2List(errorList);
        errorObject.AddBoxPosition2List(boxPositonList);
        errorObject.AddPathPosition2List(pathPositionList);
        errorObject.AddWindowErrors2List(windowErrorList);
        await AzureServices.instance.UploadTrainingInfo(ConditonalTraining.Instance.TrainingType);
    }

    void resetTraining()
    {
       
        Player.transform.position = OriginalPosition;
        CurrentPositionHolder = OriginalPosition;
        CurrentNode = 1;
        ForwardDir = true;
        HoloToolkit.Unity.InputModule.CollisionBound.Instance.FinalCount = 0;
        CheckNode();
        createUploadList();

        errorObject.ResetNodeList();
        errorList.Clear();
        boxPositonList.Clear();
        pathPositionList.Clear();
        windowErrorList.Clear();
    }


}
