using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Connect : MonoBehaviour {
    public static Connect Instance { get; protected set; }
    private LineRenderer lineRenderer;
    private float distance;
    private GameObject[] nodes;

    // Use this for initialization
    void Start () {
        //this.gameObject.SetActive(false);
        //Cache line renderer componenet
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.startWidth = .035f;
        lineRenderer.endWidth = .035f;
    }

	// Update is called once per frame
	void Update () {
        if (GestureManager.Instance.IsManipulating)
        {
            nodes = TrainingBoxManager.Instance.Connectors;
            distance = TrainingBoxManager.Instance.tempDistance;
            if (nodes != null) {
                Vector3 origin = nodes[0].transform.position;
                Vector3 destination = nodes[1].transform.position;
                double maxDistance = 0.6;
                /*nodes[0].GetComponent<Renderer>().material.color = Color.yellow;
                nodes[1].GetComponent<Renderer>().material.color = Color.yellow;*/

                Debug.Log("Distance :" + distance);
                if (distance > 0 && distance < maxDistance)
                {
                    this.gameObject.SetActive(true);
                    lineRenderer.SetPosition(0, origin);
                    lineRenderer.SetPosition(1, destination);
                    nodes[0].GetComponent<Renderer>().material.color = Color.yellow;
                    nodes[1].GetComponent<Renderer>().material.color = Color.yellow;
                    if (distance < maxDistance*0.3)
                    {
                        lineRenderer.material = new Material(Shader.Find("Particles/Additive"));
                        lineRenderer.startColor = Color.red;
                        lineRenderer.endColor = Color.red;
                    }
                    else if(distance > maxDistance * 0.7)
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
                    lineRenderer.SetPosition(0, origin);
                    lineRenderer.SetPosition(1, origin);
                }
            }

        }
    }
}
