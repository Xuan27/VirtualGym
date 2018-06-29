using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainingBoxInfo : MonoBehaviour
{
    public static TrainingBoxInfo Instance { get; private set; }
    public TrainingBoxInfo[] TrainingBoxObject;
    public GameObject TrainingBox { get; private set; }
    private GameObject ManipulatingObject { get; set; }
    
    public GameObject StaticObject { get; private set; }
    
    //Gameobject array of the selected selection lights for connection: origin and destination
    public GameObject[] Connectors { get; private set; }
    //Float array of the distance between origin selection light and destination selection light
    public float[] Distance { get; private set; }

    /*Gameobjects of training box modules for connection*/
    GameObject destinationObject;
    GameObject[] originObjects;

    /*Gameobjects of training boxes selection lights for connection*/
    GameObject destinationNearObject;
    GameObject[] originNearObject;

    float Angle;
    public bool Range = false;



    void Start()
    {
        Instance = this;
        //Training box object root
        GameObject trainingBoxRoot = GameObject.Find("trainingBox");
        //The training box info script components from the training box object root 
        TrainingBoxObject = trainingBoxRoot.GetComponentsInChildren<TrainingBoxInfo>();

        Connectors = new GameObject[TrainingBoxObject.Length];
        Distance = new float[TrainingBoxObject.Length - 1];
        originObjects = new GameObject[TrainingBoxObject.Length - 1];
    }

    void Update()
    {
        //Training box module being manipulated
        GameObject manipulatingTrainingBox = GestureManager.Instance.ManipulatingObject;
        if (manipulatingTrainingBox != null)
        {
            int originCounter = 0;

            //Stores training box objects to designated variables, not selection lights
            for (int obj = 0; obj < TrainingBoxObject.Length; obj++)
            {
                if (manipulatingTrainingBox.name == TrainingBoxObject[obj].gameObject.name)
                    destinationObject = TrainingBoxObject[obj].gameObject;
                else
                {
                    originObjects[originCounter] = TrainingBoxObject[obj].gameObject;
                    originCounter++;
                }
            }
        
            //Calls the GenerateInfo function 
            for (int sta = 0; sta < originObjects.Length; sta++)
            {
                GenerateInfo(destinationObject, originObjects[sta], sta);
            }
        }
    }

    //*************************************************
    // GenerateInfo function
    // This function computes the angle and distance 
    // between a manipulating training box module and a
    // static module.
    //
    // Return Value
    // ------------
    // None
    //
    // Parameters
    // ------------
    // GameObject   destination  Training box module being manipulated
    // GameObject   origin       Static training box module
    // int          index        Index of the static training box module
    public void GenerateInfo(GameObject destination, GameObject origin, int index)
    {
        Transform lightParent = GestureManager.Instance.ManipulatingObject_LightParent.transform;
        int lightSize;

        if (GestureManager.Instance.IsManipulating)
        {
            //Compute Angle
            float xDiff = origin.transform.position.x - destination.transform.position.x;
            float yDiff = origin.transform.position.y - destination.transform.position.y;
            Angle = Mathf.Atan2(yDiff, xDiff) * (180 / Mathf.PI);
            if (Angle < 0)
                Angle += 360;

            //Number of selection lights in training box:6
            lightSize = lightParent.childCount;

            //Select destination selection light gameobject
            for (int light = 0; light < lightSize; light++)
            {
                if ((Angle >= 315 || Angle < 45) && lightParent.GetChild(light).name == "selectionLightX")
                    destinationNearObject = lightParent.GetChild(light).gameObject;
                    
                else if (Angle >= 45 && Angle < 135 && lightParent.GetChild(light).name == "selectionLightY")
                    destinationNearObject = lightParent.GetChild(light).gameObject;

                else if (Angle >= 135 && Angle < 225 && lightParent.GetChild(light).name == "selectionLightNX")
                    destinationNearObject = lightParent.GetChild(light).gameObject;

                else if (Angle >= 225 && Angle < 315 && lightParent.GetChild(light).name == "selectionLightNY")
                    destinationNearObject = lightParent.GetChild(light).gameObject;
            }
             
            if(!Range)
                Connectors[0] = destinationNearObject;

            originNearObject = new GameObject[6];

            for(int light = 0; light < lightSize; light++)
            {
                originNearObject[light] = origin.transform.GetChild(0).transform.GetChild(light).gameObject;
            }

            float tempDistance = 0;
            if (destinationNearObject != null)
            {
                Distance[index] = Vector3.Distance(originNearObject[0].transform.position, destinationNearObject.transform.position);
                Connectors[index + 1] = originNearObject[0];

                for (int lights = 0; lights < lightSize; lights++)
                {
                    if (lights != 0)
                    {
                        tempDistance = Vector3.Distance(originNearObject[lights].transform.position, destinationNearObject.transform.position);
                        if (tempDistance < Distance[index])
                        {
                            Distance[index] = tempDistance;
                            if(!Range)
                                Connectors[index+1] = originNearObject[lights];
                        }
                    }
                }
            }
        }
    }
}
