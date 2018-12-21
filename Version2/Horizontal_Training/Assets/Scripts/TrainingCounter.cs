using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainingCounter : MonoBehaviour {

	// Use this for initialization
	void Start () {
        StartCoroutine(TrainingWait());
	}

    IEnumerator TrainingWait()
    {
        print(Time.time);
        yield return new WaitForSeconds(3);
        print(Time.time);
    }
}
