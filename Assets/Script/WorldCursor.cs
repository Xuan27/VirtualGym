using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldCursor : MonoBehaviour
{

    /*Private must be nested within a class and is accessible by any other type*/
    private MeshRenderer meshRenderer;
    // Use this for initialization
    //In this case the cursor object is the object with a mesh that will be rendered
    void Start()
    {
        meshRenderer = this.gameObject.GetComponentInChildren<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        // Do a raycast into the world based on the user's
        // head position and orientation.
        var origin = Camera.main.transform.position;
        var gazeDirection = Camera.main.transform.forward;

        RaycastHit hitInfo;

        if (Physics.Raycast(origin, gazeDirection, out hitInfo))
        {
            meshRenderer.enabled = true;
            transform.position = hitInfo.point;
            transform.rotation = Quaternion.FromToRotation(Vector3.up, hitInfo.normal);
        }
        else
            meshRenderer.enabled = false;
    }
}
