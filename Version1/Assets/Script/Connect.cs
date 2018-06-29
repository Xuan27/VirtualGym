using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Connect : MonoBehaviour {
    public static Connect Instance { get; protected set; }
    private float[] Distances;
    private GameObject[] Nodes;
    private Vector3[] OriginsPositions;
    private LineRenderer[] TrainingBoxesLineRenderers;

    // Use this for initialization
    void Start () {
        Instance = this;
        GameObject trainingBoxObject = GameObject.Find("trainingBox");
        TrainingBoxesLineRenderers = trainingBoxObject.GetComponentsInChildren<LineRenderer>();
        //Cache line renderer componenet
        for(int render = 0; render < TrainingBoxesLineRenderers.Length; render++)
        {
            TrainingBoxesLineRenderers[render].startWidth = .035f;
            TrainingBoxesLineRenderers[render].endWidth = .035f;
        }
    }

	// Update is called once per frame
	void Update () {
        if (GestureManager.Instance.IsManipulating)
        {
            Nodes = TrainingBoxInfo.Instance.Connectors;
            Distances = TrainingBoxInfo.Instance.Distance;
        }
    }

    void LateUpdate()
    {
        if (GestureManager.Instance.IsManipulating)
        {
            //Moving node
            GameObject DestinationObject = Nodes[0];
            //Origin object set to the first origin node
            GameObject OriginObject = Nodes[1];
            LineRenderer TrainingBoxLine = TrainingBoxesLineRenderers[0];
            Vector3 Destination = Nodes[0].transform.position;
            double maxDistance = 0.6;
            OriginsPositions = new Vector3[Nodes.Length - 1];

            //Destination vector points
            for (int node = 1; node < Nodes.Length; node++)
            {
                //Sets the Range bool to true to lock current connectors
                TrainingBoxInfo.Instance.Range = true;

                //If the OriginObject is not the same as the first node from the node array store next node to OriginObject
                if (OriginObject != Nodes[node])
                    OriginObject = Nodes[node];

                //Origin objects nodes vector position
                OriginsPositions[node - 1] = Nodes[node].transform.position;

                //Loops through all line render components in the game project
                for(int lineRender = 0; lineRender < Nodes.Length; lineRender++)
                {
                    //If the line render is directly related to the manipulating object the line is removed
                    if(TrainingBoxesLineRenderers[lineRender].transform.parent.name != GestureManager.Instance.ManipulatingObject.name)
                    {
                        //Use world space instead of inherit space
                        TrainingBoxesLineRenderers[lineRender].useWorldSpace = true;

                        if (TrainingBoxesLineRenderers[lineRender].transform.parent.name == OriginObject.transform.parent.parent.name)
                        {
                            TrainingBoxLine = TrainingBoxesLineRenderers[lineRender];
                            DrawLine(TrainingBoxLine, OriginObject, DestinationObject, Distances[node - 1], OriginsPositions[node - 1], Destination, maxDistance);
                        }
                        else
                            RemoveLine(TrainingBoxesLineRenderers[node]);
                    }
                    else
                        RemoveLine(TrainingBoxesLineRenderers[node]);
                }
                 
            }
        }
    }

    void DrawLine(LineRenderer lineRenderer, GameObject originNode, GameObject destinationNode, float distance, Vector3 originPoint, Vector3 destinationPoint, double maxDistance)
    {
        if (GestureManager.Instance.IsManipulating)
        {
            lineRenderer.SetPosition(0, originPoint);
            lineRenderer.SetPosition(1, destinationPoint);
            TrainingBoxInfo.Instance.Range = true;
            destinationNode.GetComponent<Renderer>().material.color = Color.yellow;
            originNode.GetComponent<Renderer>().material.color = Color.yellow;
            Debug.Log("Distance before if statement: " + distance);
            if(distance < maxDistance)
            {
                TrainingBoxInfo.Instance.Range = true;
                if (distance < maxDistance * 0.3)
                {
                    lineRenderer.material = new Material(Shader.Find("Particles/Additive"));
                    lineRenderer.startColor = Color.red;
                    lineRenderer.endColor = Color.red;
                }
                else if (distance > maxDistance * 0.7)
                {
                    lineRenderer.material = new Material(Shader.Find("Particles/Additive"));
                    lineRenderer.startColor = Color.red;
                    lineRenderer.endColor = Color.red;
                }
                else
                {
                    lineRenderer.material = new Material(Shader.Find("Particles/Additive"));
                    lineRenderer.startColor = Color.green;
                    lineRenderer.endColor = Color.green;
                }
            }
            else
            {
                Debug.Log("Distance: " + distance);
                Debug.Log("Else statement RemoveLine function call");
                RemoveLine(lineRenderer);
            }
                

        }
    }         
        
    void RemoveLine(LineRenderer lineRenderer)
    {
        TrainingBoxInfo.Instance.Range = false;
        Nodes = TrainingBoxInfo.Instance.Connectors;
        if(Nodes[0] != null)
        {
            lineRenderer.SetPosition(0, Nodes[0].transform.position);
            lineRenderer.SetPosition(1, Nodes[0].transform.position);
        }
    }
}
