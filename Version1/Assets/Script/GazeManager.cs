using UnityEngine;
using System.Collections.Generic;
using UnityEngine.XR.WSA.Input;

/// <summary>
/// GazeManager determines the location of the user's gaze, hit position and normals.
/// </summary>
public class GazeManager : MonoBehaviour
    {
    public static GazeManager Instance { get; private set; }

    //Training box group of 6 selection lights
    public GameObject TrainingBoxLightParent { get; private set; }

    //Training box containing selection lights and selection box
    public GameObject TrainingBox { get; private set; }

    //Object that collides with the cursor
    public GameObject FocusedTrainingBox { get; private set; }

    [Tooltip("Maximum gaze distance for calculating a hit.")]
        public float MaxGazeDistance = 2.0f;

        [Tooltip("Select the layers raycast should target.")]
        public LayerMask RaycastLayerMask = Physics.DefaultRaycastLayers;

        /// <summary>
        /// Physics.Raycast result is true if it hits a Hologram.
        /// </summary>
        public bool Hit { get; private set; }

        /// <summary>
        /// HitInfo property gives access
        /// to RaycastHit public members.
        /// </summary>
        public RaycastHit HitInfo { get; private set; }

        /// <summary>
        /// Position of the user's gaze.
        /// </summary>
        public Vector3 Position { get; private set; }

        /// <summary>
        /// RaycastHit Normal direction.
        /// </summary>
        public Vector3 Normal { get; private set; }

        private GazeStabilizer gazeStabilizer;

    /// <summary>
    /// Vector with gaze origin coordinates
    /// </summary>
    private Vector3 gazeOrigin;

    /// <summary>
    /// Vector with gaze direction coordinates
    /// </summary>
    private Vector3 gazeDirection;

    void Start()
    {
       Instance = this;
    }

    void Awake()
    {
        gazeStabilizer = GetComponent<GazeStabilizer>();
    }

        private void Update()
        {
            gazeOrigin = Camera.main.transform.position;

            gazeDirection = Camera.main.transform.forward;

            gazeStabilizer.UpdateHeadStability(gazeOrigin, Camera.main.transform.rotation);

            gazeOrigin = gazeStabilizer.StableHeadPosition;

            UpdateRaycast();
        }

        /// <summary>
        /// Calculates the Raycast hit position and normal.
        /// </summary>
        private void UpdateRaycast()
        {
            RaycastHit hitInfo;

            Hit = Physics.Raycast(gazeOrigin, gazeDirection, out hitInfo, MaxGazeDistance, RaycastLayerMask);

            HitInfo = hitInfo;

        if (Hit)
        {
            // If raycast hit a hologram...
            Position = hitInfo.point;
            Normal = hitInfo.normal;
            FocusedTrainingBox = hitInfo.collider.transform.parent.transform.parent.gameObject;

            TrainingBoxLightParent = FocusedTrainingBox.transform.GetChild(0).gameObject;
            TrainingBox = FocusedTrainingBox.transform.GetChild(1).gameObject;
        }

        else
        {
            // If raycast did not hit a hologram...
            // Save defaults ...               
            Position = gazeOrigin + (gazeDirection * MaxGazeDistance);
            Normal = gazeDirection;

            //Avoids null objects, so as to program start when no object has been hit ...
            if(FocusedTrainingBox != null)
            {
                TrainingBox = FocusedTrainingBox.transform.GetChild(1).gameObject;
                TrainingBoxLightParent = FocusedTrainingBox.transform.GetChild(0).gameObject;
            }
        }
    }
}
