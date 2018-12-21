using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HoloToolkit.Unity.InputModule
{
    public class CollisionBound : MonoBehaviour
    {
        public int FinalCount { get; protected set; }
        //Conditions if the collision box(TrainingBox) is within training bounds path
        public bool TrainingRange { get; protected set; }
        //Gameobject of the initial training sphere
        private GameObject InitialSphere { get; set; }
        public static CollisionBound Instance { get; protected set; }
        public int CarryCount { get; protected set; }
        public Text counter;
       
        public bool StartTraining { get; private set; }
        //If the training module is dragged beyond the path the guide will be set back to node 0
        public bool ResetTraining { get; set; }

        

        Color b0b0 = new Color32(11, 218, 3, 255);

        void Start()
        {
            Instance = this;
            CarryCount = 0;
            FinalCount = 0;
            TrainingRange = true;
            StartTraining = false;
            ResetTraining = false;
        }

        Component[] selectionLights = null;
        //On training box rigid body collision with training bounds (spheres/plane) collision mesh
        void OnTriggerEnter(Collider boxCollider)
        {
            //Component array of training box selection lights childrens
            selectionLights = boxCollider.attachedRigidbody.gameObject.transform.GetChild(0).GetComponentsInChildren(typeof(Transform));
            //Conditions if the colliding rigid body name is the same as the object being dragged
            if (boxCollider.attachedRigidbody.gameObject.name == HandDraggable.Instance.HostTransform.gameObject.name)
            {
                //Collision on sphere mesh collider
                if (transform.parent.name == "Sphere")
                {
                    CollisionBound.Instance.TrainingRange = true;

                    /**********Repetition Start**********/
                    if (transform.name == "leftStart" || transform.name == "rightStart")
                    {
                        CollisionBound.Instance.StartTraining = true;
                        CollisionBound.Instance.InitialSphere = transform.gameObject;
                        for (int lhtC = 1; lhtC < selectionLights.Length; lhtC++)
                            selectionLights[lhtC].gameObject.GetComponent<Renderer>().material.color = b0b0;
                    }

                    /**********Repetition End**********/
                    if (transform.name == "leftFinal" || transform.name == "rightFinal")
                    {
                        Debug.Log(CollisionBound.Instance.CarryCount);
                        if (CollisionBound.Instance.CarryCount == 1)
                        {
                            Debug.Log(CollisionBound.Instance.FinalCount);
                            CollisionBound.Instance.FinalCount += CollisionBound.Instance.CarryCount;
                            counter.text = "Repetitions: " + CollisionBound.Instance.FinalCount.ToString();
                            CollisionBound.Instance.CarryCount--;
                            if (CollisionBound.Instance.InitialSphere != null)
                            {
                                CollisionBound.Instance.InitialSphere.GetComponent<Renderer>().material.color = Color.green;
                                for (int lhtC = 1; lhtC < selectionLights.Length; lhtC++)
                                    selectionLights[lhtC].gameObject.GetComponent<Renderer>().material.color = Color.red;
                            }
                        }
                    }
                }
                //Collision on Plane mesh collider
                else if (transform.parent.name == "Plane")
                    transform.GetComponent<Renderer>().material.color = Color.green;
            }
        }

        void OnTriggerStay(Collider boxCollider)
        {
            counter.text = "Repetitions: " + CollisionBound.Instance.FinalCount.ToString();
        }

        void OnTriggerExit(Collider boxCollider)
        {
            
            selectionLights = boxCollider.attachedRigidbody.gameObject.transform.GetChild(0).GetComponentsInChildren(typeof(Transform));
            //Conditions if the colliding rigid body name is the same as the object being dragged
            if (boxCollider.attachedRigidbody.gameObject.name == HandDraggable.Instance.HostTransform.gameObject.name)
            {
                /**********If the box collider exits the training path**********/
                if (transform.parent.name == "Plane")
                {
                    CollisionBound.Instance.ResetTraining = true;
                    CollisionBound.Instance.TrainingRange = false;

                    PlaneMaterial();

                    if (CollisionBound.Instance.CarryCount == 1 && CollisionBound.Instance.InitialSphere != null)
                    {
                        //Initial sphere material color
                        CollisionBound.Instance.InitialSphere.GetComponent<Renderer>().material.color = Color.red;
                        PlaneMaterial();
                        CollisionBound.Instance.CarryCount--;
                    }
                }

                counter.text = "Repetitions: " + CollisionBound.Instance.FinalCount.ToString();
                if (TrainingRange && (transform.name == "rightStart" || transform.name == "leftStart"))
                {
                    if (CollisionBound.Instance.CarryCount == 0)
                        CollisionBound.Instance.CarryCount++;
                } 
            }
        }

        void PlaneMaterial()
        {
            //Plane material color
            transform.GetComponent<Renderer>().material.color = Color.red;

            for (int lhtC = 1; lhtC < selectionLights.Length; lhtC++)
                selectionLights[lhtC].gameObject.GetComponent<Renderer>().material.color = Color.red;
        }
    }
}

