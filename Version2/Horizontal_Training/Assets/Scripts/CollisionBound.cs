using UnityEngine;
using UnityEngine.UI;

namespace HoloToolkit.Unity.InputModule
{
    public class CollisionBound : MonoBehaviour
    {


        public int FinalCount { get; set; }
        public bool TrainingRange { get; protected set; }
        public static CollisionBound Instance { get; protected set; }
        public int CarryCount { get; protected set; }
        public Text counter;
        //Sound
        //public AudioClip AudioInteraction;
        private GameObject InitialSphere { get; set; }
        public bool StartTraining { get; set; }
        public bool ResetTraining { get; set; }
        public Color Material47 { get; set; }
        Component[] selectionLights = null;

        void Start()
        {
            Instance = this;
            CarryCount = 0;
            FinalCount = 0;
            TrainingRange = true;
            CollisionBound.Instance.StartTraining = false;
            ResetTraining = false;
            Material47 = new Color32(0, 168, 243, 255);
        }

        void OnTriggerEnter(Collider boxCollider)
        {
            
            selectionLights = boxCollider.attachedRigidbody.gameObject.transform.GetChild(0).GetComponentsInChildren(typeof(Transform));
            if (boxCollider.attachedRigidbody.gameObject.name == HandDraggable.Instance.HostTransform.gameObject.name)
            {
                if (transform.parent.name == "Sphere")
                {
                    CollisionBound.Instance.TrainingRange = true;
                    //Sound
                    /*GetComponent<AudioSource>().playOnAwake = false;
                    GetComponent<AudioSource>().clip = AudioInteraction;*/

                    if (transform.name == "leftStart" || transform.name == "rightStart")
                    {
                        CollisionBound.Instance.InitialSphere = transform.gameObject;
                        CollisionBound.Instance.StartTraining = true;

                        /* Sound
                        GetComponent<AudioSource>().Play();*/
                       
                        for(int lhtC = 1; lhtC < selectionLights.Length; lhtC++)
                        {
                            selectionLights[lhtC].gameObject.GetComponent<Renderer>().material.color = Material47;
                        }
                    }                    
                    if (transform.name == "leftFinal" || transform.name == "rightFinal")
                    {
                        
                        if (CollisionBound.Instance.CarryCount == 1)
                        {
                            GetComponent<AudioSource>().Play();
                            CollisionBound.Instance.FinalCount += CollisionBound.Instance.CarryCount;
                            counter.text = "Repetitions: " + CollisionBound.Instance.FinalCount.ToString();
                            CollisionBound.Instance.CarryCount--;
                            if (CollisionBound.Instance.InitialSphere != null)
                            {
                                CollisionBound.Instance.InitialSphere.GetComponent<Renderer>().material.color = Color.green;
                                for (int lhtC = 1; lhtC < selectionLights.Length; lhtC++)
                                {
                                    selectionLights[lhtC].gameObject.GetComponent<Renderer>().material.color = Color.red;
                                }
                            }
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