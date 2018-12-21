using UnityEngine;
using System.Collections.Generic;

public class ProgramInfo
{
    public string Position;
    public float Error;
    public int Repetition;
}

public class NodeList
{
    public NodeList Next;
    private ProgramInfo Value;

    public void SetInfo(string position, float error, int rep){
        Value = new ProgramInfo();
        Value.Position = position;
        Value.Error = error;
        Value.Repetition = rep;
    }

    public string GetPosition()
    {
        return Value.Position;
    }

    public float GetErrors()
    {
        return Value.Error;
    }

    public int GetRepetitions()
    {
        return Value.Repetition;
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

    public void InsertInfo(string position, float error, int rep)
    {
        NodeList newNode = new NodeList();
        
        newNode.SetInfo(position, error, rep);
        
        Current.Next = newNode;
        Current = newNode;
        Count++;
    }
    public void AddErrors(List<float> errorlist)
    {
        NodeList current = Head;
        while(current.Next != null) {
            current = current.Next;
            errorlist.Add(current.GetErrors());
        }
    }
}

//https://forum.unity.com/threads/move-gameobject-along-a-given-path.455195/
public class PathFollower : MonoBehaviour {
    public static PathFollower Instance;

    Node[] PathNode;
    NodeError[] ErrorNode;

    [HideInInspector]
    public List<float> errorList;

    public GameObject Player;
    public GameObject WindowFrames;
    private LineRenderer PlayerLine;
    private LineRenderer [] FramesLines;

    private float MoveSpeed;
    public float MoveSpeedForward;
    public float MoveSpeedBackward;

    static Vector3 CurrentPositionHolder;
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
    // Use this for initialization
    LinkedList errorObject = new LinkedList();

    void Start () {
        Instance = this;
        PathNode = GetComponentsInChildren<Node>();
        ErrorNode = GetComponentsInChildren<NodeError>();
        PlayerLine = Player.GetComponent<LineRenderer>();
        FramesLines = WindowFrames.GetComponentsInChildren<LineRenderer>();
        errorList = new List<float>();
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
        if (DraggingObject != null)
        {
            objPosition = DraggingObject.transform.position.x + ", " + DraggingObject.transform.position.y + " , " + DraggingObject.transform.position.z;
            errorObject.InsertInfo(objPosition, Error, HoloToolkit.Unity.InputModule.CollisionBound.Instance.FinalCount);
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

        if(HoloToolkit.Unity.InputModule.HandDraggable.Instance.HostTransform.gameObject != null)
            DraggingObject = HoloToolkit.Unity.InputModule.HandDraggable.Instance.HostTransform.gameObject;
        
        if (HoloToolkit.Unity.InputModule.CollisionBound.Instance.StartTraining && HoloToolkit.Unity.InputModule.HandDraggable.Instance.isDragging)
        {
            Timer += Time.deltaTime * MoveSpeed;
        
            if(Player.transform.position != CurrentPositionHolder)
                Player.transform.position = Vector3.Lerp(StartPosition, CurrentPositionHolder, Timer);
            
            else
            {
               if ((PathNode.Length-1 == CurrentNode || CurrentNode == 0) && Count != 0)
                    SwitchDir(ref ForwardDir);
                
               if(ForwardDir)
                    Forward();
                else
                    Backward();
            }
            Count++;
        }
	}

    async void OnDestroy()
    {
        errorObject.AddErrors(errorList);
        await AzureServices.instance.UploadListToAzureAsync();
    }
}
