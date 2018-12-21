using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConditonalTraining : MonoBehaviour {
    public static ConditonalTraining Instance;
    public GameObject ConditionalSphere { get; private set; }
    public string TrainingType { get; private set; }
    GameObject NonConditionalSphere;
    GameObject Parent;

	// Use this for initialization
	void Start () {
        Instance = this;
	}

    async void OnTriggerEnter(Collider collider)
    {
        Parent = transform.parent.gameObject;
        ConditionalSphere = transform.gameObject;
        int childCount = Parent.transform.childCount;
        for(int child =0; child < childCount; child++)
        {
            if (ConditionalSphere != Parent.transform.GetChild(child).gameObject)
            {
                NonConditionalSphere = Parent.transform.GetChild(child).gameObject;
                NonConditionalSphere.GetComponent<Renderer>().material.color = Color.white;
            }
            else
                ConditionalSphere.GetComponent<Renderer>().material.color = Color.white;
        }

        transform.GetComponent<Renderer>().material.color = Color.green;
        ConditonalTraining.Instance.TrainingType = ConditionalSphere.gameObject.name;
        Debug.Log(ConditonalTraining.Instance.TrainingType);
        
        await AzureServices.instance.DownloadTrainingSettings(ConditonalTraining.Instance.TrainingType);        
    }
}
