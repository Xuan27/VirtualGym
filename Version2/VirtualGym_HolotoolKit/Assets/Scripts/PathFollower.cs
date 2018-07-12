using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//https://forum.unity.com/threads/move-gameobject-along-a-given-path.455195/
public class PathFollower : MonoBehaviour {
    Node[] PathNode;
    public GameObject Player;
    public float MoveSpeedForward;
    public float MoveSpeedBackward;
    private float MoveSpeed;
    float Timer;
    int CurrentNode;
    static Vector3 CurrentPositionHolder;
    Vector3 StartPosition;
    bool ForwardDir = true;
    int Count = 0;
	// Use this for initialization

	void Start () {
        PathNode = GetComponentsInChildren<Node>();
        CheckNode();
	}
	
    void CheckNode()
    {
        Timer = 0;
        StartPosition = Player.transform.position;
        CurrentPositionHolder = PathNode[CurrentNode].transform.position;
    }

    void Forward()
    {
        CurrentNode++;
        MoveSpeed = MoveSpeedForward;
        CheckNode();
        
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
        if (HoloToolkit.Unity.InputModule.CollisionBound.Instance.StartTraining)
        {
            Timer += Time.deltaTime * MoveSpeed;
        
            if(Player.transform.position != CurrentPositionHolder)
            {
                Player.transform.position = Vector3.Lerp(StartPosition, CurrentPositionHolder, Timer);
            }
        
            else
            {
                if ((PathNode.Length-1 == CurrentNode || CurrentNode == 0) && Count != 0)
                {
                    SwitchDir(ref ForwardDir);
                }
            
               if(ForwardDir)
                    Forward();
                else
                    Backward();
            }
            Count++;
        }
	}
}
