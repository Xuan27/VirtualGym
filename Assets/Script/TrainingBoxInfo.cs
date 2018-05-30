using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainingBoxInfo : MonoBehaviour
{
    public static TrainingBoxInfo Instance { get; private set; }
    public TrainingBoxInfo[] TrainingBoxObject;
    public GameObject TrainingBox { get; private set; }
    private GameObject ManipulatingObject { get; set; }
    public GameObject[] Static_NearObject;
    public GameObject StaticObject { get; private set; }
    public float Angle { get; private set; }
    public float[] Distance { get; private set; }
    public bool IsManipulatingObject { get; private set; }
    public int LightSize { get; private set; }
    public GameObject Manipulating_NearObject { get; private set; }
    public GameObject[] Connectors { get; private set; }
    GameObject manipulatingObject;
    GameObject[] staticObjects;
    public bool Range = false;


    void Start()
    {
        Instance = this;
        //Training box object root
        GameObject trainingBoxRoot = GameObject.Find("trainingBox");
        //All training box info script components from the training box object root 
        TrainingBoxObject = trainingBoxRoot.GetComponentsInChildren<TrainingBoxInfo>();
        //Gameobject array of the selected selection lights for connection
        Connectors = new GameObject[TrainingBoxObject.Length];
        //Float array of the distance between selected selection lights
        Distance = new float[TrainingBoxObject.Length - 1];
        staticObjects = new GameObject[TrainingBoxObject.Length - 1];
    }

    void Update()
    {
        GameObject ManipulatingObject = GestureManager.Instance.ManipulatingObject;
        if (ManipulatingObject != null)
        {

            int staticCounter = 0;

            for (int obj = 0; obj < TrainingBoxObject.Length; obj++)
            {
                if (ManipulatingObject.name == TrainingBoxObject[obj].gameObject.name)
                    manipulatingObject = TrainingBoxObject[obj].gameObject;
                else
                {
                    staticObjects[staticCounter] = TrainingBoxObject[obj].gameObject;
                    staticCounter++;
                }
            }
        
            for (int sta = 0; sta < staticObjects.Length; sta++)
            {
                GenerateInfo(manipulatingObject, staticObjects[sta], sta);
            }
        }
    }

    public void GenerateInfo(GameObject manipulatingObject, GameObject staticObject, int index)
    {
        
        if (GestureManager.Instance.IsManipulating)
        {
            //Compute Angle
            float xDiff = staticObject.transform.position.x - manipulatingObject.transform.position.x;
            float yDiff = staticObject.transform.position.y - manipulatingObject.transform.position.y;
            Angle = Mathf.Atan2(yDiff, xDiff) * (180 / Mathf.PI);
            if (Angle < 0)
                Angle += 360;

            LightSize = GestureManager.Instance.ManipulatingObject_LightParent.transform.childCount;

            for (int light = 0; light < GestureManager.Instance.ManipulatingObject_LightParent.transform.childCount; light++)
            {
                if ((Angle >= 315 || Angle < 45) && GestureManager.Instance.ManipulatingObject_LightParent.transform.GetChild(light).name == "selectionLightX")
                    Manipulating_NearObject = GestureManager.Instance.ManipulatingObject_LightParent.transform.GetChild(light).gameObject;
                    
                else if (Angle >= 45 && Angle < 135 && GestureManager.Instance.ManipulatingObject_LightParent.transform.GetChild(light).name == "selectionLightY")
                    Manipulating_NearObject = GestureManager.Instance.ManipulatingObject_LightParent.transform.GetChild(light).gameObject;

                else if (Angle >= 135 && Angle < 225 && GestureManager.Instance.ManipulatingObject_LightParent.transform.GetChild(light).name == "selectionLightNX")
                    Manipulating_NearObject = GestureManager.Instance.ManipulatingObject_LightParent.transform.GetChild(light).gameObject;

                else if (Angle >= 225 && Angle < 315 && GestureManager.Instance.ManipulatingObject_LightParent.transform.GetChild(light).name == "selectionLightNY")
                    Manipulating_NearObject = GestureManager.Instance.ManipulatingObject_LightParent.transform.GetChild(light).gameObject;
            }
                  
            if(!Range)
                Connectors[0] = Manipulating_NearObject;
            

            Static_NearObject = new GameObject[6];

            for(int light = 0; light < LightSize; light++)
            {
                Static_NearObject[light] = staticObject.transform.GetChild(0).transform.GetChild(light).gameObject;
            }

            float tempDistance = 0;
            if (Manipulating_NearObject != null)
            {
                Distance[index] = Vector3.Distance(Static_NearObject[0].transform.position, Manipulating_NearObject.transform.position);
                Connectors[index + 1] = Static_NearObject[0];

                for (int lights = 0; lights < LightSize; lights++)
                {
                    if (lights != 0)
                    {
                        tempDistance = Vector3.Distance(Static_NearObject[lights].transform.position, Manipulating_NearObject.transform.position);
                        if (tempDistance < Distance[index])
                        {
                            Distance[index] = tempDistance;
                            if(!Range)
                                Connectors[index+1] = Static_NearObject[lights];
                        }
                    }
                }
            }
        }
    }
}
