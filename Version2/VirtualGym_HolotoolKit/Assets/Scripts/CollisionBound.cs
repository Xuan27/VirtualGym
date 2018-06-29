using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HoloToolkit.Unity.InputModule
{
    public class CollisionBound : MonoBehaviour
    {


        public int FinalCount { get; protected set; }
        public bool TrainingRange { get; protected set; }
        public static CollisionBound Instance { get; protected set; }
        public int CarryCount { get; protected set; }
        public Text counter;
        private GameObject InitialSphere { get; set; }

        void Start()
        {
            Instance = this;
            CarryCount = 0;
            FinalCount = 0;
            TrainingRange = true;

        }

        void OnTriggerEnter(Collider boxCollider)
        {
            if (boxCollider.attachedRigidbody.gameObject.name == HandDraggable.Instance.HostTransform.gameObject.transform.GetChild(0).name)
            {
                if (transform.parent.name == "Sphere")
                {
                    CollisionBound.Instance.TrainingRange = true;

                    if (transform.name == "leftStart" || transform.name == "rightStart")
                    {
                        CollisionBound.Instance.InitialSphere = transform.gameObject;
                        CollisionBound.Instance.InitialSphere.GetComponent<Renderer>().material.color = Color.yellow;
                    }


                    if (transform.name == "leftFinal" || transform.name == "rightFinal")
                    {
                        if (CollisionBound.Instance.CarryCount == 1)
                        {
                            CollisionBound.Instance.FinalCount += CollisionBound.Instance.CarryCount;
                            counter.text = "Repetitions: " + CollisionBound.Instance.FinalCount.ToString();
                            CollisionBound.Instance.CarryCount--;
                            if (CollisionBound.Instance.InitialSphere != null)
                                CollisionBound.Instance.InitialSphere.GetComponent<Renderer>().material.color = Color.green;
                        }
                    }
                }
                else if (transform.parent.name == "Plane")
                {
                    transform.GetComponent<Renderer>().material.color = Color.green;
                }
            }
        }

        void OnTriggerStay(Collider boxCollider)
        {
            counter.text = "Repetitions: " + CollisionBound.Instance.FinalCount.ToString();
        }

        void OnTriggerExit(Collider boxCollider)
        {
            
            if (boxCollider.attachedRigidbody.gameObject.name == HandDraggable.Instance.HostTransform.gameObject.transform.GetChild(0).name) { 
                if (transform.parent.name == "Plane")
                {
                    transform.GetComponent<Renderer>().material.color = Color.red;
                    CollisionBound.Instance.TrainingRange = false;
                    if (CollisionBound.Instance.CarryCount == 1)
                    {
                        if (CollisionBound.Instance.InitialSphere != null)
                        {
                            CollisionBound.Instance.InitialSphere.GetComponent<Renderer>().material.color = Color.red;
                            transform.GetComponent<Renderer>().material.color = Color.red;
                        }
                        CollisionBound.Instance.CarryCount--;
                    }
                }
                counter.text = "Repetitions: " + CollisionBound.Instance.FinalCount.ToString();
                if (TrainingRange && (transform.name == "rightStart" || transform.name == "leftStart"))
                {
                    CollisionBound.Instance.CarryCount++;
                    //CollisionBound.Instance.CarryCount = (int)Mathf.Pow(CollisionBound.Instance.CarryCount, 0);
                }
            }            
        }
    }
}

