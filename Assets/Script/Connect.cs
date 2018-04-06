using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Connect : MonoBehaviour {
    public static Connect Instance { get; protected set; }
    private LineRenderer lineRenderer;
    public float counter { get; private set; }
    private float distance;
    private GameObject[] connectors;

    public float lineDrawnSpeed = 5f;

	// Use this for initialization
	void Start () {
        //Cache line renderer componenet
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.startWidth = .05f;
        lineRenderer.endWidth = .05f;
        counter = 0;
        
	}

    public void setCounter() {
        counter = 0;
    }

	// Update is called once per frame
	void Update () {
        if (GestureManager.Instance.IsManipulating)
        {
            connectors = TrainingBoxManager.Instance.Connectors;
            distance = TrainingBoxManager.Instance.tempDistance;
            if (connectors != null) {
                Vector3 origin = connectors[0].transform.position;
                Vector3 destination = connectors[1].transform.position;
                if (distance > 0 && distance < 0.5)
                {
                    lineRenderer.SetPosition(0, origin);
                    lineRenderer.SetPosition(1, destination);
                    if(distance < 0.25)
                    {
                        lineRenderer.material.color = Color.red;
                    }
                }
                else
                {
                    lineRenderer.SetPosition(0, origin);
                    lineRenderer.SetPosition(1, origin);
                }
            }

        }
        
    }
}
