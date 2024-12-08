using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

namespace KayosStudios.AsteroidQuest
{
    public class CameraManager 
    {
        private CinemachineVirtualCamera selectionCamera;
        private CinemachineVirtualCamera asteroidCamera;

        public CameraManager(CinemachineVirtualCamera selectionCam, CinemachineVirtualCamera asteroidCam)
        {
            selectionCamera = selectionCam;
            asteroidCamera = asteroidCam;


        }

        public void ActivateSelectionCamera()
        {
            selectionCamera.Priority = 10;
            asteroidCamera.Priority = 0;
        }

        public void ActivateAsteroidCamera(AsteroidData selectedAsteroid)
        {
            asteroidCamera.LookAt = selectedAsteroid.asteroidObj.transform;
            asteroidCamera.Follow = selectedAsteroid.asteroidObj.transform;

            //Calculate FOV based on the asteroid's scale
            float scale = selectedAsteroid.asteroidObj.transform.localScale.x;
            float fov = GetFOVFromScale(scale);
            asteroidCamera.m_Lens.FieldOfView = fov;


            //Adjust camera priority
            asteroidCamera.Priority = 10;
            selectionCamera.Priority = 0;
        }

        public void FocusOnOrbs(Vector3 groupCenter, float groupWidth)
        {
            asteroidCamera.LookAt = null;
            asteroidCamera.Follow = null;

            //Calculate new camera position and FOV
            asteroidCamera.transform.position = new Vector3(groupCenter.x, groupCenter.y, groupCenter.z - 10);
            asteroidCamera.m_Lens.FieldOfView = Mathf.Lerp(20f, 45f, groupWidth / 10f);
        }

        // Helper method to calculate interpolated FOV
        private float GetFOVFromScale(float scale)
        {
            // Define the scale-to-FOV mapping range
            float minScale = 3f;
            float maxScale = 7f;
            float minFOV = 20f;
            float maxFOV = 45f;

            // Normalize scale to a range of [0, 1]
            float t = Mathf.InverseLerp(minScale, maxScale, scale);

            // Interpolate FOV based on normalized value
            return Mathf.Lerp(minFOV, maxFOV, t);
        }

    }
}