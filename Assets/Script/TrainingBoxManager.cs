using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TrainingBoxManager : MonoBehaviour {
    public static TrainingBoxManager Instance { get; private set; }

    public GameObject[] Connectors { get; private set; }

    public float tempDistance { get; private set; }

    private GameObject trainingBoxRoot;

    private GameObject movingObject;

    private GameObject selectionLightParent;

    private GameObject[] unselectedLightParent;

    private GameObject selectedTrainingBox;

    private List<GameObject> unselectedTrainingBox;

    private List<GameObject> selectedLights;

    private List<GameObject> unselectedLights;
   
	// Use this for initialization
	void Start () {
        Instance = this;
        MoveBoxCommand();
	}

    void Update()
    {
        
        //Get children Training box 1 and 2
        if (GestureManager.Instance.IsManipulating)
        {
            movingObject = GestureManager.Instance.ManipulatingObject;

            trainingBoxRoot = GameObject.FindGameObjectWithTag("trainingBox");

            //Number of training boxes
            int trainingBoxesNum = trainingBoxRoot.transform.childCount;

            //Selection Lights Parent by tag name
            selectionLightParent = GameObject.FindGameObjectWithTag("selectionLight");
            unselectedLightParent = GameObject.FindGameObjectsWithTag("unselectedLight");

            //Training boxes from Training box root
            for (int box = 0; box < trainingBoxesNum; box++)
            {
                //Determine Static Object and Training Boxes
                if (trainingBoxRoot.transform.GetChild(box).gameObject.name != movingObject.name && selectionLightParent != null) {

                    //Training Box
                    selectedTrainingBox = selectionLightParent.transform.parent.gameObject;
                    unselectedTrainingBox = new List<GameObject>();
                    
                    for (int unBox = 0; unBox < trainingBoxesNum - 1; unBox++)
                    {
                        unselectedTrainingBox.Add(unselectedLightParent[unBox].transform.parent.gameObject);
                        int lightSize = unselectedLightParent[unBox].transform.childCount;
                        //Get Selection Lights
                        if (unselectedTrainingBox[unBox] != null && movingObject != null && selectionLightParent != null)
                        {
                            unselectedLights = new List<GameObject>();
                            selectedLights = new List<GameObject>();
                            
                            //Training Boxes Selection Lights objects and positions
                            for (int seLight = 0; seLight < lightSize; seLight++)
                            {
                                //Selection Lights
                                unselectedLights.Add(unselectedLightParent[unBox].transform.GetChild(seLight).gameObject);
                                selectedLights.Add(selectionLightParent.transform.GetChild(seLight).gameObject);

                            }
                        }

                        Connectors = GetDistance(ref selectedLights, ref unselectedLights, ref selectedTrainingBox, ref unselectedTrainingBox, ref lightSize, ref trainingBoxesNum);
                    }

                    
                    
                }
                
            }
        }
    }

    void MoveBoxCommand()
    {
        if (GestureManager.Instance.IsManipulating)
        {
            GestureManager.Instance.Transition(GestureManager.Instance.ManipulationRecognizer, true);
        }
    }



    GameObject unselectedBox;
    GameObject movingNearLight;
    GameObject staticNearLight;
    float distance = 0;
    
    float angle = 0;

    //Arguments selected and unselected lights list, size, and static object
    GameObject[] GetDistance(ref List<GameObject> selectedLight, ref List<GameObject> unselectedLight, ref GameObject movingObject, ref List<GameObject> staticObject, ref int lightSize, ref int staticSize) {
        GameObject[] connectionObjects;

        for(int trainingBox = 0; trainingBox < staticSize -1; trainingBox++)
        {
            unselectedBox = staticObject[trainingBox].gameObject;
            float xDiff = unselectedBox.transform.position.x - movingObject.transform.position.x;
            float yDiff = unselectedBox.transform.position.y - movingObject.transform.position.y;
            angle = Mathf.Atan2(yDiff, xDiff) * (180 / Mathf.PI);
            if(angle < 0)
                angle += 360;
        }

        for(int nearSelLight = 0; nearSelLight < lightSize; nearSelLight ++)
        {
            if ((angle >= 315 || angle < 45) && selectedLight[nearSelLight].name == "selectionLightX")
            {
                movingNearLight = selectedLight[nearSelLight].gameObject;
            }

            if (angle >= 45 && angle < 135 && selectedLight[nearSelLight].name == "selectionLightY")
            {
                movingNearLight = selectedLight[nearSelLight].gameObject;
            }

            if (angle >= 135 && angle < 225 && selectedLight[nearSelLight].name == "selectionLightNX")
            {
                movingNearLight = selectedLight[nearSelLight].gameObject;
            }

            if (angle >= 225 && angle < 315 && selectedLight[nearSelLight].name == "selectionLightNY")
            {
                movingNearLight = selectedLight[nearSelLight].gameObject;
            }
        }

        tempDistance = Vector3.Distance(unselectedLight[0].transform.position, movingNearLight.transform.position);
        staticNearLight = unselectedLight[0].gameObject;

        for (int nearUnselLight = 0; nearUnselLight < lightSize; nearUnselLight++) {
            
            if (nearUnselLight != 0)
            {
                distance = Vector3.Distance(unselectedLight[nearUnselLight].transform.position, movingNearLight.transform.position);
                if (distance < tempDistance)
                {
                    staticNearLight = unselectedLight[nearUnselLight].gameObject;
                    tempDistance = distance;
                }  
            }
        }

        connectionObjects = new GameObject[2];
        connectionObjects[0] = movingNearLight;
        connectionObjects[1] = staticNearLight;

        return connectionObjects;
    }
}
