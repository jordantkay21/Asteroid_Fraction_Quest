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

        public void FocusOnOrbs(Transform orbContainer)
        {
            FocusOnContainer(orbContainer, asteroidCamera);
        }

        public void FocusOnCells(Transform cellContainer)
        {
            FocusOnContainer(cellContainer, asteroidCamera);
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

        public void FocusOnContainer(Transform container, CinemachineVirtualCamera camera)
        {
            //Calculate combined bounds of the container
            Bounds combinedBounds = new Bounds(container.position, Vector3.zero);

            foreach (Transform child in container)
            {
                Renderer childRenderer = child.GetComponent<Renderer>();
                if (childRenderer != null)
                {
                    combinedBounds.Encapsulate(childRenderer.bounds);
                }
            }

            //Ensure bounds are valid
            if (combinedBounds.size == Vector3.zero)
            {
                Debug.LogWarning("Container has no renderable children. Focusing aborted.");
                return;
            }

            //Calculate the distance from the camera to the container center
            Vector3 cameraPosition = camera.transform.position;
            float distance = Vector3.Distance(cameraPosition, combinedBounds.center);

            //Use the larger dimension (width or height) to calculate the required FOV
            float objectHeight = combinedBounds.size.y;
            float objectWidth = combinedBounds.size.x;
            float aspectRatio = Screen.width / (float)Screen.height;

            //Compute vertical and horizontal FOV
            float verticalFOV = 2 * Mathf.Atan((objectHeight / 2) / distance) * Mathf.Rad2Deg;
            float horizontalFOV = 2 * Mathf.Atan((objectWidth / 2) / (distance * aspectRatio)) * Mathf.Rad2Deg;

            //Set the camera's FOV to ensure the object fits
            camera.m_Lens.FieldOfView = Mathf.Max(verticalFOV, horizontalFOV);

            //Adjust the camera's LookAt and Follow targets
            camera.LookAt = container;
            camera.Follow = container;
        }

        public Vector2 GetFrustumSizeAtDepth(float depth)
        {
            Camera mainCamera = Camera.main;
            if (mainCamera == null) return Vector2.zero;

            float frustumHeight = 2.0f * depth * Mathf.Tan(mainCamera.fieldOfView * 0.5f * Mathf.Deg2Rad);
            float frustumWidth = frustumHeight * mainCamera.aspect;

            return new Vector2(frustumWidth, frustumHeight);
        }

    }
}