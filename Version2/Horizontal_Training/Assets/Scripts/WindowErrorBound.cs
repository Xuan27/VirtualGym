using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowErrorBound : MonoBehaviour {
    public static WindowErrorBound Instance;
    public GameObject WindowError { get; set; }
    private bool FrameFlag;
    private float FramesDistance;
    public int FrameErrorValue;

	// Use this for initialization
	void Start () {
        Instance = this;
        WindowError = transform.gameObject;
        FrameFlag = false;
    }
	
	// Update is called once per frame
	void Update () {
        FramesDistance = PathFollower.Instance.framesDistance;
        
        newScale( FramesDistance);
        frameValue();
    }

    void OnTriggerExit(Collider collider)
    {
        if (HoloToolkit.Unity.InputModule.CollisionBound.Instance.StartTraining)
        {
            FrameFlag = true;
        }
            
    }

    void OnTriggerStay(Collider collider)
    {
        if (HoloToolkit.Unity.InputModule.CollisionBound.Instance.StartTraining)
        {
            FrameFlag = false;
        }
            
    }

    public void newScale(float newSize)
    {
        //Debug.Log(newSize);
        float size = transform.GetComponent<BoxCollider>().size.x;
        //Debug.Log("Size: " + size);
        Vector3 rescale = transform.lossyScale;
        //Debug.Log("Rescale Y; " + rescale.x);

        rescale.x = size * rescale.x / newSize;

        transform.localScale = rescale;

    }

    public void frameValue()
    {
        
        if (FrameFlag)
            FrameErrorValue = 1;
        
        else
            FrameErrorValue = 0;
    }
}
