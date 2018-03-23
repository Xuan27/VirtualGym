using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.WSA.Input;

public class CursorFeedback : MonoBehaviour {

    [Tooltip("Drag a prefab object to display when a pathing enabled Interactible is detected.")]
    public GameObject PathingDetectedAsset;
    private GameObject pathingDetectedGameObject;

    [Tooltip("Drag a prefab object to parent the feedback assets.")]
    public GameObject FeedbackParent;



    // Use this for initialization
    void Awake () {
        if (PathingDetectedAsset != null)
        {
            pathingDetectedGameObject = InstantiatePrefab(PathingDetectedAsset);
        }
    }

    private GameObject InstantiatePrefab(GameObject inputPrefab)
    {
        GameObject instantiatedPrefab = null;

        if (inputPrefab != null && FeedbackParent != null)
        {
            instantiatedPrefab = GameObject.Instantiate(inputPrefab);
            // Assign parent to be the FeedbackParent
            // so that feedback assets move and rotate with this parent.
            instantiatedPrefab.transform.parent = FeedbackParent.transform;

            // Set starting state of gameobject to be inactive.
            instantiatedPrefab.gameObject.SetActive(false);
        }

        return instantiatedPrefab;
    }

    // Update is called once per frame
    void Update () {
        UpdatePathDetectedState();
    }

    private void UpdatePathDetectedState()
    {
        if (pathingDetectedGameObject == null)
        {
            return;
        }

        if (GestureManager.Instance.ManipulationRecognizer != null)
        {
            pathingDetectedGameObject.SetActive(false);
            return;
        }

        pathingDetectedGameObject.SetActive(true);
    }
}
