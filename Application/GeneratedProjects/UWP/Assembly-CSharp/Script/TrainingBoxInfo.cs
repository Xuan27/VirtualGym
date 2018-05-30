using System.Collections.Generic;
using UnityEngine;

public class TrainingBoxInfo : MonoBehaviour
{
    public static TrainingBoxInfo Instance {get; private set;}
    public TrainingBoxInfo Script;
    public GameObject TrainingBox { get; private set; }
    private GameObject ManipulatingObject { get; set; }
    public List<GameObject> StaticObjectsList { get; private set; }
    public GameObject StaticObject { get; private set; }
    public float Angle { get; private set; }
    public float Distance { get; private set; }
    public bool IsManipulating { get; private set; }
    public int LightSize { get; private set; }
    public GameObject Manipulating_NearObject { get; private set; }
    public GameObject Static_NearObject { get; private set; }
    GameObject trainingObject;
    public Dictionary<float, TrainingBoxInfo> TrainingBoxDictionary = new Dictionary<float, TrainingBoxInfo>();

    void start()
    {
        Instance = this;
        TrainingBoxInfo[] trainingScripts = trainingObject.GetComponents<TrainingBoxInfo>();
        foreach(TrainingBoxInfo trainingScript in trainingScripts)
        {
            TrainingBoxDictionary.Add(trainingScript.Distance, trainingScript);
        }
    }

    void update()
    {
        ManipulatingObject = GestureManager.Instance.ManipulatingObject;

        //Object is Manipulating
        if (GestureManager.Instance.ManipulatingObject == this)
            IsManipulating = true;
        //Object is static
        else
            IsManipulating = false;

        //Number of light in the Light Parent
        LightSize = transform.GetChild(0).gameObject.transform.childCount;

        //Compute Angle
        float xDiff = transform.position.x - ManipulatingObject.transform.position.x;
        float yDiff = transform.position.y - ManipulatingObject.transform.position.y;
        Angle = Mathf.Atan2(yDiff, xDiff) * (180 / Mathf.PI);
        if (Angle < 0)
            Angle += 360;

        //Locate Near Objects
        if (IsManipulating)
        {
            for (int light = 0; light < LightSize; light++)
            {
                if ((Angle >= 315 || Angle < 45) && ManipulatingObject.name == "selectionLightX")
                {
                    Manipulating_NearObject = ManipulatingObject.gameObject;
                    Manipulating_NearObject.gameObject.tag = "active";
                }
                else
                    ManipulatingObject.gameObject.tag = "Untagged";

                if (Angle >= 45 && Angle < 135 && ManipulatingObject.name == "selectionLightY")
                {
                    Manipulating_NearObject = ManipulatingObject.gameObject;
                    ManipulatingObject.gameObject.tag = "active";
                }
                else
                    ManipulatingObject.gameObject.tag = "Untagged";

                if (Angle >= 135 && Angle < 225 && ManipulatingObject.name == "selectionLightNX")
                {
                    Manipulating_NearObject = ManipulatingObject.gameObject;
                    ManipulatingObject.gameObject.tag = "active";
                }
                else
                    ManipulatingObject.gameObject.tag = "Untagged";

                if (Angle >= 225 && Angle < 315 && ManipulatingObject.name == "selectionLightNY")
                {
                    Manipulating_NearObject = ManipulatingObject.gameObject;
                    ManipulatingObject.gameObject.tag = "active";
                }
                else
                    ManipulatingObject.gameObject.tag = "Untagged";
            }
            //required?
            //Distance = Vector3.Distance(StaticObject.transform.position, ManipulatingObject.transform.position);
        }
        
        else
        {
            float tempDistance = 0;
            if (Manipulating_NearObject != null)
            {
                Distance = Vector3.Distance(StaticObjectsList[0].transform.position, Manipulating_NearObject.transform.position);

                for(int obj = 0; obj < StaticObjectsList.Count; obj++)
                {
                    if (StaticObjectsList[obj] == this)
                        StaticObject = this.gameObject;
                }

                for (int lights = 0; lights < LightSize; lights++)
                {

                    if (lights != 0)
                    {
                        tempDistance = Vector3.Distance(StaticObject.transform.position, ManipulatingObject.transform.position);
                        if (tempDistance < Distance)
                        {
                            StaticObject.tag = "active";
                            Distance = tempDistance;
                        }
                        else
                            StaticObject.tag = "Untagged";
                    }
                }
            }
        }
    }
}

