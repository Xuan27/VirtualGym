using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Linq;

#if WINDOWS_UWP
using Windows.Storage;
using Windows.System;
using System.Threading.Tasks;
using Windows.Storage.Streams;
#endif

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
    public void WriteInfo()
    {
        NodeList current = Head;
        while (current.Next != null)
        {
            current = current.Next;
            using (StreamWriter writer = File.AppendText("Error.txt")) { writer.WriteLine(current.GetErrors()); }
        }
    }
}

//https://forum.unity.com/threads/move-gameobject-along-a-given-path.455195/
public class PathFollower : MonoBehaviour {
    Node[] PathNode;
    NodeError[] ErrorNode;

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
    float[] ErrorArray;
    float frameXPosition;
    float frameYPosition;
    Vector3 StartPosition;
    Vector3 framePosition0;
    Vector3 framePosition1;
    // Use this for initialization
    LinkedList errorList = new LinkedList();

    void Start () {
        PathNode = GetComponentsInChildren<Node>();
        ErrorNode = GetComponentsInChildren<NodeError>();
        PlayerLine = Player.GetComponent<LineRenderer>();
        FramesLines = WindowFrames.GetComponentsInChildren<LineRenderer>();
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
            errorList.InsertInfo(objPosition, Error, HoloToolkit.Unity.InputModule.CollisionBound.Instance.FinalCount);
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
        double posElem0X;
        double posElem0Y;
        //Player line render
        double posElem1X = Math.Cos(180) + Player.transform.position.x;
        double posElem1Y = Math.Sin(180) + Player.transform.position.y;
        Vector3 posElem1 = new Vector3((float)posElem1X, (float)posElem1Y, Player.transform.position.z);
        PlayerLine.SetPosition(0, Player.transform.position);
        PlayerLine.SetPosition(1, posElem1);

        //Windows frames line renders
        for(int line = 0; line < FramesLines.Length; line++)
        {
            if (line % 2 == 1)
            {
                posElem0X = (float)(Player.transform.position.x + 0.4);
                posElem0Y = (float)(Player.transform.position.y - 0.1);
                posElem1X = Math.Cos(180) + posElem0X;
                posElem1Y = Math.Sin(180) + posElem0Y;
            }

            else
            {
                posElem0X = (float)(Player.transform.position.x - 0.4);
                posElem0Y = (float)(Player.transform.position.y + 0.5);
                posElem1X = Math.Cos(180) + posElem0X;
                posElem1Y = Math.Sin(180) + posElem0Y;
            }

            framePosition0 = new Vector3((float)posElem0X, (float)posElem0Y, Player.transform.position.z);
            framePosition1 = new Vector3((float)posElem1X, (float)posElem1Y, Player.transform.position.z);

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

    void OnDestroy()
    {
        errorList.WriteInfo();
    }
}
